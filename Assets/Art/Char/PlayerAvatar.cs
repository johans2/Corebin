using UnityEngine;
using System.Collections.Generic;
using RGCommon;
using DarkTonic.MasterAudio;
using System;
using RGEvents;

/// <summary>
/// The Player Avatar represents the player in the world scene.
/// Player Avatar is responsible for moving itself around
/// based on player input.
/// </summary>
public class PlayerAvatar : MonoBehaviour {
    const float TUTORIALTIMEOUT_MOVE = 5.0f;

    [SerializeField] float movementMultiplier = 18.0f;
    [SerializeField] string sceneForAppButton;
    [SerializeField] float delayToAppButtonScene = 1f;
    [SerializeField] int maxBackpackThrowSpaceChecks = 4;
    [SerializeField] SpeechBubble speechBubblePrefab;
    [SerializeField] Transform speechBubbleTransform;
    [SerializeField] SlowpokeMatPulse backpackNewItemMaterial;
    [SerializeField] GameObject[] backpackNewItemObjects;

    private bool isInitialized;
    private bool isMoving;

    private NavMeshAgent navMeshAgent;
    private GameObject shadowBlob;
    private LastMiniGamePlayed lastMiniGamePlayed;
    private AvatarAnimationController animationController;
    SpeechBubble speechBubble;
    GameObject ticketFrenzyInactiveObject;
    GameObject ticketFrenzyActiveObject;

    public enum AccelerationType {
        Instant,
        Linear,
        Curve,
    }

    enum InputType {
        AbsoluteFullsize,
        AbsolutePartial,
        Relative,
    };

    enum State {
        PlayerControlled,
        EnteringAttraction,
        EnteringBackpack,
    };

    AccelerationType accelerationType;
    State state;

    Vector2 lastKeyboardInput;

    Vector3 movementDirection;
    float targetVelocity;
    float currentVelocity;
    float velocityDelta;

    ClickInteraction currentClickInteraction;
    List<ClickInteraction> clickInteractions;

    bool isEnteringBackpack = false;

    // For the "swipe to move" tutorial speech bubble
    bool tutorialShowMove;
    float tutorialMoveTimer;

    // For the "app button to enter backpack" tutorial speech bubble
    bool tutorialShowBackpack;

    // For the "You need xx tickets to spin the wheel" tutorial speech bubble
    [HideInInspector]
    public bool tutorialYouNeedTickets;

    // For the "click to enter attraction" tutorial speech bubble
    SpeechBubble.TutorialID tutorialShowAttraction;
    Transform tutorialShowAttractionTransform;

    void Awake() {

        Signals.sceneLoaded.Dispatched += OnSceneLoaded;
        Signals.PlayerProgressStateUpdated.Dispatched += OnPlayerProgressStateUpdated;

        accelerationType = AccelerationType.Linear;
        state = State.PlayerControlled;

        //
        navMeshAgent = Find.ComponentOnGameObject<NavMeshAgent>(this);
        animationController = Find.ComponentOnChild<AvatarAnimationController>(this, "VisualRoot");
        shadowBlob = Find.ChildByName(this, "Shadow blob");
        ticketFrenzyInactiveObject = Find.ChildByName(this, "Balloon/TicketFrenzyInactiveObject");
        ticketFrenzyActiveObject = Find.ChildByName(this, "Balloon/TicketFrenzyActiveObject");

        bool haveFrenzy = Inventory.Instance.GetNumItems(InventoryCatalogue.ItemID.UniverseTicketFrenzy) > 0;
        ticketFrenzyInactiveObject.SetActive(!haveFrenzy);
        ticketFrenzyActiveObject.SetActive(haveFrenzy);

        // If the player returns to the world scene from another mini game move the avatar to the entrance of that game
        lastMiniGamePlayed = LastMiniGamePlayed.Instance;
        if(lastMiniGamePlayed.ExitPosition.HasValue) {
            navMeshAgent.Warp(lastMiniGamePlayed.ExitPosition.Value);
            transform.rotation = lastMiniGamePlayed.ExitRotation;
        }

        targetVelocity = 0.0f;
        currentVelocity = 0.0f;
        velocityDelta = 0.0f;
    }

    private void OnSceneLoaded(string sceneName) {

        if (sceneName != Universe.worldScene) {
            return;
        }

        speechBubble = Instantiate(speechBubblePrefab);

        // Move the speech bubble game object into the same scene as the object that spawned it
        speechBubble.transform.parent = transform;
        speechBubble.transform.parent = null;

        tutorialShowBackpack = Inventory.Instance.HaveAnyNewItemInCollection();

        // Hide all "new item in backpack" related effects
        if(!tutorialShowBackpack) {
            backpackNewItemMaterial.multiplier = 0.0f;
            backpackNewItemMaterial.offset = 0.0f;

            int i;
            for(i = 0; i < backpackNewItemObjects.Length; i++) {
                backpackNewItemObjects[i].SetActive(false);
            }
        }

        RefreshClickInteractionLabel();
        ResetMoveTimeout();
        isInitialized = true;

        Signals.sceneLoaded.Dispatched -= OnSceneLoaded;
    }

    void OnDestroy() {
        Signals.PlayerProgressStateUpdated.Dispatched -= OnPlayerProgressStateUpdated;
        Signals.sceneLoaded.Dispatched -= OnSceneLoaded;
        LeanTween.cancel(gameObject);
    }

    void OnPlayerProgressStateUpdated(int newState) {
        if (isInitialized) {
            RefreshClickInteractionLabel();
        }
    }

    private void Move(Vector3 offset) {
        navMeshAgent.Move(offset);
    }

    private void NavigateTo(Vector3 worldPosition) {
        navMeshAgent.SetDestination(worldPosition);

        Vector3 endpos = navMeshAgent.pathEndPosition;
        endpos.y = transform.position.y;
        transform.LookAt(endpos);

    }

    void ResetMoveTimeout() {
        tutorialMoveTimer = TUTORIALTIMEOUT_MOVE;
    }

    public void EnterAttraction(Attraction attraction) {
        NavigateTo(attraction.entrancePosition);
        state = State.EnteringAttraction;
        lastMiniGamePlayed.ExitPosition = attraction.GetExitPosition();
        lastMiniGamePlayed.ExitRotation = attraction.GetExitRotation();
        lastMiniGamePlayed.LoadedScene = attraction.sceneToLoad;
        Universe.LoadScene(attraction.sceneToLoad.ToString());
    }

    void FixedUpdate() {
        CalculateMovement(ref currentVelocity, ref targetVelocity, ref velocityDelta, movementMultiplier, accelerationType);
    }

    void Update() {
        if (!isInitialized) {
            return;
        }

        switch (state) {
            case State.PlayerControlled:
                DoInput();
                ResolveTutorial();
                break;

            case State.EnteringAttraction:
                AnimateFromAgent();
                break;

            case State.EnteringBackpack:
                if(!isEnteringBackpack) {
                    tutorialShowBackpack = false;
                    ResolveTutorial();
                    RotateToViableThrowDirection();
                    animationController.EnterBackpack();
                    LeanTween.scale(shadowBlob, Vector3.zero, delayToAppButtonScene / 2f);
                    isEnteringBackpack = true;
                }
                break;
        }
    }

    private void RotateToViableThrowDirection() {
        NavMeshPath path = new NavMeshPath();
        float degreesBetweenChecks = 360f / maxBackpackThrowSpaceChecks;

        for(int i = 0; i < maxBackpackThrowSpaceChecks; ++i) {
            float degreesToRotate = i * degreesBetweenChecks;
            Vector3 throwingDistanceTarget = transform.position + transform.forward;
            throwingDistanceTarget = RotatePointAroundPivot(throwingDistanceTarget, this.transform.position, Vector3.up * degreesToRotate);
            if(navMeshAgent.CalculatePath(throwingDistanceTarget, path)) {
                this.transform.Rotate(Vector3.up, degreesToRotate);
                return;
            }
        }
        Debug.Log("No viable backpack throwing spot found, using forward direction.");
    }

    private Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles) {
        Vector3 direction = point - pivot;
        Vector3 rotatedDirection = Quaternion.Euler(angles) * direction;
        point = rotatedDirection + pivot;
        return point;
    }


    void DoInput() {
        if(!Progression.Instance.IsAvatarMovementAllowed()) {
            return;
        }

        bool isPressing = IsPressing();
        bool shouldInputStart = !isMoving && isPressing;
        bool shouldInputContinue = isPressing;
        bool shouldInputStop = isMoving && !isPressing;
        isMoving = isPressing;
        if(shouldInputStart) {
            animationController.StartTouch();
        }

        float vel = 0.0f;
        targetVelocity = 0.0f;  // Assume no input
        if(shouldInputContinue) {
            Vector2 pos = GetInputCoordinates();

            Vector3 fwd, rgt;
            GetCameraVectors(out fwd, out rgt);
            Vector3 moveVec = (fwd * pos.y) + (rgt * pos.x);
            vel = moveVec.magnitude;
            if(vel > 0) {
                // Keep the old movementDirection if controller is centered
                movementDirection = moveVec / vel;
            }
            vel = Mathf.Clamp01(vel);
            vel *= movementMultiplier;

            targetVelocity = vel;

            // targetVelocity is used in FixedUpdate to add acceleration and braking and put the resolut in currentVelocity
            //visualRoot.LookAt(visualRoot.position + movementDirection);
            transform.LookAt(transform.position + movementDirection);
        }

        if(shouldInputStop) {
            animationController.EndTouch();
        }

        if(Progression.Instance.IsAvatarTutorialAllowed() && (tutorialMoveTimer >= 0.0f)) {
            tutorialMoveTimer -= Time.deltaTime;
        }
        tutorialShowMove = (tutorialMoveTimer < 0.0f);

        if(targetVelocity > 0.1f) {
            // Player is moving, don't queue toe movement tutorial
            ResetMoveTimeout();
            WorldSignals.PlayerAvatarMovement.Dispatch();
        }

        // The animator expect velocity to be from 0 to 100, inclusive
        Move(movementDirection * currentVelocity * Time.deltaTime);
        animationController.SetVelocity(currentVelocity, movementMultiplier);

        if(RGInput.Instance.ButtonWasPressed(RGInput.Button.Click)) {
            if((currentClickInteraction != null) && (Progression.Instance.IsClickInteractionIsAllowed(currentClickInteraction))) {
                currentClickInteraction.Interact(this);
            }
        }

        if(RGInput.Instance.ButtonWasPressed(RGInput.Button.App) && Progression.Instance.IsBackpackAllowed() && sceneForAppButton != null) {
            state = State.EnteringBackpack;
            lastMiniGamePlayed.ExitPosition = transform.position;
            lastMiniGamePlayed.ExitRotation = transform.rotation;
            LeanTween.delayedCall(gameObject, delayToAppButtonScene, () => { Universe.LoadScene(sceneForAppButton); });
        }
    }

    void AnimateFromAgent() {
        animationController.SetVelocity(navMeshAgent.velocity.magnitude, movementMultiplier);
    }

    bool IsPressing() {
        return IsKeyboardPressed() || RGInput.Instance.ButtonIsDown(RGInput.Button.Touch);
    }

    private bool IsKeyboardPressed() {
#if RELEASE_BUILD
        return false;
#else
        return (
            Input.GetKey(KeyCode.W) ||
            Input.GetKey(KeyCode.S) ||
            Input.GetKey(KeyCode.A) ||
            Input.GetKey(KeyCode.D)
        );
#endif
    }

    Vector2 GetInputCoordinates() {
        Vector2 ret = Vector2.Scale(RGInput.Instance.GetTouchPosition(), new Vector2(2, -2)) + new Vector2(-1, 1);
        if(RGInput.Instance.ButtonIsDown(RGInput.Button.Touch)) {
            return ret;
        }

#if !RELEASE_BUILD
        if(IsKeyboardPressed()) {
            ret = Vector2.zero;
            if(Input.GetKey(KeyCode.W)) ret.y = 1;
            if(Input.GetKey(KeyCode.S)) ret.y = -1;
            if(Input.GetKey(KeyCode.A)) ret.x = -1;
            if(Input.GetKey(KeyCode.D)) ret.x = 1;
            lastKeyboardInput = ret;
        }
        else {
            ret = lastKeyboardInput;
        }
#endif
        return ret;
    }

    void GetCameraVectors(out Vector3 fwd, out Vector3 rgt) {
        fwd = transform.position - Camera.main.transform.position;
        fwd.y = 0.0f;
        fwd.Normalize();
        rgt = new Vector3(fwd.z, 0.0f, -fwd.x);
    }

    static public void CalculateMovement(ref float currentVelocity, ref float targetVelocity, ref float velocityDelta, float velocityMax, AccelerationType accelerationType) {
        float diff = targetVelocity - currentVelocity;
        if(diff > 0.0f) {
            //
            // Accelerate
            //
            switch (accelerationType) {
                case AccelerationType.Instant:
                    velocityDelta = diff;
                    break;

                case AccelerationType.Linear:
                    velocityDelta = 0.2f;
                    break;

                case AccelerationType.Curve:
                    diff /= velocityMax;
                    diff *= 0.04f;
                    velocityDelta += diff;
                    break;
            }
        } else if(diff < 0.0f) {
            //
            // Brake
            //
            diff /= velocityMax;
            diff *= 0.02f;
            velocityDelta += diff;
        }

        currentVelocity += velocityDelta;

        if((diff > 0.0f) && (velocityDelta > 0.0f)) {
            if(currentVelocity > targetVelocity) {
                currentVelocity = targetVelocity;
                velocityDelta = 0.0f;
            }
        } else if((diff < 0.0f) && (velocityDelta < 0.0f)) {
            if(currentVelocity < targetVelocity) {
                currentVelocity = targetVelocity;
                velocityDelta = 0.0f;
            }
        }

        if(currentVelocity < 0.0f)
            currentVelocity = 0.0f;

        if(currentVelocity >= velocityMax)
            currentVelocity = velocityMax;
    }

    public void SetCurrentClickInteraction(ClickInteraction interaction) {
        currentClickInteraction = interaction;
        if (isInitialized) {
            RefreshClickInteractionLabel();
        }
    }

    void RefreshClickInteractionLabel() {
        if(Progression.Instance.IsAvatarTutorialAllowed()) {
            if(currentClickInteraction && Progression.Instance.IsClickInteractionIsAllowed(currentClickInteraction)) {
                tutorialShowAttraction = currentClickInteraction.tutorialID;
                tutorialShowAttractionTransform = currentClickInteraction.interactionTargetTransform;
                speechBubble.Show(currentClickInteraction.tutorialID, currentClickInteraction.interactionTargetTransform);
            } else {
                tutorialShowAttraction = SpeechBubble.TutorialID.Hidden;
                speechBubble.Hide();
            }
        }
    }

    void ResolveTutorial() {
        if(Progression.Instance.IsAvatarTutorialAllowed()) {
            if(tutorialShowBackpack || Progression.Instance.ForceShowBackpackTutorial()) {
                speechBubble.Show(SpeechBubble.TutorialID.AppForBackpack, speechBubbleTransform);
            }
            else if(tutorialShowAttraction != SpeechBubble.TutorialID.Hidden) {
                speechBubble.Show(tutorialShowAttraction, tutorialShowAttractionTransform);
            }
            else if(tutorialShowMove) {
                speechBubble.Show(SpeechBubble.TutorialID.SwipeToMove, speechBubbleTransform);
            }
            else if(tutorialYouNeedTickets) {
                speechBubble.Show(SpeechBubble.TutorialID.TicketsToEnterPsyduck, speechBubbleTransform);
            }
            else {
                speechBubble.Hide();
            }
        }
    }

    public Transform GetSpeechBubbleTransform() {
        return speechBubbleTransform;
    }
}

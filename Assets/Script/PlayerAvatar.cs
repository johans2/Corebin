using UnityEngine;
using System.Collections;
using RGCommon;
using System;
using CakewalkIoC.Signal;

public class PlayerAvatar : MonoBehaviour {

    public static Signal<bool> IsInteractingSignal = new Signal<bool>();

    public Transform ObjectHolder { get { return objectHolder; } }

    public GameObject CurrentLoot {
        get { return currentLoot; }
    }

    private RGInput input;
    private Quaternion startRotation;
    private Transform charTransform;
    private Animator anim;

    private float animationSpeed;
    private Interactable currentInteractable;
    private bool isInteracting = false;
    private GameObject currentLoot;
    
    private Transform objectHolder;
    
    void Awake() {
        Planet.StartScalingPlanetSignal.AddListener(OnLevelUp);
        input = RGInput.Instance;
        charTransform = Find.ComponentOnChild<Transform>(this, "Boy");
        anim = Find.ComponentOnChild<Animator>(this, "Boy");
        objectHolder = Find.ComponentOnChild<Transform>(this, "Boy/LootHolder");
    }

    private void OnLevelUp() {

        LeanTween.scale(gameObject, transform.localScale *= 1 / Constants.PlanetScaleFactor, 0.4f);
    }

    void Start () {
        startRotation = charTransform.localRotation;
	}
	
	void Update () {
        if(input.ButtonIsDown(RGInput.Button.Touch) && !isInteracting) {
            // Update rotation.
            Vector2 refVector = Vector2.up;
            Vector2 touchVector = input.GetTouchPosition() - new Vector2(0.5f, 0.5f);
            animationSpeed = touchVector.magnitude;
            
            float angle = Vector2.Angle(refVector, touchVector);
            Vector3 cross = Vector3.Cross(refVector, touchVector);

            if(cross.z < 0) {
                angle = -angle;
            }

            charTransform.localRotation = startRotation * Quaternion.Euler(0, angle, 0);
        }
        else {
            animationSpeed = 0f;
        }


        // Interactables.
        if(input.ButtonWasPressed(RGInput.Button.Click) && currentInteractable != null && !isInteracting) {
            
            if(currentInteractable.CompareTag("Lootable") && currentLoot == null && currentInteractable.isInteractable) {
                isInteracting = true;
                IsInteractingSignal.Dispatch(true);
                currentInteractable.Interact(this, OnCollectComplete);

            }

            else if(currentInteractable.CompareTag("CoreBin")) {
                isInteracting = true;
                IsInteractingSignal.Dispatch(true);
                currentInteractable.Interact(this, OnThrowToBinComplete);
            }

        }
        
        anim.SetFloat("runSpeed", animationSpeed);
    }

    private void OnThrowToBinComplete(GameObject obj) {
        isInteracting = false;
        IsInteractingSignal.Dispatch(false);
    }

    void OnCollectComplete(GameObject reward) {
        isInteracting = false;
        IsInteractingSignal.Dispatch(false);
        SpawnLootItem(reward);
    }
    

    void SpawnLootItem(GameObject item) {
        currentLoot = (GameObject)Instantiate(item, objectHolder.position, Quaternion.identity);
        currentLoot.transform.parent = objectHolder;
    }

    void OnTriggerEnter(Collider other) {
        Interactable inter = other.gameObject.GetComponent<Interactable>();
        if(inter != null) {
            currentInteractable = inter;
        }
    }

    void OnTriggerExit(Collider other) {
        currentInteractable = null;
    }
    
    void OnDestroy() {
        CameraBehaviour.LevelUpFadeDoneSignal.RemoveListener(OnLevelUp);
    }
}

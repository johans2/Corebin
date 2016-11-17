using UnityEngine;
using System.Collections;
using RGCommon;
using RGEvents;

public class SpeechBubble : MonoBehaviour {
    public enum TutorialID {
        Hidden,
        ClickForAttraction,
        AppForBackpack,
        SwipeToMove,
        AppToExitBackpack,
        HomeToRecenter,
        AppToExitAndHomeToRecenter,
        TicketsToEnterPsyduck,
    }

    Animator myAnimator;
    Transform targetTransform;
    Transform myTransform;
    Vector3 cameraPosition;

    private Vector3 INVALID_VEC3 = new Vector3(float.NaN, float.NaN, float.NaN);

    void Awake() {
        myTransform = transform;
        myAnimator = Find.ComponentOnGameObject<Animator>(this);

        // Calculate a static camera position is good enough for the rotation
        cameraPosition = INVALID_VEC3;
    }

    void Update() {
        if(targetTransform) {
            if (cameraPosition.Equals(INVALID_VEC3)) {
                cameraPosition = SceneModifier.Instance.GetCenterCamera().position;
            }

            Vector3 newPosition = targetTransform.position;
            myTransform.position = newPosition;
            Vector3 d = newPosition - cameraPosition;    // Speech bubble forward is going into the scene so if we make it look at the camera the bubble will be turned 180 degrees in the wrong way
            d += newPosition;
            d.y = newPosition.y;
            myTransform.LookAt(d);
        }
    }

    public void Show(TutorialID tutorialID, Transform followTransform) {
        myAnimator.SetInteger("TutorialID", (int)tutorialID);
        targetTransform = followTransform;
    }

    public void Hide() {
        myAnimator.SetInteger("TutorialID", (int)TutorialID.Hidden);
        targetTransform = null;
    }
}

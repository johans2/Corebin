using UnityEngine;
using System.Collections;
using RGCommon;

public class PlayerAvatar : MonoBehaviour {

    private RGInput input;
    private Quaternion startRotation;
    private Transform charTransform;
    private Animator anim;

    private float animationSpeed;

    void Awake() {
        input = RGInput.Instance;
        charTransform = Find.ComponentOnChild<Transform>(this, "Boy");
        anim = Find.ComponentOnChild<Animator>(this, "Boy");
    }

	void Start () {
        startRotation = charTransform.localRotation;
	}
	
	void Update () {
        if(input.ButtonIsDown(RGInput.Button.Touch)) {
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

        anim.SetFloat("runSpeed", animationSpeed);
    }
}

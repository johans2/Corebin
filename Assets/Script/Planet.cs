using UnityEngine;
using System.Collections;
using RGCommon;
using System;

public class Planet : MonoBehaviour {

    RGInput input;
    public float rotationSpeed = 10f;


    void Awake() {
        input = RGInput.Instance;
    }

	void Start () {
	
	}
	
	void Update () {

        HandleInput();
	}

    private void HandleInput() {
        Vector2 touchPosition = input.GetTouchPosition() - new Vector2(0.5f, 0.5f);
        if(input.ButtonIsDown(RGInput.Button.Touch)) {
            transform.RotateAround(transform.position, Vector3.up, touchPosition.x * rotationSpeed * Time.deltaTime);
            transform.RotateAround(transform.position, Vector3.right, touchPosition.y * rotationSpeed * Time.deltaTime);

        }

    }
}

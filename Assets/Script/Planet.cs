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
        Debug.Log("TP" + touchPosition);
        if(input.ButtonIsDown(RGInput.Button.Touch)) {
            transform.rotation *= Quaternion.Euler(touchPosition.y * rotationSpeed * Time.deltaTime,  touchPosition.x * rotationSpeed * Time.deltaTime ,  0  );
        }

    }
}

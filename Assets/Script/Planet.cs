using UnityEngine;
using System.Collections;
using RGCommon;
using System;

public class Planet : MonoBehaviour {

    RGInput input;
    public float rotationSpeed;


    void Awake() {
        input = RGInput.Instance;
    }

	void Start () {
	
	}
	
	void Update () {

        HandleInput();
	}

    private void HandleInput() {
        Vector2 touchPosition = input.GetTouchPosition() - new Vector2(-0.5f, -0.5f);

        transform.rotation *= Quaternion.Euler(touchPosition.y * rotationSpeed * Time.deltaTime,  touchPosition.x * rotationSpeed * Time.deltaTime ,  0  );

    }
}

﻿using UnityEngine;
using System.Collections;
using RGCommon;
using System;
using System.Collections.Generic;

public class Planet : MonoBehaviour {



    RGInput input;
    public float rotationSpeed = 100f;
    public Transform globeTransform;


    private float scaleFactor = 1.2f;
    private GameObject propsParent;
    private List<GameObject> props;

    void Awake() {
        props = new List<GameObject>();
        input = RGInput.Instance;
        globeTransform = Find.ComponentOnChild<Transform>(this, "Planet");
        propsParent = Find.ChildByName(this, "Props");

        for(int i = 0; i < propsParent.transform.childCount; i++) {
            props.Add(propsParent.transform.GetChild(i).gameObject);
        }



    }

    public void LevelUp() {
        globeTransform.localScale = new Vector3(globeTransform.localScale.x * scaleFactor, globeTransform.localScale.y * scaleFactor, globeTransform.localScale.z * scaleFactor);
        for(int i = 0; i < props.Count; i++) {
            GameObject prop = props[i];

            prop.transform.Translate(new Vector3(0, 0.9f, 0), Space.Self);

        }

    }

	void Update () {
        if(Input.GetKeyDown(KeyCode.Space)) {
            LevelUp();
        }


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

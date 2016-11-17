using UnityEngine;
using System.Collections;
using RGCommon;
using System;
using System.Collections.Generic;
using CakewalkIoC.Signal;

public class Planet : MonoBehaviour {

    public static Signal<int> LevelUpSignal = new Signal<int>();

    public GameObject boy;
    public float rotationSpeed = 100f;
    
    private RGInput input;
    private GameObject propsParent;
    private List<GameObject> props;

    private float currentRadius;
    private Renderer rend;

    private int level;

    void Awake() {
        level = 0;
        props = new List<GameObject>();
        input = RGInput.Instance;
        propsParent = Find.ChildByName(this, "Props");
        rend = Find.ComponentOnChild<Renderer>(this, "Planet");

        for(int i = 0; i < propsParent.transform.childCount; i++) {
            props.Add(propsParent.transform.GetChild(i).gameObject);
        }
    }

    void Start() {
        currentRadius = rend.bounds.extents.magnitude;
    }

    public void LevelUp() {
        level++;
        transform.localScale *= Constants.PlanetScaleFactor; 
        for(int i = 0; i < props.Count; i++) {
            GameObject prop = props[i];

            prop.transform.localScale *= 1 / Constants.PlanetScaleFactor;
        }
        LevelUpSignal.Dispatch(level);
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
            boy.transform.RotateAround(transform.position, Vector3.up, -touchPosition.x * rotationSpeed * Time.deltaTime);
            boy.transform.RotateAround(transform.position, Vector3.right, -touchPosition.y * rotationSpeed * Time.deltaTime);
        }

    }
}

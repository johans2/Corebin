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
    private bool playerIsInteracting;


    void Awake() {
        level = 0;
        props = new List<GameObject>();
        input = RGInput.Instance;
        playerIsInteracting = false;
        propsParent = Find.ChildByName(this, "Props");
        rend = Find.ComponentOnChild<Renderer>(this, "Planet");
        CameraBehaviour.LevelUpFadeDoneSignal.AddListener(OnLevelUpFadeDone);
        PlayerAvatar.IsInteractingSignal.AddListener(OnPlayerInteracting);
        ExpBar.LevelUpSignal.AddListener(LevelUp);

        for(int i = 0; i < propsParent.transform.childCount; i++) {
            props.Add(propsParent.transform.GetChild(i).gameObject);
        }
    }
    
    private void OnPlayerInteracting(bool isInteracting) {
        playerIsInteracting = isInteracting;
    }

    void Start() {
        currentRadius = rend.bounds.extents.magnitude;
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.L)) {
            LevelUp();
        }
        
        HandleInput();
    }

    public void LevelUp() {
        level++;
        LevelUpSignal.Dispatch(level);
    }
    
    private void OnLevelUpFadeDone() {
        GrowPlanet();
    }

    private void GrowPlanet() {
        LeanTween.scale(gameObject, transform.localScale * Constants.PlanetWiggleFactor, 0.05f).setLoopPingPong(5).setOnComplete(() => {
            LeanTween.scale(gameObject, transform.localScale * Constants.PlanetScaleFactor, 0.4f).setEase(LeanTweenType.easeOutElastic);

            for(int i = 0; i < props.Count; i++) {
                GameObject prop = props[i];

                LeanTween.scale(prop, prop.transform.localScale *= 1 / Constants.PlanetScaleFactor, 0.4f);
            }
            
        });
    }
    
    private void HandleInput() {
        if(playerIsInteracting) {
            return;
        }

        Vector2 touchPosition = input.GetTouchPosition() - new Vector2(0.5f, 0.5f);
        if(input.ButtonIsDown(RGInput.Button.Touch)) {
            transform.RotateAround(transform.position, Vector3.up, touchPosition.x * rotationSpeed * Time.deltaTime);
            transform.RotateAround(transform.position, Vector3.right, touchPosition.y * rotationSpeed * Time.deltaTime);
            boy.transform.RotateAround(transform.position, Vector3.up, -touchPosition.x * rotationSpeed * Time.deltaTime);
            boy.transform.RotateAround(transform.position, Vector3.right, -touchPosition.y * rotationSpeed * Time.deltaTime);
        }

        if(input.ButtonWasPressed(RGInput.Button.App)) {
            LevelUp();
        }

    }
}

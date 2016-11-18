using UnityEngine;
using System.Collections;
using RGCommon;
using System;
using CakewalkIoC.Signal;

public class ExpBar : MonoBehaviour {

    public static Signal LevelUpSignal = new Signal();

    private int requiredExp = 150;

    private GameObject expBar;
    private MeshRenderer expRenderer;
    private int currentExp;

    private Vector3 startRotation;

    void Awake() {
        currentExp = 0;
        startRotation = transform.rotation.eulerAngles;
        expBar = Find.ChildByName(this, "ExpBar/TensionMeter");
        expRenderer = expBar.GetComponent<MeshRenderer>();
        CoreBinInteractable.GainExpSignal.AddListener(OnGainExp);
        LevelUpSignal.AddListener(OnLevelUp);
        expBar.transform.localScale = new Vector3(expBar.transform.localScale.x, 0.05f, expBar.transform.localScale.z);
        expRenderer.material.SetFloat("_LerpValue", 0);
    }

    private void OnLevelUp() {
        currentExp = 0;
        transform.position *= Constants.PlanetScaleFactor;
        transform.position += new Vector3(0, 5, 0);
        expBar.transform.localScale = new Vector3(expBar.transform.localScale.x, 0.05f, expBar.transform.localScale.z);
        transform.localScale *= Constants.PlanetScaleFactor;
        expRenderer.material.SetFloat("_LerpValue", 0);
    }

    private void OnGainExp(int exp) {
        LeanTween.rotate(gameObject, startRotation + new Vector3(10f, 0, 0), 0.1f).setLoopPingPong(3).setOnComplete(() => {
            currentExp = Mathf.Clamp(0, currentExp + exp, requiredExp);

            float newScale = Mathf.Lerp(0, 1, (float)currentExp / 100f);
            float color = Mathf.Lerp(0, 1, (float)currentExp / 100f);
            expRenderer.material.SetFloat("_LerpValue", color);
            
            LeanTween.scale(expBar, new Vector3(expBar.transform.localScale.x, newScale, expBar.transform.localScale.z), 0.2f).setOnComplete(() => {
                if(currentExp >= requiredExp) {
                    LevelUpSignal.Dispatch();
                }
            });
        });
    }
}

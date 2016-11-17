using UnityEngine;
using System.Collections;
using System;
using CakewalkIoC.Signal;
using RGCommon;

public class CameraBehaviour : MonoBehaviour {

    public static Signal LevelUpFadeDoneSignal = new Signal();

    private Vector3 startPosition;
    private Fader fader;

	void Start () {
        fader = Find.ComponentOnGameObject<Fader>(this);
        Planet.LevelUpSignal.AddListener(OnLevelUp);
        startPosition = transform.position;

        fader.fadeTime = 0.7f;
    }

    private void OnLevelUp(int newLevel) {
        StartCoroutine(FadeAndMove());   
    }

    private IEnumerator FadeAndMove() {
        // Fade out

        fader.FadeOut();
        while(!fader.Done) {
            yield return null;
        }

        // Move camera
        transform.position *= Constants.PlanetScaleFactor;
        
        // Fade in
        fader.FadeIn();
        while(!fader.Done) {
            yield return null;
        }

        LevelUpFadeDoneSignal.Dispatch();
    }

    void OnDestroy () {
        Planet.LevelUpSignal.RemoveListener(OnLevelUp);
	}
}

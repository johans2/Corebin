using UnityEngine;
using System.Collections;
using System;

public class CameraBehaviour : MonoBehaviour {

    private Vector3 startPosition;
    
	void Start () {
        Planet.LevelUpSignal.AddListener(OnLevelUp);
        startPosition = transform.position;
        

	}

    private void OnLevelUp(int newLevel) {
        transform.position *= Constants.PlanetScaleFactor;

    }

    void OnDestroy () {
        Planet.LevelUpSignal.RemoveListener(OnLevelUp);
	}
}

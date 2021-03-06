﻿using UnityEngine;
using System.Collections;
using System;

public enum AnimalType {
    None,
    Cow,
    Chicken,
    Cheep
}

public class InteractableChicken : Interactable {
    
    public AnimalType type;
    public float regrowTime = 15f;
    public GameObject chicken;
    
    public override void Interact(PlayerAvatar player, Action<GameObject> OnInteractionComplete) {
        isInteractable = false;
        OnInteractionComplete(chicken);
        ReGrow();
    }
    
    private void ReGrow() {
        transform.localScale = new Vector3(0, 0, 0);
        LeanTween.scale(gameObject, Vector3.one, regrowTime).setOnComplete(() => {
            isInteractable = true;
        });
    }
}

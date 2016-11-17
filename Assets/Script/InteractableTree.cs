using UnityEngine;
using System.Collections;
using System;

public class InteractableTree : Interactable {

    public float regrowTime = 15f;
    public GameObject log;
    public GameObject cuttingAxe;
    
    public override void Interact(PlayerAvatar player, Action<GameObject> OnInteractionComplete) {
        isInteractable = false;
        GameObject axe = (GameObject)Instantiate(cuttingAxe, player.ObjectHolder.position, Quaternion.identity);

        LeanTween.rotate(axe, new Vector3(50, 0, 0), 0.1f).setLoopPingPong(5).setOnComplete(() => {
            Destroy(axe);
            OnInteractionComplete(log);
            ReGrow();
        });
    }

    private void ReGrow() {
        transform.localScale = new Vector3(0, 0, 0);
        LeanTween.scale(gameObject, Vector3.one, regrowTime).setOnComplete(() => {
            isInteractable = true;
        });
    }
}

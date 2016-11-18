using UnityEngine;
using System.Collections;
using System;

public class InteractableTree : Interactable {

    public float regrowTime = 15f;
    public GameObject log;
    public GameObject cuttingAxe;
    private Vector3 startSize;

    void Start() {
        startSize = transform.localScale;
    }

    public override void Interact(PlayerAvatar player, Action<GameObject> OnInteractionComplete) {
        isInteractable = false;
        GameObject axe = (GameObject)Instantiate(cuttingAxe, player.ObjectHolder.position, player.ObjectHolder.rotation);

        LeanTween.rotateLocal(axe, new Vector3(50, 0, 0), 0.2f).setLoopPingPong(5).setOnComplete(() => {
            Destroy(axe);
            OnInteractionComplete(log);
            ReGrow();
        });
    }

    private void ReGrow() {
        transform.localScale = new Vector3(0, 0, 0);
        LeanTween.scale(gameObject, startSize, regrowTime).setOnComplete(() => {
            isInteractable = true;
        });
    }
}

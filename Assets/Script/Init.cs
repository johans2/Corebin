using UnityEngine;
using System.Collections;

public class Init : MonoBehaviour {

    private static int TWEEN_POOL_SIZE = 50;

    void Awake() {
        LeanTween.init(TWEEN_POOL_SIZE);
    }
}

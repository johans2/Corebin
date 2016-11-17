using UnityEngine;
using System.Collections;

public class NewParentAtStart : MonoBehaviour {

    public Transform parentAtStart;

	void Start() {
        transform.parent = parentAtStart;
    }
}

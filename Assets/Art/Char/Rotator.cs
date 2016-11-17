using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour {

    [SerializeField] Vector3 axis = Vector3.zero;
    [SerializeField] float speed = 1.0f;

    Transform myTransform;

	void Start () {
        myTransform = transform;
	}
	
	void Update () {
        myTransform.Rotate(axis, speed * Time.deltaTime);
	}
}

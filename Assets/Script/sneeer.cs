using UnityEngine;
using System.Collections;

public class sneeer : MonoBehaviour {

    public float rotationSpeed = 10f;

	
	void Update () {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);    
	}
}

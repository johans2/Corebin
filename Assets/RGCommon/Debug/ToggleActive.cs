using UnityEngine;
using System.Collections;

/**
 * Toggles the active state of a bunch of objects when you press a key.
 * Useful for e.g. testing two versions of an asset without
 * having to manually click the checkboxes of two objects.
 */
public class ToggleActive : MonoBehaviour {

	public GameObject[] objects;

	public KeyCode key = KeyCode.T;

	void Update() {
		if(Input.GetKeyDown(key)) {
			foreach(var obj in objects) {
				obj.SetActive(!obj.activeSelf);
			}
		}
	}
}

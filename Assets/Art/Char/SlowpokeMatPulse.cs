using UnityEngine;
using System.Collections;

public class SlowpokeMatPulse : MonoBehaviour {

    private Renderer rend;
    private Material mat;
    public float multiplier = 0.25f;
    public float offset = 0.65f;
    public float speed;

    void Awake() {
        rend = GetComponent<Renderer>();
        mat = rend.material;
        mat.EnableKeyword("_Progress");
    }
	// Use this for initialization
	void Start () {
        mat.SetFloat("_Progress", 0.5f);
	}
	
	// Update is called once per frame
	void Update () {
        float fade = Mathf.Sin(Time.time * speed) * multiplier + offset;
        mat.SetFloat("_Progress", fade);
    }
}

using UnityEngine;

public class GodRayDust : MonoBehaviour {
	public int materialIndex = 0;
	public Vector2 uvAnimationRate = new Vector2(1.0f, 0.0f);
	public string textureName = "_MainTex";

	private Renderer dustRenderer;
	private Vector2 uvOffset = Vector2.zero;

	void Start () {
		dustRenderer = GetComponent<Renderer>();
	}

	void Update () {
		uvOffset += (uvAnimationRate * Time.deltaTime);
		dustRenderer.material.SetTextureOffset("_MainTex", uvOffset);   
	}

}

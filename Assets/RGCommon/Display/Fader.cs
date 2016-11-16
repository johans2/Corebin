using UnityEngine;
using System.Collections;

namespace RGCommon {
    /**
     * Fades in/out the screen.
     * Some techniques and code taken from OVRScreenFade.
     */
    public class Fader : MonoBehaviour {

        public float targetAlpha = 0.0f;
        public float fadeTime = 2.0f;
        public Shader fadeShader = null;
        public Color color = Color.black;

        private float currentAlpha;
        private Material fadeMaterial = null;

        public void FadeIn() {
            targetAlpha = 0.0f;
        }

        public void FadeOut() {
            //Debug.Log("Fade out");
            targetAlpha = 1.0f;
        }

        public bool Done {
            get { return currentAlpha == targetAlpha; }
        }

        void Awake() {
            fadeMaterial = (fadeShader != null) ? new Material(fadeShader) : new Material(Shader.Find("Transparent/Diffuse"));
            fadeMaterial.color = color;
        }

        void Start() {
            currentAlpha = 1.0f;
        }

        void Update() {
            if(currentAlpha == targetAlpha) {
                return;
            }
            currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, Time.deltaTime / fadeTime);
            color.a = currentAlpha;
            fadeMaterial.color = color;
            if(currentAlpha == targetAlpha) {
                //Debug.Log("Fade done on : " + this.gameObject.name);
            }
        }


        void OnLevelWasLoaded(int level) {
            FadeIn();
        }

        void OnDestroy() {
            if(fadeMaterial != null) {
                Destroy(fadeMaterial);
            }
        }


        void OnPostRender() {
            if(currentAlpha != 0) {
                fadeMaterial.SetPass(0);
                GL.PushMatrix();
                GL.LoadOrtho();
                GL.Color(fadeMaterial.color);
                GL.Begin(GL.QUADS);
                GL.Vertex3(0f, 0f, -12f);
                GL.Vertex3(0f, 1f, -12f);
                GL.Vertex3(1f, 1f, -12f);
                GL.Vertex3(1f, 0f, -12f);
                GL.End();
                GL.PopMatrix();
            }
        }
    }
}

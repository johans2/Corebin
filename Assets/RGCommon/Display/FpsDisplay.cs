using UnityEngine;

namespace RGCommon {
    public class FpsDisplay : MonoBehaviour {

        public float updateInterval = 1.0f;
        public KeyCode toggleKey = KeyCode.R;
        public bool visible = false;

        public float warningTime = .018f;
        public Color standardColor = Color.gray;
        public Color warningColor = Color.red;

        private TextMesh textMesh;
        private Renderer rendererComponent;

        private int frameCount;
        private int warningCount;
        private float elapsed;
        private float fps;
        private float msPerFrame;
        private float maxTime = 0;
        private float minTime = Mathf.Infinity;

        private string text;

        private float lastUpdate = 0;

        private float collectionStartTime;

        void Awake() {
            textMesh = Find.ComponentOnGameObject<TextMesh>(this);
            rendererComponent = Find.ComponentOnGameObject<Renderer>(this);
        }

        void Start() {
            collectionStartTime = Time.realtimeSinceStartup;

            rendererComponent.enabled = visible;
            rendererComponent.material.color = this.standardColor;
        }

        void Update() {
            float now = Time.realtimeSinceStartup;

            // Accumulate values for current frame
            float frameTime = now - lastUpdate;
            if(visible) {
                if(frameTime > warningTime) {
                    rendererComponent.material.color = this.warningColor;
                    ++warningCount;
                }
                else {
                    rendererComponent.material.color = this.standardColor;
                }
            }
            maxTime = Mathf.Max(maxTime, frameTime);
            minTime = Mathf.Min(minTime, frameTime);
            lastUpdate = now;
            ++frameCount;

            // Toggle the FPS display with the keyboard
            if(Input.GetKeyDown(toggleKey)) {
                ToggleVisibility();
            }

            // Every updateInterval seconds, update the display and clear accumulated values
            float elapsed = now - collectionStartTime;
            if(elapsed >= updateInterval) {
                fps = frameCount / elapsed;
                msPerFrame = elapsed / frameCount * 1000;
                if(visible) {
                    UpdateDisplay();
                }
                frameCount = 0;
                warningCount = 0;
                maxTime = 0;
                minTime = Mathf.Infinity;
                collectionStartTime = now;
            }
        }

        public void ToggleVisibility() {
            visible = !visible;
            rendererComponent.enabled = visible;
            if(visible) {
                UpdateDisplay();
            }
        }

        private void UpdateDisplay() {
            text = string.Format("{0:F2}FPS avg={1:F2} min={3:F2} max={2:F2} ms {4}W", fps, msPerFrame, maxTime * 1000, minTime * 1000, warningCount);
            //Debug.Log("FpsDisplay: " + text);
            textMesh.text = text;
        }
    }
}

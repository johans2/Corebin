using UnityEngine;

namespace RGCommon {
    public class SpeedControl : MonoBehaviour {

        public bool enableOnDevice = false;

        public KeyCode activationKey = KeyCode.Alpha0;
        public float speedFactorWhenActivated = .2f;
        private bool activated;

        void Start() {
#if UNITY_EDITOR
            // Allow multiple physics update per frame on computer.
            // To make game run in full speed on MacBook that has lower frame rate than phone.
            Time.maximumDeltaTime = .5f;
#endif

#if !UNITY_EDITOR
            enabled = enableOnDevice;
#endif
        }

        void Update() {
            if(Input.GetKeyDown(activationKey)) {
                activated = !activated;
                Time.timeScale = activated ? speedFactorWhenActivated : 1;
            }
        }
    }
}

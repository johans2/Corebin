using UnityEngine;

namespace RGCommon {
    /**
     * Rotates the object using the mouse, touch and keyboard.
     */
    public class EditorCameraControl : MonoBehaviour {
        public float mouseSpeed = 2.0f;
        public float maxKeyboardRotation = 30.0f;
        public KeyCode freezeKey = KeyCode.F;
        public float touchRotationSpeed = .3f;
        public float keyboardRotationSpeed = 5.0f;
        public KeyCode turnLeftKey = KeyCode.LeftArrow;
        public KeyCode turnRightKey = KeyCode.RightArrow;
        public KeyCode lookUpKey = KeyCode.UpArrow;
        public KeyCode lookDownKey = KeyCode.DownArrow;
        public KeyCode recenterKey = KeyCode.F12;

        private Vector3 mouseRotation;
        private bool mouseEnabled = false;
        
        void Start() {
            mouseRotation = transform.localRotation.eulerAngles;
#if UNITY_EDITOR
            mouseEnabled = true;
#endif
        }

        void Update() {
            if(Input.GetKeyDown(freezeKey)) {
                mouseEnabled = !mouseEnabled;
            }
            if(Input.GetKeyDown(recenterKey)) {
                UnityEngine.VR.InputTracking.Recenter();
            }

            if(Input.touchCount > 0) {
                Touch touch = Input.GetTouch(0);
                Vector2 delta = touch.deltaPosition * touchRotationSpeed;
                mouseRotation.y += delta.x;
                mouseRotation.x -= delta.y;
            }

            if(mouseEnabled) {
                float dx = Input.GetAxis("Mouse X");
                float dy = Input.GetAxis("Mouse Y");
                float dz = Input.GetAxis("Horizontal");
                mouseRotation.y += dx * mouseSpeed;
                mouseRotation.x -= dy * mouseSpeed;
                mouseRotation.z = dz * -maxKeyboardRotation;
            }
            if(Input.GetKey(turnLeftKey)) {
                mouseRotation.y -= keyboardRotationSpeed;
            }
            if(Input.GetKey(turnRightKey)) {
                mouseRotation.y += keyboardRotationSpeed;
            }
            if(Input.GetKey(lookUpKey)) {
                mouseRotation.x -= keyboardRotationSpeed;
            }
            if(Input.GetKey(lookDownKey)) {
                mouseRotation.x += keyboardRotationSpeed;
            }

            transform.localRotation = Quaternion.Euler(mouseRotation);
        }
    }
}

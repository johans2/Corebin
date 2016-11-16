using RGCommon;
using UnityEngine;

public class RGInput : SingleInstanceBehavior<RGInput> {

    public enum Button {
        Click,
        Touch,
        App,
        Count,  // dummy for counting number of values in enum
    }

    public struct ButtonState {
        /// <summary>
        /// The button is currently down.
        /// </summary>
        public bool isDown;

        /// <summary>
        /// The button was pressed during the frame.
        /// </summary>
        public bool wasPressed;

        /// <summary>
        /// The button was released during the frame.
        /// </summary>
        public bool wasReleased;

        public void Clear() {
            isDown = wasPressed = wasReleased = false;
        }

        /// <summary>
        /// Updates the state for the current frame.
        /// Calculates the wasPressed and wasReleased based on the previous values.
        /// </summary>
        /// <param name="isDown">Whether the button is currently down</param>
        public void UpdateState(bool isDown) {
            bool wasDown = this.isDown;
            this.isDown = isDown;
            wasPressed = !wasDown && isDown;
            wasReleased = wasDown && !isDown;
        }
    }

    private bool buttonsEnabled = true;

    private ButtonState[] buttonStates = new ButtonState[(int)Button.Count];

    private Quaternion orientation = Quaternion.identity;
    private Vector2 touchPos = Vector2.zero;

#if !RELEASE_BUILD
    // Let touch work as click so you don't have to double-tap on the controller
    private bool touchAsClick = false;

    // Allow rotation with keyboard or stick
    private Vector3 relativeOrientationEuler = Vector3.zero;
    private const float relativeRotationSpeed = 50f;

    /// <summary>
    /// "Pretend" the controller being connected/disconnected.
    /// </summary>
    private GvrConnectionState? forcedStatus = null;
#endif

    private void Update() {
#if !RELEASE_BUILD
        // Force the connected status with keyboard
        if(Input.GetKeyDown(KeyCode.F11)) {
            if(forcedStatus == GvrConnectionState.Connected) {
                forcedStatus = null;
            }
            else {
                forcedStatus = GvrConnectionState.Connected;
            }
        }
        if(Input.GetKeyDown(KeyCode.F9)) {
            touchAsClick = !touchAsClick;
            Debug.LogFormat("Simulating touch with click {0}", touchAsClick ? "on" : "off");
        }
#endif

        bool shouldExit = Input.GetKeyUp(KeyCode.Escape);
        if(shouldExit) {
            Application.Quit();
        }

        // Update the controller's orientation and touch position

        bool inputAvailable = IsConnected() && !IsRecentering();

        if(inputAvailable) {
            UpdateControllerStates();
        }

        // Update the current state of the buttons.
        // Note that we don't use Input.GetButtonDown, GvrController.TouchDown etc.
        // and instead calculate our own wasPressed and wasReleased
        // to make it possible to have several inputs for the same button.

        bool buttonsAvailable = buttonsEnabled && inputAvailable;

        bool clickIsDown = buttonsAvailable && (
#if !RELEASE_BUILD
            (touchAsClick && GvrController.IsTouching) ||
#endif
            GvrController.ClickButton ||
            Input.GetKey(KeyCode.Space)
        );
        buttonStates[(int)Button.Click].UpdateState(clickIsDown);

        bool touchIsDown = buttonsAvailable && (
#if !RELEASE_BUILD
            Input.GetKey(KeyCode.RightControl) ||
            Input.GetKey(KeyCode.LeftControl) ||
#endif
#if UNITY_EDITOR
            // To avoid the "magic click" (click without touch) when using keyboard in editor
            Input.GetKey(KeyCode.Space) ||
#endif
            GvrController.IsTouching
        );
        buttonStates[(int)Button.Touch].UpdateState(touchIsDown);

        bool appIsDown = buttonsAvailable && (
            Input.GetKey(KeyCode.Backspace) ||
            GvrController.AppButton
        );
        buttonStates[(int)Button.App].UpdateState(appIsDown);

        //Debug.LogFormat("touchIsDown={0} clickIsDown={1} appIsDown={2} inputAvailable={3} buttonsAvailable={4}", touchIsDown, clickIsDown, appIsDown, inputAvailable, buttonsAvailable);
    }

    /// <summary>
    /// Check whether the given button is currently pressed.
    /// </summary>
    public bool ButtonIsDown(Button button) {
        return GetButtonState(button).isDown;
    }

    /// <summary>
    /// Check whether the given button was pressed since last frame.
    /// Only true for a single frame.
    /// Do not call from FixedUpdated or you may miss updates if it runs slower than the graphics frame rate.
    /// </summary>
    public bool ButtonWasPressed(Button button) {
        return GetButtonState(button).wasPressed;
    }

    /// <summary>
    /// Check whether the given button was released since last frame.
    /// Only true for a single frame.
    /// Do not call from FixedUpdated or you may miss updates if it runs slower than the graphics frame rate.
    /// </summary>
    public bool ButtonWasReleased(Button button) {
        return GetButtonState(button).wasReleased;
    }

    /// <summary>
    /// Get the state of the given button for the current frame.
    /// </summary>
    public ButtonState GetButtonState(Button button) {
        return buttonStates[(int)button];
    }

    public Vector2 GetTouchPosition() {
        return touchPos;
    }

    public Quaternion GetOrientation() {
        return orientation;
    }

    public void DisableInteraction() {
        buttonsEnabled = false;
    }

    public void EnableInteraction() {
        buttonsEnabled = true;
    }

    public bool IsConnected() {
        return GetStatus() == GvrConnectionState.Connected;
    }

    public bool IsRecentering() {
        return GvrController.Recentering;
    }

    public GvrConnectionState GetStatus() {
#if !RELEASE_BUILD
        if(forcedStatus.HasValue) {
            return forcedStatus.Value;
        }
#endif
        return GvrController.State;
    }

    /// <summary>
    /// Update touchPos and orientation from the controller, or keyboard input for non-release builds.
    /// </summary>
    private void UpdateControllerStates() {
        if(GvrController.State == GvrConnectionState.Connected) {
            touchPos = GvrController.TouchPos;
            orientation = GvrController.Orientation;
            return;
        }
#if !RELEASE_BUILD
        relativeOrientationEuler += Time.unscaledDeltaTime * relativeRotationSpeed * new Vector3(
            -Input.GetAxis("Vertical"),
            Input.GetAxis("Horizontal"),
            0
        );
        orientation = Quaternion.Euler(relativeOrientationEuler);
#endif
    }
}

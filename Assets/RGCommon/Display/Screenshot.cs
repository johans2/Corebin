using UnityEngine;
using System.Collections;

namespace RGCommon {
    public class Screenshot : MonoBehaviour {

        public KeyCode screenshotKey = KeyCode.C;
        public int superSize = 4;

        public void LateUpdate() {
            if(Input.GetKeyDown(screenshotKey)) {
                TakeScreenshot();
            }
        }

        public static string ScreenShotName() {
            return string.Format(
                "screen-{0}.png", 
                System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")
            );
        }

        public void TakeScreenshot() {
            string filename = ScreenShotName();
            Application.CaptureScreenshot(filename, superSize);
            Debug.Log("Screenshot captured: " + filename);
        }
    }
}

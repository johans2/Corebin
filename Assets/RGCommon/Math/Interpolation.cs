using UnityEngine;

namespace RGCommon {
    public static class Interpolation {
        /*
         * Calls Unitys Easin/Easout function with the progress, the amount of times defined in depth
         * progress should be between 0 and 1
         */
        public static float EaseInEaseOut(float progress, int depth) {
            float resultingValue = progress;
            while(depth > 1) {
                resultingValue = Mathf.SmoothStep(0.0f, 1.0f, resultingValue);
                depth--;
            }
            return resultingValue;
        }

        /**
         * Interpolates a value from 0 to 1 over a specific time.
         * Makes sure that the value reaches 1.0 before the loop is terminated.
         *
         * Use similar to this:
         *
         * Move 3 units up under 5 seconds.
         * float startTime = Time.time;
         * float progress = 0;
         * while(ProgressOverTime(startTime, 5, ref progress)) {
         *   // use progress, which is 0 <= progress <= 1.
         *   transform.position = Vector3.up * progress * 3;
         *   yield return null;  // or other way to wait for next frame
         * }
         */
        public static bool ProgressOverTime(float startTime, float totalTime, ref float progress) {
            if(progress >= 1.0f) {
                // Value has already been at 1.0 for one frame, so it's safe to stop now
                progress = 1.0f;
                return false;
            }
            progress = Mathf.Clamp01((Time.time - startTime) / totalTime);
            return true;
        }
    }
}

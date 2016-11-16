using UnityEngine;
using System.Text;
using System.Collections.Generic;

namespace RGCommon {
    /**
     * Accumulates numbers over one frame and then prints the result.
     * For example, it can be used for primitive profiling.
     * To see how often a line of code runs, do something like:
     * 
     * void MyFunction() {
     *     PerFrameCounter.Add("MyFunction");
     *     .
     *     .
     * }
     * 
     * This will show the number of times MyFunction has been called each frame.
     */
    public class PerFrameCounter : MonoBehaviour {

#if UNITY_EDITOR
        private static SortedDictionary<string, int> counters = new SortedDictionary<string, int>();

        public void Update() {
            if(counters.Count == 0) {
                return;
            }
            StringBuilder text = new StringBuilder("Counters:");
            foreach(KeyValuePair<string, int> entry in counters) {
                text.Append(' ');
                text.Append(entry.Key);
                text.Append(':');
                text.Append(entry.Value);
            }
            Debug.Log(text);
            counters.Clear();
        }
#endif

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void Add(string name, int amount = 1) {
#if UNITY_EDITOR
            if(counters.ContainsKey(name)) {
                counters[name] += amount;
            }
            else {
                counters[name] = amount;
            }
#endif
        }
    }
}

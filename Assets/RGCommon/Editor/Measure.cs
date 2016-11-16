using UnityEngine;
using UnityEditor;

namespace RGCommon {
    /// Editor utility for measuring the distance between two object.
    /// Select two objects, then select the menu Tools/Measure Distance.
    /// Outputs the information to the console.
    public class Measure : MonoBehaviour {
        [MenuItem("Tools/Measure Distance")]
        public static void MeasureDistance() {
            Transform[] transforms = Selection.transforms;
            if(transforms.Length != 2) {
                Debug.LogError("Please select two objects");
                return;
            }
            Transform t1 = transforms[0];
            Transform t2 = transforms[1];
            Debug.LogFormat(
                "Objects: {0} and {1}: distance={2}",
                t1.name, t2.name,
                Vector3.Distance(t1.position, t2.position)
            );
        }
    }
}

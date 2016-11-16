using UnityEngine;
using UnityEngine.UI;

namespace RGCommon {
    /// Shows the version number on the UI.Text or TextMesh
    /// component on this game object.
    public class VersionDisplay : MonoBehaviour {
        void Start() {
            string info = VersionInformation.Version;

            // TextMesh
            {
                TextMesh component = GetComponent<TextMesh>();
                if(component != null) {
                    component.text = info;
                }
            }

            // Text
            {
                Text component = GetComponent<Text>();
                if(component != null) {
                    component.text = info;
                }
            }
        }
    }
}

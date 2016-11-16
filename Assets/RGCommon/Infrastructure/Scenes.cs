using UnityEngine;
using UnityEngine.SceneManagement;

namespace RGCommon {
    /// <summary>
    /// Functionality for managing scenes.
    /// </summary>
    public static class Scenes {
        // <summary>
        // Unload all scenes except a specified.
        // The purpose is that you may have several scenes open in the editor
        // when you press play, but to run the game just as it would on device,
        // all scenes except the first one shouldn't be loaded.
        // </summary>
        public static void MakeSingleScene(Scene sceneToKeep) {
            int i = 0;
            while(i < SceneManager.sceneCount) {
                Scene s = SceneManager.GetSceneAt(i);
                if(s != sceneToKeep) {
                    Debug.LogFormat("Found scene {0} at boot - unloading it", s.name);
                    SceneManager.UnloadScene(s.name);
                }
                else {
                    ++i;
                }
            }
        }
    }
}
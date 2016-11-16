using UnityEngine;
using System.Collections;
using RGCommon;

/// Loads a new scene asynchronously.
/// If any Fader components are in the scene, they are faded out
/// before switching to the new scene.
public class Loader : MonoBehaviour {

    public string scene;
    public float minimumTime;

    IEnumerator Start() {
        Fader[] faders = Object.FindObjectsOfType<Fader>();
        Object.DontDestroyOnLoad(this.gameObject);
        float startTime = Time.time;

        if(faders.Length > 0) {
            bool allFadersDone = false;
            do {
                yield return null;
                allFadersDone = true;
                for(int i = 0; i < faders.Length; i++) {
                    allFadersDone = allFadersDone && faders[i].Done;
                }
            } while(!allFadersDone);
        }

        // If allowSceneActivation is false, the scene is never done loading.
        // So we check for progress >= 0.9 to see when it's time to fade out.
        // See discussion at http://answers.unity3d.com/questions/137261/loading-level-async-without-switching-the-level-im.html
        var async = Application.LoadLevelAsync(scene);
        async.allowSceneActivation = false;
        do {
            yield return null;
        } while(async.progress < 0.9f);

        float timeLeft = startTime + minimumTime - Time.time;
        //Debug.Log("Waiting " + timeLeft);
        yield return new WaitForSeconds(Mathf.Max(0, timeLeft));

        if(faders.Length > 0) {
            foreach(Fader theFader in faders) {
                theFader.FadeOut();
            }
            bool allFadersDone = false;
            do {
                yield return null;
                allFadersDone = true;
                for(int i = 0; i < faders.Length; i++) {
                    allFadersDone = allFadersDone && faders[i].Done;
                }
            } while(!allFadersDone);
        }

        // This will allow the scene to finish loading
        async.allowSceneActivation = true;
        Object.Destroy(this.gameObject);
    }
}

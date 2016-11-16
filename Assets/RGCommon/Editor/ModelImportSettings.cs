using UnityEngine;
using UnityEditor;
using System.IO;

/**
 * Disables importing materials in Unity editor.
 * Based on http://answers.unity3d.com/questions/491173/i-have-unchecked-import-materials-but-two-texture.html
 */
public class ModelImportSettings : AssetPostprocessor {
    void OnPreprocessModel() {
        ModelImporter importer = (ModelImporter)assetImporter;
        string metaFile = importer.assetPath + ".meta";
        if(!File.Exists(metaFile)) {
            Debug.Log("Modifying settings for new model " + importer.assetPath);
            importer.importMaterials = false;
        }
    }
}

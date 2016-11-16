using UnityEngine;
using UnityEditor;

public class BuildSystem {
    public static void BuildForGear() {
        SetVersion();
        PlayerSettings.productName = GetApplicationTitle();
        PlayerSettings.bundleIdentifier = GetPackageName();
        PlayerSettings.Android.useAPKExpansionFiles = !string.IsNullOrEmpty(System.Environment.GetEnvironmentVariable("SPLIT_BINARY"));
        SetDefines();
        ConfigureKeys();

        string filename = GetEnv("APK_FILE");
        Debug.LogFormat("Building {0} for Android...", IsRelease() ? "release" : "development build");
        BuildPipeline.BuildPlayer(GetScenes(), filename, BuildTarget.Android, BuildOptions.None);
        Debug.Log("Built: " + filename);
    }

    private static void SetVersion() {
        int major = GetIntEnv("MAJOR_VERSION");
        int minor = GetIntEnv("MINOR_VERSION");
        int build = GetIntEnv("BUILD_NUMBER");

        string version = major + "." + minor + "." + build;
        PlayerSettings.bundleVersion = version;
#if !UNITY_5
        PlayerSettings.shortBundleVersion = version;
#endif

        // The version code is what Android uses to determine if one
        // version is later than another.
        PlayerSettings.Android.bundleVersionCode = major * 10000000 + minor * 100000 + build;
    }

    private static void ConfigureKeys() {
        PlayerSettings.Android.keystoreName = System.Environment.GetEnvironmentVariable("ANDROID_KEYSTORE_NAME");
        PlayerSettings.Android.keystorePass = System.Environment.GetEnvironmentVariable("ANDROID_KEYSTORE_PASS");
        PlayerSettings.Android.keyaliasName = System.Environment.GetEnvironmentVariable("ANDROID_KEYALIAS_NAME");
        PlayerSettings.Android.keyaliasPass = System.Environment.GetEnvironmentVariable("ANDROID_KEYALIAS_PASS");
    }

    private static string GetApplicationTitle() {
        if(IsRelease()) {
            return GetEnv("PRODUCT_NAME");
        }
        else {
            // Prepend the build number to the application title.
            // We're prepending instead of appending so it's always visible.
            string build = GetEnv("BUILD_NUMBER");
            string name = GetEnv("PRODUCT_NAME");
            return build + " " + name;
        }
    }

    private static string GetPackageName() {
        if(IsRelease()) {
            return "com.resolutiongames." + GetEnv("PACKAGE_SUFFIX");
        }
        else {
            return "com.resolutiongames.dev." + GetEnv("PACKAGE_SUFFIX");
        }
    }

    private static bool IsRelease() {
        return System.Environment.GetEnvironmentVariable("RELEASE_BUILD") != null;
    }

    private static string GetEnv(string variable) {
        string v = System.Environment.GetEnvironmentVariable(variable);
        if(string.IsNullOrEmpty(v)) {
            throw new System.ApplicationException("Environment variable " + variable + " not set");
        }
        return v;
    }

    private static void SetDefines() {
        string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
        if(IsRelease()) {
            if(defines != "") {
                defines += ";";
            }
            if(IsRelease()) {
                defines += "RELEASE_BUILD";
            }
        }
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, defines);
    }

    private static int GetIntEnv(string variable) {
        string v = GetEnv(variable);
        int number;
        if(int.TryParse(v, out number)) {
            return number;
        }
        throw new System.ApplicationException(
            "Environment variable " + variable + " is not numeric: \"" + v + "\""
        );
    }

    private static string[] GetScenes() {
        string all = GetEnv("SCENES");
        return all.Split(' ');
    }

}

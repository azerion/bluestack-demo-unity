using UnityEditor;
using UnityEngine;
using UnityEditor.Build.Reporting;
using System.Text.RegularExpressions;
using System;
using System.Linq;
using UnityEditor.SearchService;

public class AzerionBuilder : MonoBehaviour
{
    private static string[] args;
    private static BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();

    [MenuItem("Azerion/Build/iOS")]
    public static void BuildIPA()
    {
        buildPlayerOptions.scenes = FetchScenes();

        buildPlayerOptions.target = BuildTarget.iOS;
        buildPlayerOptions.options = BuildOptions.None;

        buildPlayerOptions.locationPathName = "builds/ios";

        EditorUserBuildSettings.development = true;
        EditorUserBuildSettings.allowDebugging = true;

        PlayerSettings.iOS.appleEnableAutomaticSigning = true;
        PlayerSettings.iOS.appleDeveloperTeamID = "3DT94432UL";

        Build();
    }

    [MenuItem("Azerion/Build/APK")]
    public static void BuildAPK()
    {
        buildPlayerOptions.scenes = FetchScenes();

        buildPlayerOptions.target = BuildTarget.Android;
        buildPlayerOptions.options = BuildOptions.None;

        buildPlayerOptions.locationPathName = "builds/android/bluestack-demo.apk";
        
        PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;

        EditorUserBuildSettings.development = true;
        EditorUserBuildSettings.allowDebugging = true;
        
        PlayerSettings.Android.useCustomKeystore = false;

        Build();
    }

    private static void Build()
    {
        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
            EditorApplication.Exit(0);
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
            EditorApplication.Exit(1);
        }
    }

    // Helper function for getting the command line arguments
    private static string GetArg(string name)
    {
        args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i].Contains(name) && args.Length > i + 1)
            {
                return args[i + 1];
            }
        }

        return null;
    }

    private static string[] FetchScenes()
    {
        // Place all your scenes here
        string[] scenes = new string[EditorBuildSettings.scenes.Length];

        for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
        {
            scenes[i] = EditorBuildSettings.scenes[i].path;
        }

        return scenes;
    }
}
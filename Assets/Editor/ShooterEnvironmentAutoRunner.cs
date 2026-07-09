using System.IO;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class ShooterEnvironmentAutoRunner
{
    private const string FlagPath = "Temp/RebuildShooterCity.flag";
    private static bool waitingForPlayMode;
    private static double nextFlagCheckTime;

    static ShooterEnvironmentAutoRunner()
    {
        EditorApplication.update += PollForFlag;
        ShooterEditorVisualCleaner.Apply();

        // This flag lets an already-open Unity editor rebuild the saved scene after scripts reload.
        if (!File.Exists(GetFlagPath()))
        {
            return;
        }

        EditorApplication.delayCall += RunWhenReady;
    }

    private static void PollForFlag()
    {
        if (EditorApplication.timeSinceStartup < nextFlagCheckTime)
        {
            return;
        }

        nextFlagCheckTime = EditorApplication.timeSinceStartup + 2.0;
        if (File.Exists(GetFlagPath()))
        {
            RunWhenReady();
        }
    }

    private static void RunWhenReady()
    {
        if (!File.Exists(GetFlagPath()))
        {
            return;
        }

        if (EditorApplication.isPlaying || EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling)
        {
            waitingForPlayMode = true;
            EditorApplication.isPlaying = false;
            EditorApplication.update += WaitForEditor;
            return;
        }

        RunBuilder();
    }

    private static void WaitForEditor()
    {
        if (!waitingForPlayMode)
        {
            EditorApplication.update -= WaitForEditor;
            return;
        }

        if (EditorApplication.isPlaying || EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling)
        {
            return;
        }

        waitingForPlayMode = false;
        EditorApplication.update -= WaitForEditor;
        RunBuilder();
    }

    private static void RunBuilder()
    {
        EditorApplication.update -= PollForFlag;
        File.Delete(GetFlagPath());
        ShooterEnvironmentBuilder.BuildForBatchMode();
        Debug.Log("Shooter environment auto-run completed.");
        EditorApplication.update += PollForFlag;
    }

    private static string GetFlagPath()
    {
        string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        return Path.Combine(projectRoot, FlagPath);
    }
}

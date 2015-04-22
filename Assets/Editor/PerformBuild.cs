// C# example
using UnityEditor;
using System.IO;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System;

class PerformBuild
{
    static string[] GetBuildScenes()
    {
        List<string> names = new List<string>();

        foreach (EditorBuildSettingsScene e in EditorBuildSettings.scenes)
        {
            if (e == null)
                continue;

            if (e.enabled)
                names.Add(e.path);
        }
        return names.ToArray();
    }

    static string GetBuildPathAndroid()
    {
		return "C:\\WebServer\\teamsite\\build.apk";
    }

	static string GetBuildPathUnityWeb()
	{
		return "C:\\WebServer\\teamsite\\unity-web";
	}

	static string GetBuildPathStandaloneWindows()
	{
		return "C:\\WebServer\\teamsite\\standalonebuild\\build.exe";
	}

    [UnityEditor.MenuItem("CUSTOM/Test Command Line Build Step Android")]
    static void CommandLineBuildAndroid()
    {
		//System.IO.StreamWriter file = new System.IO.StreamWriter("c:\\buildlog.txt");

		//file.WriteLine("Command line build android version\n------------------\n------------------");

        string[] scenes = GetBuildScenes();
        string path = GetBuildPathAndroid();
        if (scenes == null || scenes.Length == 0 || path == null)
            return;

		//file.WriteLine (string.Format ("Path: \"{0}\"", path));
        for (int i = 0; i < scenes.Length; ++i)
        {
			//file.WriteLine(string.Format("Scene[{0}]: \"{1}\"", i, scenes[i]));
        }

		//file.WriteLine("Starting Android Build!");
        BuildPipeline.BuildPlayer(scenes, path, BuildTarget.Android, BuildOptions.None);
		//file.Close();
    }

	[UnityEditor.MenuItem("CUSTOM/Test Command Line Build Step Unity Web Player")]
	static void CommandLineBuildUnityWeb() {
		BuildPipeline.BuildPlayer (GetBuildScenes (), GetBuildPathUnityWeb (), BuildTarget.WebPlayer, BuildOptions.InstallInBuildFolder);
	}

	[UnityEditor.MenuItem("CUSTOM/Test Command Line Build Step Standalone Windows")]
	static void CommandLineBuildWindows() {
		BuildPipeline.BuildPlayer (GetBuildScenes (), GetBuildPathStandaloneWindows (), BuildTarget.StandaloneWindows, BuildOptions.None);
	}
}
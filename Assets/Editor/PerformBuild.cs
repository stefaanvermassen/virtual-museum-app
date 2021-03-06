// C# example
using UnityEditor;
using UnityEditor.Callbacks;
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

	static string GetBuildPathDevAndroid()
	{
		return "C:\\WebServer\\teamsite\\dev.apk";
	}

	static string GetBuildPathStandaloneWindows()
	{
		return "C:\\WebServer\\teamsite\\standalonebuild\\build.exe";
	}

	static string GetBuildPathStandaloneOSX()
	{
		return "C:\\WebServer\\teamsite\\osxbuild\\build.app";
	}

    [UnityEditor.MenuItem("CUSTOM/Test Command Line Build Step Android")]
    static void CommandLineBuildAndroid()
    {
		BuildPipeline.BuildPlayer(GetBuildScenes(), GetBuildPathAndroid(), BuildTarget.Android, BuildOptions.None);
    }

	[UnityEditor.MenuItem("CUSTOM/Test Command Line Build Step Standalone Windows")]
	static void CommandLineBuildWindows() {
		BuildPipeline.BuildPlayer (GetBuildScenes(), GetBuildPathStandaloneWindows(), BuildTarget.StandaloneWindows, BuildOptions.None);
	}

	[UnityEditor.MenuItem("CUSTOM/Test Command Line Build Step Standalone OSX")]
	static void CommandLineBuildOSX() {
		BuildPipeline.BuildPlayer (GetBuildScenes(), GetBuildPathStandaloneOSX(), BuildTarget.StandaloneOSXIntel, BuildOptions.None);
	}

	[UnityEditor.MenuItem("CUSTOM/Test Command Line Build Step Dev Android")]
	static void CommandLineBuildDevAndroid()
	{
		BuildPipeline.BuildPlayer(GetBuildScenes(), GetBuildPathDevAndroid(), BuildTarget.Android, BuildOptions.None);
	}

	/// <summary>
	/// Adds protocol installer to build
	/// </summary>
	/*[PostProcessBuild]
	public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
	{
		if(target == BuildTarget.StandaloneWindows || target == BuildTarget.StandaloneWindows64) {
			string path = pathToBuiltProject.Substring(0,pathToBuiltProject.LastIndexOf("/")) + "/protocol_installer.exe";
			if (File.Exists(path)) File.Delete(path);
			File.Copy(Application.dataPath + "/Editor/protocol_installer.exe", path); 
		}
	}*/
}
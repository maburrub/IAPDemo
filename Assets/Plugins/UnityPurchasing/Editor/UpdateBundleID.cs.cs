// C# example:
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

public class UpdateBundleID {
	[PostProcessBuildAttribute(1)]
	public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject) 
	{
		string filename = pathToBuiltProject + "/Contents/Info.plist";
		var fileContents = System.IO.File.ReadAllText(filename);
		fileContents = fileContents.Replace("unity.Unity3d.IAPDemo", "com.unity3d.unityiap.mac.demo"); 
		System.IO.File.WriteAllText(filename, fileContents);
	}
}
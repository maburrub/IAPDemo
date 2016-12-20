using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace UnityEditor.Purchasing
{


	static class UnityIAPInstaller
	{
		static readonly string k_InstallerFile = "Assets/Plugins/UnityPurchasing/Editor/UnityIAPInstaller.cs";
		static readonly string k_PackageFile = "Assets/Plugins/UnityPurchasing/UnityIAP.unitypackage";
		static readonly string k_PackageName = "Unity IAP";
		static readonly string k_HelpURL = "https://docs.unity3d.com/Manual/UnityIAPSettingUp.html";

		static readonly bool k_RunningInBatchMode = Environment.CommandLine.ToLower().Contains(" -batchmode");
		static readonly bool k_DieAfterUnityIAPInstall = Environment.CommandLine.ToLower().Contains(" -dieafterunityiapinstall");

		// File and directory list to be removed on any installation attempt. 
		// For previously released assets.
		// E.g.: 
		//   "Assets/Plugins/UnityPurchasing/AFile.cs",
		//   "Assets/Plugins/UnityPurchasing/RootDirectory",
		static readonly string[] k_LegacyFilesOrDirectories = { };
		// GUID list of files and directories to be removed on any installation attempt.
		// For previously released assets.
		// E.g.: 
		//   "3bc8ae8feaede4ad8a2ed815da096e44", // Assets/Plugins/UnityPurchasing/AFile.cs
		//   "152438a175713431396ce0941f312c78", // Assets/Plugins/UnityPurchasing/ADirectory
		static readonly string[] k_LegacyGUIDs = { };

		static readonly Type k_Analytics = (
			from assembly in AppDomain.CurrentDomain.GetAssemblies()
			from type in assembly.GetTypes()
			where type.Name == "Analytics" && type.GetMethods().Any(m => m.Name == "CustomEvent")
			select type).FirstOrDefault();

		static readonly Type k_Purchasing = (
			from assembly in AppDomain.CurrentDomain.GetAssemblies()
			from type in assembly.GetTypes()
			where type.Name == "UnityPurchasing" && type.GetMethods().Any(m => m.Name == "Initialize")
			select type).FirstOrDefault();

#if UNITY_5_3 || UNITY_5_3_OR_NEWER
		static readonly bool k_IsIAPSupported = true;
#else
		static readonly bool k_IsIAPSupported = false;
#endif

#if UNITY_5_5_OR_NEWER
		static readonly bool k_IsEditorSettingsSupported = true;
#else
		static readonly bool k_IsEditorSettingsSupported = false;
#endif

		enum ServiceType
		{
			Analytics,
			IAP,
		}

#if !DISABLE_UNITY_IAP_INSTALLER
		[Callbacks.DidReloadScripts]
#endif
		static void Install ()
		{
			EditorApplication.update += OnEditorUpdate;
			// EditorApplication.LockReloadAssemblies();

			// if (k_IsIAPSupported &&
			// 	AssetExists(k_PackageFile) &&
			// 	(k_Purchasing != null || EnableService(ServiceType.IAP, true)) &&
			// 	DeleteLegacyAssets(k_LegacyFilesOrDirectories, k_LegacyGUIDs))
			// {
			// 	AssetDatabase.ImportPackage(k_PackageFile, false);
			// }

			// AssetDatabase.DeleteAsset(k_PackageFile);
			// AssetDatabase.DeleteAsset(k_InstallerFile);

			// EditorApplication.UnlockReloadAssemblies();

			// AssetDatabase.Refresh();
			// SaveAssets();

			// if (k_RunningInBatchMode && k_DieAfterUnityIAPInstall)
			// {
			// 	// Debug.Log("Exiting after UnityIAPInstaller.Install complete");
			// 	// EditorApplication.Exit(0);

				Debug.Log("NOT Exiting after UnityIAPInstaller.Install complete");
			// 	// EditorApplication.update += OnEditorUpdate;

			// }
		}

		static void OnEditorUpdate()
		{
			Debug.Log("NICK: OnEditorUpdate");
		}


#if !DISABLE_UNITY_IAP_INSTALLER
		[Callbacks.DidReloadScripts(-1)]
#endif
		static void Snoopy ()
		{
			Debug.Log("NICK: Snoopy");
		}


#if !DISABLE_UNITY_IAP_INSTALLER
		[Callbacks.DidReloadScripts(0)]
#endif
		static void Zoopy ()
		{
			Debug.Log("NICK: Zoopy");
		}

#if !DISABLE_UNITY_IAP_INSTALLER
		[Callbacks.DidReloadScripts(1)]
#endif
		static void Poopy ()
		{
			Debug.Log("NICK: Poopy");
		}
#if !DISABLE_UNITY_IAP_INSTALLER
		[Callbacks.DidReloadScripts(1000)]
#endif
		static void Stinky ()
		{
			Debug.Log("NICK: Stinky");
		}

		static bool DisplayEnableServiceDialog (ServiceType serviceType)
		{
			return EditorUtility.DisplayDialog(
				serviceType + " Service is Disabled",
				"To avoid compiler errors, the " + serviceType + " service must be enabled first " +
				"before importing the latest " + k_PackageName + " asset package.\n\n" +
				"Would you like to enable the " + serviceType + " service now?",
				"Enable Now",
				"Cancel"
			);
		}

		static bool DisplayCanceledEnableServiceDialog (ServiceType serviceType)
		{
			return EditorUtility.DisplayDialog(
				"Import Canceled",
				"Please enable the " + serviceType + " service through the Services window " +
				"before attempting to import the latest " + k_PackageName + " asset package.",
				"OK",
				"Help..."
			);
		}

		static bool DisplayEnableServiceManuallyDialog (ServiceType serviceType)
		{
			return EditorUtility.DisplayDialog(
				serviceType + " Service is Disabled",
				"Please enable the " + serviceType + " service through the Services window. " +
				"Then re-import the latest " + k_PackageName + " asset package.",
				"OK",
				"Help..."
			);
		}

		static bool DisplayDeleteAssetsDialog ()
		{
			return EditorUtility.DisplayDialog(
				"Found Outdated Assets",
				"Assets from a previous version of the " + k_PackageName + " asset package " +
				"were found in the current project. Outdated assets must be deleted " +
				"before importing the latest version of the " + k_PackageName + " asset package.\n\n" +
				"Would you like to delete outdated assets now?",
				"Delete Now",
				"Cancel"
			);
		}

		static bool DisplayCanceledDeleteAssetsDialog ()
		{
			return EditorUtility.DisplayDialog(
				"Import Canceled",
				"Please delete any previously imported " + k_PackageName + " assets from your project " +
				"before attempting to re-import the latest " + k_PackageName + " asset package.",
				"OK"
			);
		}

		static bool EnableService (ServiceType serviceType, bool enabled)
		{
			if (!k_IsEditorSettingsSupported)
			{
				if (!DisplayEnableServiceManuallyDialog(serviceType))
				{
					Application.OpenURL(k_HelpURL);
				}

				return false;
			}

			if (DisplayEnableServiceDialog(serviceType))
			{
				switch (serviceType)
				{
				case ServiceType.Analytics:
#if UNITY_5_5_OR_NEWER
					Analytics.AnalyticsSettings.enabled = enabled;
#endif
					break;
				case ServiceType.IAP:
					if (k_Analytics == null && !EnableService(ServiceType.Analytics, true))
					{
						DisplayCanceledEnableServiceDialog(ServiceType.Analytics);
						return false;
					}
#if UNITY_5_5_OR_NEWER
					PurchasingSettings.enabled = enabled;
#endif
					break;
				}

				SaveAssets();
				return true;
			}

			if (!DisplayCanceledEnableServiceDialog(serviceType))
			{
				Application.OpenURL(k_HelpURL);
			}

			return false;
		}

		static bool AssetExists (string path)
		{
			if (path.Length > 7)
				path = path.Substring(7);
			else return false;

			if (Application.platform == RuntimePlatform.WindowsEditor)
			{
				path = path.Replace("/", @"\");
			}

			path = Path.Combine(Application.dataPath, path);

			return File.Exists(path) || Directory.Exists(path);
		}

		static bool AssetsExist (string[] legacyAssetPaths, string[] legacyAssetGUIDs, out string[] existingAssetPaths)
		{
			var paths = new List<string>();

			for (int i = 0; i < legacyAssetPaths.Length; i++)
			{
				if (AssetExists(legacyAssetPaths[i]))
				{
					paths.Add(legacyAssetPaths[i]);
				}
			}

			for (int i = 0; i < legacyAssetGUIDs.Length; i++)
			{
				string path = AssetDatabase.GUIDToAssetPath(legacyAssetGUIDs[i]);

				if (AssetExists(path) && !paths.Contains(path))
				{
					paths.Add(path);
				}
			}

			existingAssetPaths = paths.ToArray();

			return paths.Count > 0;
		}

		static bool DeleteLegacyAssets (string[] paths, string[] guids)
		{
			var assets = new string[0];

			if (!AssetsExist(paths, guids, out assets)) return true;

			if (DisplayDeleteAssetsDialog())
			{
				for (int i = 0; i < assets.Length; i++)
				{
					FileUtil.DeleteFileOrDirectory(assets[i]);
				}

				AssetDatabase.Refresh();
				SaveAssets();
				return true;
			}

			DisplayCanceledDeleteAssetsDialog();
			return false;
		}

		static void SaveAssets ()
		{
#if UNITY_5_5_OR_NEWER
			AssetDatabase.SaveAssets(); // Not reliable.
#else
			EditorApplication.SaveAssets(); // Reliable, but removed in Unity 5.5.
#endif
		}
	}
}

#if UNITY_5_6_OR_NEWER && !UNITY_5_6_0
using UnityEngine;
using UnityEngine.Store;

namespace AppStoresSupport
{
    [System.Serializable]
    public class AppStoreSetting 
    {
        public string AppID = "";
        public string AppKey = "";
        public bool IsTestMode = false;
    }

    [CreateAssetMenu(fileName = "Assets/Plugins/UnityChannel/XiaomiSupport/Resources/AppStoreSettings.asset", menuName = "App Store Settings")]
    [System.Serializable]
    public class AppStoreSettings : ScriptableObject
    {
        public string UnityClientID = "";
        public string UnityClientKey = "";
        public string UnityClientRSAPublicKey = "";

        public AppStoreSetting XiaomiAppStoreSetting = new AppStoreSetting();
		
		public AppInfo getAppInfo() {
			AppInfo appInfo = new AppInfo();
			appInfo.clientId = UnityClientID;
			appInfo.clientKey = UnityClientKey;
			appInfo.appId = XiaomiAppStoreSetting.AppID;
			appInfo.appKey = XiaomiAppStoreSetting.AppKey;
			appInfo.debug = XiaomiAppStoreSetting.IsTestMode;
			return appInfo;
		}
    }
}
#endif

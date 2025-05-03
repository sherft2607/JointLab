namespace Oculus.Platform
{
    using UnityEngine;
    using System.Collections;

#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    /**
     * This class contains the configurations for the platform settings of a Model.Application. To modify the values, there is an editor accessible from the menu bar via: Oculus Platform -> Edit Settings. This is important for initializing the Platform SDK.
    */
    public sealed class PlatformSettings : ScriptableObject
    {
        /**
        * This is the ID that belongs to your Models.Application. You can find this on the Oculus Dashboard. This is will be used to build the rift/windows target.
        */
        public static string AppID
        {
            get { return Instance.ovrAppID; }
            set { Instance.ovrAppID = value; }
        }

        /**
        * This is the ID that belongs to your Models.application. You can find this on the Oculus Dashboard. This will be used to build the android target.
        */
        public static string MobileAppID
        {
            get { return Instance.ovrMobileAppID; }
            set { Instance.ovrMobileAppID = value; }
        }

        /**
        * This is a flag that determines whether your app will build using the Oculus Platform or a debug platform.
        */
        public static bool UseStandalonePlatform
        {
            get { return Instance.ovrUseStandalonePlatform; }
            set { Instance.ovrUseStandalonePlatform = value; }
        }

        /**
        * This is a flag that determines whethe Platform SDK will use the mobile app id instead of the rift app id in the editor.
        */
        public static bool UseMobileAppIDInEditor
        {
            get { return Instance.ovrUseMobileAppIDInEditor; }
            set { Instance.ovrUseMobileAppIDInEditor = value; }
        }

        [SerializeField]
        private string ovrAppID = "";

        [SerializeField]
        private string ovrMobileAppID = "";

        [SerializeField]
        private bool ovrUseMobileAppIDInEditor = false;

#if UNITY_EDITOR_WIN
        [SerializeField]
        private bool ovrUseStandalonePlatform = false;
#else
        [SerializeField]
        private bool ovrUseStandalonePlatform = true;
#endif

        private static PlatformSettings instance;

        public static PlatformSettings Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load<PlatformSettings>("OculusPlatformSettings");

                    // This can happen if the developer never input their App Id into the Unity Editor
                    // and therefore never created the OculusPlatformSettings.asset file
                    // Use a dummy object with defaults for the getters so we don't have a null pointer exception
                    if (instance == null)
                    {
                        instance = ScriptableObject.CreateInstance<PlatformSettings>();

#if UNITY_EDITOR
                        // Only in the editor should we save it to disk
                        string properPath = System.IO.Path.Combine(UnityEngine.Application.dataPath, "Resources");
                        if (!System.IO.Directory.Exists(properPath))
                        {
                            UnityEditor.AssetDatabase.CreateFolder("Assets", "Resources");
                        }

                        string fullPath = System.IO.Path.Combine(
                            System.IO.Path.Combine("Assets", "Resources"),
                            "OculusPlatformSettings.asset"
                        );
                        UnityEditor.AssetDatabase.CreateAsset(instance, fullPath);
#endif
                    }
                }

                return instance;
            }

            set { instance = value; }
        }
    }
}

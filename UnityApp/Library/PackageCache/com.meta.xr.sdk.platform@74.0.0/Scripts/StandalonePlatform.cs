namespace Oculus.Platform
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    /// Standalone mode allows you to initialize the Meta Platform SDK in test and development environments
    /// It is useful for testing Matchmaking where you can run multiple apps on the same box to test your integration.
    /// See more information here: https://developer.oculus.com/documentation/unity/ps-setup/#use-the-platform-in-standalone-mode.
    public sealed class StandalonePlatform
    {
        /// This is a delegate function that allows you to receive log message from the platform.
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void UnityLogDelegate(IntPtr tag, IntPtr msg);

        /// It initializes the platform in standalone mode within the Unity Editor.
        /// It checks for the presence of an App ID and access token, and throws an exception if either is missing.
        /// If the necessary credentials are present, it calls the AsyncInitialize() function to perform the actual initialization.
        public Request<Models.PlatformInitialize> InitializeInEditor()
        {
#if UNITY_ANDROID
            if (String.IsNullOrEmpty(PlatformSettings.MobileAppID))
            {
                throw new UnityException("Update your App ID by selecting 'Oculus Platform' -> 'Edit Settings'");
            }

            var appID = PlatformSettings.MobileAppID;
#else
            var appID = "";
            if (PlatformSettings.UseMobileAppIDInEditor)
            {
                appID = PlatformSettings.MobileAppID;
            }
            else
            {
                appID = PlatformSettings.AppID;
            }
            if (String.IsNullOrEmpty(appID))
            {
                throw new UnityException("Update your App ID by selecting 'Oculus Platform' -> 'Edit Settings'");
            }
#endif
            if (String.IsNullOrEmpty(StandalonePlatformSettings.OculusPlatformTestUserAccessToken))
            {
                throw new UnityException(
                    "Update your standalone credentials by selecting 'Oculus Platform' -> 'Edit Settings'");
            }

            var accessToken = StandalonePlatformSettings.OculusPlatformTestUserAccessToken;

            return AsyncInitialize(UInt64.Parse(appID), accessToken);
        }
        /// It initializes the platform in standalone mode with the provided App ID and access token.
        ///It resets the test platform, initializes the globals, and then calls CAPI.ovr_PlatformInitializeWithAccessToken() to perform the actual initialization.
        public Request<Models.PlatformInitialize> AsyncInitialize(ulong appID, string accessToken)
        {
            CAPI.ovr_UnityResetTestPlatform();
            CAPI.ovr_UnityInitGlobals(IntPtr.Zero);

            return new Request<Models.PlatformInitialize>(
                CAPI.ovr_PlatformInitializeWithAccessToken(appID, accessToken));
        }

        /// It initializes the platform in standalone mode with the provided App ID, access token,
        /// and initialization options.
        public Request<Models.PlatformInitialize> AsyncInitializeWithAccessTokenAndOptions(string appId,
            string accessToken, Dictionary<InitConfigOptions, bool> initConfigOptions)
        {
            var configCount = (UIntPtr)initConfigOptions.Count;
            var configPairs = CAPI.DictionaryToOVRKeyValuePairs(initConfigOptions);
            return new Request<Models.PlatformInitialize>(
                CAPI.ovr_PlatformInitializeWithAccessTokenAndOptions(UInt64.Parse(appId), accessToken, configPairs,
                    configCount));
        }
    }
}

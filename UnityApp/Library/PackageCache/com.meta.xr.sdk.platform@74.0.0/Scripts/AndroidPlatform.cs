namespace Oculus.Platform
{
    using UnityEngine;
    using System.Collections;
    using System;

    /// The AndroidPlatform class provides methods for initializing the Meta Platform SDK features with an app ID.
    /// It offers two initialization functions: initialize and asyncInitialize.
    /// It is recommended to use the asynchronous method (asyncInitialize) for better app performance and less state management, especially on mobile devices.
    /// See more information here: https://developer.oculus.com/documentation/unity/ps-setup/.
    public class AndroidPlatform
    {
        /// This method initializes the platform features synchronously, running on the thread you initialize on. It takes an app ID as a parameter.
        /// It checks for the presence of an App ID, and throws an exception if it is missing.
        /// If the necessary credentials are present, it calls the CAPI.ovr_UnityInitWrapper() function to perform the actual initialization.
        public bool Initialize(string appId)
        {
#if UNITY_ANDROID
            if (String.IsNullOrEmpty(appId))
            {
                throw new UnityException("AppID must not be null or empty");
            }

            return CAPI.ovr_UnityInitWrapper(appId);
#else
            return false;
#endif
        }

        /// This method initializes the platform features asynchronously, allowing you to perform other functions,
        /// including calls to the Platform SDK, concurrently while the SDK is initializing. It also takes an app ID as a parameter.
        /// It checks for the presence of an App ID, and throws an exception if it is missing.
        /// If the necessary credentials are present, it calls the CAPI.ovr_UnityInitWrapper() function to perform the actual initialization.
        public Request<Models.PlatformInitialize> AsyncInitialize(string appId)
        {
#if UNITY_ANDROID
            if (String.IsNullOrEmpty(appId))
            {
                throw new UnityException("AppID must not be null or empty");
            }

            return new Request<Models.PlatformInitialize>(CAPI.ovr_UnityInitWrapperAsynchronous(appId));
#else
            return new Request<Models.PlatformInitialize>(0);
#endif
        }
    }
}

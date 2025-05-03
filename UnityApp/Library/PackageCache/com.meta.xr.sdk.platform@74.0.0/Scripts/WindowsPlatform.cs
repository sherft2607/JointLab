namespace Oculus.Platform
{
    using UnityEngine;
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;

    /// Object that has ability of initializing the Platform SDK. This initialization is specifically for the Windows operating
    /// system and unity development. You can get more information about the initializing the Platform SDK in our [docs](https://developer.oculus.com/documentation/unity/ps-setup/).
    public class WindowsPlatform
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void UnityLogDelegate(IntPtr tag, IntPtr msg);

        void CPPLogCallback(IntPtr tag, IntPtr message)
        {
            Debug.Log(string.Format("{0}: {1}", Marshal.PtrToStringAnsi(tag), Marshal.PtrToStringAnsi(message)));
        }

        IntPtr getCallbackPointer()
        {
            //UnityLogDelegate callback_delegate = new UnityLogDelegate(CPPLogCallback);
            //IntPtr intptr_delegate = Marshal.GetFunctionPointerForDelegate(callback_delegate);
            return IntPtr.Zero;
        }

        /// Takes a string representing the app ID of the Application for initialization. An invalid app ID will result in a unity exception. Upon success, will return true.
        public bool Initialize(string appId)
        {
            if (String.IsNullOrEmpty(appId))
            {
                throw new UnityException("AppID must not be null or empty");
            }

            try
            {
                CAPI.ovr_UnityInitWrapperWindows(appId, getCallbackPointer());
            }
            catch (DllNotFoundException e)
            {
                Debug.LogWarning("Oculus Platform Runtime was not found. Please ensure that the Oculus PC App is installed and up-to-date.");
                throw e;
            }

            return true;
        }

        /// Takes a string representing the app ID of the Application for initialization. This is an asynchronous call and will return a request of type Models.PlatformInitialize.
        public Request<Models.PlatformInitialize> AsyncInitialize(string appId)
        {
            if (String.IsNullOrEmpty(appId))
            {
                throw new UnityException("AppID must not be null or empty");
            }

            try
            {
                return new Request<Models.PlatformInitialize>(
                    CAPI.ovr_UnityInitWrapperWindowsAsynchronous(appId, getCallbackPointer()));
            }
            catch (DllNotFoundException e)
            {
                Debug.LogWarning("Oculus Platform Runtime was not found. Please ensure that the Oculus PC App is installed and up-to-date.");
                throw e;
            }
        }
    }
}

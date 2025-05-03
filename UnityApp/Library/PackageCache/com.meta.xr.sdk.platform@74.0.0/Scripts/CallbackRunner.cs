using System.Runtime.InteropServices;
using UnityEngine;

namespace Oculus.Platform
{
    /// This class, CallbackRunner, is a MonoBehaviour that manages callbacks for the Meta Quest Platform.
    /// It ensures that only one instance of the class exists in the scene and persists between scene loads if specified.
    /// The class also updates the callbacks: CallbackRunner.Update() on each frame and resets the test platform on destruction.
    public class CallbackRunner : MonoBehaviour
    {
        /// This is a DllImport attribute that imports the ovr_UnityResetTestPlatform function from the CAPI.DLL_NAME library.
        [DllImport(CAPI.DLL_NAME)]
        static extern void ovr_UnityResetTestPlatform();

        /// This is a public boolean variable that determines whether the CallbackRunner instance should persist between scene loads.
        /// The default value is true, and it is used in the CallbackRunner.Awake() method.
        public bool IsPersistantBetweenSceneLoads = true;

        /// This is the Awake method, which is called when the script is initialized.
        /// It checks for existing instances of the CallbackRunner script and warns the user if there are multiple instances.
        /// If the IsPersistantBetweenSceneLoads variable is true, it sets the game object to not be destroyed on load.
        void Awake()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            var existingCallbackRunner = FindObjectOfType<CallbackRunner>();
#pragma warning restore CS0618 // Type or member is obsolete
            if (existingCallbackRunner != this)
            {
                Debug.LogWarning("You only need one instance of CallbackRunner");
            }

            if (IsPersistantBetweenSceneLoads)
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        /// This is the Update method, which is called every frame.
        /// It runs the callbacks using the Request.RunCallbacks() method.
        void Update()
        {
            Request.RunCallbacks();
        }

        /// This is the OnDestroy method, which is called when the script is destroyed.
        /// It resets the test platform using the ovr_UnityResetTestPlatform() function.
        void OnDestroy()
        {
#if UNITY_EDITOR
            ovr_UnityResetTestPlatform();
#endif
        }

        /// This is the OnApplicationQuit method, which is called when the application quits.
        /// It calls the Callback.OnApplicationQuit() method to handle the quit event.
        void OnApplicationQuit()
        {
            Callback.OnApplicationQuit();
        }
    }
}

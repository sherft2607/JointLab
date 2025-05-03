namespace Oculus.Platform
{
    using System.Text.RegularExpressions;
    /// Represents standalone platform settings in the Unity editor.These settings are used for StandalonePlatform initialization.
    /// It contains properties and methods to manage test user credentials such as email, password, and access token.
    public sealed class StandalonePlatformSettings
    {
#if UNITY_EDITOR
        private static string _OculusPlatformTestUserPassword = "";
        private static string[] ProjectPath = UnityEngine.Application.dataPath.Split('/');
        // Only keeping alphanumeric, -, _, from the project directory name just in case
        private static string _ProjectName = Regex.Replace(ProjectPath[ProjectPath.Length - 2], @"[^a-zA-Z0-9_-]", string.Empty);

        private static void ClearOldStoredPassword()
        {
          // Ensure that we are not storing the old passwords anywhere on the machine
          string key = "OculusStandaloneUserPassword_" + _ProjectName;
          if (UnityEditor.EditorPrefs.HasKey(key))
            {
                UnityEditor.EditorPrefs.SetString(key, "0000");
                UnityEditor.EditorPrefs.DeleteKey(key);
            }
        }
#endif

        /// Represents the test user's email address for the Unity Editor settings. It has two methods to get and set the email address of the test user.
        public static string OculusPlatformTestUserEmail
        {
            get
            {
#if UNITY_EDITOR
                return UnityEditor.EditorPrefs.GetString("OculusStandaloneUserEmail_" + _ProjectName);
#else
                return string.Empty;
#endif
            }
            set
            {
#if UNITY_EDITOR
                UnityEditor.EditorPrefs.SetString("OculusStandaloneUserEmail_" + _ProjectName, value);
#endif
            }
        }
        /// Represents the test user's password for the Unity Editor settings. It has two methods to get and set the password of the test user.
        public static string OculusPlatformTestUserPassword
        {
            get
            {
#if UNITY_EDITOR
                ClearOldStoredPassword();
                return _OculusPlatformTestUserPassword;
#else
                return string.Empty;
#endif
            }
            set
            {
#if UNITY_EDITOR
                ClearOldStoredPassword();
                _OculusPlatformTestUserPassword = value;
#endif
            }
        }
        /// Represents the test user's access token for the Unity Editor settings. It has two methods to get and set the access token of the test user. It is used by StandalonePlatform.InitializeInEditor().
        public static string OculusPlatformTestUserAccessToken
        {
            get
            {
#if UNITY_EDITOR
                return UnityEditor.EditorPrefs.GetString("OculusStandaloneUserAccessToken_" + _ProjectName);
#else
                return string.Empty;
#endif
            }
            set
            {
#if UNITY_EDITOR
                UnityEditor.EditorPrefs.SetString("OculusStandaloneUserAccessToken_" + _ProjectName, value);
#endif
            }
        }
    }
}

/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
 * you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https://developer.oculus.com/licenses/oculussdk/
 *
 * Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace Oculus.Interaction.Editor
{
    public static class OpenXRUpgrade
    {
        public static bool UpgradeAvailable => !HasDefineSymbol(DEFINE_SYMBOL, _buildTargets);

        private const string DEFINE_SYMBOL = "ISDK_OPENXR_HAND";

        private static readonly BuildTargetGroup[] _buildTargets =
            { BuildTargetGroup.Android, BuildTargetGroup.Standalone };

        public static event Action OnUpdate;
        public static void UpgradeToOpenXR()
        {
            Debug.Log($"OpenXR Upgrade: Adding script define symbol '{DEFINE_SYMBOL}' to build targets '{String.Join(", ", _buildTargets)}'");
            SetDefineSymbol(DEFINE_SYMBOL, _buildTargets);
            OnUpdate?.Invoke();
            EditorApplication.ExecuteMenuItem("File/Save Project");
        }

        private static void SetDefineSymbol(string symbol, params BuildTargetGroup[] targetGroups)
        {
            foreach (var targetGroup in targetGroups)
            {
#pragma warning disable CS0618 // Type or member is obsolete
                PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup, out string[] defines);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup,
                    defines.Union(new[] { symbol }).ToArray());
#pragma warning restore CS0618 // Type or member is obsolete
            }
        }

        private static bool HasDefineSymbol(string symbol, params BuildTargetGroup[] targetGroups)
        {
            bool result = true;
            foreach (var targetGroup in targetGroups)
            {
#pragma warning disable CS0618 // Type or member is obsolete
                PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup, out string[] defines);
#pragma warning restore CS0618 // Type or member is obsolete
                result &= defines.Contains(symbol);
            }
            return result;
        }
    }

    internal class OpenXRUpgradeWindow : EditorWindow
    {
        public enum Response
        {
            None,
            Ignore,
            AskLater,
            Upgrade,
        }

        public event Action<Response> WhenResponded = delegate { };

        private static ISDKEditorStyles _styles;

        public static OpenXRUpgradeWindow ShowWindow()
        {
            var curWindow = GetWindow<OpenXRUpgradeWindow>(true);
            return curWindow;
        }

        public static string GetIconPath(string iconName)
        {
            var g = AssetDatabase.FindAssets($"t:Script {nameof(PackageUpgradeOpenXR)}");
            return Path.Combine(Path.GetDirectoryName(AssetDatabase.GUIDToAssetPath(g[0])), "Icons/", iconName);
        }

        private void Awake()
        {
            _styles ??= new ISDKEditorStyles();
            titleContent = new GUIContent("Interaction SDK OpenXR Upgrade");
            minSize = new Vector2(640, 340);
            maxSize = minSize + new Vector2(2, 2);
            EditorApplication.delayCall += () => maxSize = new Vector2(4000, 4000);
        }

        private void OnGUI()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                CloseWindow();
            }

            using (new EditorGUILayout.VerticalScope(_styles.WindowContents))
            {
                DrawTitle();
                EditorGUILayout.Space(8);
                DrawContent();
                GUILayout.FlexibleSpace();
                EditorGUILayout.Space(8);
                DrawFooter();
            }
        }

        private void DrawTitle()
        {
            _styles.DrawTitle("Interaction SDK OpenXR Hand Skeleton Upgrade");
        }

        private void DrawContent()
        {
            string subtitle =
                    "As of version 71, Interaction SDK begins deprecating the OVR hand skeleton in favor of the OpenXR skeleton. " +
                    "Support for the OVR skeleton will be discontinued in early 2025." +
                    "\n\n" +
                    "<b>New projects are strongly encouraged to use the OpenXR Hand Skeleton.</b>" +
                    "\n" +
                    "Performing the upgrade will set the scripting define ISDK_OPENXR_HAND in your player settings, and change the HandSkeletonVersion " +
                    "in OVRRuntimeSettings to OVRHandSkeletonVersion.OpenXR." +
                    "\n\n" +
                    "<b>Existing projects may continue using the OVR Hand Skeleton through the end of 2024.</b>" +
                    "\n" +
                    "While upgrading existing projects is not officially supported, a number of upgrade tools do exist for built-in components. " +
                    "If you do upgrade an existing project, ensure you have a backup of your project in case you encounter unexpected behavior.";

            _styles.DrawSubtitle(subtitle, true);
        }

        private void DrawFooter()
        {
            using (new EditorGUILayout.VerticalScope(GUILayout.Height(36)))
            {
                using (new GUILayout.HorizontalScope())
                {
                    bool upgrade = GUILayout.Button("Use OpenXR Hand\n<size=11>(New Projects)</size>", _styles.Button);
                    bool hide = GUILayout.Button("Keep Using OVR Hand\n<size=11>(Existing Projects)</size>", _styles.Button);
                    bool defer = GUILayout.Button("Remind Me Later", _styles.Button);

                    if (upgrade)
                    {
                        WhenResponded.Invoke(Response.Upgrade);
                        CloseWindow();
                    }
                    else if (defer)
                    {
                        WhenResponded.Invoke(Response.AskLater);
                        CloseWindow();
                    }
                    else if (hide)
                    {
                        WhenResponded.Invoke(Response.Ignore);
                        CloseWindow();
                    }
                }
            }
        }

        private void CloseWindow()
        {
            Close();
        }
    }

    [InitializeOnLoad]
    internal class PackageUpgradeOpenXR
    {
        private const string MENU_NAME = "Meta/Interaction/Upgrade to OpenXR Hand Skeleton";
        private const string ESSENTIALS_PACKAGE = "com.meta.xr.sdk.interaction";

        private const string KEY_BASE = "Oculus_Interaction_PackageUpgradeOpenXR";
        private const string KEY_DONTASK = KEY_BASE + "_DontAsk";
        private const string KEY_ASKLATER = KEY_BASE + "_AskLater";

        static PackageUpgradeOpenXR()
        {
            Events.registeredPackages += HandlePackagesChanged;
            EditorApplication.delayCall += ShowUpgradeWindowInternal;
        }

        [MenuItem(MENU_NAME)]
        public static void ShowUpgradeWindow()
        {
            EditorPrefs.SetBool(KEY_DONTASK, false);
            SessionState.SetBool(KEY_ASKLATER, false);
            ShowUpgradeWindowInternal();
        }

        [MenuItem(MENU_NAME, isValidateFunction: true)]
        public static bool ValidateShowUpgradeWindow()
        {
            return OpenXRUpgrade.UpgradeAvailable &&
                !EditorApplication.isPlayingOrWillChangePlaymode;
        }

        private static void HandlePackagesChanged(PackageRegistrationEventArgs args)
        {
            Func<PackageInfo, bool> nameMatches =
                pkg => pkg.name == ESSENTIALS_PACKAGE;

            if (args.changedFrom.Any(nameMatches) ||
                args.changedTo.Any(nameMatches) ||
                args.added.Any(nameMatches))
            {
                // Clear all suppressed notifications if ISDK package is changed
                EditorPrefs.SetBool(KEY_DONTASK, false);
                SessionState.SetBool(KEY_ASKLATER, false);
                ShowUpgradeWindowInternal();
            }
        }

        private static void ShowUpgradeWindowInternal()
        {
            if (Application.isBatchMode ||
                EditorApplication.isPlayingOrWillChangePlaymode ||
                !OpenXRUpgrade.UpgradeAvailable ||
                EditorPrefs.GetBool(KEY_DONTASK, false) ||
                SessionState.GetBool(KEY_ASKLATER, false))
            {
                return;
            }

            CloseOpenWindows();
            OpenXRUpgradeWindow window = OpenXRUpgradeWindow.ShowWindow();
            window.WhenResponded += HandleResponse;

            void HandleResponse(OpenXRUpgradeWindow.Response response)
            {
                window.WhenResponded -= HandleResponse;
                switch (response)
                {
                    default:
                    case OpenXRUpgradeWindow.Response.None:
                        break;
                    case OpenXRUpgradeWindow.Response.Ignore:
                        EditorPrefs.SetBool(KEY_DONTASK, true);
                        ShowReminder("Upgrade prompt suppressed for this version of Interaction SDK.");
                        break;
                    case OpenXRUpgradeWindow.Response.AskLater:
                        SessionState.SetBool(KEY_ASKLATER, true);
                        ShowReminder("Upgrade prompt suppressed for the current editor session.");
                        break;
                    case OpenXRUpgradeWindow.Response.Upgrade:
                        OpenXRUpgrade.UpgradeToOpenXR();
                        break;
                }
            }
        }

        private static void ShowReminder(string message)
        {
            EditorUtility.DisplayDialog("ISDK OpenXR Upgrade",
                message + "\n\n" +
                "You can open the Interaction SDK OpenXR upgrade window at any time using the menu item:\n\n" +
                MENU_NAME, "Ok");
        }

        private static void CloseOpenWindows()
        {
            if (EditorWindow.HasOpenInstances<OpenXRUpgradeWindow>())
            {
                EditorWindow.GetWindow<OpenXRUpgradeWindow>()?.Close();
            }
        }
    }

    internal class LogSkeletonVersion : IPreprocessBuildWithReport
    {
#if ISDK_OPENXR_HAND
        private static readonly bool IsOpenXR = true;
#else
        private static readonly bool IsOpenXR = false;
#endif

        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            LogHandSkeletonVersion();
        }

        private static void LogHandSkeletonVersion()
        {
            string version = IsOpenXR ? "OpenXR" : "OVR";
            Debug.Log($"[ISDK] Hand Skeleton Version is set to {version}");
        }
    }
}

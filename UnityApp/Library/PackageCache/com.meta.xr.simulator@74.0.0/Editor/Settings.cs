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

using UnityEngine;
using UnityEditor;

using Meta.XR.Simulator.Editor.SyntheticEnvironments;

namespace Meta.XR.Simulator.Editor
{
    [InitializeOnLoad]
    internal static class Settings
    {
        private const string LastEnvironmentKey = Utils.PackageName + ".lastEnvironment_key";

        internal static string LastEnvironment
        {
            get => EditorPrefs.GetString(LastEnvironmentKey, "LivingRoom");
            set => EditorPrefs.SetString(LastEnvironmentKey, value);
        }

        private const string AutomaticServersKey = Utils.PackageName + ".automaticservers_key";

        internal static bool AutomaticServers
        {
            get => EditorPrefs.GetBool(AutomaticServersKey, true);
            set => EditorPrefs.SetBool(AutomaticServersKey, value);
        }

        private const string DisplayServersKey = Utils.PackageName + ".displayservers_key";
        internal static bool DisplayServers
        {
            get => EditorPrefs.GetBool(DisplayServersKey, false);
            set => EditorPrefs.SetBool(DisplayServersKey, value);
        }

        static Settings()
        {
#if META_XR_SDK_CORE_SUPPORTS_TOOLBAR
            OVRUserSettingsProvider.Register("Simulator", OnSettingsGUI);
#endif
        }

        private static void OnSettingsGUI()
        {
            // Enable Toggle
            EditorGUI.BeginChangeCheck();
            var enable = EditorGUILayout.Toggle(new GUIContent("Enable", "Set Play mode to use Meta XR Simulator"),
                Enabler.Activated);
            if (EditorGUI.EndChangeCheck())
            {
                if (enable)
                {
                    Enabler.ActivateSimulator(false, Utils.Origins.Settings);
                }
                else
                {
                    Enabler.DeactivateSimulator(false, Utils.Origins.Settings);
                }
            }

            {
                // Automatic Synthetic Environment Toggle
                EditorGUI.BeginChangeCheck();
                var value = EditorGUILayout.Toggle(
                    new GUIContent("Automatically Start/Stop Servers",
                        "Whether or not the Synthetic Environment Servers and the Local Sharing Servers are started and stopped when entering and exiting Play Mode."),
                    AutomaticServers);
                if (EditorGUI.EndChangeCheck())
                {
                    AutomaticServers = value;
                }
            }

            {
                // Display Synthetic Environment Window
                EditorGUI.BeginChangeCheck();
                var value = EditorGUILayout.Toggle(
                    new GUIContent("Display Servers",
                        "Whether or not the Synthetic Environment Server and the Local Sharing Server are displayed or hidden."),
                    DisplayServers);
                if (EditorGUI.EndChangeCheck())
                {
                    DisplayServers = value;
                }
            }

            {
                if (!Installer.IsInstalled)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Synthetic Environments", GUILayout.Width(EditorGUIUtility.labelWidth));
                    if (GUILayout.Button("Install Synthetic Environments"))
                    {
                        Installer.InstallSESPackage();
                    }
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    // Preferred Environment Dropdown
                    Registry.RefreshNames();
                    var selectedRoom = Registry.GetByInternalName(LastEnvironment);
                    var selectedIndex = selectedRoom?.Index ?? 0;
                    EditorGUI.BeginChangeCheck();
                    var index = EditorGUILayout.Popup("Selected Environment", selectedIndex, Registry.Names);
                    if (EditorGUI.EndChangeCheck())
                    {
                        selectedRoom = Registry.GetByIndex(index);
                        LastEnvironment = selectedRoom.InternalName;
                    }
                }
            }
        }
    }
}

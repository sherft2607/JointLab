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
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using static Meta.XR.Simulator.Utils;
using Menu = UnityEditor.Menu;

namespace Meta.XR.Simulator.Editor
{
    /// <summary>
    /// Static Helper that controls activation and deactivation of Meta XR Simulator.
    /// </summary>
    /// <seealso cref="Activated"/>
    /// <seealso cref="ActivateSimulator"/>
    /// <seealso cref="DeactivateSimulator"/>
    /// <seealso cref="ToggleSimulator"/>
    [InitializeOnLoad]
    public static class Enabler
    {
        private const string OpenXrRuntimeEnvKey = "XR_RUNTIME_JSON";
        private const string PreviousOpenXrRuntimeEnvKey = "XR_RUNTIME_JSON_PREV";
        private const string OpenXrSelectedRuntimeEnvKey = "XR_SELECTED_RUNTIME_JSON";
        private const string PreviousOpenXrSelectedRuntimeEnvKey = "XR_SELECTED_RUNTIME_JSON_PREV";
        private const string OpenXrOtherRuntimeEnvKey = "OTHER_XR_RUNTIME_JSON";
        private const string XrSimConfigEnvKey = "META_XRSIM_CONFIG_JSON";
        private const string PreviousXrSimConfigEnvKey = "META_XRSIM_CONFIG_JSON_PREV";
        private const string ProjectTelemetryId = "META_PROJECT_TELEMETRY_ID";
        private const string ActivateSimulatorMenuPath = MenuPath + "/Activate";
        private const string DeactivateSimulatorMenuPath = MenuPath + "/Deactivate";

#if UNITY_EDITOR_OSX
        private const string UnityXRPackage = "com.unity.xr.openxr@>=1.13.0";
#else
        private const string UnityXRPackage = "com.unity.xr.openxr";
#endif
        private const string OculusXRPackageName = "com.unity.xr.oculus";
        public const string ErrorMessage = "error_message";

        private static string getJsonPath()
        {
            // Check for package builds
#if UNITY_EDITOR_WIN
            var path = Path.GetFullPath(PackagePath + "/MetaXRSimulator/meta_openxr_simulator_win64.json");
#else
            var path = Path.GetFullPath(PackagePath + "/MetaXRSimulator/meta_openxr_simulator_posix.json");
#endif
            if (!File.Exists(path))
            {
                // For local builds
                path = Path.GetFullPath(PackagePath + "/MetaXRSimulator/meta_openxr_simulator.json");
            }
            return path;
        }
        private static readonly string JsonPath = getJsonPath();

#if UNITY_EDITOR_OSX
        private static readonly string DllPath = Path.GetFullPath(PackagePath + "/MetaXRSimulator/SIMULATOR.so");
#else
        private static readonly string DllPath = Path.GetFullPath(PackagePath + "/MetaXRSimulator/SIMULATOR.dll");
#endif

        private static string ConfigPath => Path.GetFullPath(PackagePath + (UnityRunningInBatchmode
            ? "/MetaXRSimulator/config/sim_core_configuration_ci.json"
            : "/MetaXRSimulator/config/sim_core_configuration.json"));

        internal static bool UnityRunningInBatchmode = false;

        /// <summary>
        /// Whether or not Meta XR Simulator is Activated.
        /// </summary>
        public static bool Activated => HasSimulatorInstalled() && IsSimulatorActivated();

        static Enabler()
        {
            if (Environment.CommandLine.Contains("-batchmode"))
            {
                UnityRunningInBatchmode = true;
            }

            if (HasSimulatorInstalled())
            {
                // Set OTHER_XR_RUNTIME_JSON to register the XR Simulator path with OpenXR Plugin
                Environment.SetEnvironmentVariable(OpenXrOtherRuntimeEnvKey, JsonPath);
            }

            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange change)
        {
            // ReportInfo("Meta XR Simulator", $"OnPlayModeStateChanged: {change}");
            // ReportInfo("Meta XR Simulator", $"{OpenXrRuntimeEnvKey} = {Environment.GetEnvironmentVariable(OpenXrRuntimeEnvKey)}");
            // ReportInfo("Meta XR Simulator", $"{OpenXrSelectedRuntimeEnvKey} = {Environment.GetEnvironmentVariable(OpenXrSelectedRuntimeEnvKey)}");

            if (change == PlayModeStateChange.EnteredPlayMode)
            {
                if (HasSimulatorInstalled() && IsSimulatorActivated())
                {
                    if (Environment.GetEnvironmentVariable(OpenXrRuntimeEnvKey) != Environment.GetEnvironmentVariable(OpenXrSelectedRuntimeEnvKey))
                    {
                        ReportInfo("Meta XR Simulator", $"{OpenXrRuntimeEnvKey} was modified. Reset it to {Environment.GetEnvironmentVariable(OpenXrSelectedRuntimeEnvKey)}");
                        Environment.SetEnvironmentVariable(OpenXrRuntimeEnvKey, Environment.GetEnvironmentVariable(OpenXrSelectedRuntimeEnvKey));
                    }
                }
            }
        }

        private static bool HasSimulatorInstalled()
        {
            return (!string.IsNullOrEmpty(JsonPath) &&
                    !string.IsNullOrEmpty(DllPath) &&
                    File.Exists(JsonPath) &&
                    File.Exists(DllPath));
        }

        private static bool IsSimulatorActivated()
        {
            return Environment.GetEnvironmentVariable(OpenXrSelectedRuntimeEnvKey) == JsonPath;
        }

        [MenuItem(ActivateSimulatorMenuPath, true, 0)]
        private static bool ValidateSimulatorActivated()
        {
            Menu.SetChecked(ActivateSimulatorMenuPath, Activated);
            return true;
        }

        [MenuItem(ActivateSimulatorMenuPath, false, 0)]
        private static void ActivateSimulatorMenuItem()
        {
            ActivateSimulator(false, Origins.Menu);
        }

        /// <summary>
        /// Toggle Activated state of Meta XR Simulator
        /// </summary>
        /// <param name="forceHideDialog">Forces any error dialog triggered by this method to be hidden.</param>
        public static void ToggleSimulator(bool forceHideDialog)
        {
            ToggleSimulator(forceHideDialog, Origins.Unknown);
        }

        internal static void ToggleSimulator(bool forceHideDialog, Origins origin)
        {
            if (HasSimulatorInstalled() && IsSimulatorActivated())
            {
                DeactivateSimulator(forceHideDialog, origin);
            }
            else
            {
                ActivateSimulator(forceHideDialog, origin);
            }
        }

        /// <summary>
        /// Activates Meta XR Simulator
        /// </summary>
        /// <param name="forceHideDialog">Forces any error dialog triggered by this method to be hidden.</param>
        public static void ActivateSimulator(bool forceHideDialog)
        {
            ActivateSimulator(forceHideDialog, Origins.Unknown);
        }

        internal static void ActivateSimulator(bool forceHideDialog, Origins origin)
        {
#if META_XR_SDK_CORE_74_OR_NEWER
            Meta.XR.Editor.Settings.UsageSettings.UsesXRSimulator.SetValue(true);
#endif
#if META_XR_SDK_CORE_SUPPORTS_TELEMETRY
            using var marker = new OVRTelemetryMarker(OVRTelemetryConstants.XRSim.MarkerId.ToggleState);
            marker.AddAnnotation(OVRTelemetryConstants.XRSim.AnnotationType.IsActive, true.ToString());
#if META_XR_SDK_CORE_SUPPORTS_TOOLBAR
            marker.AddAnnotation(OVRTelemetryConstants.Editor.AnnotationType.Origin, origin.ToString());
#endif
#endif

#if UNITY_EDITOR_OSX
            if (CultureInfo.InvariantCulture.CompareInfo.IndexOf(SystemInfo.processorType, "Intel", CompareOptions.IgnoreCase) >= 0)
            {
                DisplayDialogOrError("Meta XR Simulator Not Supported",
                                "Apple Silicon Mac is required. Intel-based Mac is not currently supported.",
                                forceHideDialog);
#if META_XR_SDK_CORE_SUPPORTS_TELEMETRY
                marker.AddAnnotation(ErrorMessage, "Mac intel is not supported");
                marker.SetResult(OVRPlugin.Qpl.ResultType.Fail);
#endif
                return;
            }
#endif

            if (!HasSimulatorInstalled())
            {
                DisplayDialogOrError("Meta XR Simulator Not Found",
                    "SIMULATOR.json is not found. Please enable OVRPlugin through Meta/Tools/OVR Utilities Plugin/Set OVRPlugin To OpenXR",
                    forceHideDialog);
#if META_XR_SDK_CORE_SUPPORTS_TELEMETRY
                marker.AddAnnotation(ErrorMessage, "Meta XR Simulator Not Found");
                marker.SetResult(OVRPlugin.Qpl.ResultType.Fail);
#endif
                return;
            }



#if UNITY_EDITOR_OSX
            var openXRLoaderInstalled = PackageList.IsPackageInstalledWithValidVersion(UnityXRPackage);
            var openXRLoaderErrorMessage = $"OpenXR Plugin ({UnityXRPackage}) package must be installed through the Unity Package Manager in order for your application to be run with XRSimulator.";
#else
            var openXRLoaderInstalled = PackageList.IsPackageInstalled(OculusXRPackageName) || PackageList.IsPackageInstalled(UnityXRPackage);
            var openXRLoaderErrorMessage =
                            $"Either the Oculus XR ({OculusXRPackageName}) or OpenXR Plugin ({UnityXRPackage}) package must be installed through the Unity Package Manager in order for your application to be run with XRSimulator.";
#endif
            if (!openXRLoaderInstalled)
            {
                DisplayDialogOrError("XRSimulator requires an Open XR Loader to function",
                    openXRLoaderErrorMessage,
                    forceHideDialog);
#if META_XR_SDK_CORE_SUPPORTS_TELEMETRY
                marker.AddAnnotation(ErrorMessage, "OpenXR Loader not installed");
                marker.SetResult(OVRPlugin.Qpl.ResultType.Fail);
#endif
                return;
            }

            if (IsSimulatorActivated())
            {
                ReportInfo("Meta XR Simulator", "Meta XR Simulator is already activated.");
                return;
            }

            if (Settings.AutomaticServers && (!SyntheticEnvironments.Installer.IsInstalled))
            {
                ReportInfo("Meta XR Simulator", "Installing MetaXR Simulator Synthetic Environments, as these are set to be started automatically");
                SyntheticEnvironments.Installer.InstallSESPackage();
            }

            // update XR_RUNTIME_JSON
            {
                var runtimeEnv = Environment.GetEnvironmentVariable(OpenXrRuntimeEnvKey);
                if (runtimeEnv == JsonPath)
                {
                    // Set the PreviouseOpenXrRuntimeEnvKey to empty string to avoid unable to deactivate the simulator
                    runtimeEnv = "";
                }
                // ReportInfo("Meta XR Simulator", "changing Env from " + runtimeEnv + " to " + JsonPath);
                Environment.SetEnvironmentVariable(PreviousOpenXrRuntimeEnvKey,
                    runtimeEnv);
                Environment.SetEnvironmentVariable(OpenXrRuntimeEnvKey, JsonPath);
            }

            // update XR_SELECTED_RUNTIME_JSON
            {
                var selectedRuntimeEnv = Environment.GetEnvironmentVariable(OpenXrSelectedRuntimeEnvKey);
                if (selectedRuntimeEnv == JsonPath)
                {
                    // Set the PreviousOpenXrSelectedRuntimeEnvKey to empty string to avoid unable to deactivate the simulator
                    selectedRuntimeEnv = "";
                }
                // ReportInfo("Meta XR Simulator", "changing Env from " + runtimeEnv + " to " + JsonPath);
                Environment.SetEnvironmentVariable(PreviousOpenXrSelectedRuntimeEnvKey,
                    selectedRuntimeEnv);

                Environment.SetEnvironmentVariable(OpenXrSelectedRuntimeEnvKey, JsonPath);
            }

            Environment.SetEnvironmentVariable(PreviousXrSimConfigEnvKey,
                Environment.GetEnvironmentVariable(XrSimConfigEnvKey));
            Environment.SetEnvironmentVariable(XrSimConfigEnvKey, ConfigPath);

#if META_XR_SDK_CORE_SUPPORTS_TELEMETRY
            var runtimeSettings = OVRRuntimeSettings.GetRuntimeSettings();
            if (runtimeSettings != null)
            {
                Environment.SetEnvironmentVariable(ProjectTelemetryId, runtimeSettings.TelemetryProjectGuid);
            }
#endif

            ReportInfo("Meta XR Simulator is activated",
                $"{OpenXrSelectedRuntimeEnvKey} is set to {Environment.GetEnvironmentVariable(OpenXrSelectedRuntimeEnvKey)}\n{XrSimConfigEnvKey} is set to {Environment.GetEnvironmentVariable(XrSimConfigEnvKey)}");
        }

        [MenuItem(DeactivateSimulatorMenuPath, true, 1)]
        private static bool ValidateSimulatorDeactivated()
        {
            Menu.SetChecked(DeactivateSimulatorMenuPath, !Activated);
            return true;
        }

        [MenuItem(DeactivateSimulatorMenuPath, false, 1)]
        private static void DeactivateSimulatorMenuItem()
        {
            DeactivateSimulator(false, Origins.Menu);
        }

        /// <summary>
        /// Deactivates Meta XR Simulator
        /// </summary>
        /// <param name="forceHideDialog">Forces any error dialog triggered by this method to be hidden.</param>
        public static void DeactivateSimulator(bool forceHideDialog)
        {
            DeactivateSimulator(forceHideDialog, Origins.Unknown);
        }

        internal static void DeactivateSimulator(bool forceHideDialog, Origins origin)
        {
#if META_XR_SDK_CORE_SUPPORTS_TELEMETRY
            using var marker = new OVRTelemetryMarker(OVRTelemetryConstants.XRSim.MarkerId.ToggleState);
            marker.AddAnnotation(OVRTelemetryConstants.XRSim.AnnotationType.IsActive, false.ToString());
#if META_XR_SDK_CORE_SUPPORTS_TOOLBAR
            marker.AddAnnotation(OVRTelemetryConstants.Editor.AnnotationType.Origin, origin.ToString());
#endif
#endif

            if (!HasSimulatorInstalled())
            {
                DisplayDialogOrError("Meta XR Simulator",
                    $"{JsonPath} is not found. Please enable OVRPlugin through Meta/Tools/OVR Utilities Plugin/Set OVRPlugin To OpenXR",
                    forceHideDialog);
#if META_XR_SDK_CORE_SUPPORTS_TELEMETRY
                marker.SetResult(OVRPlugin.Qpl.ResultType.Fail);
#endif
            }

            if (!IsSimulatorActivated())
            {
                ReportInfo("Meta XR Simulator", "Meta XR Simulator is not activated.");
#if META_XR_SDK_CORE_SUPPORTS_TELEMETRY
                marker.SetResult(OVRPlugin.Qpl.ResultType.Fail);
#endif
                return;
            }

            Environment.SetEnvironmentVariable(OpenXrRuntimeEnvKey,
                Environment.GetEnvironmentVariable(PreviousOpenXrRuntimeEnvKey));
            Environment.SetEnvironmentVariable(PreviousOpenXrRuntimeEnvKey, "");

            Environment.SetEnvironmentVariable(OpenXrSelectedRuntimeEnvKey,
                Environment.GetEnvironmentVariable(PreviousOpenXrSelectedRuntimeEnvKey));
            Environment.SetEnvironmentVariable(PreviousOpenXrSelectedRuntimeEnvKey, "");

            Environment.SetEnvironmentVariable(XrSimConfigEnvKey,
                Environment.GetEnvironmentVariable(PreviousXrSimConfigEnvKey));
            Environment.SetEnvironmentVariable(PreviousXrSimConfigEnvKey, "");

#if META_XR_SDK_CORE_SUPPORTS_TELEMETRY
            ReportInfo("Meta XR Simulator is deactivated",
                $"{OpenXrSelectedRuntimeEnvKey} is set to {Environment.GetEnvironmentVariable(OpenXrSelectedRuntimeEnvKey)}\n{XrSimConfigEnvKey} is set to {Environment.GetEnvironmentVariable(XrSimConfigEnvKey)}");
#endif
        }

    }
}

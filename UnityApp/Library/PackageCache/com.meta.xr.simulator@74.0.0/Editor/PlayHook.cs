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

#if META_XR_SDK_CORE_SUPPORTS_TELEMETRY

using System;
using UnityEditor;
using UnityEngine;

namespace Meta.XR.Simulator.Editor
{
    [InitializeOnLoad]
    internal static class PlayHook
    {
        private const string UnityXRSimSessionEnvironmentKey = "ENGINE_XRSIM_SESSION";
        static PlayHook()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static bool? _isBatchMode;
        private static bool IsBatchMode => _isBatchMode ??= Application.isBatchMode;

        private static string _applicationIdentifier;
        private static string ApplicationIdentifier => _applicationIdentifier ??= Application.identifier;
        private static void OnPlayModeStateChanged(PlayModeStateChange change)
        {
            if (change == PlayModeStateChange.ExitingEditMode)
            {
                var engineXRSimSession = Guid.NewGuid().ToString();
#if META_XR_SDK_CORE_74_OR_NEWER
                OVRTelemetry.Client.MarkerStart(OVRTelemetryConstants.XRSim.MarkerId.EditorRun,  OVRPlugin.Qpl.DefaultInstanceKey, OVRPlugin.Qpl.AutoSetTimestampMs, engineXRSimSession);
#else
                OVRTelemetry.Client.MarkerStart(OVRTelemetryConstants.XRSim.MarkerId.EditorRun,  OVRPlugin.Qpl.DefaultInstanceKey, OVRPlugin.Qpl.AutoSetTimestampMs);
#endif
                Environment.SetEnvironmentVariable(UnityXRSimSessionEnvironmentKey, engineXRSimSession);
                OVRTelemetry.Client.MarkerAnnotation(OVRTelemetryConstants.XRSim.MarkerId.EditorRun, OVRTelemetryConstants.XRSim.AnnotationType.XRSimEnabled,
                    Enabler.Activated.ToString());
#if META_XR_SDK_CORE_74_OR_NEWER
                OVRTelemetry.Client.MarkerAnnotation(OVRTelemetryConstants.XRSim.MarkerId.EditorRun, OVRTelemetryConstants.XRSim.AnnotationType.EngineXRSimSession,
                    engineXRSimSession);
#endif

                OVRTelemetry.Client.MarkerAnnotation(OVRTelemetryConstants.XRSim.MarkerId.EditorRun,
                                OVRTelemetryConstants.OVRManager.AnnotationTypes.ProjectName, ApplicationIdentifier);
                OVRTelemetry.Client.MarkerAnnotation(OVRTelemetryConstants.XRSim.MarkerId.EditorRun,
                                OVRTelemetryConstants.OVRManager.AnnotationTypes.ProjectGuid,
                                OVRRuntimeSettings.Instance.TelemetryProjectGuid);
                OVRTelemetry.Client.MarkerAnnotation(OVRTelemetryConstants.XRSim.MarkerId.EditorRun,
                        OVRTelemetryConstants.OVRManager.AnnotationTypes.BatchMode, IsBatchMode);
#if META_XR_SDK_CORE_74_OR_NEWER
                OVRTelemetry.Client.MarkerAnnotation(OVRTelemetryConstants.XRSim.MarkerId.EditorRun,
                        OVRTelemetryConstants.OVRManager.AnnotationTypes.ProcessorType, SystemInfo.processorType);
#endif
                OVRTelemetry.Client.MarkerPoint(OVRTelemetryConstants.XRSim.MarkerId.EditorRun, "exiting_edit_mode");
                return;
            }

            if (change == PlayModeStateChange.EnteredPlayMode)
            {
                OVRTelemetry.Client.MarkerPoint(OVRTelemetryConstants.XRSim.MarkerId.EditorRun, "enter_play_mode");
                OVRTelemetry.Client.MarkerEnd(OVRTelemetryConstants.XRSim.MarkerId.EditorRun);
                return;
            }
        }
    }
}

#endif

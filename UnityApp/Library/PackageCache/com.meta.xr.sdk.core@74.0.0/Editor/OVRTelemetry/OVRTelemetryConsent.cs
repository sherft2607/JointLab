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
using UnityEditor;
using UnityEngine;
using static OVRPlugin;

internal static class OVRTelemetryConsent
{
    public static Action<bool> OnTelemetrySet;
    private const string HasSentConsentEventKey = "OVRTelemetry.HasSentConsentEvent";

    private static bool HasSentConsentEvent
    {
        get => EditorPrefs.GetBool(HasSentConsentEventKey, false);
        set => EditorPrefs.SetBool(HasSentConsentEventKey, value);
    }

    private static bool? _shareAdditionalData;
    public static bool ShareAdditionalData
    {
        get
        {
            if (_shareAdditionalData.HasValue)
            {
                return _shareAdditionalData.Value;
            }

            var consent = UnifiedConsent.GetUnifiedConsent();
            _shareAdditionalData = consent is true;
            return _shareAdditionalData.Value;
        }
        private set
        {
            _shareAdditionalData = value;
            UnifiedConsent.SaveUnifiedConsent(value);
        }
    }

    public static void SetTelemetryEnabled(bool enabled, OVRTelemetryConstants.OVRManager.ConsentOrigins origin)
    {
        SetLibrariesConsent(enabled);
        ShareAdditionalData = enabled;
        SendConsentEvent(origin);
        OnTelemetrySet?.Invoke(enabled);
    }

    public static void SetLibrariesConsent(bool enabled)
    {
        SetDeveloperTelemetryConsent(enabled ? Bool.True : Bool.False);
        Qpl.SetConsent(enabled ? Bool.True : Bool.False);
    }

    public static void SendConsentEvent(OVRTelemetryConstants.OVRManager.ConsentOrigins origin)
    {
        if (HasSentConsentEvent && origin != OVRTelemetryConstants.OVRManager.ConsentOrigins.Settings)
        {
            return;
        }


        OVRTelemetry.Start(OVRTelemetryConstants.OVRManager.MarkerId.Consent)
            .AddAnnotation(OVRTelemetryConstants.OVRManager.AnnotationTypes.Origin, origin.ToString())
            .SetResult(ShareAdditionalData ? Qpl.ResultType.Success : Qpl.ResultType.Fail)
            .Send();
        SendEvent("editor_consent", ShareAdditionalData ? "granted" : "withheld");

        HasSentConsentEvent = true;
    }
}

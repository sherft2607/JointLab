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

using Oculus.Platform.Models;
using System;
using UnityEngine;

namespace Oculus.Platform.BuildingBlocks
{
    public class EntitlementCheck : MonoBehaviour
    {
        public bool quitAppOnNotEntitled = false;

        // The app can optionally subscribe to these events to do custom entitlement check logic.
        public event Action UserFailedEntitlementCheck;
        public event Action UserPassedEntitlementCheck;

        private void Start()
        {
            if (quitAppOnNotEntitled)
            {
                UserFailedEntitlementCheck += QuitAppOnFailure;
            }

            PerformUserEntitlementCheck();
        }

        public void PerformUserEntitlementCheck()
        {
            // Init the Oculust Platform SDK and send an entitlement check request.
            if (!Core.IsInitialized())
            {
                try
                {
                    Core.AsyncInitialize().OnComplete(PlatformInitializeCallback);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Exception occured during OvrPlatform init - {e.Message}");
                }
            }
        }

        public void PlatformInitializeCallback(Message<PlatformInitialize> msg)
        {
            if (msg.Data?.Result != PlatformInitializeResult.Success)
            {
                Debug.LogError($"OvrPlatform init resulted in failure. - {msg.Data.Result}\n{msg.GetError().Message}");
                UserFailedEntitlementCheck?.Invoke();
            }
            else
            {
                try
                {
                    Entitlements.IsUserEntitledToApplication().OnComplete(EntitlementCheckCallback);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Exception occured during Entitlement Check - {e.Message}");
                }
            }
        }

        private void EntitlementCheckCallback(Message msg)
        {
            if (!msg.IsError)
            {
                Debug.Log("You are entitled to use this app.");
                UserPassedEntitlementCheck?.Invoke();
            }
            else
            {
                Debug.LogError("You are NOT entitled to use this app.");
                UserFailedEntitlementCheck?.Invoke();
            }
        }

        private void QuitAppOnFailure()
        {
            // Implements a default behavior for an entitlement check failure -- log the failure and exit the app.
            Debug.LogError("Oculus user entitlement check failed. Exiting now...");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            UnityEngine.Application.Quit();
#endif
        }
    }
}

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
using Oculus.Assistant.VoiceCommand.Data;
using Oculus.Voice.Core.Utilities;
using UnityEngine;
using UnityEngine.Events;


namespace Oculus.Assistant.VoiceCommand.Listeners
{
    [Serializable]
    public class SlotHandler
    {
        [Tooltip("The name of the slot to listen for")]
        public string slotName;
        public OnCommandSlotReceived onCommandSlotReceived = new OnCommandSlotReceived();
        public override string ToString()
        {
            return slotName;
        }
    }

    [Serializable]
    public class VoiceCommandResultHandler : VoiceCommandListener
    {
        public Configuration.VoiceCommand voiceCommand;
        public VoiceCommandCallbackEvent onVoiceCommandReceived = new VoiceCommandCallbackEvent();
        [ArrayElementTitle("slotName", "Unassigned Slot")]
        public SlotHandler[] slotHandlers = Array.Empty<SlotHandler>();

        public void OnCallback(VoiceCommandResult result)
        {
            if (voiceCommand.actionId == result.ActionId)
            {
                onVoiceCommandReceived.Invoke(result);
                foreach (var slotHandler in slotHandlers)
                {
                    if (result.TryGetSlot(slotHandler.slotName, out string value))
                    {
                        slotHandler.onCommandSlotReceived.Invoke(value);
                    }
                }
            }
        }
    }


    #region Callback Events

    [Serializable]
    public class OnCommandSlotReceived : UnityEvent<string>
    {
    }

    [Serializable]
    public class VoiceCommandCallbackEvent : UnityEvent<VoiceCommandResult>
    {
    }

    #endregion
}

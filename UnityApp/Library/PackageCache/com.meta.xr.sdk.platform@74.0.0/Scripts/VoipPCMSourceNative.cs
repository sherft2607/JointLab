using UnityEngine;
using System.Collections;
using System;

namespace Oculus.Platform
{
    /// The VoipPCMSource class represents the PCM (pulse code modulation) API. It provides ways to get the pcm data for a user and update the pcm data.
    /// Read more about VoIP [here](https://developer.oculus.com/documentation/unity/ps-parties/#voip-options).
    public class VoipPCMSourceNative : IVoipPCMSource
    {
        ulong senderID;

        /// Retrieves the PCM object.
        public int GetPCM(float[] dest, int length)
        {
            return (int)CAPI.ovr_Voip_GetPCMFloat(senderID, dest, (UIntPtr)length);
        }

        /// Sets the sender ID associated with this Voip source. The ID will belong to a Models.User.
        public void SetSenderID(ulong senderID)
        {
            this.senderID = senderID;
        }

        /// Uses the sender ID and will return an int that represents the size of the current PCM data stack.
        public int PeekSizeElements()
        {
            return (int)CAPI.ovr_Voip_GetPCMSize(senderID);
        }

        public void Update()
        {
        }
    }
}

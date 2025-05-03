// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// It represents the availability of a microphone device. It can be retrieved
  /// using Voip.GetMicrophoneAvailability(). It has only one field,
  /// microphone_available, which is a boolean value that indicates whether a
  /// microphone is available or not. This data structure can be used by
  /// applications to determine whether they can use the microphone for audio
  /// input or not.
  public class MicrophoneAvailabilityState
  {
    /// A `boolean` indicates whether the microphone is currently available or not.
    /// If there is any update on the microphone availability, it will be retrieved
    /// as a notification using
    /// Message::MessageType::Notification_Voip_MicrophoneAvailabilityStateUpdate.
    public readonly bool MicrophoneAvailable;


    public MicrophoneAvailabilityState(IntPtr o)
    {
      MicrophoneAvailable = CAPI.ovr_MicrophoneAvailabilityState_GetMicrophoneAvailable(o);
    }
  }

}

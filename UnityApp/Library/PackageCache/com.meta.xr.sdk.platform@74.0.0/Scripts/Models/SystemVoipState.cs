// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// The state of the voip that is used in parties and horizon home. This class
  /// contains different statuses that is used to control the microphone and
  /// input/output for commands or chat in your application. You can read more
  /// about system voip
  /// [here](https://developer.oculus.com/documentation/unity/ps-parties/#voip-
  /// options).
  public class SystemVoipState
  {
    /// A flag that is used to indicate the current state of the microphone. The
    /// status can be of the following types:
    ///
    /// - Unknown: VoipMuteState.Unknown
    ///
    /// - Muted: VoipMuteState.Muted
    ///
    /// - Unmuted: VoipMuteState.Unmuted
    public readonly VoipMuteState MicrophoneMuted;
    /// The status enum that indicates the current state of the system voip. The
    /// status can be of the following types:
    ///
    /// - Unknown: SystemVoipStatus.Unknown
    ///
    /// - Unavailable: SystemVoipStatus.Unavailable
    ///
    /// - Suppressed: SystemVoipStatus.Suppressed
    ///
    /// - Active: SystemVoipStatus.Active
    public readonly SystemVoipStatus Status;


    public SystemVoipState(IntPtr o)
    {
      MicrophoneMuted = CAPI.ovr_SystemVoipState_GetMicrophoneMuted(o);
      Status = CAPI.ovr_SystemVoipState_GetStatus(o);
    }
  }

}

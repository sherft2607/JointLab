// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// The livestreaming application status represents the status result of
  /// whether the livestreaming is enabled for an app. If your app is enabled,
  /// you will receive a notification by
  /// Message::MessageType::Notification_Livestreaming_StatusChange when the
  /// livestreaming session gets updated.
  public class LivestreamingApplicationStatus
  {
    /// This is a boolean field and represents whether the app is allowed to do the
    /// livestreaming or not.
    public readonly bool StreamingEnabled;


    public LivestreamingApplicationStatus(IntPtr o)
    {
      StreamingEnabled = CAPI.ovr_LivestreamingApplicationStatus_GetStreamingEnabled(o);
    }
  }

}

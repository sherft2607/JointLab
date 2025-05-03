// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// The livestreaming status represents the status of a livestreaming event in
  /// your app. You will receive a notification by
  /// Message::MessageType::Notification_Livestreaming_StatusChange whenever your
  /// livestreaming session gets updated. The status contains info about your
  /// livestream type, whether your mic is enabled, whether the comments are
  /// visible and etc.
  public class LivestreamingStatus
  {
    /// This boolean field indicates if the comments from the audience in your
    /// livestreaming are visible.
    public readonly bool CommentsVisible;
    /// This boolean field indicates if your livestreaming in the app is paused or
    /// not.
    public readonly bool IsPaused;
    /// This boolean field indicates if your app is livestreaming enabled. If your
    /// app is enabled, you will receive a notification by
    /// Message::MessageType::Notification_Livestreaming_StatusChange when the
    /// livestreaming session gets updated.
    public readonly bool LivestreamingEnabled;
    /// This field indicates the type of your livestreaming.
    public readonly int LivestreamingType;
    /// This boolean field indicates if your connected mic is enabled. The speaker
    /// will be muted if the field is false.
    public readonly bool MicEnabled;


    public LivestreamingStatus(IntPtr o)
    {
      CommentsVisible = CAPI.ovr_LivestreamingStatus_GetCommentsVisible(o);
      IsPaused = CAPI.ovr_LivestreamingStatus_GetIsPaused(o);
      LivestreamingEnabled = CAPI.ovr_LivestreamingStatus_GetLivestreamingEnabled(o);
      LivestreamingType = CAPI.ovr_LivestreamingStatus_GetLivestreamingType(o);
      MicEnabled = CAPI.ovr_LivestreamingStatus_GetMicEnabled(o);
    }
  }

}

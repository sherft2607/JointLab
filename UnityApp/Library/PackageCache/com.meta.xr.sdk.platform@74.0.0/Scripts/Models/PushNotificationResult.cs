// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// A PushNotificationResult represents the outcome of a user registering for
  /// third-party (3P) notifications. This object contains essential information
  /// about the registered notification, which can be used to send push
  /// notifications to the user. It can be retrieved using
  /// PushNotification.Register()
  public class PushNotificationResult
  {
    /// The registered notification id is a type of string which you can push
    /// notification to.
    public readonly string Id;


    public PushNotificationResult(IntPtr o)
    {
      Id = CAPI.ovr_PushNotificationResult_GetId(o);
    }
  }

}

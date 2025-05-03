// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// Users can initiate follow requests to other users encountered in the app by
  /// launching the process from within the app. After the follow request has
  /// been sent via a deeplinked modal, the viewer is returned to the app. Users
  /// may find this process more convenient than using the Meta Quest mobile app
  /// or returning to Meta Horizon Home to send follow requests since it is less
  /// disruptive to the app experience they are currently focused on. You can
  /// retrieve it using Users.LaunchFriendRequestFlow().
  public class LaunchFriendRequestFlowResult
  {
    /// User can choose to cancel the friend request flow after sending it. You can
    /// use this to check whether the viewer chose to cancel the friend request
    /// flow.
    public readonly bool DidCancel;
    /// Whether the viewer successfully sent the friend request.
    public readonly bool DidSendRequest;


    public LaunchFriendRequestFlowResult(IntPtr o)
    {
      DidCancel = CAPI.ovr_LaunchFriendRequestFlowResult_GetDidCancel(o);
      DidSendRequest = CAPI.ovr_LaunchFriendRequestFlowResult_GetDidSendRequest(o);
    }
  }

}

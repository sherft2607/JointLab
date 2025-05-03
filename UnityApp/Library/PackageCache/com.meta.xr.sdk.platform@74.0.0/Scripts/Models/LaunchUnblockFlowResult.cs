// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// It's used to unblock a user. Results of the launched unblock dialog
  /// including whether the user was successfully unblocked and whether the
  /// viewer canceled the unblock flow. It can be retrieved using
  /// Users.LaunchUnblockFlow().
  public class LaunchUnblockFlowResult
  {
    /// A `boolean` indicates whether the viewer chose to cancel the unblock flow.
    /// It will be 'true' if the viewer canceled 'Unblock' from the modal.
    public readonly bool DidCancel;
    /// A `boolean` indicates whether the viewer successfully unblocked the user.
    /// Learn more about [unblocking
    /// users](https://developer.oculus.com/documentation/unity/ps-blockingsdk/)
    /// from our website.
    public readonly bool DidUnblock;


    public LaunchUnblockFlowResult(IntPtr o)
    {
      DidCancel = CAPI.ovr_LaunchUnblockFlowResult_GetDidCancel(o);
      DidUnblock = CAPI.ovr_LaunchUnblockFlowResult_GetDidUnblock(o);
    }
  }

}

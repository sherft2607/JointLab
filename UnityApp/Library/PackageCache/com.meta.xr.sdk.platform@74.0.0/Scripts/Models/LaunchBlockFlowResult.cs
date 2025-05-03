// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// It's used to block a user. Results of the launched block dialog including
  /// whether the user was successfully blocked and whether the viewer canceled
  /// the block flow. It can be retrieved using Users.LaunchBlockFlow()
  public class LaunchBlockFlowResult
  {
    /// A `boolean` indicates whether the viewer successfully blocked the user.
    /// Learn more about [blocking
    /// users](https://developer.oculus.com/documentation/unity/ps-blockingsdk/)
    /// from our website.
    public readonly bool DidBlock;
    /// A `boolean` indicates whether the viewer chose to cancel the block flow. It
    /// will be 'true' if the viewer canceled 'Block' from the modal.
    public readonly bool DidCancel;


    public LaunchBlockFlowResult(IntPtr o)
    {
      DidBlock = CAPI.ovr_LaunchBlockFlowResult_GetDidBlock(o);
      DidCancel = CAPI.ovr_LaunchBlockFlowResult_GetDidCancel(o);
    }
  }

}

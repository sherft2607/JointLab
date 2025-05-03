// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// This is the result that can be extracted from message payload when the
  /// avatar editor is launched via a call to Avatar.LaunchAvatarEditor(). It
  /// contains information about whether the avatar editor result has
  /// successfully been sent.
  public class AvatarEditorResult
  {
    /// This indicates whether the request has been sent successfully. This is an
    /// optional `boolean`. If the boolean field isn't there, it indicates that the
    /// response is an error and will throw an error message.
    public readonly bool RequestSent;


    public AvatarEditorResult(IntPtr o)
    {
      RequestSent = CAPI.ovr_AvatarEditorResult_GetRequestSent(o);
    }
  }

}

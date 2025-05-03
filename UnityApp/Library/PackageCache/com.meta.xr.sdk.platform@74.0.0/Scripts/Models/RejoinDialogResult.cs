// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// A boolean that indicates the result of GroupPresence.LaunchRejoinDialog().
  /// 'True' indicates that the application will rejoin the dialog, otherwise the
  /// application will not rejoin the dialog. Read more about the [rejoin
  /// dialog](https://developer.oculus.com/documentation/unity/ps-rejoin/#rejoin-
  /// apis).
  public class RejoinDialogResult
  {
    /// A boolean for if the user has decided to rejoin. This is used in
    /// GroupPresence.LaunchRejoinDialog().
    public readonly bool RejoinSelected;


    public RejoinDialogResult(IntPtr o)
    {
      RejoinSelected = CAPI.ovr_RejoinDialogResult_GetRejoinSelected(o);
    }
  }

}

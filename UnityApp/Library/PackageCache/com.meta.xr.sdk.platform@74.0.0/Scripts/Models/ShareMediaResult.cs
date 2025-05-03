// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// The result from users launching the Share to Facebook modal which enables
  /// them to share local media files through Media.ShareToFacebook(). The result
  /// will contain a status field, #ShareMediaStatus.
  public class ShareMediaResult
  {
    /// The status of the share media result. The status can be of the following
    /// types:
    ///
    /// ShareMediaStatus.Unknown
    ///
    /// ShareMediaStatus.Shared
    ///
    /// ShareMediaStatus.Canceled
    public readonly ShareMediaStatus Status;


    public ShareMediaResult(IntPtr o)
    {
      Status = CAPI.ovr_ShareMediaResult_GetStatus(o);
    }
  }

}

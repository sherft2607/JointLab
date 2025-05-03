// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// You will receive this livestreaming start result once you start a
  /// livestream to Facebook or to a party in your app. This result will show if
  /// the start status is a success or unknown or a failure because of various
  /// reasons including no Facebook connection or missing parameters.
  public class LivestreamingStartResult
  {
    /// This livestreaming result represents the start status of your livestream.
    /// You can refer to #LivestreamingStartStatus for possible status info.
    public readonly LivestreamingStartStatus StreamingResult;


    public LivestreamingStartResult(IntPtr o)
    {
      StreamingResult = CAPI.ovr_LivestreamingStartResult_GetStreamingResult(o);
    }
  }

}

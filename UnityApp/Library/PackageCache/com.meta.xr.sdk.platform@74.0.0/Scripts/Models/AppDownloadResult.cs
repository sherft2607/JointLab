// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// Represents the result of an app download. You will use it when you start an
  /// app download, cancel an app download or install an app update which was
  /// previous downloaded. In those scenarios, the app download result will be
  /// the payload of Application.StartAppDownload(),
  /// Application.CancelAppDownload() or
  /// Application.InstallAppUpdateAndRelaunch() API calls.
  public class AppDownloadResult
  {
    /// Result of the install operation returned by the installer. You can find
    /// more information about possible members from #AppInstallResult. In case of
    /// an error during install process, the error message contains the string
    /// representation of this result.
    public readonly AppInstallResult AppInstallResult;
    /// Timestamp in milliseconds when the operation finished.
    public readonly long Timestamp;


    public AppDownloadResult(IntPtr o)
    {
      AppInstallResult = CAPI.ovr_AppDownloadResult_GetAppInstallResult(o);
      Timestamp = CAPI.ovr_AppDownloadResult_GetTimestamp(o);
    }
  }

}

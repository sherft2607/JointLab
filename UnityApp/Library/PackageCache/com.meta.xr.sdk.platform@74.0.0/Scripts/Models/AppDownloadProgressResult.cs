// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// Represents the result of the download of an application. You can retrieve
  /// it using CheckAppDownloadProgress API
  /// (Application.CheckAppDownloadProgress()). You can use this to check the
  /// progress and the status of an ongoing app download operation.
  public class AppDownloadProgressResult
  {
    /// Total number of bytes that need to be downloaded
    public readonly long DownloadBytes;
    /// Number of bytes that have already been downloaded. You can use this and
    /// AppDownloadProgressResult#DownloadBytes to implement the progress bar.
    public readonly long DownloadedBytes;
    /// Status code of the current app status. You can use it to find out whether
    /// the app is being downloaded or queued for downloading
    public readonly AppStatus StatusCode;


    public AppDownloadProgressResult(IntPtr o)
    {
      DownloadBytes = CAPI.ovr_AppDownloadProgressResult_GetDownloadBytes(o);
      DownloadedBytes = CAPI.ovr_AppDownloadProgressResult_GetDownloadedBytes(o);
      StatusCode = CAPI.ovr_AppDownloadProgressResult_GetStatusCode(o);
    }
  }

}

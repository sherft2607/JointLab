// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// An AssetFileDownloadCancelResult represents the result of a canceled
  /// download action of an asset file. You can cancel a download of an asset
  /// file by using AssetFile.DownloadCancelById() or
  /// AssetFile.DownloadCancelByName(). The result contains three fields which
  /// are the asset file ID(use AssetFileDownloadCancelResult#AssetId to access),
  /// the file path, and success status of the canceled download.
  public class AssetFileDownloadCancelResult
  {
    /// \deprecated You can use AssetFileDownloadCancelResult#AssetId to retrieve the ID of the asset file instead.
    public readonly UInt64 AssetFileId;
    /// ID of the asset file. NOTE: this does not represent the ID of the asset.
    public readonly UInt64 AssetId;
    /// File path of the asset file.
    public readonly string Filepath;
    /// You can use this to determine whether the cancel request of downloading an
    /// asset file has succeeded.
    public readonly bool Success;


    public AssetFileDownloadCancelResult(IntPtr o)
    {
      AssetFileId = CAPI.ovr_AssetFileDownloadCancelResult_GetAssetFileId(o);
      AssetId = CAPI.ovr_AssetFileDownloadCancelResult_GetAssetId(o);
      Filepath = CAPI.ovr_AssetFileDownloadCancelResult_GetFilepath(o);
      Success = CAPI.ovr_AssetFileDownloadCancelResult_GetSuccess(o);
    }
  }

}

// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// An AssetFileDownloadResult represents an asset that has been successfully
  /// downloaded. It's used to store information about an asset that has been
  /// downloaded, such as its location on the local file system and its unique
  /// identifier asset file ID. It can be retrieved using
  /// AssetFile.DownloadByName().
  public class AssetFileDownloadResult
  {
    /// ID of the asset file. It can be retrieved using AssetDetails#AssetId. It
    /// can be used to retrieve the AssetFileDownloadResult by
    /// AssetFile.DownloadById().
    public readonly UInt64 AssetId;
    /// File path of the asset file.
    public readonly string Filepath;


    public AssetFileDownloadResult(IntPtr o)
    {
      AssetId = CAPI.ovr_AssetFileDownloadResult_GetAssetId(o);
      Filepath = CAPI.ovr_AssetFileDownloadResult_GetFilepath(o);
    }
  }

}

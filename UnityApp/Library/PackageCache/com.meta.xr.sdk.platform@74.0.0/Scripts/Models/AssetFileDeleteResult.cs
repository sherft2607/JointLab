// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// An AssetFileDeleteResult contains the result of a deleted asset file. You
  /// can delete an asset file by using AssetFile.DeleteById() or
  /// AssetFile.DeleteByName(). The delete result contains the
  /// AssetFileDeleteResult#AssetId, the file path, and the success status of the
  /// deleted asset.
  public class AssetFileDeleteResult
  {
    /// \deprecated You can use AssetFileDeleteResult#AssetId to retrieve the ID of the asset file.
    public readonly UInt64 AssetFileId;
    /// This represents the ID of the asset file. When you want to use
    /// AssetFileDeleteResult#AssetFileId, you need to use
    /// AssetFileDeleteResult#AssetId instead. It can be retrieved using
    /// AssetDetails#AssetId.
    public readonly UInt64 AssetId;
    /// File path of the asset file.
    public readonly string Filepath;
    /// You can use this to determine whether deleting an asset file was successful
    /// or not.
    public readonly bool Success;


    public AssetFileDeleteResult(IntPtr o)
    {
      AssetFileId = CAPI.ovr_AssetFileDeleteResult_GetAssetFileId(o);
      AssetId = CAPI.ovr_AssetFileDeleteResult_GetAssetId(o);
      Filepath = CAPI.ovr_AssetFileDeleteResult_GetFilepath(o);
      Success = CAPI.ovr_AssetFileDeleteResult_GetSuccess(o);
    }
  }

}

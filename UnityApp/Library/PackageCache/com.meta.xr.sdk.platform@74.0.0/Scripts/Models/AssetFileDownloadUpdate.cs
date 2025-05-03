// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// An AssetFileDownloadUpdate represents the download status of an update for
  /// an asset file. It contains the asset file ID, the download progress of the
  /// update, and its completion status. It can be retrieved using
  /// Message::MessageType::Notification_AssetFile_DownloadUpdate.
  public class AssetFileDownloadUpdate
  {
    /// \deprecated Use AssetFileDownloadUpdate#AssetId.
    public readonly UInt64 AssetFileId;
    /// ID of the asset file. It can be retrieved using AssetDetails#AssetId.
    public readonly UInt64 AssetId;
    /// This field is of type uint and represents the total number of bytes in the
    /// asset file.
    public readonly ulong BytesTotal;
    /// An integer represents the number of bytes that have been downloaded. -1 If
    /// the download hasn't started yet.
    public readonly long BytesTransferred;
    /// This field is of type boolean and indicates whether the download has been
    /// completed or not.
    public readonly bool Completed;


    public AssetFileDownloadUpdate(IntPtr o)
    {
      AssetFileId = CAPI.ovr_AssetFileDownloadUpdate_GetAssetFileId(o);
      AssetId = CAPI.ovr_AssetFileDownloadUpdate_GetAssetId(o);
      BytesTotal = CAPI.ovr_AssetFileDownloadUpdate_GetBytesTotalLong(o);
      BytesTransferred = CAPI.ovr_AssetFileDownloadUpdate_GetBytesTransferredLong(o);
      Completed = CAPI.ovr_AssetFileDownloadUpdate_GetCompleted(o);
    }
  }

}

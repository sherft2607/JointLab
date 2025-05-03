// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

#pragma warning disable 0618

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// An AssetDetails object contains detailed metadata for an asset file,
  /// including the asset file ID, file path, asset type, and additional metadata
  /// that characterizes the asset. You can retrieve the asset details using
  /// AssetFile.GetList()
  public class AssetDetails
  {
    /// ID of the asset file. You can use this ID to uniquely identify a single
    /// asset. You can find more details from AssetFile.StatusById()
    public readonly UInt64 AssetId;
    /// One of 'default', 'store', or 'language_pack'. The 'default' type denotes
    /// this Asset File is used purely as an implementation detail (to download
    /// extra content post-installation). The 'store' type shows, that the Asset
    /// File should be shown in Store. The 'language_pack' is a special type used
    /// to manage different languages and translation data, which can be downloaded
    /// post-installation.
    public readonly string AssetType;
    /// One of 'installed', 'available', or 'in-progress'
    public readonly string DownloadStatus;
    /// File path of the asset file
    public readonly string Filepath;
    /// The status of in app purchases which are some extra content that users can
    /// buy right from apps. The value could be one of 'free', 'entitled', or 'not-
    /// entitled'.
    public readonly string IapStatus;
    /// This field gives information about currently selected and installed
    /// language for the asset. The language info contains language name and tag in
    /// BCP47 format.
    // May be null. Check before using.
    public readonly LanguagePackInfo LanguageOptional;
    [Obsolete("Deprecated in favor of LanguageOptional")]
    public readonly LanguagePackInfo Language;
    /// Extra metadata associated with this asset file
    public readonly string Metadata;


    public AssetDetails(IntPtr o)
    {
      AssetId = CAPI.ovr_AssetDetails_GetAssetId(o);
      AssetType = CAPI.ovr_AssetDetails_GetAssetType(o);
      DownloadStatus = CAPI.ovr_AssetDetails_GetDownloadStatus(o);
      Filepath = CAPI.ovr_AssetDetails_GetFilepath(o);
      IapStatus = CAPI.ovr_AssetDetails_GetIapStatus(o);
      {
        var pointer = CAPI.ovr_AssetDetails_GetLanguage(o);
        Language = new LanguagePackInfo(pointer);
        if (pointer == IntPtr.Zero) {
          LanguageOptional = null;
        } else {
          LanguageOptional = Language;
        }
      }
      Metadata = CAPI.ovr_AssetDetails_GetMetadata(o);
    }
  }

  /// Represents a paginated list of Models.AssetDetails elements. It allows you
  /// to easily access and manipulate the elements in the paginated list, such as
  /// the size of the list and if there is a next page of elements available.
  public class AssetDetailsList : DeserializableList<AssetDetails> {
    /// Instantiates a C# wrapper class that wraps a native list by pointer. Used internally by Platform SDK to wrap the list.
    public AssetDetailsList(IntPtr a) {
      var count = (int)CAPI.ovr_AssetDetailsArray_GetSize(a);
      _Data = new List<AssetDetails>(count);
      for (int i = 0; i < count; i++) {
        _Data.Add(new AssetDetails(CAPI.ovr_AssetDetailsArray_GetElement(a, (UIntPtr)i)));
      }

    }

  }
}

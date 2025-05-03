// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// Cowatch viewer represents a viewer in a cowatching session, including their
  /// user ID and any data that they have set. The cowatch viewer data can be
  /// retrieved using Cowatching.GetViewersData(). It can be useful for tracking
  /// the participants in a cowatching session and managing their data.
  public class CowatchViewer
  {
    /// Represents the viewer data set by this cowatching viewer. It's an optional
    /// `string` and can be set by Cowatching.SetViewerData().
    public readonly string Data;
    /// A unique user ID of the viewer.
    public readonly UInt64 Id;


    public CowatchViewer(IntPtr o)
    {
      Data = CAPI.ovr_CowatchViewer_GetData(o);
      Id = CAPI.ovr_CowatchViewer_GetId(o);
    }
  }

  /// Represents a paginated list of Models.CowatchViewer elements. It allows you
  /// to easily access and manipulate the elements in the paginated list, such as
  /// the size of the list and if there is a next page of elements available.
  public class CowatchViewerList : DeserializableList<CowatchViewer> {
    /// Instantiates a C# wrapper class that wraps a native list by pointer. Used internally by Platform SDK to wrap the list.
    public CowatchViewerList(IntPtr a) {
      var count = (int)CAPI.ovr_CowatchViewerArray_GetSize(a);
      _Data = new List<CowatchViewer>(count);
      for (int i = 0; i < count; i++) {
        _Data.Add(new CowatchViewer(CAPI.ovr_CowatchViewerArray_GetElement(a, (UIntPtr)i)));
      }

      _NextUrl = CAPI.ovr_CowatchViewerArray_GetNextUrl(a);
    }

  }
}

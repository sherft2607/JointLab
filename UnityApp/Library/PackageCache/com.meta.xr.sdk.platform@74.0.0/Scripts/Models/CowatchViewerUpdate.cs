// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// CowatchWiewerUpdate is used to represent updates to viewer data within a
  /// cowatching session. It's essential for managing and synchronizing viewer
  /// data in real-time during cowatching sessions. It can be retrieved using
  /// Message::MessageType::Notification_Cowatching_ViewersDataChanged when a
  /// user joins or updates their viewer data.
  public class CowatchViewerUpdate
  {
    /// List of viewer data of all cowatch participants who is in a cowatching
    /// session. It can be retrieved using Cowatching.GetViewersData().
    public readonly CowatchViewerList DataList;
    /// A unique user ID of the user whose viewer data has been updated.
    public readonly UInt64 Id;


    public CowatchViewerUpdate(IntPtr o)
    {
      DataList = new CowatchViewerList(CAPI.ovr_CowatchViewerUpdate_GetDataList(o));
      Id = CAPI.ovr_CowatchViewerUpdate_GetId(o);
    }
  }

}

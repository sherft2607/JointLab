// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// When a new list of sessions was added or the list of known connected
  /// sessions has changed, this
  /// Message::MessageType::Notification_NetSync_SessionsChanged will be sent.
  /// When the status of a connection has changed, the
  /// Message::MessageType::Notification_NetSync_ConnectionStatusChanged will be
  /// sent.
  public class NetSyncSessionsChangedNotification
  {
    /// A `long` integer ID which can be used to uniquely identify the network
    /// synchronization connection.
    public readonly long ConnectionId;
    /// An `array` which contains the new list of Models.NetSyncSession.
    public readonly NetSyncSessionList Sessions;


    public NetSyncSessionsChangedNotification(IntPtr o)
    {
      ConnectionId = CAPI.ovr_NetSyncSessionsChangedNotification_GetConnectionId(o);
      Sessions = new NetSyncSessionList(CAPI.ovr_NetSyncSessionsChangedNotification_GetSessions(o));
    }
  }

}

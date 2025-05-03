// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// It represents the connection of a network synchronization system that
  /// allows multiple clients to connect and communicate with each other in real-
  /// time. It provides a way to manage and facilitate real-time communication
  /// and data synchronization between multiple clients in a networked
  /// environment.
  public class NetSyncConnection
  {
    /// A 'long' integer ID which can be used to uniquely identify the network
    /// synchronization connection.
    public readonly long ConnectionId;
    /// If the status is NetSyncConnectionStatus.Disconnected,
    /// #NetSyncDisconnectReason specifies the reason.
    /// NetSyncDisconnectReason.Unknown - The disconnect reason was unknown.
    /// NetSyncDisconnectReason.LocalTerminated - The disconnect was requested by
    /// an user. NetSyncDisconnectReason.ServerTerminated - The server closed the
    /// connection. NetSyncDisconnectReason.Failed - The initial connection never
    /// succeeded. NetSyncDisconnectReason.Lost - The disconnect was caused by
    /// network timeout.
    public readonly NetSyncDisconnectReason DisconnectReason;
    /// The ID of the local session. Will be null if the connection is not active.
    public readonly UInt64 SessionId;
    /// A #NetSyncConnectionStatus that defines the different status of the network
    /// synchronization connection. NetSyncConnectionStatus.Unknown - The current
    /// connection status is unknown. NetSyncConnectionStatus.Connecting - The
    /// connection has been started and the process is ongoing.
    /// NetSyncConnectionStatus.Disconnected - The current connection status is
    /// disconnected. NetSyncConnectionStatus.Connected - The connection has been
    /// established.
    public readonly NetSyncConnectionStatus Status;
    /// A `string` represents the unique identifier within the current application
    /// grouping.
    public readonly string ZoneId;


    public NetSyncConnection(IntPtr o)
    {
      ConnectionId = CAPI.ovr_NetSyncConnection_GetConnectionId(o);
      DisconnectReason = CAPI.ovr_NetSyncConnection_GetDisconnectReason(o);
      SessionId = CAPI.ovr_NetSyncConnection_GetSessionId(o);
      Status = CAPI.ovr_NetSyncConnection_GetStatus(o);
      ZoneId = CAPI.ovr_NetSyncConnection_GetZoneId(o);
    }
  }

}

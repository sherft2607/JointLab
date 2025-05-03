// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// It represents the session of Models.NetSyncConnection that allows multiple
  /// clients to connect and communicate with each other in real-time. It
  /// provides a way to manage and facilitate real-time communication and data
  /// synchronization between multiple clients in a networked environment.
  public class NetSyncSession
  {
    /// A `long` integer represents the unique ID of the Models.NetSyncConnection
    /// within which this session exists.
    public readonly long ConnectionId;
    /// `True` if the local session has muted this session.
    public readonly bool Muted;
    /// The cloud networking internal session ID that can uniquely represent this
    /// session within the connection.
    public readonly UInt64 SessionId;
    /// The ovrID of the user behind this session.
    public readonly UInt64 UserId;
    /// A `string` represents the name of the voip group that this session is
    /// subscribed to.
    public readonly string VoipGroup;


    public NetSyncSession(IntPtr o)
    {
      ConnectionId = CAPI.ovr_NetSyncSession_GetConnectionId(o);
      Muted = CAPI.ovr_NetSyncSession_GetMuted(o);
      SessionId = CAPI.ovr_NetSyncSession_GetSessionId(o);
      UserId = CAPI.ovr_NetSyncSession_GetUserId(o);
      VoipGroup = CAPI.ovr_NetSyncSession_GetVoipGroup(o);
    }
  }

  /// Represents a paginated list of Models.NetSyncSession elements. It allows
  /// you to easily access and manipulate the elements in the paginated list,
  /// such as the size of the list and if there is a next page of elements
  /// available.
  public class NetSyncSessionList : DeserializableList<NetSyncSession> {
    /// Instantiates a C# wrapper class that wraps a native list by pointer. Used internally by Platform SDK to wrap the list.
    public NetSyncSessionList(IntPtr a) {
      var count = (int)CAPI.ovr_NetSyncSessionArray_GetSize(a);
      _Data = new List<NetSyncSession>(count);
      for (int i = 0; i < count; i++) {
        _Data.Add(new NetSyncSession(CAPI.ovr_NetSyncSessionArray_GetElement(a, (UIntPtr)i)));
      }

    }

  }
}

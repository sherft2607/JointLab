// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// A Models.NetSyncConnection allows multiple clients to connect and
  /// communicate with each other in real-time. This is the payload from setting
  /// the properties of Models.NetSyncSession. You can retrieve the result
  /// session via NetSyncSetSessionPropertyResult#Session.
  public class NetSyncSetSessionPropertyResult
  {
    /// It contains the Models.NetSyncSession that the operation was modifying.
    public readonly NetSyncSession Session;


    public NetSyncSetSessionPropertyResult(IntPtr o)
    {
      Session = new NetSyncSession(CAPI.ovr_NetSyncSetSessionPropertyResult_GetSession(o));
    }
  }

}

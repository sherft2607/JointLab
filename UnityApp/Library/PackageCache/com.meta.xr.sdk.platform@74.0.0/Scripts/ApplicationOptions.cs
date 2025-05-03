// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// The Application option contains additional configuration to be passed in to
  /// Application.LaunchOtherApp() and Application.InstallAppUpdateAndRelaunch().
  /// It contains 5 fields ApplicationOptions.SetDeeplinkMessage(),
  /// ApplicationOptions.SetDestinationApiName(),
  /// ApplicationOptions.SetLobbySessionId(),
  /// ApplicationOptions.SetMatchSessionId() and ApplicationOptions.SetRoomId().
  public class ApplicationOptions {

    /// Creates a new instance of ::ApplicationOptions which is used to customize the option flow. It returns a handle to the newly created options object, which can be used to set various properties for the options.
    public ApplicationOptions() {
      Handle = CAPI.ovr_ApplicationOptions_Create();
    }

    /// A message to be passed to a launched app, which can be retrieved with
    /// LaunchDetails#DeeplinkMessage.
    public void SetDeeplinkMessage(string value) {
      CAPI.ovr_ApplicationOptions_SetDeeplinkMessage(Handle, value);
    }

    /// If provided, the intended destination to be passed to the launched app,
    /// which can be retrieved with LaunchDetails#DestinationApiName.
    public void SetDestinationApiName(string value) {
      CAPI.ovr_ApplicationOptions_SetDestinationApiName(Handle, value);
    }

    /// If provided, the intended lobby where the launched app should take the
    /// user. All users with the same lobby_session_id should end up grouped
    /// together in the launched app, which can be retrieved with
    /// LaunchDetails#LobbySessionID.
    public void SetLobbySessionId(string value) {
      CAPI.ovr_ApplicationOptions_SetLobbySessionId(Handle, value);
    }

    /// If provided, the intended instance of the destination that a user should be
    /// launched into, which can be retrieved with LaunchDetails#MatchSessionID.
    public void SetMatchSessionId(string value) {
      CAPI.ovr_ApplicationOptions_SetMatchSessionId(Handle, value);
    }

    /// [Deprecated]If provided, the intended room where the launched app should
    /// take the user (all users heading to the same place should have the same
    /// value). A room_id of 0 is INVALID.
    public void SetRoomId(UInt64 value) {
      CAPI.ovr_ApplicationOptions_SetRoomId(Handle, value);
    }


    /// This operator allows you to pass an instance of the ::ApplicationOptions class to native C code as an IntPtr. The operator returns the handle of the options object, or IntPtr.Zero if the object is null.
    public static explicit operator IntPtr(ApplicationOptions options) {
      return options != null ? options.Handle : IntPtr.Zero;
    }

    /// Destroys an existing instance of the ::ApplicationOptions and frees up memory when you're done using it.
    ~ApplicationOptions() {
      CAPI.ovr_ApplicationOptions_Destroy(Handle);
    }

    IntPtr Handle;
  }
}

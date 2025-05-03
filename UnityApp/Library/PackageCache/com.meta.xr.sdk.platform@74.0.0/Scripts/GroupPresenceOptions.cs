// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// The Group Presence Option, to be passed in to GroupPresence.Set(), is a set
  /// of fields that allows developers to specify the presence of a user in a
  /// group/squad/party. It provides a way for developers to create a more
  /// immersive and social experience for their users by allowing them to join
  /// and interact with other users.
  public class GroupPresenceOptions {

    /// Creates a new instance of ::GroupPresenceOptions which is used to customize the option flow. It returns a handle to the newly created options object, which can be used to set various properties for the options.
    public GroupPresenceOptions() {
      Handle = CAPI.ovr_GroupPresenceOptions_Create();
    }

    /// Use GroupPresenceOptions.SetLobbySessionId() or
    /// GroupPresenceOptions.SetMatchSessionId() to specify the session. Use the
    /// deeplink message override for any additional data in whatever format you
    /// wish to aid in bringing users together. If not specified, the
    /// deeplink_message for the user will default to the one on the destination.
    public void SetDeeplinkMessageOverride(string value) {
      CAPI.ovr_GroupPresenceOptions_SetDeeplinkMessageOverride(Handle, value);
    }

    /// This the unique API Name that refers to an in-app destination
    public void SetDestinationApiName(string value) {
      CAPI.ovr_GroupPresenceOptions_SetDestinationApiName(Handle, value);
    }

    /// Set whether or not the person is shown as joinable or not to others. A user
    /// that is joinable can invite others to join them. Set this to false if other
    /// users would not be able to join this user. For example: the current session
    /// is full, or only the host can invite others and the current user is not the
    /// host.
    public void SetIsJoinable(bool value) {
      CAPI.ovr_GroupPresenceOptions_SetIsJoinable(Handle, value);
    }

    /// This is a session that represents a closer group/squad/party of users. It
    /// is expected that all users with the same lobby session id can see or hear
    /// each other. Users with the same lobby session id in their group presence
    /// will show up in the roster and will show up as "Recently Played With" for
    /// future invites if they aren't already Oculus friends. This must be set in
    /// addition to is_joinable being true for a user to use invites.
    public void SetLobbySessionId(string value) {
      CAPI.ovr_GroupPresenceOptions_SetLobbySessionId(Handle, value);
    }

    /// This is a session that represents all the users that are playing a specific
    /// instance of a map, game mode, round, etc. This can include users from
    /// multiple different lobbies that joined together and the users may or may
    /// not remain together after the match is over. Users with the same match
    /// session id in their group presence will not show up in the Roster, but will
    /// show up as "Recently Played with" for future invites.
    public void SetMatchSessionId(string value) {
      CAPI.ovr_GroupPresenceOptions_SetMatchSessionId(Handle, value);
    }


    /// This operator allows you to pass an instance of the ::GroupPresenceOptions class to native C code as an IntPtr. The operator returns the handle of the options object, or IntPtr.Zero if the object is null.
    public static explicit operator IntPtr(GroupPresenceOptions options) {
      return options != null ? options.Handle : IntPtr.Zero;
    }

    /// Destroys an existing instance of the ::GroupPresenceOptions and frees up memory when you're done using it.
    ~GroupPresenceOptions() {
      CAPI.ovr_GroupPresenceOptions_Destroy(Handle);
    }

    IntPtr Handle;
  }
}

// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// An GroupPresenceLeaveIntent represents a user's intent to leave a group
  /// presence which is user's presence to be at a Models.Destination and
  /// session. People with same session id are considered to be co-present
  /// together. Every combination of destination api name, lobby session id and
  /// match session id can uniquely identify a destination.
  public class GroupPresenceLeaveIntent
  {
    /// Destination#ApiName is the unique API Name that refers to an in-app
    /// destination.
    public readonly string DestinationApiName;
    /// This is the match session that the current user wants to leave. A lobby
    /// session is a session ID that represents a closer group/squad/party of
    /// users. It is expected that all users with the same lobby session id can see
    /// or hear each other. Users with the same lobby session id in their group
    /// presence will show up in the roster and will show up as "Recently Played
    /// With" for future invites if they aren't already Oculus friends.
    public readonly string LobbySessionId;
    /// This is the match session that the current user wants to leave. A match
    /// session represents all the users that are playing a specific instance of a
    /// map, game mode, round, etc. This can include users from multiple different
    /// lobbies that joined together and the users may or may not remain together
    /// after the match is over.
    public readonly string MatchSessionId;


    public GroupPresenceLeaveIntent(IntPtr o)
    {
      DestinationApiName = CAPI.ovr_GroupPresenceLeaveIntent_GetDestinationApiName(o);
      LobbySessionId = CAPI.ovr_GroupPresenceLeaveIntent_GetLobbySessionId(o);
      MatchSessionId = CAPI.ovr_GroupPresenceLeaveIntent_GetMatchSessionId(o);
    }
  }

}

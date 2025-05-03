// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

#pragma warning disable 0618

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// An Application Invite contains information about the invite, including the
  /// application ID, destination, activity status, the lobby session id, the
  /// match session id, and recipient's user id or user alias. If the invite is
  /// generated from rich presence, the lobby/match session id will be auto
  /// populated by calling User#PresenceLobbySessionId and
  /// User#PresenceMatchSessionId from the inviter's viewer context.
  public class ApplicationInvite
  {
    /// The destination to which the recipient is invited.
    // May be null. Check before using.
    public readonly Destination DestinationOptional;
    [Obsolete("Deprecated in favor of DestinationOptional")]
    public readonly Destination Destination;
    /// The ID of the application invite.
    public readonly UInt64 ID;
    /// A boolean value indicating whether the invite is still active or not.
    public readonly bool IsActive;
    /// The lobby session id to which the recipient is invited. You can retrieve
    /// this by calling User#PresenceLobbySessionId from the inviter's viewer
    /// context.
    public readonly string LobbySessionId;
    /// The match session id to which the recipient is invited. You can retrieve
    /// this by calling User#PresenceMatchSessionId from the inviter's viewer
    /// context.
    public readonly string MatchSessionId;
    /// The recipient's user information, such as their ID and alias. You can get
    /// the ID of an user by calling User#ID.
    // May be null. Check before using.
    public readonly User RecipientOptional;
    [Obsolete("Deprecated in favor of RecipientOptional")]
    public readonly User Recipient;


    public ApplicationInvite(IntPtr o)
    {
      {
        var pointer = CAPI.ovr_ApplicationInvite_GetDestination(o);
        Destination = new Destination(pointer);
        if (pointer == IntPtr.Zero) {
          DestinationOptional = null;
        } else {
          DestinationOptional = Destination;
        }
      }
      ID = CAPI.ovr_ApplicationInvite_GetID(o);
      IsActive = CAPI.ovr_ApplicationInvite_GetIsActive(o);
      LobbySessionId = CAPI.ovr_ApplicationInvite_GetLobbySessionId(o);
      MatchSessionId = CAPI.ovr_ApplicationInvite_GetMatchSessionId(o);
      {
        var pointer = CAPI.ovr_ApplicationInvite_GetRecipient(o);
        Recipient = new User(pointer);
        if (pointer == IntPtr.Zero) {
          RecipientOptional = null;
        } else {
          RecipientOptional = Recipient;
        }
      }
    }
  }

  /// Represents a paginated list of Models.ApplicationInvite elements. It allows
  /// you to easily access and manipulate the elements in the paginated list,
  /// such as the size of the list and if there is a next page of elements
  /// available.
  public class ApplicationInviteList : DeserializableList<ApplicationInvite> {
    /// Instantiates a C# wrapper class that wraps a native list by pointer. Used internally by Platform SDK to wrap the list.
    public ApplicationInviteList(IntPtr a) {
      var count = (int)CAPI.ovr_ApplicationInviteArray_GetSize(a);
      _Data = new List<ApplicationInvite>(count);
      for (int i = 0; i < count; i++) {
        _Data.Add(new ApplicationInvite(CAPI.ovr_ApplicationInviteArray_GetElement(a, (UIntPtr)i)));
      }

      _NextUrl = CAPI.ovr_ApplicationInviteArray_GetNextUrl(a);
    }

  }
}

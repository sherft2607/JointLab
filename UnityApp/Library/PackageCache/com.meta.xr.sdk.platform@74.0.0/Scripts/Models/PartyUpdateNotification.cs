// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// The notification about updating a party status information to a
  /// participating user. Party Notifications are intended to update the user on
  /// various actions that are occurring in the user's party. It can be retrieved
  /// using Message::MessageType::Notification_Party_PartyUpdate.
  public class PartyUpdateNotification
  {
    /// An enum that specifies the type of action related to the party and user
    /// that this notification holds.
    ///
    /// PartyUpdateAction.Join: The user joined the party.
    ///
    /// PartyUpdateAction.Leave: The user left the party.
    ///
    /// PartyUpdateAction.Invite: The user was invited to the party.
    ///
    /// PartyUpdateAction.Uninvite: The user was uninvited to the party.
    public readonly PartyUpdateAction Action;
    /// The ID of the party that will be updated. This can be retrieved with
    /// Models.PartyID.
    public readonly UInt64 PartyId;
    /// The id of the Models.User who initiated the action that this party update
    /// status notification is in reference to. This is equivalent to User#ID for
    /// this user.
    public readonly UInt64 SenderId;
    /// A timestamp denoting when the party action occurred that this status update
    /// notification refers to.
    public readonly string UpdateTimestamp;
    /// The alias of the Models.User whose party status has changed. This is an
    /// alias that is set by the user.
    public readonly string UserAlias;
    /// The ID of the user, User#ID, whose party status has changed. This is a
    /// unique value for every Models.User.
    public readonly UInt64 UserId;
    /// The displayable name of the Models.User whose party status has changed.
    /// This is equivalent to User#DisplayName for this user.
    public readonly string UserName;


    public PartyUpdateNotification(IntPtr o)
    {
      Action = CAPI.ovr_PartyUpdateNotification_GetAction(o);
      PartyId = CAPI.ovr_PartyUpdateNotification_GetPartyId(o);
      SenderId = CAPI.ovr_PartyUpdateNotification_GetSenderId(o);
      UpdateTimestamp = CAPI.ovr_PartyUpdateNotification_GetUpdateTimestamp(o);
      UserAlias = CAPI.ovr_PartyUpdateNotification_GetUserAlias(o);
      UserId = CAPI.ovr_PartyUpdateNotification_GetUserId(o);
      UserName = CAPI.ovr_PartyUpdateNotification_GetUserName(o);
    }
  }

}

// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

#pragma warning disable 0618

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// Parties allow users to start a voice chat with other members of the party.
  /// Party voice chats persist across apps in VR and users can continue to
  /// interact while navigating between apps. You can use Parties.GetCurrent() to
  /// load the party the current user is in.
  public class Party
  {
    /// A unique identifier of this party.
    public readonly UInt64 ID;
    /// An array of users who are invited to this party. These users are not a part
    /// of the party yet but have been invited.
    // May be null. Check before using.
    public readonly UserList InvitedUsersOptional;
    [Obsolete("Deprecated in favor of InvitedUsersOptional")]
    public readonly UserList InvitedUsers;
    /// The user who initialized this party. It's also the first user who joined
    /// the party. The leader can invite and kick other users.
    // May be null. Check before using.
    public readonly User LeaderOptional;
    [Obsolete("Deprecated in favor of LeaderOptional")]
    public readonly User Leader;
    /// An array that contains the users who are currently in this party. These
    /// users will remain in the party while navigating between apps. Each user in
    /// the party will get the update by
    /// Message::MessageType::Notification_Party_PartyUpdate which is intended to
    /// update the user on various actions that are occurring in the party.
    // May be null. Check before using.
    public readonly UserList UsersOptional;
    [Obsolete("Deprecated in favor of UsersOptional")]
    public readonly UserList Users;


    public Party(IntPtr o)
    {
      ID = CAPI.ovr_Party_GetID(o);
      {
        var pointer = CAPI.ovr_Party_GetInvitedUsers(o);
        InvitedUsers = new UserList(pointer);
        if (pointer == IntPtr.Zero) {
          InvitedUsersOptional = null;
        } else {
          InvitedUsersOptional = InvitedUsers;
        }
      }
      {
        var pointer = CAPI.ovr_Party_GetLeader(o);
        Leader = new User(pointer);
        if (pointer == IntPtr.Zero) {
          LeaderOptional = null;
        } else {
          LeaderOptional = Leader;
        }
      }
      {
        var pointer = CAPI.ovr_Party_GetUsers(o);
        Users = new UserList(pointer);
        if (pointer == IntPtr.Zero) {
          UsersOptional = null;
        } else {
          UsersOptional = Users;
        }
      }
    }
  }

}

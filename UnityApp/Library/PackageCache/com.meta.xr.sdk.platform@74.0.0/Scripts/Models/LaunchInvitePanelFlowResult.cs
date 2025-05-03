// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// Represents the result of a user's interaction with the invite panel, which
  /// is used to send out invitations to other users. It provides a way for
  /// developers to track the results of a user's interaction with the invite
  /// panel, including the list of users who were invited to the session. It can
  /// be retrieved using
  /// Message::MessageType::Notification_GroupPresence_InvitationsSent.
  public class LaunchInvitePanelFlowResult
  {
    /// A list of Models.User who were invited to the session by the user who
    /// interacted with the invite panel.
    public readonly UserList InvitedUsers;


    public LaunchInvitePanelFlowResult(IntPtr o)
    {
      InvitedUsers = new UserList(CAPI.ovr_LaunchInvitePanelFlowResult_GetInvitedUsers(o));
    }
  }

}

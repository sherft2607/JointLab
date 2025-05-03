// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// The result of sending an invite to a user or list of users shows invites
  /// were sent successfully through the resulting array. This is the model
  /// returned in a successful response to the GroupPresence.SendInvites() api.
  public class SendInvitesResult
  {
    /// The list of invites that was sent through GroupPresence.SendInvites(). This
    /// invite list can comprise of friends and recently met users.
    public readonly ApplicationInviteList Invites;


    public SendInvitesResult(IntPtr o)
    {
      Invites = new ApplicationInviteList(CAPI.ovr_SendInvitesResult_GetInvites(o));
    }
  }

}

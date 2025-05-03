// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// It contains information about the user's invitation to others to join their
  /// current session. It is used to provide feedback to the user about whether
  /// their invitations have been successfully sent. It can be retrieved using
  /// GroupPresence.LaunchInvitePanel().
  public class InvitePanelResultInfo
  {
    /// This field indicates whether any invitations have been sent successfully.
    /// It is a boolean value where true means that one or more invites have been
    /// successfully sent, and false indicates that no invites were sent or the
    /// sending process failed.
    public readonly bool InvitesSent;


    public InvitePanelResultInfo(IntPtr o)
    {
      InvitesSent = CAPI.ovr_InvitePanelResultInfo_GetInvitesSent(o);
    }
  }

}

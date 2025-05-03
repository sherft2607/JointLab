// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// The Invite Option, to be passed in to GroupPresence.GetInvitableUsers() and
  /// GroupPresence.LaunchInvitePanel(), is a field that allows developers to
  /// specify a list of suggested users to be added to the invitable users list,
  /// making it easier for users to connect with others and create a more social
  /// experience.
  public class InviteOptions {

    /// Creates a new instance of ::InviteOptions which is used to customize the option flow. It returns a handle to the newly created options object, which can be used to set various properties for the options.
    public InviteOptions() {
      Handle = CAPI.ovr_InviteOptions_Create();
    }

    /// Passing in these users will add them to the invitable users list. From the
    /// GroupPresence.LaunchInvitePanel(), the user can open the invite list, where
    /// the suggested users will be added.
    public void AddSuggestedUser(UInt64 userID) {
      CAPI.ovr_InviteOptions_AddSuggestedUser(Handle, userID);
    }

    /// This method clears the SuggestedUsers options associated with this instance, and the instance will be in its default state.
    public void ClearSuggestedUsers() {
      CAPI.ovr_InviteOptions_ClearSuggestedUsers(Handle);
    }


    /// This operator allows you to pass an instance of the ::InviteOptions class to native C code as an IntPtr. The operator returns the handle of the options object, or IntPtr.Zero if the object is null.
    public static explicit operator IntPtr(InviteOptions options) {
      return options != null ? options.Handle : IntPtr.Zero;
    }

    /// Destroys an existing instance of the ::InviteOptions and frees up memory when you're done using it.
    ~InviteOptions() {
      CAPI.ovr_InviteOptions_Destroy(Handle);
    }

    IntPtr Handle;
  }
}

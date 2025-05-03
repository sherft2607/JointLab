// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// The roster option, to be passed into GroupPresence.LaunchRosterPanel()
  /// which is used to launch the panel displaying the current users in the
  /// roster/party. You can read more about rosters in our
  /// [docs](https://developer.oculus.com/documentation/unity/ps-roster/).
  public class RosterOptions {

    /// Creates a new instance of ::RosterOptions which is used to customize the option flow. It returns a handle to the newly created options object, which can be used to set various properties for the options.
    public RosterOptions() {
      Handle = CAPI.ovr_RosterOptions_Create();
    }

    /// Passing in these users will add them to the invitable users list. From the
    /// GroupPresence.LaunchRosterPanel(), the Models.User can open the invite
    /// list, where the suggested users will be added.
    public void AddSuggestedUser(UInt64 userID) {
      CAPI.ovr_RosterOptions_AddSuggestedUser(Handle, userID);
    }

    /// This method clears the SuggestedUsers options associated with this instance, and the instance will be in its default state.
    public void ClearSuggestedUsers() {
      CAPI.ovr_RosterOptions_ClearSuggestedUsers(Handle);
    }


    /// This operator allows you to pass an instance of the ::RosterOptions class to native C code as an IntPtr. The operator returns the handle of the options object, or IntPtr.Zero if the object is null.
    public static explicit operator IntPtr(RosterOptions options) {
      return options != null ? options.Handle : IntPtr.Zero;
    }

    /// Destroys an existing instance of the ::RosterOptions and frees up memory when you're done using it.
    ~RosterOptions() {
      CAPI.ovr_RosterOptions_Destroy(Handle);
    }

    IntPtr Handle;
  }
}

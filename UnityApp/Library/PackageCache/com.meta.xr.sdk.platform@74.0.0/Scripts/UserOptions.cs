// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// The user_options configuration is used to specify additional settings for
  /// the Models.User. It allows you to customize the response by specifying the
  /// time window, maximum number of users, and service providers for which
  /// linked accounts should be retrieved.
  public class UserOptions {

    /// Creates a new instance of ::UserOptions which is used to customize the option flow. It returns a handle to the newly created options object, which can be used to set various properties for the options.
    public UserOptions() {
      Handle = CAPI.ovr_UserOptions_Create();
    }

    /// This field specifies the maximum number of Models.User that should be
    /// returned in the response.
    public void SetMaxUsers(uint value) {
      CAPI.ovr_UserOptions_SetMaxUsers(Handle, value);
    }

    /// It's an array of #ServiceProvider objects that specifies the service
    /// providers for which linked accounts should be retrieved.
    public void AddServiceProvider(ServiceProvider value) {
      CAPI.ovr_UserOptions_AddServiceProvider(Handle, value);
    }

    /// This method clears the ServiceProviders options associated with this instance, and the instance will be in its default state.
    public void ClearServiceProviders() {
      CAPI.ovr_UserOptions_ClearServiceProviders(Handle);
    }

    /// This field specifies the time window in seconds for which the linked
    /// accounts should be retrieved.
    public void SetTimeWindow(TimeWindow value) {
      CAPI.ovr_UserOptions_SetTimeWindow(Handle, value);
    }


    /// This operator allows you to pass an instance of the ::UserOptions class to native C code as an IntPtr. The operator returns the handle of the options object, or IntPtr.Zero if the object is null.
    public static explicit operator IntPtr(UserOptions options) {
      return options != null ? options.Handle : IntPtr.Zero;
    }

    /// Destroys an existing instance of the ::UserOptions and frees up memory when you're done using it.
    ~UserOptions() {
      CAPI.ovr_UserOptions_Destroy(Handle);
    }

    IntPtr Handle;
  }
}

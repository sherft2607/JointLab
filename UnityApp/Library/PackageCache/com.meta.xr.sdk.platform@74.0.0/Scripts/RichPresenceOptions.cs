// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  public class RichPresenceOptions {

    /// Creates a new instance of ::RichPresenceOptions which is used to customize the option flow. It returns a handle to the newly created options object, which can be used to set various properties for the options.
    public RichPresenceOptions() {
      Handle = CAPI.ovr_RichPresenceOptions_Create();
    }


    /// This operator allows you to pass an instance of the ::RichPresenceOptions class to native C code as an IntPtr. The operator returns the handle of the options object, or IntPtr.Zero if the object is null.
    public static explicit operator IntPtr(RichPresenceOptions options) {
      return options != null ? options.Handle : IntPtr.Zero;
    }

    /// Destroys an existing instance of the ::RichPresenceOptions and frees up memory when you're done using it.
    ~RichPresenceOptions() {
      CAPI.ovr_RichPresenceOptions_Destroy(Handle);
    }

    IntPtr Handle;
  }
}

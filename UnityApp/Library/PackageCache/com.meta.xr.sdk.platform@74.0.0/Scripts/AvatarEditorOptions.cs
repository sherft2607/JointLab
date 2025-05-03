// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// The Avatar Editor Option is a feature that allows users to create and
  /// customize their avatars. It is launched by the Avatar.LaunchAvatarEditor()
  /// request and provides a way for users to specify the source of the request,
  /// allowing for more flexibility and customization in the avatar creation
  /// process.
  public class AvatarEditorOptions {

    /// Creates a new instance of ::AvatarEditorOptions which is used to customize the option flow. It returns a handle to the newly created options object, which can be used to set various properties for the options.
    public AvatarEditorOptions() {
      Handle = CAPI.ovr_AvatarEditorOptions_Create();
    }

    /// Optional override for where the request is coming from. This field allows
    /// you to specify the source of the request in the launched editor by calling
    /// Avatar.LaunchAvatarEditor(), which can be useful in cases where you want to
    /// track or identify the origin of the request.
    public void SetSourceOverride(string value) {
      CAPI.ovr_AvatarEditorOptions_SetSourceOverride(Handle, value);
    }


    /// This operator allows you to pass an instance of the ::AvatarEditorOptions class to native C code as an IntPtr. The operator returns the handle of the options object, or IntPtr.Zero if the object is null.
    public static explicit operator IntPtr(AvatarEditorOptions options) {
      return options != null ? options.Handle : IntPtr.Zero;
    }

    /// Destroys an existing instance of the ::AvatarEditorOptions and frees up memory when you're done using it.
    ~AvatarEditorOptions() {
      CAPI.ovr_AvatarEditorOptions_Destroy(Handle);
    }

    IntPtr Handle;
  }
}

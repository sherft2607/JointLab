// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// The multiplayer error option is a feature that allows developers to display
  /// general error messages to Models.User in invokable error dialogs. This
  /// option is particularly useful in multiplayer games or applications where
  /// errors can occur due to various reasons such as network connectivity
  /// issues, server downtime, or other technical problems. Read more about error
  /// dialogues in our
  /// [docs](https://developer.oculus.com/documentation/unity/ps-error-dialogs/).
  public class MultiplayerErrorOptions {

    /// Creates a new instance of ::MultiplayerErrorOptions which is used to customize the option flow. It returns a handle to the newly created options object, which can be used to set various properties for the options.
    public MultiplayerErrorOptions() {
      Handle = CAPI.ovr_MultiplayerErrorOptions_Create();
    }

    /// Key associated with the predefined error message to be shown to users.
    ///
    /// Key List:
    ///
    /// - MultiplayerErrorErrorKey.DestinationUnavailable
    ///
    /// - MultiplayerErrorErrorKey.DlcRequired
    ///
    /// - MultiplayerErrorErrorKey.General
    ///
    /// - MultiplayerErrorErrorKey.GroupFull
    ///
    /// - MultiplayerErrorErrorKey.InviterNotJoinable
    ///
    /// - MultiplayerErrorErrorKey.LevelNotHighEnough
    ///
    /// - MultiplayerErrorErrorKey.LevelNotUnlocked
    ///
    /// - MultiplayerErrorErrorKey.NetworkTimeout
    ///
    /// - MultiplayerErrorErrorKey.NoLongerAvailable
    ///
    /// - MultiplayerErrorErrorKey.UpdateRequired
    ///
    /// - MultiplayerErrorErrorKey.TutorialRequired
    public void SetErrorKey(MultiplayerErrorErrorKey value) {
      CAPI.ovr_MultiplayerErrorOptions_SetErrorKey(Handle, value);
    }


    /// This operator allows you to pass an instance of the ::MultiplayerErrorOptions class to native C code as an IntPtr. The operator returns the handle of the options object, or IntPtr.Zero if the object is null.
    public static explicit operator IntPtr(MultiplayerErrorOptions options) {
      return options != null ? options.Handle : IntPtr.Zero;
    }

    /// Destroys an existing instance of the ::MultiplayerErrorOptions and frees up memory when you're done using it.
    ~MultiplayerErrorOptions() {
      CAPI.ovr_MultiplayerErrorOptions_Destroy(Handle);
    }

    IntPtr Handle;
  }
}

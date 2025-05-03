// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// The voip_options configuration is used to specify additional settings for
  /// the VoIP transmission for a Models.User. It consists of two primary options
  /// which include using DTX for transmission and setting a maxmimum bitrate for
  /// the network connection. You can read more about VoIP
  /// [here](https://developer.oculus.com/documentation/unity/ps-parties/#voip-
  /// options).
  public class VoipOptions {

    /// Creates a new instance of ::VoipOptions which is used to customize the option flow. It returns a handle to the newly created options object, which can be used to set various properties for the options.
    public VoipOptions() {
      Handle = CAPI.ovr_VoipOptions_Create();
    }

    /// Sets the maximum average bitrate the audio codec should use. Higher
    /// bitrates will increase audio quality at the expense of increased network
    /// usage. Use a lower bitrate if you think the quality is good but the network
    /// usage is too much. Use a higher bitrate if you think the quality is bad and
    /// you can afford to have a large streaming bitrate.
    public void SetBitrateForNewConnections(VoipBitrate value) {
      CAPI.ovr_VoipOptions_SetBitrateForNewConnections(Handle, value);
    }

    /// Set the opus codec to use discontinous transmission (DTX). DTX only
    /// transmits data when a person is speaking. Setting this to
    /// VoipDtxState.Enabled takes advantage of the fact that in a two-way
    /// converstation each individual speaks for less than half the time. Enabling
    /// DTX will conserve battery power and reduce transmission rate when a pause
    /// in the voice chat is detected.
    public void SetCreateNewConnectionUseDtx(VoipDtxState value) {
      CAPI.ovr_VoipOptions_SetCreateNewConnectionUseDtx(Handle, value);
    }


    /// This operator allows you to pass an instance of the ::VoipOptions class to native C code as an IntPtr. The operator returns the handle of the options object, or IntPtr.Zero if the object is null.
    public static explicit operator IntPtr(VoipOptions options) {
      return options != null ? options.Handle : IntPtr.Zero;
    }

    /// Destroys an existing instance of the ::VoipOptions and frees up memory when you're done using it.
    ~VoipOptions() {
      CAPI.ovr_VoipOptions_Destroy(Handle);
    }

    IntPtr Handle;
  }
}

// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// A PlatformInitialize object defines an attempt at initializing the Platform
  /// SDK. It contains the result of attempting to initialize the platform. The
  /// different types of initialization results are #PlatformInitializeResult.
  public class PlatformInitialize
  {
    /// The result of attempting to initialize the platform:
    ///
    /// PlatformInitializeResult.Success - Platform SDK initialization succeeded.
    ///
    /// PlatformInitializeResult.Uninitialized - Platform SDK was not initialized.
    ///
    /// PlatformInitializeResult.PreLoaded - Platform SDK failed to initialize
    /// because the pre-loaded module was on a different path than the validated
    /// library.
    ///
    /// PlatformInitializeResult.FileInvalid - Platform SDK files failed to load.
    ///
    /// PlatformInitializeResult.SignatureInvalid - Platform SDK failed to
    /// initialize due to an invalid signature in the signed certificate.
    ///
    /// PlatformInitializeResult.UnableToVerify - Platform SDK failed to verify the
    /// application's signature during initialization.
    ///
    /// PlatformInitializeResult.VersionMismatch - There was a mismatch between the
    /// version of Platform SDK used by the application and the version installed
    /// on the user's device.
    ///
    /// PlatformInitializeResult.InvalidCredentials - Platform SDK failed to
    /// initialize because the user had an invalid account access token.
    ///
    /// PlatformInitializeResult.NotEntitled - Platform SDK failed to initialize
    /// because the user does not have the application entitlement.
    public readonly PlatformInitializeResult Result;


    public PlatformInitialize(IntPtr o)
    {
      Result = CAPI.ovr_PlatformInitialize_GetResult(o);
    }
  }

}

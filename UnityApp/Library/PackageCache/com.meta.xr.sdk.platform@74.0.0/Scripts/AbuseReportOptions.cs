// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// The Abuse Report Options provide a way for developers to customize the
  /// reporting flow and specify the type of content being reported, which can be
  /// either a AbuseReportType.User or an AbuseReportType.Object, helping to
  /// maintain a safe and respectful community within their application.
  public class AbuseReportOptions {

    /// Creates a new instance of ::AbuseReportOptions which is used to customize the option flow. It returns a handle to the newly created options object, which can be used to set various properties for the options.
    public AbuseReportOptions() {
      Handle = CAPI.ovr_AbuseReportOptions_Create();
    }

    /// Set whether or not to show the user selection step. If the reported object
    /// is a user, they can choose to block the reported user from further
    /// interactions within the platform.
    public void SetPreventPeopleChooser(bool value) {
      CAPI.ovr_AbuseReportOptions_SetPreventPeopleChooser(Handle, value);
    }

    /// The intended entity type #AbuseReportType being reported, it can be either
    /// a user AbuseReportType.User or an object/content AbuseReportType.Object.
    public void SetReportType(AbuseReportType value) {
      CAPI.ovr_AbuseReportOptions_SetReportType(Handle, value);
    }


    /// This operator allows you to pass an instance of the ::AbuseReportOptions class to native C code as an IntPtr. The operator returns the handle of the options object, or IntPtr.Zero if the object is null.
    public static explicit operator IntPtr(AbuseReportOptions options) {
      return options != null ? options.Handle : IntPtr.Zero;
    }

    /// Destroys an existing instance of the ::AbuseReportOptions and frees up memory when you're done using it.
    ~AbuseReportOptions() {
      CAPI.ovr_AbuseReportOptions_Destroy(Handle);
    }

    IntPtr Handle;
  }
}

// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// The Advanced Abuse Report Options provides a way for developers to
  /// customize the reporting flow and specify the type of content being
  /// reported, which can be either a AbuseReportType.User or an
  /// AbuseReportType.Object, helping to maintain a safe and respectful community
  /// within their application.
  public class AdvancedAbuseReportOptions {

    /// Creates a new instance of ::AdvancedAbuseReportOptions which is used to customize the option flow. It returns a handle to the newly created options object, which can be used to set various properties for the options.
    public AdvancedAbuseReportOptions() {
      Handle = CAPI.ovr_AdvancedAbuseReportOptions_Create();
    }

    /// This field is intended to allow developers to pass custom metadata through
    /// the report flow. The metadata passed through is included with the report
    /// received by the developer.
    public void SetDeveloperDefinedContext(string key, string value) {
      CAPI.ovr_AdvancedAbuseReportOptions_SetDeveloperDefinedContextString(Handle, key, value);
    }

    /// This method clears the DeveloperDefinedContext options associated with this instance, and the instance will be in its default state.
    public void ClearDeveloperDefinedContext() {
      CAPI.ovr_AdvancedAbuseReportOptions_ClearDeveloperDefinedContext(Handle);
    }

    /// If #AbuseReportType is AbuseReportType.Object, a string representing the
    /// type of content being reported. This should correspond to the object_type
    /// string used in the UI.
    public void SetObjectType(string value) {
      CAPI.ovr_AdvancedAbuseReportOptions_SetObjectType(Handle, value);
    }

    /// The intended entity type #AbuseReportType being reported, it can be either
    /// a user AbuseReportType.User or an object/content AbuseReportType.Object.
    public void SetReportType(AbuseReportType value) {
      CAPI.ovr_AdvancedAbuseReportOptions_SetReportType(Handle, value);
    }

    /// Provide a list of users to suggest for reporting. This list should include
    /// users that the reporter has recently interacted with to aid them in
    /// selecting the right user to report.
    public void AddSuggestedUser(UInt64 userID) {
      CAPI.ovr_AdvancedAbuseReportOptions_AddSuggestedUser(Handle, userID);
    }

    /// This method clears the SuggestedUsers options associated with this instance, and the instance will be in its default state.
    public void ClearSuggestedUsers() {
      CAPI.ovr_AdvancedAbuseReportOptions_ClearSuggestedUsers(Handle);
    }

    /// The video mode #AbuseReportVideoMode controls whether or not the abuse
    /// report flow should collect evidence and whether it is optional or not.
    /// AbuseReportVideoMode.Collect requires video evidence to be provided by the
    /// user. AbuseReportVideoMode.Optional presents the user with the option to
    /// provide video evidence. AbuseReportVideoMode.Skip bypasses the video
    /// evidence collection step altogether.
    public void SetVideoMode(AbuseReportVideoMode value) {
      CAPI.ovr_AdvancedAbuseReportOptions_SetVideoMode(Handle, value);
    }


    /// This operator allows you to pass an instance of the ::AdvancedAbuseReportOptions class to native C code as an IntPtr. The operator returns the handle of the options object, or IntPtr.Zero if the object is null.
    public static explicit operator IntPtr(AdvancedAbuseReportOptions options) {
      return options != null ? options.Handle : IntPtr.Zero;
    }

    /// Destroys an existing instance of the ::AdvancedAbuseReportOptions and frees up memory when you're done using it.
    ~AdvancedAbuseReportOptions() {
      CAPI.ovr_AdvancedAbuseReportOptions_Destroy(Handle);
    }

    IntPtr Handle;
  }
}

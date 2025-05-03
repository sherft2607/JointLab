// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// Results of the launched report dialog including resulting report ID and
  /// user action. It can be retrieved using AbuseReport.ReportRequestHandled()
  /// to handle the result of a report request. Learn more about our [User
  /// Reporting Service](https://developer.oculus.com/resources/reporting-
  /// service#faq_856753478660534).
  public class LaunchReportFlowResult
  {
    /// A `boolean` which indicates whether the viewer chose to cancel the report
    /// flow before completing it.
    public readonly bool DidCancel;
    /// ID of the report created by the user. It's optional and may not be present
    /// if the user cancelled the report flow. Learn more about the [user reporting
    /// plugin](https://developer.oculus.com/resources/reporting-plugin) in our
    /// website.
    public readonly UInt64 UserReportId;


    public LaunchReportFlowResult(IntPtr o)
    {
      DidCancel = CAPI.ovr_LaunchReportFlowResult_GetDidCancel(o);
      UserReportId = CAPI.ovr_LaunchReportFlowResult_GetUserReportId(o);
    }
  }

}

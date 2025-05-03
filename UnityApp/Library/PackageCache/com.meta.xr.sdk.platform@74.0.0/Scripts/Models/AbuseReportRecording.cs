// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// A video recording evidence that can be used to collect video evidence when
  /// reporting abusive behavior or content within a platform. More details are
  /// available in our [User Reporting Service Virtual Reality Check
  /// guideline](https://developer.oculus.com/resources/reporting-service/)
  public class AbuseReportRecording
  {
    /// A unique UUID associated with the Abuse Report recording. It can be
    /// retrieved using LaunchReportFlowResult#UserReportId
    public readonly string RecordingUuid;


    public AbuseReportRecording(IntPtr o)
    {
      RecordingUuid = CAPI.ovr_AbuseReportRecording_GetRecordingUuid(o);
    }
  }

}

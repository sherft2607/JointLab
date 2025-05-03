// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform
{

  using Description = System.ComponentModel.DescriptionAttribute;

  /// Determines under what circumstances the frontend UI will attempt to gather
  /// video evidence to support a report, and the object_type is defined in
  /// AdvancedAbuseReportOptions.SetObjectType(). This setting is crucial in
  /// ensuring that the reporting process is thorough and accurate, as video
  /// evidence can provide valuable context and proof of misconduct.
  public enum AbuseReportVideoMode : int
  {
    [Description("UNKNOWN")]
    Unknown,

    /// The UI will collect video evidence if the object_type supports it, the
    /// object_type is defined in AdvancedAbuseReportOptions.SetObjectType().
    [Description("COLLECT")]
    Collect,

    /// The UI will try to collect video evidence if the object_type supports it,
    /// but will allow the user to skip that step if they wish.
    [Description("OPTIONAL")]
    Optional,

    /// The UI will not collect video evidence.
    [Description("SKIP")]
    Skip,

  }

}

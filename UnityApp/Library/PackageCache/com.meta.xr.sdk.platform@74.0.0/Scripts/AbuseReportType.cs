// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform
{

  using Description = System.ComponentModel.DescriptionAttribute;

  /// Represents the type of abuse report, can be categorized as either
  /// concerning a user, an object, or being unknown. It can be specified as a
  /// field in the AbuseReportOptions.SetReportType() option.
  public enum AbuseReportType : int
  {
    [Description("UNKNOWN")]
    Unknown,

    /// This refers to reports that are related to non-user entities, such as a
    /// virtual environment or an inanimate object within a platform. An example
    /// provided is a report concerning a "world," which could be a virtual space
    /// or environment.
    [Description("OBJECT")]
    Object,

    /// This category is used for reports that directly involve a user's actions or
    /// behavior. This could include reports on harassment, inappropriate behavior,
    /// or other violations that are directly linked to a user's conduct on a
    /// platform.
    [Description("USER")]
    User,

  }

}

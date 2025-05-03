// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform
{

  using Description = System.ComponentModel.DescriptionAttribute;

  /// An application user can report abusive behavior or content following the
  /// in-app reporting flow. This report request response represents the possible
  /// states that the app can respond to the platform notification, i.e., the
  /// running application indicates whether they want to show their in-app
  /// reporting flow or that they choose to ignore the request via
  /// AbuseReport.ReportRequestHandled().
  public enum ReportRequestResponse : int
  {
    [Description("UNKNOWN")]
    Unknown,

    /// This 'enum' member represents the response to the platform notification
    /// that the in-app reporting flow request is handled.
    [Description("HANDLED")]
    Handled,

    /// This 'enum' member represents the response to the platform notification
    /// that the in-app reporting flow request is not handled.
    [Description("UNHANDLED")]
    Unhandled,

    /// This 'enum' member represents the response to the platform notification
    /// that the in-app reporting flow is unavailable or non-existent.
    [Description("UNAVAILABLE")]
    Unavailable,

  }

}

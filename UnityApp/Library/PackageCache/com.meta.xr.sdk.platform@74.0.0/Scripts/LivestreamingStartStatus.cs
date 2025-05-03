// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform
{

  using Description = System.ComponentModel.DescriptionAttribute;

  /// It's an enum that represents the possible outcomes of starting a
  /// livestreaming session. This allows the API to return a standardized and
  /// well-defined set of values to indicate the outcome of starting a
  /// livestreaming session. It can be used in
  /// LivestreamingStartResult#StreamingResult as type information.
  public enum LivestreamingStartStatus : int
  {
    /// This member represents a successful start of the livestreaming session.
    [Description("SUCCESS")]
    Success = 1,

    [Description("UNKNOWN")]
    Unknown = 0,

    /// This member represents an error where the package was not set during the
    /// livestreaming start process.
    [Description("NO_PACKAGE_SET")]
    NoPackageSet = -1,

    /// This member represents an error where Facebook Connect was not enabled
    /// during the livestreaming start process.
    [Description("NO_FB_CONNECT")]
    NoFbConnect = -2,

    /// This member represents an error where a session ID was not provided during
    /// the livestreaming start process.
    [Description("NO_SESSION_ID")]
    NoSessionId = -3,

    /// This member represents an error where required parameters were missing
    /// during the livestreaming start process.
    [Description("MISSING_PARAMETERS")]
    MissingParameters = -4,

  }

}

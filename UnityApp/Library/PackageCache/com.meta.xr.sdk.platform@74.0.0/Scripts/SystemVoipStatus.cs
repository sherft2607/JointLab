// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform
{

  using Description = System.ComponentModel.DescriptionAttribute;

  /// The system voip status is the priamary status in every
  /// Models.SystemVoipState. The system voip state is used in parties and
  /// horizon home to describe the current state of the input/output for voip in
  /// an application. You can read more about system voip
  /// [here](https://developer.oculus.com/documentation/unity/ps-parties/#voip-
  /// options).
  public enum SystemVoipStatus : int
  {
    [Description("UNKNOWN")]
    Unknown,

    [Description("UNAVAILABLE")]
    Unavailable,

    [Description("SUPPRESSED")]
    Suppressed,

    [Description("ACTIVE")]
    Active,

  }

}

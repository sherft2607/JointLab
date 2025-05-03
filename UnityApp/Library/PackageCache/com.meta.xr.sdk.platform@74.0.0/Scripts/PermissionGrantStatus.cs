// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform
{

  using Description = System.ComponentModel.DescriptionAttribute;

  /// This `enum` represents the all possible statuses of a premission request.
  /// For example, if an user wants to participate in a challenge, the user may
  /// request the permission to join the Models.Challenge first. If the
  /// permission grant status is granted, the user can call Challenges.Join() to
  /// join the challenge.
  public enum PermissionGrantStatus : int
  {
    [Description("UNKNOWN")]
    Unknown,

    /// This `enum` member indicates the status of the permission grant was
    /// approved.
    [Description("GRANTED")]
    Granted,

    /// This `enum` member indicates the status of the permission grant was
    /// rejected.
    [Description("DENIED")]
    Denied,

    /// This `enum` member indicates the status of the permission grant was
    /// blocked.
    [Description("BLOCKED")]
    Blocked,

  }

}

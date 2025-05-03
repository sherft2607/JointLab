// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform
{

  using Description = System.ComponentModel.DescriptionAttribute;

  /// Describe the current status of the user and it can be retrieved with
  /// User#PresenceStatus.
  public enum UserPresenceStatus : int
  {
    [Description("UNKNOWN")]
    Unknown,

    /// The user status is currently online.
    [Description("ONLINE")]
    Online,

    /// The user status is currently offline.
    [Description("OFFLINE")]
    Offline,

  }

}

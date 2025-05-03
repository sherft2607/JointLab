// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform
{

  using Description = System.ComponentModel.DescriptionAttribute;

  /// A user can have multiple sdk accounts associated with it. An
  /// Models.SdkAccount represents the oculus user and the particular x-account
  /// that is linked to the user. SDK accounts can be retrived for a particular
  /// user by calling Users.GetSdkAccounts(). This enumeration represents the
  /// specific type of SDK account that is associated.
  public enum SdkAccountType : int
  {
    [Description("UNKNOWN")]
    Unknown,

    [Description("OCULUS")]
    Oculus,

    [Description("FACEBOOK_GAMEROOM")]
    FacebookGameroom,

  }

}

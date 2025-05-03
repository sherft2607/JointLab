// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform
{

  using Description = System.ComponentModel.DescriptionAttribute;

  /// The service provider enum describes the specific provider that is
  /// associated with the Models.LinkedAccount of a Models.User. Linked accounts
  /// for users represent the third party identities that are used for services
  /// or apps in association with user.
  public enum ServiceProvider : int
  {
    [Description("UNKNOWN")]
    Unknown,

    [Description("DROPBOX")]
    Dropbox,

    [Description("FACEBOOK")]
    Facebook,

    [Description("GOOGLE")]
    Google,

    [Description("INSTAGRAM")]
    Instagram,

    [Description("REMOTE_MEDIA")]
    RemoteMedia,

  }

}

// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform
{

  using Description = System.ComponentModel.DescriptionAttribute;

  /// It's an enum that defines several different types of launches for an
  /// application. It provides a way to differentiate between different types of
  /// launches and to access additional information about the launch details. It
  /// can be retrieved using LaunchDetails#LaunchType to get the launch type
  /// information of an application.
  public enum LaunchType : int
  {
    [Description("UNKNOWN")]
    Unknown,

    /// Normal launch from the user's library
    [Description("NORMAL")]
    Normal,

    /// Launch from the user accepting an invite. Check
    /// LaunchDetails#LobbySessionID, LaunchDetails#MatchSessionID,
    /// LaunchDetails#DestinationApiName and LaunchDetails#DeeplinkMessage.
    [Description("INVITE")]
    Invite,

    /// DEPRECATED
    [Description("COORDINATED")]
    Coordinated,

    /// Launched from Application.LaunchOtherApp(). Check
    /// LaunchDetails#LaunchSource and LaunchDetails#DeeplinkMessage.
    [Description("DEEPLINK")]
    Deeplink,

  }

}

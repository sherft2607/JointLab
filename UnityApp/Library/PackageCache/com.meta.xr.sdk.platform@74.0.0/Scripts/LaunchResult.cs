// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform
{

  using Description = System.ComponentModel.DescriptionAttribute;

  /// An enum that specifies the whether the attempt to launch this application
  /// via a deeplink was successful. The value is meant to be reported after a
  /// deeplink by calling ApplicationLifecycle.LogDeeplinkResult()
  public enum LaunchResult : int
  {
    [Description("UNKNOWN")]
    Unknown,

    /// The application launched successfully.
    [Description("SUCCESS")]
    Success,

    /// The application launch failed because the room was full.
    [Description("FAILED_ROOM_FULL")]
    FailedRoomFull,

    /// The application launch failed because the game has already started.
    [Description("FAILED_GAME_ALREADY_STARTED")]
    FailedGameAlreadyStarted,

    /// The appplicatin launch failed because the room couldn't be found.
    [Description("FAILED_ROOM_NOT_FOUND")]
    FailedRoomNotFound,

    /// The application launch failed because the user declined the invitation.
    [Description("FAILED_USER_DECLINED")]
    FailedUserDeclined,

    /// The application launch failed due to some other reason.
    [Description("FAILED_OTHER_REASON")]
    FailedOtherReason,

  }

}

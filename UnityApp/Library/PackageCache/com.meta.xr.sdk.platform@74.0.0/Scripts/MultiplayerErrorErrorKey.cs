// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform
{

  using Description = System.ComponentModel.DescriptionAttribute;

  /// Possible keys of the errors which could occurred when using the
  /// [multiplayer features](https://developer.oculus.com/documentation/unity/ps-
  /// multiplayer-overview/). e.g., you may need to use `destination_unavailable`
  /// for destinations feature and use `inviter_not_joinable` for invite feature
  /// and so on.
  public enum MultiplayerErrorErrorKey : int
  {
    [Description("UNKNOWN")]
    Unknown,

    /// This error key will be used to tell the user that the travel destination is
    /// not available any more.
    [Description("DESTINATION_UNAVAILABLE")]
    DestinationUnavailable,

    /// This error will tell the user that the downloadable content will be needed.
    [Description("DLC_REQUIRED")]
    DlcRequired,

    /// This error key will be used in a broad range of general errors which are
    /// not be covered by the members of the enum.
    [Description("GENERAL")]
    General,

    /// This error key may be used to explain to the user the reason why she/he
    /// failed in joining a group.
    [Description("GROUP_FULL")]
    GroupFull,

    /// This error key will be used in explaining why an inviter cannot invite a
    /// recepient successfully. The group presence can be set to joinable by using
    /// GroupPresence.SetIsJoinable().
    [Description("INVITER_NOT_JOINABLE")]
    InviterNotJoinable,

    /// Certain features will not be available to the user in the app because the
    /// user's level does not reach to certain level.
    [Description("LEVEL_NOT_HIGH_ENOUGH")]
    LevelNotHighEnough,

    /// This error key may be used to explain to the user the failure was occurred
    /// becasue some level has not been reached.
    [Description("LEVEL_NOT_UNLOCKED")]
    LevelNotUnlocked,

    /// When the predefined network timeout has reached, the ongoing activity would
    /// be stopped. The dialog will use this error key to give the user the
    /// information.
    [Description("NETWORK_TIMEOUT")]
    NetworkTimeout,

    [Description("NO_LONGER_AVAILABLE")]
    NoLongerAvailable,

    [Description("UPDATE_REQUIRED")]
    UpdateRequired,

    [Description("TUTORIAL_REQUIRED")]
    TutorialRequired,

  }

}

// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform
{

  using Description = System.ComponentModel.DescriptionAttribute;

  /// An enum that specifies the type of action related to the party and the
  /// user. For example, PartyUpdateNotification#Action contains the information
  /// about the user joined or left the party or the user was invited or
  /// uninvited to the party.
  public enum PartyUpdateAction : int
  {
    [Description("UNKNOWN")]
    Unknown,

    /// This `enum` member indicates the user joined the party.
    [Description("Join")]
    Join,

    /// This `enum` member indicates the user left the party.
    [Description("Leave")]
    Leave,

    /// This `enum` member indicates the user was invited to the party.
    [Description("Invite")]
    Invite,

    /// This `enum` member indicates the user was uninvited to the party.
    [Description("Uninvite")]
    Uninvite,

  }

}

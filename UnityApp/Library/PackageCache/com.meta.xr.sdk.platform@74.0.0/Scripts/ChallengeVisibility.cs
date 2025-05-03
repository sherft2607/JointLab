// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform
{

  using Description = System.ComponentModel.DescriptionAttribute;

  /// The visibility of the challenge. A challenge may be invite-only, public, or
  /// private. The visibility setting determines who can see and participate in
  /// the challenge. See more details of Challenges in
  /// [here](https://developer.oculus.com/documentation/unity/ps-challenges/).
  public enum ChallengeVisibility : int
  {
    [Description("UNKNOWN")]
    Unknown,

    /// Only those invited can participate in it. Everyone can see it, but only
    /// those with an invitation can joinand participate in the challenge. This
    /// setting is useful for challenges that are meant to be exclusive or for a
    /// specific group of people.
    [Description("INVITE_ONLY")]
    InviteOnly,

    /// Everyone can participate and see this challenge. This setting makes the
    /// challenge open to anyone who wants to join, and everyone can see the
    /// challenge details and progress. This setting is useful for challenges that
    /// are meant to be open and inclusive.
    [Description("PUBLIC")]
    Public,

    /// Only those invited can participate and see this challenge. This setting
    /// makes the challenge invisible to everyone except those who have been
    /// explicitly invited. Only those with an invitation can see the challenge
    /// details and progress, and only they can participate. This setting is useful
    /// for challenges that are meant to be highly exclusive or confidential.
    [Description("PRIVATE")]
    Private,

  }

}

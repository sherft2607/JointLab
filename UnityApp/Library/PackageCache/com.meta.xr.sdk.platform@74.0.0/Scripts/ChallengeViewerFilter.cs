// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform
{

  using Description = System.ComponentModel.DescriptionAttribute;

  /// The available filtering options on the Models.Challenge returned by
  /// Challenges.GetList(). These filters allow users to customize their search
  /// results and retrieve only the challenges that meet specific criteria. See
  /// more details of Challenges in
  /// [here](https://developer.oculus.com/documentation/unity/ps-challenges/).
  public enum ChallengeViewerFilter : int
  {
    [Description("UNKNOWN")]
    Unknown,

    /// Returns all public ((ChallengeVisibility.Public)) and invite-only
    /// (ChallengeVisibility.InviteOnly) Models.Challenges in which the user is a
    /// participant or invitee. Excludes private (ChallengeVisibility.Private)
    /// challenges. This filter is useful for users who want to see all challenges
    /// they are involved in, regardless of their visibility settings.
    [Description("ALL_VISIBLE")]
    AllVisible,

    /// Returns challenges in which the user is a participant. This filter is
    /// useful for users who want to see only the challenges they are actively
    /// participating in.
    [Description("PARTICIPATING")]
    Participating,

    /// Returns challenges that the user has been invited to. This filter is useful
    /// for users who want to see only the challenges they have been explicitly
    /// invited to.
    [Description("INVITED")]
    Invited,

    /// Returns challenges the user is either participating in or invited to. This
    /// filter is useful for users who want to see all challenges they are involved
    /// in, whether as a participant or an invitee.
    [Description("PARTICIPATING_OR_INVITED")]
    ParticipatingOrInvited,

  }

}

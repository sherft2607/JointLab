// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform
{

  using Description = System.ComponentModel.DescriptionAttribute;

  /// An enum that allows you to specify the starting point for the leaderboard
  /// entries. It can be used as a params in Leaderboards.GetEntries() to the
  /// starting point for the leaderboard entries that are returned in the
  /// response.
  public enum LeaderboardStartAt : int
  {
    /// This value indicates that the leaderboard entries should start at the top
    /// of the leaderboard.
    [Description("TOP")]
    Top,

    /// This value indicates that the leaderboard entries should start at the
    /// viewer's position on the leaderboard
    [Description("CENTERED_ON_VIEWER")]
    CenteredOnViewer,

    /// This value indicates that the leaderboard entries should start at the
    /// viewer's position on the leaderboard, or at the top of the leaderboard if
    /// the viewer is not present.
    [Description("CENTERED_ON_VIEWER_OR_TOP")]
    CenteredOnViewerOrTop,

    [Description("UNKNOWN")]
    Unknown,

  }

}

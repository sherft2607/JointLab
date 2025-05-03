// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// The livestreaming video statistics represents the statistics info about a
  /// livestreaming video in your app. The statistics include the total number of
  /// views, the number of reactions and the number of comments. You can retrieve
  /// the information about whether your comments are visible from
  /// LivestreamingStatus#CommentsVisible.
  public class LivestreamingVideoStats
  {
    /// An 'integer' represents the information about the total number of comments
    /// left for your livestream video.
    public readonly int CommentCount;
    /// An 'integer' represents the information about the total number of reactions
    /// your livestream video received.
    public readonly int ReactionCount;
    /// This field gives the information about the total number of views of your
    /// livestream video.
    public readonly string TotalViews;


    public LivestreamingVideoStats(IntPtr o)
    {
      CommentCount = CAPI.ovr_LivestreamingVideoStats_GetCommentCount(o);
      ReactionCount = CAPI.ovr_LivestreamingVideoStats_GetReactionCount(o);
      TotalViews = CAPI.ovr_LivestreamingVideoStats_GetTotalViews(o);
    }
  }

}

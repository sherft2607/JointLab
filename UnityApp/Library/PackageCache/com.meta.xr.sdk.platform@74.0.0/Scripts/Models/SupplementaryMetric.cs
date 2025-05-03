// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// This is a supplemental piece of data that is used for a single write to
  /// leaderboard entries. This metric is used for tiebreaker scenarios. You can
  /// make such an entry by using
  /// Leaderboards.WriteEntryWithSupplementaryMetric()
  public class SupplementaryMetric
  {
    /// The ID of the leaderboard that this supplementary metric belongs to. This
    /// is the unique value for every Models.Leaderboard.
    public readonly UInt64 ID;
    /// This is the metric that is used to determine tiebreaks.
    public readonly long Metric;


    public SupplementaryMetric(IntPtr o)
    {
      ID = CAPI.ovr_SupplementaryMetric_GetID(o);
      Metric = CAPI.ovr_SupplementaryMetric_GetMetric(o);
    }
  }

}

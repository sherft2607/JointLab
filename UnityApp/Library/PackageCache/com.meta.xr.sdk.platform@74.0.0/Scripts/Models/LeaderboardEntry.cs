// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

#pragma warning disable 0618

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// A leaderboard entry object contains information about the Models.User who
  /// made the entry, their score, and other relevant details in the leaderboard.
  /// It provides a way for a game to keep track of players and their scores in
  /// relation to other. A single leaderboard entry can be written by
  /// Leaderboards.WriteEntry(). A block of leaderboard entries can be retrieved
  /// using Leaderboards.GetEntries(). Visit our
  /// [website](https://developer.oculus.com/documentation/unity/ps-
  /// leaderboards/) for more information about the leaderboard entry.
  public class LeaderboardEntry
  {
    /// The formatted score that will be displayed in the leaderboard of this
    /// entry. You can select a score type to determine how scores are displayed on
    /// Leaderboard. See
    /// [here](https://developer.oculus.com/documentation/unity/ps-
    /// leaderboards/#create) for examples of different score type.
    public readonly string DisplayScore;
    /// A 2KB custom data field that is associated with the leaderboard entry. This
    /// can be a game replay or anything that provides more detail about the entry
    /// to the viewer. It will be used by two entry methods:
    /// Leaderboards.WriteEntry() and
    /// Leaderboards.WriteEntryWithSupplementaryMetric()
    public readonly byte[] ExtraData;
    /// This is a unique identifier for the leaderboard entry. It is of type `id`
    /// and is optional.
    public readonly UInt64 ID;
    /// The rank of this leaderboard entry in the leaderboard. It is of type `int`.
    /// It can be used in Leaderboards.GetEntriesAfterRank() to retrieve
    /// leaderboard entries starting from a specified rank.
    public readonly int Rank;
    /// The raw underlying value of the score achieved by the user in the
    /// leaderboard. It's of type `long_as_string` and it's used to determine the
    /// user's rank in the leaderboard.
    public readonly long Score;
    /// Models.SupplementaryMetric is a supplemental piece of data that can be used
    /// for tiebreakers by Leaderboards.WriteEntryWithSupplementaryMetric().
    // May be null. Check before using.
    public readonly SupplementaryMetric SupplementaryMetricOptional;
    [Obsolete("Deprecated in favor of SupplementaryMetricOptional")]
    public readonly SupplementaryMetric SupplementaryMetric;
    /// The timestamp of this entry being created in the leaderboard.
    public readonly DateTime Timestamp;
    /// User of this leaderboard entry. It is of type Models.User. You can request
    /// a block of leaderboard entries for the specified user ID(s) by
    /// Leaderboards.GetEntriesByIds().
    public readonly User User;


    public LeaderboardEntry(IntPtr o)
    {
      DisplayScore = CAPI.ovr_LeaderboardEntry_GetDisplayScore(o);
      ExtraData = CAPI.ovr_LeaderboardEntry_GetExtraData(o);
      ID = CAPI.ovr_LeaderboardEntry_GetID(o);
      Rank = CAPI.ovr_LeaderboardEntry_GetRank(o);
      Score = CAPI.ovr_LeaderboardEntry_GetScore(o);
      {
        var pointer = CAPI.ovr_LeaderboardEntry_GetSupplementaryMetric(o);
        SupplementaryMetric = new SupplementaryMetric(pointer);
        if (pointer == IntPtr.Zero) {
          SupplementaryMetricOptional = null;
        } else {
          SupplementaryMetricOptional = SupplementaryMetric;
        }
      }
      Timestamp = CAPI.ovr_LeaderboardEntry_GetTimestamp(o);
      User = new User(CAPI.ovr_LeaderboardEntry_GetUser(o));
    }
  }

  /// Represents a paginated list of LeaderboardEntry elements. It allows you to easily access and manipulate the elements in the paginated list, such as the size of the list and the next and previous URLs.
  public class LeaderboardEntryList : DeserializableList<LeaderboardEntry> {
    /// Instantiates a C# wrapper class that wraps a native list by pointer. Used internally by Platform SDK to wrap the list.
    public LeaderboardEntryList(IntPtr a) {
      var count = (int)CAPI.ovr_LeaderboardEntryArray_GetSize(a);
      _Data = new List<LeaderboardEntry>(count);
      for (int i = 0; i < count; i++) {
        _Data.Add(new LeaderboardEntry(CAPI.ovr_LeaderboardEntryArray_GetElement(a, (UIntPtr)i)));
      }

      TotalCount = CAPI.ovr_LeaderboardEntryArray_GetTotalCount(a);
      _PreviousUrl = CAPI.ovr_LeaderboardEntryArray_GetPreviousUrl(a);
      _NextUrl = CAPI.ovr_LeaderboardEntryArray_GetNextUrl(a);
    }

    /// It indicates the total number of entries in the list, across all pages from LeaderboardEntry.
    public readonly ulong TotalCount;
  }
}

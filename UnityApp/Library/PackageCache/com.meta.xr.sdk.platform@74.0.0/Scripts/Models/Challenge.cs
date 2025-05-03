// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

#pragma warning disable 0618

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// Challenges leverage Models.Destination and Group Presence to create
  /// shareable links that allow players to compete directly based on score.
  ///
  /// Challenges can be ranked by highest or lowest scores within a time period.
  /// Any application that uses Leaderboards gets Challenges for free. It appears
  /// in the Scoreboards UI. Players can create Challenges and send invites
  /// through the Challenges app.
  public class Challenge
  {
    /// An enum that specifies if this challenge was created by a user
    /// (ChallengeCreationType.UserCreated) or the app developer
    /// (ChallengeCreationType.DeveloperCreated).
    public readonly ChallengeCreationType CreationType;
    /// A displayable string of the challenge's description.
    public readonly string Description;
    /// The timestamp when this challenge ends. You can retrieve this field from
    /// the response of the challenge creation request.
    public readonly DateTime EndDate;
    /// The ID of the challenge. This is an unique string that the application will
    /// refer to this challenge in your app.
    public readonly UInt64 ID;
    /// Users that are invited to this challenge.
    // May be null. Check before using.
    public readonly UserList InvitedUsersOptional;
    [Obsolete("Deprecated in favor of InvitedUsersOptional")]
    public readonly UserList InvitedUsers;
    /// The Models.Leaderboard associated with this challenge. You can retrieve the
    /// leaderboard ID from the response of the challenge creation request.
    public readonly Leaderboard Leaderboard;
    /// Users that have participated in this challenge.
    // May be null. Check before using.
    public readonly UserList ParticipantsOptional;
    [Obsolete("Deprecated in favor of ParticipantsOptional")]
    public readonly UserList Participants;
    /// The timestamp when this challenge begins. You can retrieve this field from
    /// the response of the challenge creation request.
    public readonly DateTime StartDate;
    /// A displayable string of the challenge's title.
    public readonly string Title;
    /// An enum that specifies who can see and participate in this challenge.
    ///
    /// ChallengeVisibility.InviteOnly - Only those invited can participate in it.
    /// Everyone can see it.
    ///
    /// ChallengeVisibility.Public - Everyone can participate and see this
    /// challenge.
    ///
    /// ChallengeVisibility.Private - Only those invited can participate and see
    /// this challenge.
    public readonly ChallengeVisibility Visibility;


    public Challenge(IntPtr o)
    {
      CreationType = CAPI.ovr_Challenge_GetCreationType(o);
      Description = CAPI.ovr_Challenge_GetDescription(o);
      EndDate = CAPI.ovr_Challenge_GetEndDate(o);
      ID = CAPI.ovr_Challenge_GetID(o);
      {
        var pointer = CAPI.ovr_Challenge_GetInvitedUsers(o);
        InvitedUsers = new UserList(pointer);
        if (pointer == IntPtr.Zero) {
          InvitedUsersOptional = null;
        } else {
          InvitedUsersOptional = InvitedUsers;
        }
      }
      Leaderboard = new Leaderboard(CAPI.ovr_Challenge_GetLeaderboard(o));
      {
        var pointer = CAPI.ovr_Challenge_GetParticipants(o);
        Participants = new UserList(pointer);
        if (pointer == IntPtr.Zero) {
          ParticipantsOptional = null;
        } else {
          ParticipantsOptional = Participants;
        }
      }
      StartDate = CAPI.ovr_Challenge_GetStartDate(o);
      Title = CAPI.ovr_Challenge_GetTitle(o);
      Visibility = CAPI.ovr_Challenge_GetVisibility(o);
    }
  }

  /// Represents a paginated list of Challenge elements. It allows you to easily access and manipulate the elements in the paginated list, such as the size of the list and the next and previous URLs.
  public class ChallengeList : DeserializableList<Challenge> {
    /// Instantiates a C# wrapper class that wraps a native list by pointer. Used internally by Platform SDK to wrap the list.
    public ChallengeList(IntPtr a) {
      var count = (int)CAPI.ovr_ChallengeArray_GetSize(a);
      _Data = new List<Challenge>(count);
      for (int i = 0; i < count; i++) {
        _Data.Add(new Challenge(CAPI.ovr_ChallengeArray_GetElement(a, (UIntPtr)i)));
      }

      TotalCount = CAPI.ovr_ChallengeArray_GetTotalCount(a);
      _PreviousUrl = CAPI.ovr_ChallengeArray_GetPreviousUrl(a);
      _NextUrl = CAPI.ovr_ChallengeArray_GetNextUrl(a);
    }

    /// It indicates the total number of entries in the list, across all pages from Challenge.
    public readonly ulong TotalCount;
  }
}

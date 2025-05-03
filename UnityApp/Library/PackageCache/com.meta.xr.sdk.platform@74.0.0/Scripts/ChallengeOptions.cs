// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// The Challenge Option is a parameter that can be passed in the
  /// Challenges.GetList() method to retrieve a list of challenges that match the
  /// specified options. The ChallengeOptions() parameter allows you to specify
  /// the criteria for the challenges you want to retrieve, such as the viewer
  /// filter, the visibility, or the date range.
  public class ChallengeOptions {

    /// Creates a new instance of ::ChallengeOptions which is used to customize the option flow. It returns a handle to the newly created options object, which can be used to set various properties for the options.
    public ChallengeOptions() {
      Handle = CAPI.ovr_ChallengeOptions_Create();
    }

    /// The description of the challenge is a detailed and informative text that
    /// provides a comprehensive overview of the challenge's objectives, rules, and
    /// requirements, which can be retrieved with Challenge#Description.
    public void SetDescription(string value) {
      CAPI.ovr_ChallengeOptions_SetDescription(Handle, value);
    }

    /// The challenge end date is the timestamp when this challenge ends, which can
    /// be retrieved using Challenge#EndDate.
    public void SetEndDate(DateTime value) {
      CAPI.ovr_ChallengeOptions_SetEndDate(Handle, value);
    }

    /// This option indicates whether to include challenges that are currently
    /// active in the search results. By default, this is set to true, meaning that
    /// only active challenges will be returned.
    public void SetIncludeActiveChallenges(bool value) {
      CAPI.ovr_ChallengeOptions_SetIncludeActiveChallenges(Handle, value);
    }

    /// This option indicates whether to include challenges that have not yet
    /// started in the search results. By default, this is set to false, meaning
    /// that only active will be returned.
    public void SetIncludeFutureChallenges(bool value) {
      CAPI.ovr_ChallengeOptions_SetIncludeFutureChallenges(Handle, value);
    }

    /// This option indicates whether to include challenges that have already ended
    /// in the search results. By default, this is set to false, meaning that only
    /// active will be returned.
    public void SetIncludePastChallenges(bool value) {
      CAPI.ovr_ChallengeOptions_SetIncludePastChallenges(Handle, value);
    }

    /// Optional: Only find challenges belonging to this leaderboard. This filter
    /// allows you to narrow down the search results to only include challenges
    /// that are associated with a specific leaderboard.
    public void SetLeaderboardName(string value) {
      CAPI.ovr_ChallengeOptions_SetLeaderboardName(Handle, value);
    }

    /// The challenge start date is the timestamp when this challenge begins, which
    /// can be retrieved using Challenge#StartDate.
    public void SetStartDate(DateTime value) {
      CAPI.ovr_ChallengeOptions_SetStartDate(Handle, value);
    }

    /// The title of the challenge is a descriptive label that provides a concise
    /// summary of the challenge's purpose and objectives, which can be retrieved
    /// with Challenge#Title.
    public void SetTitle(string value) {
      CAPI.ovr_ChallengeOptions_SetTitle(Handle, value);
    }

    /// An enum that specifies what filter to apply to the list of returned
    /// challenges.
    ///
    /// Returns all public (ChallengeVisibility.Public) and invite-only
    /// (ChallengeVisibility.InviteOnly) Models.Challenge in which the user is a
    /// participant or invitee. Excludes private (ChallengeVisibility.Private)
    /// challenges.
    ///
    /// ChallengeViewerFilter.Participating - Returns challenges the user is
    /// participating in.
    ///
    /// ChallengeViewerFilter.Invited - Returns challenges the user is invited to.
    ///
    /// ChallengeViewerFilter.ParticipatingOrInvited - Returns challenges the user
    /// is either participating in or invited to.
    public void SetViewerFilter(ChallengeViewerFilter value) {
      CAPI.ovr_ChallengeOptions_SetViewerFilter(Handle, value);
    }

    /// The challenge visibility setting specifies who can see and participate in
    /// this challenge, which be retrieved with Challenge#Visibility. There are
    /// three visibility levels: ChallengeVisibility.Public,
    /// ChallengeVisibility.InviteOnly and ChallengeVisibility.Private.
    public void SetVisibility(ChallengeVisibility value) {
      CAPI.ovr_ChallengeOptions_SetVisibility(Handle, value);
    }


    /// This operator allows you to pass an instance of the ::ChallengeOptions class to native C code as an IntPtr. The operator returns the handle of the options object, or IntPtr.Zero if the object is null.
    public static explicit operator IntPtr(ChallengeOptions options) {
      return options != null ? options.Handle : IntPtr.Zero;
    }

    /// Destroys an existing instance of the ::ChallengeOptions and frees up memory when you're done using it.
    ~ChallengeOptions() {
      CAPI.ovr_ChallengeOptions_Destroy(Handle);
    }

    IntPtr Handle;
  }
}

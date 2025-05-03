// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

#pragma warning disable 0618

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// Details about the launch of the appplication. It can be used to check if
  /// your app is being launched using App to App Travel. It can be retrieved
  /// using ApplicationLifecycle.GetLaunchDetails().
  public class LaunchDetails
  {
    /// An opaque string provided by the developer to help them deeplink to content
    /// on app startup.
    public readonly string DeeplinkMessage;
    /// If provided, the intended destination the user would like to go to
    public readonly string DestinationApiName;
    /// A string typically used to distinguish where the deeplink came from. For
    /// instance, a DEEPLINK launch type could be coming from events or rich
    /// presence.
    public readonly string LaunchSource;
    /// A #LaunchType that defines the different ways in which an application can
    /// be launched. LaunchType.Normal - Normal launch from the user's library.
    /// LaunchType.Invite - Launch from the user accepting an invite.
    /// LaunchType.Deeplink - Launched from a deeplink. This flow is typically
    /// kicked off from Application.LaunchOtherApp()
    public readonly LaunchType LaunchType;
    /// If provided, the intended lobby the user would like to be in
    public readonly string LobbySessionID;
    /// If provided, the intended session the user would like to be in
    public readonly string MatchSessionID;
    /// A unique identifier to keep track of a user going through the deeplinking
    /// flow
    public readonly string TrackingID;
    /// If provided, the intended users the user would like to be with
    // May be null. Check before using.
    public readonly UserList UsersOptional;
    [Obsolete("Deprecated in favor of UsersOptional")]
    public readonly UserList Users;


    public LaunchDetails(IntPtr o)
    {
      DeeplinkMessage = CAPI.ovr_LaunchDetails_GetDeeplinkMessage(o);
      DestinationApiName = CAPI.ovr_LaunchDetails_GetDestinationApiName(o);
      LaunchSource = CAPI.ovr_LaunchDetails_GetLaunchSource(o);
      LaunchType = CAPI.ovr_LaunchDetails_GetLaunchType(o);
      LobbySessionID = CAPI.ovr_LaunchDetails_GetLobbySessionID(o);
      MatchSessionID = CAPI.ovr_LaunchDetails_GetMatchSessionID(o);
      TrackingID = CAPI.ovr_LaunchDetails_GetTrackingID(o);
      {
        var pointer = CAPI.ovr_LaunchDetails_GetUsers(o);
        Users = new UserList(pointer);
        if (pointer == IntPtr.Zero) {
          UsersOptional = null;
        } else {
          UsersOptional = Users;
        }
      }
    }
  }

}

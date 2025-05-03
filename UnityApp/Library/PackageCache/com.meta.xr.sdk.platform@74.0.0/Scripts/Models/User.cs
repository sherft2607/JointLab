// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

#pragma warning disable 0618

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// This is the class that represents the individual users who use your
  /// application. Use the User object to retrieve information about your users,
  /// help them interact with your application, and customize their experience.
  /// You can query for information about a particular user using their user id,
  /// User#ID. To learn more, read our
  /// [docs](https://developer.oculus.com/documentation/unity/ps-presence/#user-
  /// and-friends) about users. Note: You must complete a Data Use Checkup(DUC)
  /// in order to gain access to user platform features. Read more about DUC
  /// [here](https://developer.oculus.com/resources/publish-data-use/).
  public class User
  {
    /// A potentially non unique displayable name chosen by the user. Could also be
    /// the same as the oculus_ID. This is often the name shown to other users.
    public readonly string DisplayName;
    /// The ID of the user, User#ID. This is a unique value for every individual
    /// user.
    public readonly UInt64 ID;
    /// The url of the profile picture that is chosen by the user. Retrieve this
    /// url by using Users.GetLoggedInUser().
    public readonly string ImageURL;
    /// Managed account info, Models.ManagedInfo, for the user which contains
    /// further metadata that is only available if the user is a Meta Managed
    /// Account(MMA). There must be user consent via dialog during installation,
    /// your app must have DUC enabled, and the app must be admin-approved.
    // May be null. Check before using.
    public readonly ManagedInfo ManagedInfoOptional;
    [Obsolete("Deprecated in favor of ManagedInfoOptional")]
    public readonly ManagedInfo ManagedInfo;
    /// The oculus ID of the user. This is used across the developer dashboard and
    /// is unique to oculus.
    public readonly string OculusID;
    /// Human readable string of what the user is currently doing. Not intended to
    /// be parsed as it might change at anytime or be translated.
    public readonly string Presence;
    /// Intended to be parsed and used to deeplink to parts of the app. Read more
    /// about deeplinking
    /// [here](https://developer.oculus.com/documentation/unity/ps-deep-linking/).
    public readonly string PresenceDeeplinkMessage;
    /// If provided, this is the unique API Name that refers to the
    /// Models.Destination this user is currently at in the app. Read more about
    /// destinations [here](https://developer.oculus.com/documentation/unity/ps-
    /// destinations-overview/)
    public readonly string PresenceDestinationApiName;
    /// If provided, the lobby session this user is currently at in the
    /// application. If the ApplicationInvite is generated from rich presence, the
    /// lobby session id will be auto populated by calling
    /// User#PresenceLobbySessionId from the inviters' viewer context.
    public readonly string PresenceLobbySessionId;
    /// If provided, the match session this user is currently at in the
    /// application. If the ApplicationInvite is generated from rich presence, the
    /// match session id will be auto populated by calling
    /// User#PresenceMatchSessionId from the inviters' viewer context.
    public readonly string PresenceMatchSessionId;
    /// An enum value for the different statuses representing what the user is
    /// currently doing. The different statuses can be UserPresenceStatus.Unknown,
    /// UserPresenceStatus.Offline, UserPresenceStatus.Online.
    public readonly UserPresenceStatus PresenceStatus;
    /// The url of the smaller/secondary profile picture that is chosen by the
    /// user. Retrieve this url by using User#SmallImageUrl.
    public readonly string SmallImageUrl;


    public User(IntPtr o)
    {
      DisplayName = CAPI.ovr_User_GetDisplayName(o);
      ID = CAPI.ovr_User_GetID(o);
      ImageURL = CAPI.ovr_User_GetImageUrl(o);
      {
        var pointer = CAPI.ovr_User_GetManagedInfo(o);
        ManagedInfo = new ManagedInfo(pointer);
        if (pointer == IntPtr.Zero) {
          ManagedInfoOptional = null;
        } else {
          ManagedInfoOptional = ManagedInfo;
        }
      }
      OculusID = CAPI.ovr_User_GetOculusID(o);
      Presence = CAPI.ovr_User_GetPresence(o);
      PresenceDeeplinkMessage = CAPI.ovr_User_GetPresenceDeeplinkMessage(o);
      PresenceDestinationApiName = CAPI.ovr_User_GetPresenceDestinationApiName(o);
      PresenceLobbySessionId = CAPI.ovr_User_GetPresenceLobbySessionId(o);
      PresenceMatchSessionId = CAPI.ovr_User_GetPresenceMatchSessionId(o);
      PresenceStatus = CAPI.ovr_User_GetPresenceStatus(o);
      SmallImageUrl = CAPI.ovr_User_GetSmallImageUrl(o);
    }
  }

  /// Represents a paginated list of Models.User elements. It allows you to
  /// easily access and manipulate the elements in the paginated list, such as
  /// the size of the list and if there is a next page of elements available.
  public class UserList : DeserializableList<User> {
    /// Instantiates a C# wrapper class that wraps a native list by pointer. Used internally by Platform SDK to wrap the list.
    public UserList(IntPtr a) {
      var count = (int)CAPI.ovr_UserArray_GetSize(a);
      _Data = new List<User>(count);
      for (int i = 0; i < count; i++) {
        _Data.Add(new User(CAPI.ovr_UserArray_GetElement(a, (UIntPtr)i)));
      }

      _NextUrl = CAPI.ovr_UserArray_GetNextUrl(a);
    }

  }
}

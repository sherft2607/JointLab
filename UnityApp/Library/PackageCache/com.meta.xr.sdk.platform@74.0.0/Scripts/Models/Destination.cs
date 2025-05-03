// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// The destination represents where the user would like to go to in the app.
  /// It's usually associated with a travel or an invitation. Each destination
  /// has Destination#ApiName, Destination#DisplayName,
  /// Destination#DeeplinkMessage and Destination#ShareableUri link. Please refer
  /// to member data documentation for details.
  public class Destination
  {
    /// You can pass it into GroupPresenceOptions.SetDestinationApiName() when
    /// calling GroupPresence.Set() to set this user's group presence.
    public readonly string ApiName;
    /// The information that will be in LaunchDetails#DeeplinkMessage when a user
    /// enters via a deeplink. Alternatively will be in
    /// User#PresenceDeeplinkMessage if the rich presence is set for the user.
    public readonly string DeeplinkMessage;
    /// A displayable string of the destination name and it can be retrieved with
    /// Destination#DisplayName.
    public readonly string DisplayName;
    /// A URI that allows the user to deeplink directly to this destination
    public readonly string ShareableUri;


    public Destination(IntPtr o)
    {
      ApiName = CAPI.ovr_Destination_GetApiName(o);
      DeeplinkMessage = CAPI.ovr_Destination_GetDeeplinkMessage(o);
      DisplayName = CAPI.ovr_Destination_GetDisplayName(o);
      ShareableUri = CAPI.ovr_Destination_GetShareableUri(o);
    }
  }

  /// Represents a paginated list of Models.Destination elements. It allows you
  /// to easily access and manipulate the elements in the paginated list, such as
  /// the size of the list and if there is a next page of elements available.
  public class DestinationList : DeserializableList<Destination> {
    /// Instantiates a C# wrapper class that wraps a native list by pointer. Used internally by Platform SDK to wrap the list.
    public DestinationList(IntPtr a) {
      var count = (int)CAPI.ovr_DestinationArray_GetSize(a);
      _Data = new List<Destination>(count);
      for (int i = 0; i < count; i++) {
        _Data.Add(new Destination(CAPI.ovr_DestinationArray_GetElement(a, (UIntPtr)i)));
      }

      _NextUrl = CAPI.ovr_DestinationArray_GetNextUrl(a);
    }

  }
}

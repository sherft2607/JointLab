// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// The party ID is a unique identifier of the party that will be generated for
  /// every distinct party. This ID can be used to make requests such as
  /// Parties.GetCurrent() to get the current party by its id. Read more about
  /// [parties](https://developer.oculus.com/documentation/unity/ps-parties/).
  public class PartyID
  {
    /// The party ID can be used to retrieve Models.Party. Every party will have a
    /// unique ID that is associated with it.
    public readonly UInt64 ID;


    public PartyID(IntPtr o)
    {
      ID = CAPI.ovr_PartyID_GetID(o);
    }
  }

}

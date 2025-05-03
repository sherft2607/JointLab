// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// The value is used in the [VoIP
  /// system](https://developer.oculus.com/documentation/unity/ps-parties/#voip-
  /// options). The value determines how much the audio volume is reduced as the
  /// distance between players increases. This helps to create a more immersive
  /// experience by making distant players' voices sound fainter.
  public class NetSyncVoipAttenuationValue
  {
    /// fall-off value in decibel
    public readonly float Decibels;
    /// The starting distance of the attenuation value. As the distance between
    /// players increases, the audio volumn is reduced.
    public readonly float Distance;


    public NetSyncVoipAttenuationValue(IntPtr o)
    {
      Decibels = CAPI.ovr_NetSyncVoipAttenuationValue_GetDecibels(o);
      Distance = CAPI.ovr_NetSyncVoipAttenuationValue_GetDistance(o);
    }
  }

  /// Represents a paginated list of Models.NetSyncVoipAttenuationValue elements.
  /// It allows you to easily access and manipulate the elements in the paginated
  /// list, such as the size of the list and if there is a next page of elements
  /// available.
  public class NetSyncVoipAttenuationValueList : DeserializableList<NetSyncVoipAttenuationValue> {
    /// Instantiates a C# wrapper class that wraps a native list by pointer. Used internally by Platform SDK to wrap the list.
    public NetSyncVoipAttenuationValueList(IntPtr a) {
      var count = (int)CAPI.ovr_NetSyncVoipAttenuationValueArray_GetSize(a);
      _Data = new List<NetSyncVoipAttenuationValue>(count);
      for (int i = 0; i < count; i++) {
        _Data.Add(new NetSyncVoipAttenuationValue(CAPI.ovr_NetSyncVoipAttenuationValueArray_GetElement(a, (UIntPtr)i)));
      }

    }

  }
}

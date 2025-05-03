// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// The pid refers to "Process ID," which is a unique identifier assigned to
  /// each process running in a system. This identifier plays a crucial role in
  /// managing and tracking processes. See more info about Platform Solutions
  /// [here](https://developer.oculus.com/documentation/unity/ps-platform-
  /// intro/).
  public class Pid
  {
    /// Unique identifier assigned to each process running in a system, used for
    /// tracking and managing purposes.
    public readonly string Id;


    public Pid(IntPtr o)
    {
      Id = CAPI.ovr_Pid_GetId(o);
    }
  }

  /// Represents a paginated list of Models.Pid elements. It allows you to easily
  /// access and manipulate the elements in the paginated list, such as the size
  /// of the list and if there is a next page of elements available.
  public class PidList : DeserializableList<Pid> {
    /// Instantiates a C# wrapper class that wraps a native list by pointer. Used internally by Platform SDK to wrap the list.
    public PidList(IntPtr a) {
      var count = (int)CAPI.ovr_PidArray_GetSize(a);
      _Data = new List<Pid>(count);
      for (int i = 0; i < count; i++) {
        _Data.Add(new Pid(CAPI.ovr_PidArray_GetElement(a, (UIntPtr)i)));
      }

    }

  }
}

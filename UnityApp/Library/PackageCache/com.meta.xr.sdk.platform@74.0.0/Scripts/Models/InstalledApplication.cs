// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// InstalledApplication provides a way to retrieve information about installed
  /// application on a device, including their package name, application ID,
  /// version name, version code, and status. This information can be useful for
  /// tracking the status of applications on a device and identifying any issues
  /// that may need to be addressed.
  public class InstalledApplication
  {
    /// It's a type of `string` represents the ID of the application, which is a
    /// unique identifier for the app.
    public readonly string ApplicationId;
    /// The package name of the installed application.
    public readonly string PackageName;
    /// A `string` represents the status of the installed application.
    public readonly string Status;
    /// It's a type of `int` represents the current version code of the installed
    /// application. It can be retreiving using ApplicationVersion#CurrentCode
    public readonly int VersionCode;
    /// It's a type of `string` represents the current version name of the
    /// installed application. It can be retreiving using
    /// ApplicationVersion#CurrentName
    public readonly string VersionName;


    public InstalledApplication(IntPtr o)
    {
      ApplicationId = CAPI.ovr_InstalledApplication_GetApplicationId(o);
      PackageName = CAPI.ovr_InstalledApplication_GetPackageName(o);
      Status = CAPI.ovr_InstalledApplication_GetStatus(o);
      VersionCode = CAPI.ovr_InstalledApplication_GetVersionCode(o);
      VersionName = CAPI.ovr_InstalledApplication_GetVersionName(o);
    }
  }

  /// Represents a paginated list of Models.InstalledApplication elements. It
  /// allows you to easily access and manipulate the elements in the paginated
  /// list, such as the size of the list and if there is a next page of elements
  /// available.
  public class InstalledApplicationList : DeserializableList<InstalledApplication> {
    /// Instantiates a C# wrapper class that wraps a native list by pointer. Used internally by Platform SDK to wrap the list.
    public InstalledApplicationList(IntPtr a) {
      var count = (int)CAPI.ovr_InstalledApplicationArray_GetSize(a);
      _Data = new List<InstalledApplication>(count);
      for (int i = 0; i < count; i++) {
        _Data.Add(new InstalledApplication(CAPI.ovr_InstalledApplicationArray_GetElement(a, (UIntPtr)i)));
      }

    }

  }
}

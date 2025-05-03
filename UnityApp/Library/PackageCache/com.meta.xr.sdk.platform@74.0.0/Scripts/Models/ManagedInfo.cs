// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// If the user is a Meta Managed Account(MMA), the managed account for the
  /// user will contain further metadata information. There must be user consent
  /// via dialog during installation, your app must have
  /// [DUC](https://developer.oculus.com/resources/publish-data-use/) enabled,
  /// and the app must be admin-approved.
  public class ManagedInfo
  {
    /// A string represents the department name in the organization to which the
    /// user blongs to.
    public readonly string Department;
    /// The email address of the account user which owns the MMA, i.e., Meta
    /// Managed Account.
    public readonly string Email;
    /// A string represents the employee number of the person who owns MMA, i.e.,
    /// Meta Managed Account.
    public readonly string EmployeeNumber;
    /// A string which can be used to uniquely identify the user of the MMA, i.e.,
    /// Meta Managed Account.
    public readonly string ExternalId;
    /// A string contains the information about the location of the user.
    public readonly string Location;
    /// A string contains the information about the manager of the user.
    public readonly string Manager;
    /// A string contrains the information about the user's name.
    public readonly string Name;
    /// A string which can be used to uniquely identify the organization which owns
    /// the MMA, i.e., Meta Managed Account.
    public readonly string OrganizationId;
    /// The name of the organization to which the MMA(i.e., Meta Managed Account)
    /// account user belongs to.
    public readonly string OrganizationName;
    /// A string contains the position information of the user.
    public readonly string Position;


    public ManagedInfo(IntPtr o)
    {
      Department = CAPI.ovr_ManagedInfo_GetDepartment(o);
      Email = CAPI.ovr_ManagedInfo_GetEmail(o);
      EmployeeNumber = CAPI.ovr_ManagedInfo_GetEmployeeNumber(o);
      ExternalId = CAPI.ovr_ManagedInfo_GetExternalId(o);
      Location = CAPI.ovr_ManagedInfo_GetLocation(o);
      Manager = CAPI.ovr_ManagedInfo_GetManager(o);
      Name = CAPI.ovr_ManagedInfo_GetName(o);
      OrganizationId = CAPI.ovr_ManagedInfo_GetOrganizationId(o);
      OrganizationName = CAPI.ovr_ManagedInfo_GetOrganizationName(o);
      Position = CAPI.ovr_ManagedInfo_GetPosition(o);
    }
  }

}

// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

#pragma warning disable 0618

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// Contains the detailed billing plan information for a
  /// ProductType.SUBSCRIPTION. A BillingPlan can have a Models.PaidOffer and an
  /// array of Models.TrialOffer. The Models.TrialOfferArray can contain a
  /// FREE_TRIAL and an INTRO_OFFER.
  public class BillingPlan
  {
    /// Represents the Models.PaidOffer associated with the billing_plan.
    public readonly PaidOffer PaidOffer;
    /// A list of Models.TrialOffer associated with the billing_plan.
    // May be null. Check before using.
    public readonly TrialOfferList TrialOffersOptional;
    [Obsolete("Deprecated in favor of TrialOffersOptional")]
    public readonly TrialOfferList TrialOffers;


    public BillingPlan(IntPtr o)
    {
      PaidOffer = new PaidOffer(CAPI.ovr_BillingPlan_GetPaidOffer(o));
      {
        var pointer = CAPI.ovr_BillingPlan_GetTrialOffers(o);
        TrialOffers = new TrialOfferList(pointer);
        if (pointer == IntPtr.Zero) {
          TrialOffersOptional = null;
        } else {
          TrialOffersOptional = TrialOffers;
        }
      }
    }
  }

  /// Represents a paginated list of Models.BillingPlan elements. It allows you
  /// to easily access and manipulate the elements in the paginated list, such as
  /// the size of the list and if there is a next page of elements available.
  public class BillingPlanList : DeserializableList<BillingPlan> {
    /// Instantiates a C# wrapper class that wraps a native list by pointer. Used internally by Platform SDK to wrap the list.
    public BillingPlanList(IntPtr a) {
      var count = (int)CAPI.ovr_BillingPlanArray_GetSize(a);
      _Data = new List<BillingPlan>(count);
      for (int i = 0; i < count; i++) {
        _Data.Add(new BillingPlan(CAPI.ovr_BillingPlanArray_GetElement(a, (UIntPtr)i)));
      }

    }

  }
}

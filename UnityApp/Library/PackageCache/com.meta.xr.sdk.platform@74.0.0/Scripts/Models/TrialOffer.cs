// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// Contains the details about the trial offer associated with a
  /// Models.BillingPlan.
  public class TrialOffer
  {
    /// Represents the maximum term for which a trial_offer is valid.
    public readonly int MaxTermCount;
    /// The Models.Price of the trial offer contains the currency code, the amount
    /// in hundredths, and the formatted string representation.
    public readonly Price Price;
    /// An enum that specifies the term.
    ///
    /// OfferTerm.Unknown: unknown
    ///
    /// OfferTerm.WEEKLY: WEEKLY
    ///
    /// OfferTerm.BIWEEKLY: BIWEEKLY
    ///
    /// OfferTerm.MONTHLY: MONTHLY
    ///
    /// OfferTerm.QUARTERLY: QUARTERLY
    ///
    /// OfferTerm.SEMIANNUAL: SEMIANNUAL
    ///
    /// OfferTerm.ANNUAL: ANNUAL
    ///
    /// OfferTerm.BIANNUAL: BIANNUAL
    public readonly OfferTerm TrialTerm;
    /// An enum that specifies the type of the trial.
    ///
    /// OfferType.INTROOFFER: Intro Offer.
    ///
    /// OfferType.FREETRIAL: Free Trial.
    public readonly OfferType TrialType;


    public TrialOffer(IntPtr o)
    {
      MaxTermCount = CAPI.ovr_TrialOffer_GetMaxTermCount(o);
      Price = new Price(CAPI.ovr_TrialOffer_GetPrice(o));
      TrialTerm = CAPI.ovr_TrialOffer_GetTrialTerm(o);
      TrialType = CAPI.ovr_TrialOffer_GetTrialType(o);
    }
  }

  /// Represents a paginated list of Models.TrialOffer elements. It allows you to
  /// easily access and manipulate the elements in the paginated list, such as
  /// the size of the list and if there is a next page of elements available.
  public class TrialOfferList : DeserializableList<TrialOffer> {
    /// Instantiates a C# wrapper class that wraps a native list by pointer. Used internally by Platform SDK to wrap the list.
    public TrialOfferList(IntPtr a) {
      var count = (int)CAPI.ovr_TrialOfferArray_GetSize(a);
      _Data = new List<TrialOffer>(count);
      for (int i = 0; i < count; i++) {
        _Data.Add(new TrialOffer(CAPI.ovr_TrialOfferArray_GetElement(a, (UIntPtr)i)));
      }

    }

  }
}

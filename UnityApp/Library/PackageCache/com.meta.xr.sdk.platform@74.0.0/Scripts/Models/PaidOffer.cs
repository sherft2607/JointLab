// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// Contains the details about the paid offer associated with a
  /// Models.BillingPlan.
  public class PaidOffer
  {
    /// The Models.Price of the paid offer contains the currency code, the amount
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
    public readonly OfferTerm SubscriptionTerm;


    public PaidOffer(IntPtr o)
    {
      Price = new Price(CAPI.ovr_PaidOffer_GetPrice(o));
      SubscriptionTerm = CAPI.ovr_PaidOffer_GetSubscriptionTerm(o);
    }
  }

}

// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// The price of a Models.Product. A price contains a currency code, an amount
  /// in hundredths, and its formatted string representation. For example, a
  /// price with a currency code of "USD" and an amount in hundredths of 99 has a
  /// formatted string of "$0.99".
  public class Price
  {
    /// The price of the product in hundredths of currency units.
    public readonly uint AmountInHundredths;
    /// The ISO 4217 currency code for the price of the product. For example,
    /// "USD", "GBP", "JPY".
    public readonly string Currency;
    /// The formatted string representation of the price, e.g., "$0.78". The value
    /// depends on the Price#Currency and Price#AmountInHundredths.
    public readonly string Formatted;


    public Price(IntPtr o)
    {
      AmountInHundredths = CAPI.ovr_Price_GetAmountInHundredths(o);
      Currency = CAPI.ovr_Price_GetCurrency(o);
      Formatted = CAPI.ovr_Price_GetFormatted(o);
    }
  }

}

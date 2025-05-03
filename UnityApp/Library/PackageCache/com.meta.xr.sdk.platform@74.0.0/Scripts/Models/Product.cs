// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

#pragma warning disable 0618

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// The class that represents the product information for a specific IAP which
  /// is available for purchase in your app. You can retrieve more information
  /// about the product(s) by using their SKU with IAP.GetProductsBySKU()
  public class Product
  {
    /// Billing plans related to the product.
    // May be null. Check before using.
    public readonly BillingPlanList BillingPlansOptional;
    [Obsolete("Deprecated in favor of BillingPlansOptional")]
    public readonly BillingPlanList BillingPlans;
    /// The content rating of a Models.Product that specifies the age rating as
    /// well as other important information that needs to be displayed to the user
    /// per local regulations.
    // May be null. Check before using.
    public readonly ContentRating ContentRatingOptional;
    [Obsolete("Deprecated in favor of ContentRatingOptional")]
    public readonly ContentRating ContentRating;
    /// The URI for the cover image for the Models.Product being sold.
    public readonly string CoverUrl;
    /// The description for the product. The description should be meaningful and
    /// explanatory to help outline the product and its features.
    public readonly string Description;
    /// The formatted string for the Models.Price. This is the same value stored in
    /// Models.Price.
    public readonly string FormattedPrice;
    /// The URI for Models.Product icon.
    public readonly string IconUrl;
    /// The name of the product. This will be used as a the display name and should
    /// be aligned with the user facing title.
    public readonly string Name;
    /// The Models.Price of the product contains the currency code, the amount in
    /// hundredths, and the formatted string representation.
    public readonly Price Price;
    /// The short description of a Models.Product which provides more information
    /// about the Models.Product. To be used in conjunction with the description of
    /// the Models.Product.
    public readonly string ShortDescription;
    /// The unique string that you use to reference the product in your app. The
    /// SKU is case-sensitive and should match the SKU reference in your code.
    public readonly string Sku;
    /// The type of product. An In-app purchase (IAP) add-on can be
    /// ProductType.DURABLE, ProductType.CONSUMABLE, or a ProductType.SUBSCRIPTION.
    public readonly ProductType Type;


    public Product(IntPtr o)
    {
      {
        var pointer = CAPI.ovr_Product_GetBillingPlans(o);
        BillingPlans = new BillingPlanList(pointer);
        if (pointer == IntPtr.Zero) {
          BillingPlansOptional = null;
        } else {
          BillingPlansOptional = BillingPlans;
        }
      }
      {
        var pointer = CAPI.ovr_Product_GetContentRating(o);
        ContentRating = new ContentRating(pointer);
        if (pointer == IntPtr.Zero) {
          ContentRatingOptional = null;
        } else {
          ContentRatingOptional = ContentRating;
        }
      }
      CoverUrl = CAPI.ovr_Product_GetCoverUrl(o);
      Description = CAPI.ovr_Product_GetDescription(o);
      FormattedPrice = CAPI.ovr_Product_GetFormattedPrice(o);
      IconUrl = CAPI.ovr_Product_GetIconUrl(o);
      Name = CAPI.ovr_Product_GetName(o);
      Price = new Price(CAPI.ovr_Product_GetPrice(o));
      ShortDescription = CAPI.ovr_Product_GetShortDescription(o);
      Sku = CAPI.ovr_Product_GetSKU(o);
      Type = CAPI.ovr_Product_GetType(o);
    }
  }

  /// Represents a paginated list of Models.Product elements. It allows you to
  /// easily access and manipulate the elements in the paginated list, such as
  /// the size of the list and if there is a next page of elements available.
  public class ProductList : DeserializableList<Product> {
    /// Instantiates a C# wrapper class that wraps a native list by pointer. Used internally by Platform SDK to wrap the list.
    public ProductList(IntPtr a) {
      var count = (int)CAPI.ovr_ProductArray_GetSize(a);
      _Data = new List<Product>(count);
      for (int i = 0; i < count; i++) {
        _Data.Add(new Product(CAPI.ovr_ProductArray_GetElement(a, (UIntPtr)i)));
      }

      _NextUrl = CAPI.ovr_ProductArray_GetNextUrl(a);
    }

  }
}

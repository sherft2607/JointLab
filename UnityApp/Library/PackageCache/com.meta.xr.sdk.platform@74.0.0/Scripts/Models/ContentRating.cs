// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// A Content Rating of a Models.Product. This rating is sourced by the
  /// International Age Rating Coalition (IARC) certificate from the local rating
  /// authority. An Add-on's Content Rating can be configured by adding a IARC
  /// certificate in the developer dashboard. The Add-on can also inherit the
  /// Content Rating of the base App.
  public class ContentRating
  {
    /// URI for the image that needs to be shown for the content rating of the
    /// Models.Product.
    public readonly string AgeRatingImageUri;
    /// The age rating text is the text version of the rating used to describe age
    /// appropriateness by the International Age Rating Coalition (IARC).
    public readonly string AgeRatingText;
    /// The list of descriptors which indicate content within the product that may
    /// have triggered a particular age rating or may be of interest or concern to
    /// consumers, e.g., "Blood and Gore", "Intense Violence", etc.
    public readonly String[] Descriptors;
    /// The list of interactive elements, which advise consumers up front that a
    /// Models.Product includes interactive or online behaviors/options that may be
    /// of interest or concern, e.g., "In-App Purchases".
    public readonly String[] InteractiveElements;
    /// The URI pointing to a website with International Age Rating Coalition
    /// (IARC) rating definitions from local rating authorities (e.g., Australian
    /// Classification Board, ESRB, GRAC, etc).
    public readonly string RatingDefinitionUri;


    public ContentRating(IntPtr o)
    {
      AgeRatingImageUri = CAPI.ovr_ContentRating_GetAgeRatingImageUri(o);
      AgeRatingText = CAPI.ovr_ContentRating_GetAgeRatingText(o);
      var descriptorsCount = CAPI.ovr_ContentRating_GetDescriptorsSize(o);
      Descriptors = new String[descriptorsCount];
      for (uint i = 0; i < descriptorsCount; i++) {
        Descriptors[i] = CAPI.ovr_ContentRating_GetDescriptor(o, i);
      }
      var interactiveElementsCount = CAPI.ovr_ContentRating_GetInteractiveElementsSize(o);
      InteractiveElements = new String[interactiveElementsCount];
      for (uint i = 0; i < interactiveElementsCount; i++) {
        InteractiveElements[i] = CAPI.ovr_ContentRating_GetInteractiveElement(o, i);
      }
      RatingDefinitionUri = CAPI.ovr_ContentRating_GetRatingDefinitionUri(o);
    }
  }

}

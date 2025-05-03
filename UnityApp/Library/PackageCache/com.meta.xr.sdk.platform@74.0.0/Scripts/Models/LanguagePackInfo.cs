// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// It contains information about a specific language, including its
  /// identification tag, and names in both English and the native language. It
  /// is useful for applications supporting multiple languages. It can be
  /// retrieved using AssetDetails#Language. Learn more about language pack in
  /// our [website](https://developer.oculus.com/documentation/unity/ps-language-
  /// packs/)
  public class LanguagePackInfo
  {
    /// Language name in English language. For example, the English name for
    /// "de.lang" will be "German".
    public readonly string EnglishName;
    /// Language name in its native language. For example, the native name for
    /// "de.lang" will be "Deutsch".
    public readonly string NativeName;
    /// Language tag in [BCP47](https://www.rfc-editor.org/info/bcp47) format with
    /// a suffix of "lang". For example, "de.lang" is a valid language pack name
    /// and its `tag` will be "de".
    public readonly string Tag;


    public LanguagePackInfo(IntPtr o)
    {
      EnglishName = CAPI.ovr_LanguagePackInfo_GetEnglishName(o);
      NativeName = CAPI.ovr_LanguagePackInfo_GetNativeName(o);
      Tag = CAPI.ovr_LanguagePackInfo_GetTag(o);
    }
  }

}

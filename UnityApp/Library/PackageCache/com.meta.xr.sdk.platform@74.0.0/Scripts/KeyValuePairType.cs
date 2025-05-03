// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform
{

  using Description = System.ComponentModel.DescriptionAttribute;

  /// It's a data structure that represents a pair of values, where one value
  /// serves as the key and the other value serves as the corresponding value. It
  /// can be used to store a different type of data, allowing for flexibility in
  /// how data is stored and retrieved.
  public enum KeyValuePairType : int
  {
    /// This member represents the key value as a string. It is used to store text-
    /// based data, such as names.
    [Description("STRING")]
    String,

    /// This member represents the key value as an integer. It is used to store
    /// numerical data, such as ages
    [Description("INTEGER")]
    Int,

    /// This member represents the key value as a double-precision floating-point
    /// number. It is used to store numerical data with decimal points, such as
    /// prices.
    [Description("DOUBLE")]
    Double,

    [Description("UNKNOWN")]
    Unknown,

  }

}

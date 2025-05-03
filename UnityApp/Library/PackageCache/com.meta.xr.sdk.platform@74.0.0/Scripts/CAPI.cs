// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

#pragma warning disable 414
namespace Oculus.Platform
{
  /// The CAPI class provides an interface to the Oculus Platform SDK, which allows developers to
  /// access various features of the Meta Quest platform such as user authentication, entitlement checks,
  /// cloud storage, and more. The class contains several methods that can be used to interact with the platform,
  /// including initialization, message queue access, VOIP, and logging. The class also includes several structs
  /// and enums that are used to represent data types and constants specific to the platform.
  /// See more details [here](https://developer.oculus.com/reference/platform-unity/latest/).
  public class CAPI
  {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
  #if UNITY_64 || UNITY_EDITOR_64
    /// The DLL_NAME constant specifies the name of the DLL (Dynamic Link Library) file that contains the implementation of the Oculus Platform API. The value of this constant depends on the platform and architecture of the application.
    public const string DLL_NAME = "LibOVRPlatformImpl64_1";
  #else
    /// The DLL_NAME constant specifies the name of the DLL (Dynamic Link Library) file that contains the implementation of the Oculus Platform API. The value of this constant depends on the platform and architecture of the application.
    public const string DLL_NAME = "LibOVRPlatformImpl32_1";
  #endif
#elif UNITY_EDITOR || UNITY_EDITOR_64
    /// The DLL_NAME constant specifies the name of the DLL (Dynamic Link Library) file that contains the implementation of the Oculus Platform API. The value of this constant depends on the platform and architecture of the application.
    public const string DLL_NAME = "ovrplatform";
#elif UNITY_ANDROID && OVR_STANDALONE_PLATFORM
    /// The DLL_NAME constant specifies the name of the DLL (Dynamic Link Library) file that contains the implementation of the Oculus Platform API. The value of this constant depends on the platform and architecture of the application.
    public const string DLL_NAME = "ovrplatform_standalone";
#else
    /// The DLL_NAME constant specifies the name of the DLL (Dynamic Link Library) file that contains the implementation of the Oculus Platform API. The value of this constant depends on the platform and architecture of the application.
    public const string DLL_NAME = "ovrplatformloader";
#endif

    private static UTF8Encoding nativeStringEncoding = new UTF8Encoding(false);

    [StructLayout(LayoutKind.Sequential)]
    /// The ovrKeyValuePair struct represents a key-value pair, where the key is a string and the value can be either an integer, a double, or a string. This struct is used to represent data in the Oculus Platform API.
    /// See more details [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public struct ovrKeyValuePair {
      public ovrKeyValuePair(string key, string value) {
        key_ = key;
        valueType_ = KeyValuePairType.String;
        stringValue_ = value;

        intValue_ = 0;
        doubleValue_ = 0.0;
      }

      public ovrKeyValuePair(string key, int value) {
        key_ = key;
        valueType_ = KeyValuePairType.Int;
        intValue_ = value;

        stringValue_ = null;
        doubleValue_ = 0.0;
      }

      public ovrKeyValuePair(string key, double value) {
        key_ = key;
        valueType_ = KeyValuePairType.Double;
        doubleValue_ = value;

        stringValue_ = null;
        intValue_ = 0;
      }

      /// This is a string that is used as a key for the ovrKeyValuePair struct. It will map to a value of either type integer, double, or string.
      public string key_;
      /// This is a explicit value that is used to indicate the type of the data that is stored in the ovrKeyValuePair struct. It can be either an integer, string, or double.
      KeyValuePairType valueType_;

      /// This is a string value that is stored in the ovrKeyValuePair struct. It is only used if the the value type is of type string.
      public string stringValue_;
      /// This is an integer value that is stored in the ovrKeyValuePair struct. It is only used if the the value type is of type integer.
      public int intValue_;
      /// This is a double value that is stored in the ovrKeyValuePair struct. It is only used if the the value type is of type double.
      public double doubleValue_;
    };

    [StructLayout(LayoutKind.Sequential)]
    /// The ovrNetSyncVec3 struct represents a 3D vector with float components. It is used to represent net sync data in the Oculus Platform API. The struct consists of 3 float values: x, y, and z each representing an axis.
    public struct ovrNetSyncVec3 {
      /// The x component of the ovrNetSyncVec3 struct. It is a float value representing the x-axis.
      public float x;
      /// The y component of the ovrNetSyncVec3 struct. It is a float value representing the y-axis.
      public float y;
      /// The z component of the ovrNetSyncVec3 struct. It is a float value representing the z-axis.
      public float z;
    }

    public static IntPtr ArrayOfStructsToIntPtr(Array ar)
    {
      int totalSize = 0;
      for(int i=0; i<ar.Length; i++) {
        totalSize += Marshal.SizeOf(ar.GetValue(i));
      }

      IntPtr childrenPtr = Marshal.AllocHGlobal(totalSize);
      IntPtr curr = childrenPtr;
      for(int i=0; i<ar.Length; i++) {
        Marshal.StructureToPtr(ar.GetValue(i), curr, false);
        curr = (IntPtr)((long)curr + Marshal.SizeOf(ar.GetValue(i)));
      }
      return childrenPtr;
    }

    public static CAPI.ovrKeyValuePair[] DictionaryToOVRKeyValuePairs(Dictionary<InitConfigOptions, bool> dict)
    {
      if(dict == null || dict.Count == 0)
      {
        return null;
      }

      var nativeCustomData = new CAPI.ovrKeyValuePair[dict.Count];

      int i = 0;
      foreach(var item in dict)
      {
        nativeCustomData[i] = new CAPI.ovrKeyValuePair(item.Key.ToString(), item.Value ? 1 : 0);
        i++;
      }
      return nativeCustomData;
    }

    public static CAPI.ovrKeyValuePair[] DictionaryToOVRKeyValuePairs(Dictionary<string, object> dict)
    {
      if(dict == null || dict.Count == 0)
      {
        return null;
      }

      var nativeCustomData = new CAPI.ovrKeyValuePair[dict.Count];

      int i = 0;
      foreach(var item in dict)
      {
        if(item.Value.GetType() == typeof(int))
        {
          nativeCustomData[i] = new CAPI.ovrKeyValuePair(item.Key, (int)item.Value);
        }
        else if(item.Value.GetType() == typeof(string))
        {
          nativeCustomData[i] = new CAPI.ovrKeyValuePair(item.Key, (string)item.Value);
        }
        else if(item.Value.GetType() == typeof(double))
        {
          nativeCustomData[i] = new CAPI.ovrKeyValuePair(item.Key, (double)item.Value);
        }
        else
        {
          throw new Exception("Only int, double or string are allowed types in CustomQuery.data");
        }
        i++;
      }
      return nativeCustomData;
    }

    public static byte[] IntPtrToByteArray(IntPtr data, ulong size)
    {
      byte[] outArray = new byte[size];
      Marshal.Copy(data, outArray, 0, (int)size);
      return outArray;
    }

    public static Dictionary<string, string> DataStoreFromNative(IntPtr pointer) {
      var d = new Dictionary<string, string>();
      var size = (int)CAPI.ovr_DataStore_GetNumKeys(pointer);
      for (var i = 0; i < size; i++) {
        string key = CAPI.ovr_DataStore_GetKey(pointer, i);
        d[key] = CAPI.ovr_DataStore_GetValue(pointer, key);
      }
      return d;
    }

    public static string StringFromNative(IntPtr pointer) {
      if (pointer == IntPtr.Zero) {
        return null;
      }
      var l = GetNativeStringLengthNotIncludingNullTerminator(pointer);
      var data = new byte[l];
      Marshal.Copy(pointer, data, 0, l);
      return nativeStringEncoding.GetString(data);
    }

    public static int GetNativeStringLengthNotIncludingNullTerminator(IntPtr pointer) {
      var l = 0;
      while (true) {
        if (Marshal.ReadByte(pointer, l) == 0) {
          return l;
        }
        l++;
      }
    }

    public static DateTime DateTimeFromNative(ulong seconds_since_the_one_true_epoch) {
      var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0);
      return dt.AddSeconds(seconds_since_the_one_true_epoch).ToLocalTime();
    }

    public static ulong DateTimeToNative(DateTime dt) {
      var universal = (dt.Kind != DateTimeKind.Utc) ? dt.ToUniversalTime() : dt;
      var epochStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
      return (ulong) (universal - epochStart).TotalSeconds;
    }

    public static byte[] BlobFromNative(uint size, IntPtr pointer) {
      var a = new byte[(int)size];
      for (int i = 0; i < (int)size; i++) {
        a[i] = Marshal.ReadByte(pointer, i);
      }
      return a;
    }

    public static byte[] FiledataFromNative(uint size, IntPtr pointer) {
      var data = new byte[(int)size];
      Marshal.Copy(pointer, data, 0, (int)size);
      return data;
    }

    public static IntPtr StringToNative(string s) {
      if (s == null) {
        throw new Exception("StringFromNative: null argument");
      }
      var l = nativeStringEncoding.GetByteCount(s);
      var data = new byte[l + 1];
      nativeStringEncoding.GetBytes(s, 0, s.Length, data, 0);
      var pointer = Marshal.AllocCoTaskMem(l + 1);
      Marshal.Copy(data, 0, pointer, l + 1);
      return pointer;
    }

    // Initialization
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern bool ovr_UnityInitWrapper(string appId);

    // Initializes just the global variables to use the Unity api without calling the init logic
    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern void ovr_UnityInitGlobals(IntPtr loggingCB);

    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_UnityInitWrapperAsynchronous(string appId);

    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern bool ovr_UnityInitWrapperStandalone(string accessToken, IntPtr loggingCB);

    [StructLayout(LayoutKind.Sequential)]
    /// The OculusInitParams struct is used to specify initialization parameters for the Oculus Platform SDK. Specifically, it is used in the `Platform.InitializeStandaloneOculus` call. It is used to represent vital data that is used to initialize the SDK.
    /// See more details [here](https://developer.oculus.com/documentation/unity/ps-setup/#use-the-platform-in-standalone-mode).
    public struct OculusInitParams
    {
      /// Credentials struct type
      public int sType;
      /// The email field is a string that represents the email address that is associated with the Oculus account.
      public string email;
      /// The password field is a string that represents the password that is associated with the Oculus account.
      public string password;
      /// The appID field is an unsigned integer that represents the id of the Application that is initializing the Oculus Platform SDK.
      public UInt64 appId;
      /// Optional override for 'https://graph.oculus.com'
      public string uriPrefixOverride;
    }

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern ulong ovr_Platform_InitializeStandaloneOculus(ref OculusInitParams init);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern ulong ovr_PlatformInitializeWithAccessToken(UInt64 appId, string accessToken);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern ulong ovr_PlatformInitializeWithAccessTokenAndOptions(UInt64 appId, string accessToken, ovrKeyValuePair[] configOptions, UIntPtr numOptions);

    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern bool ovr_UnityInitWrapperWindows(string appId, IntPtr loggingCB);

    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_UnityInitWrapperWindowsAsynchronous(string appId, IntPtr loggingCB);

    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern bool ovr_SetDeveloperAccessToken(string accessToken);

    public static string ovr_GetLoggedInUserLocale() {
      var result = StringFromNative(ovr_GetLoggedInUserLocale_Native());
      return result;
    }

    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_GetLoggedInUserLocale")]
    private static extern IntPtr ovr_GetLoggedInUserLocale_Native();


    // Message queue access

    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_PopMessage();

    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_FreeMessage(IntPtr message);


    // VOIP

    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Voip_CreateEncoder();

    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_Voip_DestroyEncoder(IntPtr encoder);

    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Voip_CreateDecoder();

    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_Voip_DestroyDecoder(IntPtr decoder);

    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_VoipDecoder_Decode(IntPtr obj, byte[] compressedData, ulong compressedSize);

    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Microphone_Create();

    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_Microphone_Destroy(IntPtr obj);

    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_Voip_SetSystemVoipPassthrough(bool passthrough);

    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_Voip_SetSystemVoipMicrophoneMuted(VoipMuteState muted);

    // Misc

    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_UnityResetTestPlatform();

    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_HTTP_GetWithMessageType(string url, int messageType);

    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_CrashApplication();

    /// The VoipFilterBufferSize constant specifies the size of the buffer used for VOIP (Voice over Internet Protocol) filtering. It gets passed to the FilterCallback() function to specify the size of the buffer.
    public const int VoipFilterBufferSize = 480;
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void FilterCallback([MarshalAs(UnmanagedType.LPArray, SizeConst = VoipFilterBufferSize), In, Out] short[] pcmData, UIntPtr pcmDataLength, int frequency, int numChannels);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern void ovr_Voip_SetMicrophoneFilterCallback(FilterCallback cb);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern void ovr_Voip_SetMicrophoneFilterCallbackWithFixedSizeBuffer(FilterCallback cb, UIntPtr bufferSizeElements);


    // Logging
    public static void LogNewUnifiedEvent(LogEventName eventName, Dictionary<string, string> values) {
      LogNewEvent(eventName.ToString(), values);
    }

    public static void LogNewEvent(string eventName, Dictionary<string, string> values) {
      var eventNameNative = StringToNative(eventName);

      var count = values == null ? 0 : values.Count;

      IntPtr[] valuesNative = new IntPtr[count * 2];

      if (count > 0) {
        int i = 0;
        foreach(var item in values) {
          valuesNative[i * 2 + 0] = StringToNative(item.Key);
          valuesNative[i * 2 + 1] = StringToNative(item.Value);
          i++;
        }
      }

      ovr_Log_NewEvent(eventNameNative, valuesNative, (UIntPtr)count);

      Marshal.FreeCoTaskMem(eventNameNative);
      foreach (var nativeItem in valuesNative) {
        Marshal.FreeCoTaskMem(nativeItem);
      }
    }

    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_Log_NewEvent(IntPtr eventName, IntPtr[] values, UIntPtr length);


    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_ApplicationLifecycle_GetLaunchDetails();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static void ovr_ApplicationLifecycle_LogDeeplinkResult(string trackingID, LaunchResult result) {
      IntPtr trackingID_native = StringToNative(trackingID);
      ovr_ApplicationLifecycle_LogDeeplinkResult_Native(trackingID_native, result);
      Marshal.FreeCoTaskMem(trackingID_native);
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_ApplicationLifecycle_LogDeeplinkResult")]
    private static extern void ovr_ApplicationLifecycle_LogDeeplinkResult_Native(IntPtr trackingID, LaunchResult result);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static ulong ovr_HTTP_StartTransfer(string url, ovrKeyValuePair[] headers) {
      IntPtr url_native = StringToNative(url);
      UIntPtr headers_length = (UIntPtr)headers.Length;
      var result = (ovr_HTTP_StartTransfer_Native(url_native, headers, headers_length));
      Marshal.FreeCoTaskMem(url_native);
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_HTTP_StartTransfer")]
    private static extern ulong ovr_HTTP_StartTransfer_Native(IntPtr url, ovrKeyValuePair[] headers, UIntPtr numItems);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern bool ovr_HTTP_Write(ulong transferId, byte[] bytes, UIntPtr length);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_HTTP_WriteEOM(ulong transferId);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_Message_GetStringForJavascript(IntPtr message) {
      var result = StringFromNative(ovr_Message_GetStringForJavascript_Native(message));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_Message_GetStringForJavascript")]
    private static extern IntPtr ovr_Message_GetStringForJavascript_Native(IntPtr message);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UIntPtr ovr_NetSync_GetAmbisonicFloatPCM(long connection_id, float[] outputBuffer, UIntPtr outputBufferNumElements);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UIntPtr ovr_NetSync_GetAmbisonicInt16PCM(long connection_id, Int16[] outputBuffer, UIntPtr outputBufferNumElements);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UIntPtr ovr_NetSync_GetAmbisonicInterleavedFloatPCM(long connection_id, float[] outputBuffer, UIntPtr outputBufferNumElements);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UIntPtr ovr_NetSync_GetAmbisonicInterleavedInt16PCM(long connection_id, Int16[] outputBuffer, UIntPtr outputBufferNumElements);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern bool ovr_NetSync_GetListenerPosition(long connection_id, UInt64 sessionId, ref ovrNetSyncVec3 position);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UIntPtr ovr_NetSync_GetMonostreamFloatPCM(long connection_id, UInt64 sessionId, float[] outputBuffer, UIntPtr outputBufferNumElements);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UIntPtr ovr_NetSync_GetMonostreamInt16PCM(long connection_id, UInt64 session_id, Int16[] outputBuffer, UIntPtr outputBufferNumElements);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UIntPtr ovr_NetSync_GetPcmBufferMaxSamples();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern bool ovr_NetSync_GetVoipAmplitude(long connection_id, UInt64 sessionId, ref float amplitude);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_NetSync_SetListenerPosition(long connection_id, ref ovrNetSyncVec3 position);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern int ovr_Party_PluginGetSharedMemHandle();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern VoipMuteState ovr_Party_PluginGetVoipMicrophoneMuted();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern bool ovr_Party_PluginGetVoipPassthrough();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern SystemVoipStatus ovr_Party_PluginGetVoipStatus();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_Voip_Accept(UInt64 userID);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern VoipDtxState ovr_Voip_GetIsConnectionUsingDtx(UInt64 peerID);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern VoipBitrate ovr_Voip_GetLocalBitrate(UInt64 peerID);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UIntPtr ovr_Voip_GetOutputBufferMaxSize();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UIntPtr ovr_Voip_GetPCM(UInt64 senderID, Int16[] outputBuffer, UIntPtr outputBufferNumElements);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UIntPtr ovr_Voip_GetPCMFloat(UInt64 senderID, float[] outputBuffer, UIntPtr outputBufferNumElements);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UIntPtr ovr_Voip_GetPCMSize(UInt64 senderID);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UIntPtr ovr_Voip_GetPCMWithTimestamp(UInt64 senderID, Int16[] outputBuffer, UIntPtr outputBufferNumElements, UInt32[] timestamp);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UIntPtr ovr_Voip_GetPCMWithTimestampFloat(UInt64 senderID, float[] outputBuffer, UIntPtr outputBufferNumElements, UInt32[] timestamp);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern VoipBitrate ovr_Voip_GetRemoteBitrate(UInt64 peerID);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UInt32 ovr_Voip_GetSyncTimestamp(UInt64 userID);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern long ovr_Voip_GetSyncTimestampDifference(UInt32 lhs, UInt32 rhs);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern VoipMuteState ovr_Voip_GetSystemVoipMicrophoneMuted();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern SystemVoipStatus ovr_Voip_GetSystemVoipStatus();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_Voip_SetMicrophoneMuted(VoipMuteState state);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_Voip_SetNewConnectionOptions(IntPtr voipOptions);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_Voip_SetOutputSampleRate(VoipSampleRate rate);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_Voip_Start(UInt64 userID);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_Voip_Stop(UInt64 userID);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_AbuseReport_LaunchAdvancedReportFlow(UInt64 content_id, IntPtr abuse_report_options);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_AbuseReport_ReportRequestHandled(ReportRequestResponse response);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static ulong ovr_Achievements_AddCount(string name, ulong count) {
      IntPtr name_native = StringToNative(name);
      var result = (ovr_Achievements_AddCount_Native(name_native, count));
      Marshal.FreeCoTaskMem(name_native);
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_Achievements_AddCount")]
    private static extern ulong ovr_Achievements_AddCount_Native(IntPtr name, ulong count);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static ulong ovr_Achievements_AddFields(string name, string fields) {
      IntPtr name_native = StringToNative(name);
      IntPtr fields_native = StringToNative(fields);
      var result = (ovr_Achievements_AddFields_Native(name_native, fields_native));
      Marshal.FreeCoTaskMem(name_native);
      Marshal.FreeCoTaskMem(fields_native);
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_Achievements_AddFields")]
    private static extern ulong ovr_Achievements_AddFields_Native(IntPtr name, IntPtr fields);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_Achievements_GetAllDefinitions();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_Achievements_GetAllProgress();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_Achievements_GetDefinitionsByName(string[] names, int count);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_Achievements_GetProgressByName(string[] names, int count);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static ulong ovr_Achievements_Unlock(string name) {
      IntPtr name_native = StringToNative(name);
      var result = (ovr_Achievements_Unlock_Native(name_native));
      Marshal.FreeCoTaskMem(name_native);
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_Achievements_Unlock")]
    private static extern ulong ovr_Achievements_Unlock_Native(IntPtr name);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_Application_CancelAppDownload();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_Application_CheckAppDownloadProgress();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_Application_GetInstalledApplications();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_Application_GetVersion();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_Application_InstallAppUpdateAndRelaunch(IntPtr deeplink_options);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_Application_LaunchOtherApp(UInt64 appID, IntPtr deeplink_options);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_Application_StartAppDownload();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_ApplicationLifecycle_GetRegisteredPIDs();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_ApplicationLifecycle_GetSessionKey();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static ulong ovr_ApplicationLifecycle_RegisterSessionKey(string sessionKey) {
      IntPtr sessionKey_native = StringToNative(sessionKey);
      var result = (ovr_ApplicationLifecycle_RegisterSessionKey_Native(sessionKey_native));
      Marshal.FreeCoTaskMem(sessionKey_native);
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_ApplicationLifecycle_RegisterSessionKey")]
    private static extern ulong ovr_ApplicationLifecycle_RegisterSessionKey_Native(IntPtr sessionKey);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_AssetFile_Delete(UInt64 assetFileID);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_AssetFile_DeleteById(UInt64 assetFileID);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static ulong ovr_AssetFile_DeleteByName(string assetFileName) {
      IntPtr assetFileName_native = StringToNative(assetFileName);
      var result = (ovr_AssetFile_DeleteByName_Native(assetFileName_native));
      Marshal.FreeCoTaskMem(assetFileName_native);
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_AssetFile_DeleteByName")]
    private static extern ulong ovr_AssetFile_DeleteByName_Native(IntPtr assetFileName);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_AssetFile_Download(UInt64 assetFileID);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_AssetFile_DownloadById(UInt64 assetFileID);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static ulong ovr_AssetFile_DownloadByName(string assetFileName) {
      IntPtr assetFileName_native = StringToNative(assetFileName);
      var result = (ovr_AssetFile_DownloadByName_Native(assetFileName_native));
      Marshal.FreeCoTaskMem(assetFileName_native);
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_AssetFile_DownloadByName")]
    private static extern ulong ovr_AssetFile_DownloadByName_Native(IntPtr assetFileName);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_AssetFile_DownloadCancel(UInt64 assetFileID);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_AssetFile_DownloadCancelById(UInt64 assetFileID);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static ulong ovr_AssetFile_DownloadCancelByName(string assetFileName) {
      IntPtr assetFileName_native = StringToNative(assetFileName);
      var result = (ovr_AssetFile_DownloadCancelByName_Native(assetFileName_native));
      Marshal.FreeCoTaskMem(assetFileName_native);
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_AssetFile_DownloadCancelByName")]
    private static extern ulong ovr_AssetFile_DownloadCancelByName_Native(IntPtr assetFileName);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_AssetFile_GetList();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_AssetFile_Status(UInt64 assetFileID);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_AssetFile_StatusById(UInt64 assetFileID);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static ulong ovr_AssetFile_StatusByName(string assetFileName) {
      IntPtr assetFileName_native = StringToNative(assetFileName);
      var result = (ovr_AssetFile_StatusByName_Native(assetFileName_native));
      Marshal.FreeCoTaskMem(assetFileName_native);
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_AssetFile_StatusByName")]
    private static extern ulong ovr_AssetFile_StatusByName_Native(IntPtr assetFileName);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_Avatar_LaunchAvatarEditor(IntPtr options);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static ulong ovr_Avatar_UpdateMetaData(string avatarMetaData, string imageFilePath) {
      IntPtr avatarMetaData_native = StringToNative(avatarMetaData);
      IntPtr imageFilePath_native = StringToNative(imageFilePath);
      var result = (ovr_Avatar_UpdateMetaData_Native(avatarMetaData_native, imageFilePath_native));
      Marshal.FreeCoTaskMem(avatarMetaData_native);
      Marshal.FreeCoTaskMem(imageFilePath_native);
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_Avatar_UpdateMetaData")]
    private static extern ulong ovr_Avatar_UpdateMetaData_Native(IntPtr avatarMetaData, IntPtr imageFilePath);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static ulong ovr_Challenges_Create(string leaderboardName, IntPtr challengeOptions) {
      IntPtr leaderboardName_native = StringToNative(leaderboardName);
      var result = (ovr_Challenges_Create_Native(leaderboardName_native, challengeOptions));
      Marshal.FreeCoTaskMem(leaderboardName_native);
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_Challenges_Create")]
    private static extern ulong ovr_Challenges_Create_Native(IntPtr leaderboardName, IntPtr challengeOptions);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_Challenges_DeclineInvite(UInt64 challengeID);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_Challenges_Delete(UInt64 challengeID);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_Challenges_Get(UInt64 challengeID);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_Challenges_GetEntries(UInt64 challengeID, int limit, LeaderboardFilterType filter, LeaderboardStartAt startAt);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_Challenges_GetEntriesAfterRank(UInt64 challengeID, int limit, ulong afterRank);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_Challenges_GetEntriesByIds(UInt64 challengeID, int limit, LeaderboardStartAt startAt, UInt64[] userIDs, uint userIDLength);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_Challenges_GetList(IntPtr challengeOptions, int limit);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_Challenges_GetNextChallenges(IntPtr handle);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_Challenges_GetNextEntries(IntPtr handle);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_Challenges_GetPreviousChallenges(IntPtr handle);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_Challenges_GetPreviousEntries(IntPtr handle);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_Challenges_Join(UInt64 challengeID);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_Challenges_Leave(UInt64 challengeID);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_Challenges_UpdateInfo(UInt64 challengeID, IntPtr challengeOptions);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_Colocation_GetCurrentMapUuid();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static ulong ovr_Colocation_RequestMap(string uuid) {
      IntPtr uuid_native = StringToNative(uuid);
      var result = (ovr_Colocation_RequestMap_Native(uuid_native));
      Marshal.FreeCoTaskMem(uuid_native);
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_Colocation_RequestMap")]
    private static extern ulong ovr_Colocation_RequestMap_Native(IntPtr uuid);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static ulong ovr_Colocation_ShareMap(string uuid) {
      IntPtr uuid_native = StringToNative(uuid);
      var result = (ovr_Colocation_ShareMap_Native(uuid_native));
      Marshal.FreeCoTaskMem(uuid_native);
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_Colocation_ShareMap")]
    private static extern ulong ovr_Colocation_ShareMap_Native(IntPtr uuid);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_Cowatching_GetPresenterData();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_Cowatching_GetViewersData();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_Cowatching_IsInSession();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_Cowatching_JoinSession();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_Cowatching_LaunchInviteDialog();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_Cowatching_LeaveSession();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_Cowatching_RequestToPresent();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_Cowatching_ResignFromPresenting();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static ulong ovr_Cowatching_SetPresenterData(string video_title, string presenter_data) {
      IntPtr video_title_native = StringToNative(video_title);
      IntPtr presenter_data_native = StringToNative(presenter_data);
      var result = (ovr_Cowatching_SetPresenterData_Native(video_title_native, presenter_data_native));
      Marshal.FreeCoTaskMem(video_title_native);
      Marshal.FreeCoTaskMem(presenter_data_native);
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_Cowatching_SetPresenterData")]
    private static extern ulong ovr_Cowatching_SetPresenterData_Native(IntPtr video_title, IntPtr presenter_data);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static ulong ovr_Cowatching_SetViewerData(string viewer_data) {
      IntPtr viewer_data_native = StringToNative(viewer_data);
      var result = (ovr_Cowatching_SetViewerData_Native(viewer_data_native));
      Marshal.FreeCoTaskMem(viewer_data_native);
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_Cowatching_SetViewerData")]
    private static extern ulong ovr_Cowatching_SetViewerData_Native(IntPtr viewer_data);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static ulong ovr_DeviceApplicationIntegrity_GetAttestationToken(string challenge_nonce) {
      IntPtr challenge_nonce_native = StringToNative(challenge_nonce);
      var result = (ovr_DeviceApplicationIntegrity_GetAttestationToken_Native(challenge_nonce_native));
      Marshal.FreeCoTaskMem(challenge_nonce_native);
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_DeviceApplicationIntegrity_GetAttestationToken")]
    private static extern ulong ovr_DeviceApplicationIntegrity_GetAttestationToken_Native(IntPtr challenge_nonce);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static ulong ovr_DeviceApplicationIntegrity_GetIntegrityToken(string challenge_nonce) {
      IntPtr challenge_nonce_native = StringToNative(challenge_nonce);
      var result = (ovr_DeviceApplicationIntegrity_GetIntegrityToken_Native(challenge_nonce_native));
      Marshal.FreeCoTaskMem(challenge_nonce_native);
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_DeviceApplicationIntegrity_GetIntegrityToken")]
    private static extern ulong ovr_DeviceApplicationIntegrity_GetIntegrityToken_Native(IntPtr challenge_nonce);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_Entitlement_GetIsViewerEntitled();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static ulong ovr_GraphAPI_Get(string url) {
      IntPtr url_native = StringToNative(url);
      var result = (ovr_GraphAPI_Get_Native(url_native));
      Marshal.FreeCoTaskMem(url_native);
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_GraphAPI_Get")]
    private static extern ulong ovr_GraphAPI_Get_Native(IntPtr url);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static ulong ovr_GraphAPI_Post(string url) {
      IntPtr url_native = StringToNative(url);
      var result = (ovr_GraphAPI_Post_Native(url_native));
      Marshal.FreeCoTaskMem(url_native);
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_GraphAPI_Post")]
    private static extern ulong ovr_GraphAPI_Post_Native(IntPtr url);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_GroupPresence_Clear();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_GroupPresence_GetInvitableUsers(IntPtr options);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_GroupPresence_GetSentInvites();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_GroupPresence_LaunchInvitePanel(IntPtr options);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_GroupPresence_LaunchMultiplayerErrorDialog(IntPtr options);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static ulong ovr_GroupPresence_LaunchRejoinDialog(string lobby_session_id, string match_session_id, string destination_api_name) {
      IntPtr lobby_session_id_native = StringToNative(lobby_session_id);
      IntPtr match_session_id_native = StringToNative(match_session_id);
      IntPtr destination_api_name_native = StringToNative(destination_api_name);
      var result = (ovr_GroupPresence_LaunchRejoinDialog_Native(lobby_session_id_native, match_session_id_native, destination_api_name_native));
      Marshal.FreeCoTaskMem(lobby_session_id_native);
      Marshal.FreeCoTaskMem(match_session_id_native);
      Marshal.FreeCoTaskMem(destination_api_name_native);
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_GroupPresence_LaunchRejoinDialog")]
    private static extern ulong ovr_GroupPresence_LaunchRejoinDialog_Native(IntPtr lobby_session_id, IntPtr match_session_id, IntPtr destination_api_name);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_GroupPresence_LaunchRosterPanel(IntPtr options);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_GroupPresence_SendInvites(UInt64[] userIDs, uint userIDLength);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_GroupPresence_Set(IntPtr groupPresenceOptions);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static ulong ovr_GroupPresence_SetDeeplinkMessageOverride(string deeplink_message) {
      IntPtr deeplink_message_native = StringToNative(deeplink_message);
      var result = (ovr_GroupPresence_SetDeeplinkMessageOverride_Native(deeplink_message_native));
      Marshal.FreeCoTaskMem(deeplink_message_native);
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_GroupPresence_SetDeeplinkMessageOverride")]
    private static extern ulong ovr_GroupPresence_SetDeeplinkMessageOverride_Native(IntPtr deeplink_message);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static ulong ovr_GroupPresence_SetDestination(string api_name) {
      IntPtr api_name_native = StringToNative(api_name);
      var result = (ovr_GroupPresence_SetDestination_Native(api_name_native));
      Marshal.FreeCoTaskMem(api_name_native);
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_GroupPresence_SetDestination")]
    private static extern ulong ovr_GroupPresence_SetDestination_Native(IntPtr api_name);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_GroupPresence_SetIsJoinable(bool is_joinable);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static ulong ovr_GroupPresence_SetLobbySession(string id) {
      IntPtr id_native = StringToNative(id);
      var result = (ovr_GroupPresence_SetLobbySession_Native(id_native));
      Marshal.FreeCoTaskMem(id_native);
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_GroupPresence_SetLobbySession")]
    private static extern ulong ovr_GroupPresence_SetLobbySession_Native(IntPtr id);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static ulong ovr_GroupPresence_SetMatchSession(string id) {
      IntPtr id_native = StringToNative(id);
      var result = (ovr_GroupPresence_SetMatchSession_Native(id_native));
      Marshal.FreeCoTaskMem(id_native);
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_GroupPresence_SetMatchSession")]
    private static extern ulong ovr_GroupPresence_SetMatchSession_Native(IntPtr id);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static ulong ovr_HTTP_Get(string url) {
      IntPtr url_native = StringToNative(url);
      var result = (ovr_HTTP_Get_Native(url_native));
      Marshal.FreeCoTaskMem(url_native);
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_HTTP_Get")]
    private static extern ulong ovr_HTTP_Get_Native(IntPtr url);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static ulong ovr_HTTP_GetToFile(string url, string diskFile) {
      IntPtr url_native = StringToNative(url);
      IntPtr diskFile_native = StringToNative(diskFile);
      var result = (ovr_HTTP_GetToFile_Native(url_native, diskFile_native));
      Marshal.FreeCoTaskMem(url_native);
      Marshal.FreeCoTaskMem(diskFile_native);
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_HTTP_GetToFile")]
    private static extern ulong ovr_HTTP_GetToFile_Native(IntPtr url, IntPtr diskFile);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static ulong ovr_HTTP_MultiPartPost(string url, string filepath_param_name, string filepath, string access_token, ovrKeyValuePair[] post_params) {
      IntPtr url_native = StringToNative(url);
      IntPtr filepath_param_name_native = StringToNative(filepath_param_name);
      IntPtr filepath_native = StringToNative(filepath);
      IntPtr access_token_native = StringToNative(access_token);
      UIntPtr post_params_length = (UIntPtr)post_params.Length;
      var result = (ovr_HTTP_MultiPartPost_Native(url_native, filepath_param_name_native, filepath_native, access_token_native, post_params, post_params_length));
      Marshal.FreeCoTaskMem(url_native);
      Marshal.FreeCoTaskMem(filepath_param_name_native);
      Marshal.FreeCoTaskMem(filepath_native);
      Marshal.FreeCoTaskMem(access_token_native);
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_HTTP_MultiPartPost")]
    private static extern ulong ovr_HTTP_MultiPartPost_Native(IntPtr url, IntPtr filepath_param_name, IntPtr filepath, IntPtr access_token, ovrKeyValuePair[] post_params, UIntPtr numItems);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static ulong ovr_HTTP_Post(string url) {
      IntPtr url_native = StringToNative(url);
      var result = (ovr_HTTP_Post_Native(url_native));
      Marshal.FreeCoTaskMem(url_native);
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_HTTP_Post")]
    private static extern ulong ovr_HTTP_Post_Native(IntPtr url);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static ulong ovr_IAP_ConsumePurchase(string sku) {
      IntPtr sku_native = StringToNative(sku);
      var result = (ovr_IAP_ConsumePurchase_Native(sku_native));
      Marshal.FreeCoTaskMem(sku_native);
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_IAP_ConsumePurchase")]
    private static extern ulong ovr_IAP_ConsumePurchase_Native(IntPtr sku);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_IAP_GetProductsBySKU(string[] skus, int count);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_IAP_GetViewerPurchases();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_IAP_GetViewerPurchasesDurableCache();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static ulong ovr_IAP_LaunchCheckoutFlow(string sku) {
      IntPtr sku_native = StringToNative(sku);
      var result = (ovr_IAP_LaunchCheckoutFlow_Native(sku_native));
      Marshal.FreeCoTaskMem(sku_native);
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_IAP_LaunchCheckoutFlow")]
    private static extern ulong ovr_IAP_LaunchCheckoutFlow_Native(IntPtr sku);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_LanguagePack_GetCurrent();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static ulong ovr_LanguagePack_SetCurrent(string tag) {
      IntPtr tag_native = StringToNative(tag);
      var result = (ovr_LanguagePack_SetCurrent_Native(tag_native));
      Marshal.FreeCoTaskMem(tag_native);
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_LanguagePack_SetCurrent")]
    private static extern ulong ovr_LanguagePack_SetCurrent_Native(IntPtr tag);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static ulong ovr_Leaderboard_Get(string leaderboardName) {
      IntPtr leaderboardName_native = StringToNative(leaderboardName);
      var result = (ovr_Leaderboard_Get_Native(leaderboardName_native));
      Marshal.FreeCoTaskMem(leaderboardName_native);
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_Leaderboard_Get")]
    private static extern ulong ovr_Leaderboard_Get_Native(IntPtr leaderboardName);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static ulong ovr_Leaderboard_GetEntries(string leaderboardName, int limit, LeaderboardFilterType filter, LeaderboardStartAt startAt) {
      IntPtr leaderboardName_native = StringToNative(leaderboardName);
      var result = (ovr_Leaderboard_GetEntries_Native(leaderboardName_native, limit, filter, startAt));
      Marshal.FreeCoTaskMem(leaderboardName_native);
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_Leaderboard_GetEntries")]
    private static extern ulong ovr_Leaderboard_GetEntries_Native(IntPtr leaderboardName, int limit, LeaderboardFilterType filter, LeaderboardStartAt startAt);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static ulong ovr_Leaderboard_GetEntriesAfterRank(string leaderboardName, int limit, ulong afterRank) {
      IntPtr leaderboardName_native = StringToNative(leaderboardName);
      var result = (ovr_Leaderboard_GetEntriesAfterRank_Native(leaderboardName_native, limit, afterRank));
      Marshal.FreeCoTaskMem(leaderboardName_native);
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_Leaderboard_GetEntriesAfterRank")]
    private static extern ulong ovr_Leaderboard_GetEntriesAfterRank_Native(IntPtr leaderboardName, int limit, ulong afterRank);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static ulong ovr_Leaderboard_GetEntriesByIds(string leaderboardName, int limit, LeaderboardStartAt startAt, UInt64[] userIDs, uint userIDLength) {
      IntPtr leaderboardName_native = StringToNative(leaderboardName);
      var result = (ovr_Leaderboard_GetEntriesByIds_Native(leaderboardName_native, limit, startAt, userIDs, userIDLength));
      Marshal.FreeCoTaskMem(leaderboardName_native);
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_Leaderboard_GetEntriesByIds")]
    private static extern ulong ovr_Leaderboard_GetEntriesByIds_Native(IntPtr leaderboardName, int limit, LeaderboardStartAt startAt, UInt64[] userIDs, uint userIDLength);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_Leaderboard_GetNextEntries(IntPtr handle);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_Leaderboard_GetPreviousEntries(IntPtr handle);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static ulong ovr_Leaderboard_WriteEntry(string leaderboardName, long score, byte[] extraData, uint extraDataLength, bool forceUpdate) {
      IntPtr leaderboardName_native = StringToNative(leaderboardName);
      var result = (ovr_Leaderboard_WriteEntry_Native(leaderboardName_native, score, extraData, extraDataLength, forceUpdate));
      Marshal.FreeCoTaskMem(leaderboardName_native);
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_Leaderboard_WriteEntry")]
    private static extern ulong ovr_Leaderboard_WriteEntry_Native(IntPtr leaderboardName, long score, byte[] extraData, uint extraDataLength, bool forceUpdate);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static ulong ovr_Leaderboard_WriteEntryWithSupplementaryMetric(string leaderboardName, long score, long supplementaryMetric, byte[] extraData, uint extraDataLength, bool forceUpdate) {
      IntPtr leaderboardName_native = StringToNative(leaderboardName);
      var result = (ovr_Leaderboard_WriteEntryWithSupplementaryMetric_Native(leaderboardName_native, score, supplementaryMetric, extraData, extraDataLength, forceUpdate));
      Marshal.FreeCoTaskMem(leaderboardName_native);
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_Leaderboard_WriteEntryWithSupplementaryMetric")]
    private static extern ulong ovr_Leaderboard_WriteEntryWithSupplementaryMetric_Native(IntPtr leaderboardName, long score, long supplementaryMetric, byte[] extraData, uint extraDataLength, bool forceUpdate);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static ulong ovr_Livestreaming_IsAllowedForApplication(string packageName) {
      IntPtr packageName_native = StringToNative(packageName);
      var result = (ovr_Livestreaming_IsAllowedForApplication_Native(packageName_native));
      Marshal.FreeCoTaskMem(packageName_native);
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_Livestreaming_IsAllowedForApplication")]
    private static extern ulong ovr_Livestreaming_IsAllowedForApplication_Native(IntPtr packageName);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_Livestreaming_StartPartyStream();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_Livestreaming_StartStream(LivestreamingAudience audience, LivestreamingMicrophoneStatus micStatus);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_Livestreaming_StopPartyStream();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_Livestreaming_StopStream();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_Livestreaming_UpdateMicStatus(LivestreamingMicrophoneStatus micStatus);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static ulong ovr_Media_ShareToFacebook(string postTextSuggestion, string filePath, MediaContentType contentType) {
      IntPtr postTextSuggestion_native = StringToNative(postTextSuggestion);
      IntPtr filePath_native = StringToNative(filePath);
      var result = (ovr_Media_ShareToFacebook_Native(postTextSuggestion_native, filePath_native, contentType));
      Marshal.FreeCoTaskMem(postTextSuggestion_native);
      Marshal.FreeCoTaskMem(filePath_native);
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_Media_ShareToFacebook")]
    private static extern ulong ovr_Media_ShareToFacebook_Native(IntPtr postTextSuggestion, IntPtr filePath, MediaContentType contentType);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_NetSync_Connect(IntPtr connect_options);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_NetSync_Disconnect(long connection_id);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_NetSync_GetSessions(long connection_id);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_NetSync_GetVoipAttenuation(long connection_id);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_NetSync_GetVoipAttenuationDefault();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_NetSync_SetVoipAttenuation(long connection_id, float[] distances, float[] decibels, UIntPtr count);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static ulong ovr_NetSync_SetVoipAttenuationModel(long connection_id, string name, float[] distances, float[] decibels, UIntPtr count) {
      IntPtr name_native = StringToNative(name);
      var result = (ovr_NetSync_SetVoipAttenuationModel_Native(connection_id, name_native, distances, decibels, count));
      Marshal.FreeCoTaskMem(name_native);
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_NetSync_SetVoipAttenuationModel")]
    private static extern ulong ovr_NetSync_SetVoipAttenuationModel_Native(long connection_id, IntPtr name, float[] distances, float[] decibels, UIntPtr count);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static ulong ovr_NetSync_SetVoipChannelCfg(long connection_id, string channel_name, string attnmodel, bool disable_spatialization) {
      IntPtr channel_name_native = StringToNative(channel_name);
      IntPtr attnmodel_native = StringToNative(attnmodel);
      var result = (ovr_NetSync_SetVoipChannelCfg_Native(connection_id, channel_name_native, attnmodel_native, disable_spatialization));
      Marshal.FreeCoTaskMem(channel_name_native);
      Marshal.FreeCoTaskMem(attnmodel_native);
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_NetSync_SetVoipChannelCfg")]
    private static extern ulong ovr_NetSync_SetVoipChannelCfg_Native(long connection_id, IntPtr channel_name, IntPtr attnmodel, bool disable_spatialization);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static ulong ovr_NetSync_SetVoipGroup(long connection_id, string group_id) {
      IntPtr group_id_native = StringToNative(group_id);
      var result = (ovr_NetSync_SetVoipGroup_Native(connection_id, group_id_native));
      Marshal.FreeCoTaskMem(group_id_native);
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_NetSync_SetVoipGroup")]
    private static extern ulong ovr_NetSync_SetVoipGroup_Native(long connection_id, IntPtr group_id);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_NetSync_SetVoipListentoChannels(long connection_id, string[] listento_channels, UIntPtr count);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_NetSync_SetVoipMicSource(long connection_id, NetSyncVoipMicSource mic_source);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_NetSync_SetVoipSessionMuted(long connection_id, UInt64 session_id, bool muted);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_NetSync_SetVoipSpeaktoChannels(long connection_id, string[] speakto_channels, UIntPtr count);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_NetSync_SetVoipStreamMode(long connection_id, UInt64 sessionId, NetSyncVoipStreamMode streamMode);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_Notification_MarkAsRead(UInt64 notificationID);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_Party_Create();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_Party_GatherInApplication(UInt64 partyID, UInt64 appID);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_Party_Get(UInt64 partyID);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_Party_GetCurrent();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_Party_GetCurrentForUser(UInt64 userID);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_Party_Invite(UInt64 partyID, UInt64 userID);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_Party_Join(UInt64 partyID);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_Party_Leave(UInt64 partyID);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_PushNotification_Register();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_RichPresence_Clear();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_RichPresence_GetDestinations();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_RichPresence_Set(IntPtr richPresenceOptions);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static ulong ovr_RichPresence_SetDestination(string api_name) {
      IntPtr api_name_native = StringToNative(api_name);
      var result = (ovr_RichPresence_SetDestination_Native(api_name_native));
      Marshal.FreeCoTaskMem(api_name_native);
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_RichPresence_SetDestination")]
    private static extern ulong ovr_RichPresence_SetDestination_Native(IntPtr api_name);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_RichPresence_SetIsJoinable(bool is_joinable);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static ulong ovr_RichPresence_SetLobbySession(string id) {
      IntPtr id_native = StringToNative(id);
      var result = (ovr_RichPresence_SetLobbySession_Native(id_native));
      Marshal.FreeCoTaskMem(id_native);
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_RichPresence_SetLobbySession")]
    private static extern ulong ovr_RichPresence_SetLobbySession_Native(IntPtr id);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static ulong ovr_RichPresence_SetMatchSession(string id) {
      IntPtr id_native = StringToNative(id);
      var result = (ovr_RichPresence_SetMatchSession_Native(id_native));
      Marshal.FreeCoTaskMem(id_native);
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_RichPresence_SetMatchSession")]
    private static extern ulong ovr_RichPresence_SetMatchSession_Native(IntPtr id);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static ulong ovr_User_CancelRecordingForReportFlow(string recordingUUID) {
      IntPtr recordingUUID_native = StringToNative(recordingUUID);
      var result = (ovr_User_CancelRecordingForReportFlow_Native(recordingUUID_native));
      Marshal.FreeCoTaskMem(recordingUUID_native);
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_User_CancelRecordingForReportFlow")]
    private static extern ulong ovr_User_CancelRecordingForReportFlow_Native(IntPtr recordingUUID);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_User_Get(UInt64 userID);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_User_GetAccessToken();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_User_GetBlockedUsers();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_User_GetLinkedAccounts(IntPtr userOptions);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_User_GetLoggedInUser();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_User_GetLoggedInUserFriends();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_User_GetLoggedInUserManagedInfo();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_User_GetOrgScopedID(UInt64 userID);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_User_GetSdkAccounts();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_User_GetUserCapabilities();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_User_GetUserProof();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_User_LaunchBlockFlow(UInt64 userID);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_User_LaunchFriendRequestFlow(UInt64 userID);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_User_LaunchReportFlow(UInt64 userID);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_User_LaunchReportFlow2(UInt64 optionalUserID, IntPtr abuseReportOptions);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_User_LaunchUnblockFlow(UInt64 userID);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_User_NewEntitledTestUser();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_User_NewTestUser();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_User_NewTestUserFriends();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_User_StartRecordingForReportFlow();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static ulong ovr_User_StopRecordingAndLaunchReportFlow(UInt64 optionalUserID, string optionalRecordingUUID) {
      IntPtr optionalRecordingUUID_native = StringToNative(optionalRecordingUUID);
      var result = (ovr_User_StopRecordingAndLaunchReportFlow_Native(optionalUserID, optionalRecordingUUID_native));
      Marshal.FreeCoTaskMem(optionalRecordingUUID_native);
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_User_StopRecordingAndLaunchReportFlow")]
    private static extern ulong ovr_User_StopRecordingAndLaunchReportFlow_Native(UInt64 optionalUserID, IntPtr optionalRecordingUUID);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static ulong ovr_User_StopRecordingAndLaunchReportFlow2(UInt64 optionalUserID, string optionalRecordingUUID, IntPtr abuseReportOptions) {
      IntPtr optionalRecordingUUID_native = StringToNative(optionalRecordingUUID);
      var result = (ovr_User_StopRecordingAndLaunchReportFlow2_Native(optionalUserID, optionalRecordingUUID_native, abuseReportOptions));
      Marshal.FreeCoTaskMem(optionalRecordingUUID_native);
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_User_StopRecordingAndLaunchReportFlow2")]
    private static extern ulong ovr_User_StopRecordingAndLaunchReportFlow2_Native(UInt64 optionalUserID, IntPtr optionalRecordingUUID, IntPtr abuseReportOptions);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static ulong ovr_User_TestUserCreateDeviceManifest(string deviceID, UInt64[] appIDs, int numAppIDs) {
      IntPtr deviceID_native = StringToNative(deviceID);
      var result = (ovr_User_TestUserCreateDeviceManifest_Native(deviceID_native, appIDs, numAppIDs));
      Marshal.FreeCoTaskMem(deviceID_native);
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_User_TestUserCreateDeviceManifest")]
    private static extern ulong ovr_User_TestUserCreateDeviceManifest_Native(IntPtr deviceID, UInt64[] appIDs, int numAppIDs);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_UserAgeCategory_Get();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_UserAgeCategory_Report(AppAgeCategory age_category);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_Voip_GetMicrophoneAvailability();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_Voip_ReportAppVoipSessions(UInt64[] sessionIDs);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_Voip_SetSystemVoipSuppressed(bool suppressed);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_AbuseReportRecording_GetRecordingUuid(IntPtr obj) {
      var result = StringFromNative(ovr_AbuseReportRecording_GetRecordingUuid_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_AbuseReportRecording_GetRecordingUuid")]
    private static extern IntPtr ovr_AbuseReportRecording_GetRecordingUuid_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern uint ovr_AchievementDefinition_GetBitfieldLength(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_AchievementDefinition_GetName(IntPtr obj) {
      var result = StringFromNative(ovr_AchievementDefinition_GetName_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_AchievementDefinition_GetName")]
    private static extern IntPtr ovr_AchievementDefinition_GetName_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_AchievementDefinition_GetTarget(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern AchievementType ovr_AchievementDefinition_GetType(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_AchievementDefinitionArray_GetElement(IntPtr obj, UIntPtr index);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_AchievementDefinitionArray_GetNextUrl(IntPtr obj) {
      var result = StringFromNative(ovr_AchievementDefinitionArray_GetNextUrl_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_AchievementDefinitionArray_GetNextUrl")]
    private static extern IntPtr ovr_AchievementDefinitionArray_GetNextUrl_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UIntPtr ovr_AchievementDefinitionArray_GetSize(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern bool ovr_AchievementDefinitionArray_HasNextPage(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_AchievementProgress_GetBitfield(IntPtr obj) {
      var result = StringFromNative(ovr_AchievementProgress_GetBitfield_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_AchievementProgress_GetBitfield")]
    private static extern IntPtr ovr_AchievementProgress_GetBitfield_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_AchievementProgress_GetCount(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern bool ovr_AchievementProgress_GetIsUnlocked(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_AchievementProgress_GetName(IntPtr obj) {
      var result = StringFromNative(ovr_AchievementProgress_GetName_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_AchievementProgress_GetName")]
    private static extern IntPtr ovr_AchievementProgress_GetName_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static DateTime ovr_AchievementProgress_GetUnlockTime(IntPtr obj) {
      var result = DateTimeFromNative(ovr_AchievementProgress_GetUnlockTime_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_AchievementProgress_GetUnlockTime")]
    private static extern ulong ovr_AchievementProgress_GetUnlockTime_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_AchievementProgressArray_GetElement(IntPtr obj, UIntPtr index);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_AchievementProgressArray_GetNextUrl(IntPtr obj) {
      var result = StringFromNative(ovr_AchievementProgressArray_GetNextUrl_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_AchievementProgressArray_GetNextUrl")]
    private static extern IntPtr ovr_AchievementProgressArray_GetNextUrl_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UIntPtr ovr_AchievementProgressArray_GetSize(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern bool ovr_AchievementProgressArray_HasNextPage(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern bool ovr_AchievementUpdate_GetJustUnlocked(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_AchievementUpdate_GetName(IntPtr obj) {
      var result = StringFromNative(ovr_AchievementUpdate_GetName_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_AchievementUpdate_GetName")]
    private static extern IntPtr ovr_AchievementUpdate_GetName_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern long ovr_AppDownloadProgressResult_GetDownloadBytes(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern long ovr_AppDownloadProgressResult_GetDownloadedBytes(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern AppStatus ovr_AppDownloadProgressResult_GetStatusCode(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern AppInstallResult ovr_AppDownloadResult_GetAppInstallResult(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern long ovr_AppDownloadResult_GetTimestamp(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UInt64 ovr_Application_GetID(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_ApplicationInvite_GetDestination(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UInt64 ovr_ApplicationInvite_GetID(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern bool ovr_ApplicationInvite_GetIsActive(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_ApplicationInvite_GetLobbySessionId(IntPtr obj) {
      var result = StringFromNative(ovr_ApplicationInvite_GetLobbySessionId_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_ApplicationInvite_GetLobbySessionId")]
    private static extern IntPtr ovr_ApplicationInvite_GetLobbySessionId_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_ApplicationInvite_GetMatchSessionId(IntPtr obj) {
      var result = StringFromNative(ovr_ApplicationInvite_GetMatchSessionId_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_ApplicationInvite_GetMatchSessionId")]
    private static extern IntPtr ovr_ApplicationInvite_GetMatchSessionId_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_ApplicationInvite_GetRecipient(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_ApplicationInviteArray_GetElement(IntPtr obj, UIntPtr index);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_ApplicationInviteArray_GetNextUrl(IntPtr obj) {
      var result = StringFromNative(ovr_ApplicationInviteArray_GetNextUrl_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_ApplicationInviteArray_GetNextUrl")]
    private static extern IntPtr ovr_ApplicationInviteArray_GetNextUrl_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UIntPtr ovr_ApplicationInviteArray_GetSize(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern bool ovr_ApplicationInviteArray_HasNextPage(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern int ovr_ApplicationVersion_GetCurrentCode(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_ApplicationVersion_GetCurrentName(IntPtr obj) {
      var result = StringFromNative(ovr_ApplicationVersion_GetCurrentName_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_ApplicationVersion_GetCurrentName")]
    private static extern IntPtr ovr_ApplicationVersion_GetCurrentName_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern int ovr_ApplicationVersion_GetLatestCode(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_ApplicationVersion_GetLatestName(IntPtr obj) {
      var result = StringFromNative(ovr_ApplicationVersion_GetLatestName_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_ApplicationVersion_GetLatestName")]
    private static extern IntPtr ovr_ApplicationVersion_GetLatestName_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern long ovr_ApplicationVersion_GetReleaseDate(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_ApplicationVersion_GetSize(IntPtr obj) {
      var result = StringFromNative(ovr_ApplicationVersion_GetSize_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_ApplicationVersion_GetSize")]
    private static extern IntPtr ovr_ApplicationVersion_GetSize_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UInt64 ovr_AssetDetails_GetAssetId(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_AssetDetails_GetAssetType(IntPtr obj) {
      var result = StringFromNative(ovr_AssetDetails_GetAssetType_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_AssetDetails_GetAssetType")]
    private static extern IntPtr ovr_AssetDetails_GetAssetType_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_AssetDetails_GetDownloadStatus(IntPtr obj) {
      var result = StringFromNative(ovr_AssetDetails_GetDownloadStatus_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_AssetDetails_GetDownloadStatus")]
    private static extern IntPtr ovr_AssetDetails_GetDownloadStatus_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_AssetDetails_GetFilepath(IntPtr obj) {
      var result = StringFromNative(ovr_AssetDetails_GetFilepath_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_AssetDetails_GetFilepath")]
    private static extern IntPtr ovr_AssetDetails_GetFilepath_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_AssetDetails_GetIapStatus(IntPtr obj) {
      var result = StringFromNative(ovr_AssetDetails_GetIapStatus_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_AssetDetails_GetIapStatus")]
    private static extern IntPtr ovr_AssetDetails_GetIapStatus_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_AssetDetails_GetLanguage(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_AssetDetails_GetMetadata(IntPtr obj) {
      var result = StringFromNative(ovr_AssetDetails_GetMetadata_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_AssetDetails_GetMetadata")]
    private static extern IntPtr ovr_AssetDetails_GetMetadata_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_AssetDetailsArray_GetElement(IntPtr obj, UIntPtr index);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UIntPtr ovr_AssetDetailsArray_GetSize(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UInt64 ovr_AssetFileDeleteResult_GetAssetFileId(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UInt64 ovr_AssetFileDeleteResult_GetAssetId(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_AssetFileDeleteResult_GetFilepath(IntPtr obj) {
      var result = StringFromNative(ovr_AssetFileDeleteResult_GetFilepath_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_AssetFileDeleteResult_GetFilepath")]
    private static extern IntPtr ovr_AssetFileDeleteResult_GetFilepath_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern bool ovr_AssetFileDeleteResult_GetSuccess(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UInt64 ovr_AssetFileDownloadCancelResult_GetAssetFileId(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UInt64 ovr_AssetFileDownloadCancelResult_GetAssetId(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_AssetFileDownloadCancelResult_GetFilepath(IntPtr obj) {
      var result = StringFromNative(ovr_AssetFileDownloadCancelResult_GetFilepath_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_AssetFileDownloadCancelResult_GetFilepath")]
    private static extern IntPtr ovr_AssetFileDownloadCancelResult_GetFilepath_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern bool ovr_AssetFileDownloadCancelResult_GetSuccess(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UInt64 ovr_AssetFileDownloadResult_GetAssetId(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_AssetFileDownloadResult_GetFilepath(IntPtr obj) {
      var result = StringFromNative(ovr_AssetFileDownloadResult_GetFilepath_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_AssetFileDownloadResult_GetFilepath")]
    private static extern IntPtr ovr_AssetFileDownloadResult_GetFilepath_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UInt64 ovr_AssetFileDownloadUpdate_GetAssetFileId(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UInt64 ovr_AssetFileDownloadUpdate_GetAssetId(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern uint ovr_AssetFileDownloadUpdate_GetBytesTotal(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_AssetFileDownloadUpdate_GetBytesTotalLong(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern int ovr_AssetFileDownloadUpdate_GetBytesTransferred(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern long ovr_AssetFileDownloadUpdate_GetBytesTransferredLong(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern bool ovr_AssetFileDownloadUpdate_GetCompleted(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern bool ovr_AvatarEditorResult_GetRequestSent(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_BillingPlan_GetPaidOffer(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_BillingPlan_GetTrialOffers(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_BillingPlanArray_GetElement(IntPtr obj, UIntPtr index);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UIntPtr ovr_BillingPlanArray_GetSize(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UInt64 ovr_BlockedUser_GetId(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_BlockedUserArray_GetElement(IntPtr obj, UIntPtr index);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_BlockedUserArray_GetNextUrl(IntPtr obj) {
      var result = StringFromNative(ovr_BlockedUserArray_GetNextUrl_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_BlockedUserArray_GetNextUrl")]
    private static extern IntPtr ovr_BlockedUserArray_GetNextUrl_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UIntPtr ovr_BlockedUserArray_GetSize(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern bool ovr_BlockedUserArray_HasNextPage(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ChallengeCreationType ovr_Challenge_GetCreationType(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_Challenge_GetDescription(IntPtr obj) {
      var result = StringFromNative(ovr_Challenge_GetDescription_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_Challenge_GetDescription")]
    private static extern IntPtr ovr_Challenge_GetDescription_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static DateTime ovr_Challenge_GetEndDate(IntPtr obj) {
      var result = DateTimeFromNative(ovr_Challenge_GetEndDate_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_Challenge_GetEndDate")]
    private static extern ulong ovr_Challenge_GetEndDate_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UInt64 ovr_Challenge_GetID(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Challenge_GetInvitedUsers(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Challenge_GetLeaderboard(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Challenge_GetParticipants(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static DateTime ovr_Challenge_GetStartDate(IntPtr obj) {
      var result = DateTimeFromNative(ovr_Challenge_GetStartDate_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_Challenge_GetStartDate")]
    private static extern ulong ovr_Challenge_GetStartDate_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_Challenge_GetTitle(IntPtr obj) {
      var result = StringFromNative(ovr_Challenge_GetTitle_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_Challenge_GetTitle")]
    private static extern IntPtr ovr_Challenge_GetTitle_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ChallengeVisibility ovr_Challenge_GetVisibility(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_ChallengeArray_GetElement(IntPtr obj, UIntPtr index);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_ChallengeArray_GetNextUrl(IntPtr obj) {
      var result = StringFromNative(ovr_ChallengeArray_GetNextUrl_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_ChallengeArray_GetNextUrl")]
    private static extern IntPtr ovr_ChallengeArray_GetNextUrl_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_ChallengeArray_GetPreviousUrl(IntPtr obj) {
      var result = StringFromNative(ovr_ChallengeArray_GetPreviousUrl_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_ChallengeArray_GetPreviousUrl")]
    private static extern IntPtr ovr_ChallengeArray_GetPreviousUrl_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UIntPtr ovr_ChallengeArray_GetSize(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_ChallengeArray_GetTotalCount(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern bool ovr_ChallengeArray_HasNextPage(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern bool ovr_ChallengeArray_HasPreviousPage(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_ChallengeEntry_GetDisplayScore(IntPtr obj) {
      var result = StringFromNative(ovr_ChallengeEntry_GetDisplayScore_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_ChallengeEntry_GetDisplayScore")]
    private static extern IntPtr ovr_ChallengeEntry_GetDisplayScore_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static byte[] ovr_ChallengeEntry_GetExtraData(IntPtr obj) {
      var result = BlobFromNative(ovr_LeaderboardEntry_GetExtraDataLength(obj), ovr_ChallengeEntry_GetExtraData_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_ChallengeEntry_GetExtraData")]
    private static extern IntPtr ovr_ChallengeEntry_GetExtraData_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern uint ovr_ChallengeEntry_GetExtraDataLength(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UInt64 ovr_ChallengeEntry_GetID(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern int ovr_ChallengeEntry_GetRank(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern long ovr_ChallengeEntry_GetScore(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static DateTime ovr_ChallengeEntry_GetTimestamp(IntPtr obj) {
      var result = DateTimeFromNative(ovr_ChallengeEntry_GetTimestamp_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_ChallengeEntry_GetTimestamp")]
    private static extern ulong ovr_ChallengeEntry_GetTimestamp_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_ChallengeEntry_GetUser(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_ChallengeEntryArray_GetElement(IntPtr obj, UIntPtr index);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_ChallengeEntryArray_GetNextUrl(IntPtr obj) {
      var result = StringFromNative(ovr_ChallengeEntryArray_GetNextUrl_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_ChallengeEntryArray_GetNextUrl")]
    private static extern IntPtr ovr_ChallengeEntryArray_GetNextUrl_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_ChallengeEntryArray_GetPreviousUrl(IntPtr obj) {
      var result = StringFromNative(ovr_ChallengeEntryArray_GetPreviousUrl_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_ChallengeEntryArray_GetPreviousUrl")]
    private static extern IntPtr ovr_ChallengeEntryArray_GetPreviousUrl_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UIntPtr ovr_ChallengeEntryArray_GetSize(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_ChallengeEntryArray_GetTotalCount(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern bool ovr_ChallengeEntryArray_HasNextPage(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern bool ovr_ChallengeEntryArray_HasPreviousPage(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_ContentRating_GetAgeRatingImageUri(IntPtr obj) {
      var result = StringFromNative(ovr_ContentRating_GetAgeRatingImageUri_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_ContentRating_GetAgeRatingImageUri")]
    private static extern IntPtr ovr_ContentRating_GetAgeRatingImageUri_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_ContentRating_GetAgeRatingText(IntPtr obj) {
      var result = StringFromNative(ovr_ContentRating_GetAgeRatingText_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_ContentRating_GetAgeRatingText")]
    private static extern IntPtr ovr_ContentRating_GetAgeRatingText_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_ContentRating_GetDescriptor(IntPtr obj, uint index) {
      var result = StringFromNative(ovr_ContentRating_GetDescriptor_Native(obj, index));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_ContentRating_GetDescriptor")]
    private static extern IntPtr ovr_ContentRating_GetDescriptor_Native(IntPtr obj, uint index);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern uint ovr_ContentRating_GetDescriptorsSize(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_ContentRating_GetInteractiveElement(IntPtr obj, uint index) {
      var result = StringFromNative(ovr_ContentRating_GetInteractiveElement_Native(obj, index));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_ContentRating_GetInteractiveElement")]
    private static extern IntPtr ovr_ContentRating_GetInteractiveElement_Native(IntPtr obj, uint index);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern uint ovr_ContentRating_GetInteractiveElementsSize(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_ContentRating_GetRatingDefinitionUri(IntPtr obj) {
      var result = StringFromNative(ovr_ContentRating_GetRatingDefinitionUri_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_ContentRating_GetRatingDefinitionUri")]
    private static extern IntPtr ovr_ContentRating_GetRatingDefinitionUri_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_CowatchViewer_GetData(IntPtr obj) {
      var result = StringFromNative(ovr_CowatchViewer_GetData_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_CowatchViewer_GetData")]
    private static extern IntPtr ovr_CowatchViewer_GetData_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UInt64 ovr_CowatchViewer_GetId(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_CowatchViewerArray_GetElement(IntPtr obj, UIntPtr index);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_CowatchViewerArray_GetNextUrl(IntPtr obj) {
      var result = StringFromNative(ovr_CowatchViewerArray_GetNextUrl_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_CowatchViewerArray_GetNextUrl")]
    private static extern IntPtr ovr_CowatchViewerArray_GetNextUrl_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UIntPtr ovr_CowatchViewerArray_GetSize(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern bool ovr_CowatchViewerArray_HasNextPage(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_CowatchViewerUpdate_GetDataList(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UInt64 ovr_CowatchViewerUpdate_GetId(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern bool ovr_CowatchingState_GetInSession(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static uint ovr_DataStore_Contains(IntPtr obj, string key) {
      IntPtr key_native = StringToNative(key);
      var result = (ovr_DataStore_Contains_Native(obj, key_native));
      Marshal.FreeCoTaskMem(key_native);
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_DataStore_Contains")]
    private static extern uint ovr_DataStore_Contains_Native(IntPtr obj, IntPtr key);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_DataStore_GetKey(IntPtr obj, int index) {
      var result = StringFromNative(ovr_DataStore_GetKey_Native(obj, index));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_DataStore_GetKey")]
    private static extern IntPtr ovr_DataStore_GetKey_Native(IntPtr obj, int index);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UIntPtr ovr_DataStore_GetNumKeys(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_DataStore_GetValue(IntPtr obj, string key) {
      IntPtr key_native = StringToNative(key);
      var result = StringFromNative(ovr_DataStore_GetValue_Native(obj, key_native));
      Marshal.FreeCoTaskMem(key_native);
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_DataStore_GetValue")]
    private static extern IntPtr ovr_DataStore_GetValue_Native(IntPtr obj, IntPtr key);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_Destination_GetApiName(IntPtr obj) {
      var result = StringFromNative(ovr_Destination_GetApiName_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_Destination_GetApiName")]
    private static extern IntPtr ovr_Destination_GetApiName_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_Destination_GetDeeplinkMessage(IntPtr obj) {
      var result = StringFromNative(ovr_Destination_GetDeeplinkMessage_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_Destination_GetDeeplinkMessage")]
    private static extern IntPtr ovr_Destination_GetDeeplinkMessage_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_Destination_GetDisplayName(IntPtr obj) {
      var result = StringFromNative(ovr_Destination_GetDisplayName_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_Destination_GetDisplayName")]
    private static extern IntPtr ovr_Destination_GetDisplayName_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_Destination_GetShareableUri(IntPtr obj) {
      var result = StringFromNative(ovr_Destination_GetShareableUri_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_Destination_GetShareableUri")]
    private static extern IntPtr ovr_Destination_GetShareableUri_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_DestinationArray_GetElement(IntPtr obj, UIntPtr index);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_DestinationArray_GetNextUrl(IntPtr obj) {
      var result = StringFromNative(ovr_DestinationArray_GetNextUrl_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_DestinationArray_GetNextUrl")]
    private static extern IntPtr ovr_DestinationArray_GetNextUrl_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UIntPtr ovr_DestinationArray_GetSize(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern bool ovr_DestinationArray_HasNextPage(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern int ovr_Error_GetCode(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_Error_GetDisplayableMessage(IntPtr obj) {
      var result = StringFromNative(ovr_Error_GetDisplayableMessage_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_Error_GetDisplayableMessage")]
    private static extern IntPtr ovr_Error_GetDisplayableMessage_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern int ovr_Error_GetHttpCode(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_Error_GetMessage(IntPtr obj) {
      var result = StringFromNative(ovr_Error_GetMessage_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_Error_GetMessage")]
    private static extern IntPtr ovr_Error_GetMessage_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_GroupPresenceJoinIntent_GetDeeplinkMessage(IntPtr obj) {
      var result = StringFromNative(ovr_GroupPresenceJoinIntent_GetDeeplinkMessage_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_GroupPresenceJoinIntent_GetDeeplinkMessage")]
    private static extern IntPtr ovr_GroupPresenceJoinIntent_GetDeeplinkMessage_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_GroupPresenceJoinIntent_GetDestinationApiName(IntPtr obj) {
      var result = StringFromNative(ovr_GroupPresenceJoinIntent_GetDestinationApiName_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_GroupPresenceJoinIntent_GetDestinationApiName")]
    private static extern IntPtr ovr_GroupPresenceJoinIntent_GetDestinationApiName_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_GroupPresenceJoinIntent_GetLobbySessionId(IntPtr obj) {
      var result = StringFromNative(ovr_GroupPresenceJoinIntent_GetLobbySessionId_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_GroupPresenceJoinIntent_GetLobbySessionId")]
    private static extern IntPtr ovr_GroupPresenceJoinIntent_GetLobbySessionId_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_GroupPresenceJoinIntent_GetMatchSessionId(IntPtr obj) {
      var result = StringFromNative(ovr_GroupPresenceJoinIntent_GetMatchSessionId_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_GroupPresenceJoinIntent_GetMatchSessionId")]
    private static extern IntPtr ovr_GroupPresenceJoinIntent_GetMatchSessionId_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_GroupPresenceLeaveIntent_GetDestinationApiName(IntPtr obj) {
      var result = StringFromNative(ovr_GroupPresenceLeaveIntent_GetDestinationApiName_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_GroupPresenceLeaveIntent_GetDestinationApiName")]
    private static extern IntPtr ovr_GroupPresenceLeaveIntent_GetDestinationApiName_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_GroupPresenceLeaveIntent_GetLobbySessionId(IntPtr obj) {
      var result = StringFromNative(ovr_GroupPresenceLeaveIntent_GetLobbySessionId_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_GroupPresenceLeaveIntent_GetLobbySessionId")]
    private static extern IntPtr ovr_GroupPresenceLeaveIntent_GetLobbySessionId_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_GroupPresenceLeaveIntent_GetMatchSessionId(IntPtr obj) {
      var result = StringFromNative(ovr_GroupPresenceLeaveIntent_GetMatchSessionId_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_GroupPresenceLeaveIntent_GetMatchSessionId")]
    private static extern IntPtr ovr_GroupPresenceLeaveIntent_GetMatchSessionId_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_HttpTransferUpdate_GetBytes(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_HttpTransferUpdate_GetID(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UIntPtr ovr_HttpTransferUpdate_GetSize(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern bool ovr_HttpTransferUpdate_IsCompleted(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_InstalledApplication_GetApplicationId(IntPtr obj) {
      var result = StringFromNative(ovr_InstalledApplication_GetApplicationId_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_InstalledApplication_GetApplicationId")]
    private static extern IntPtr ovr_InstalledApplication_GetApplicationId_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_InstalledApplication_GetPackageName(IntPtr obj) {
      var result = StringFromNative(ovr_InstalledApplication_GetPackageName_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_InstalledApplication_GetPackageName")]
    private static extern IntPtr ovr_InstalledApplication_GetPackageName_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_InstalledApplication_GetStatus(IntPtr obj) {
      var result = StringFromNative(ovr_InstalledApplication_GetStatus_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_InstalledApplication_GetStatus")]
    private static extern IntPtr ovr_InstalledApplication_GetStatus_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern int ovr_InstalledApplication_GetVersionCode(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_InstalledApplication_GetVersionName(IntPtr obj) {
      var result = StringFromNative(ovr_InstalledApplication_GetVersionName_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_InstalledApplication_GetVersionName")]
    private static extern IntPtr ovr_InstalledApplication_GetVersionName_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_InstalledApplicationArray_GetElement(IntPtr obj, UIntPtr index);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UIntPtr ovr_InstalledApplicationArray_GetSize(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern bool ovr_InvitePanelResultInfo_GetInvitesSent(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_LanguagePackInfo_GetEnglishName(IntPtr obj) {
      var result = StringFromNative(ovr_LanguagePackInfo_GetEnglishName_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_LanguagePackInfo_GetEnglishName")]
    private static extern IntPtr ovr_LanguagePackInfo_GetEnglishName_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_LanguagePackInfo_GetNativeName(IntPtr obj) {
      var result = StringFromNative(ovr_LanguagePackInfo_GetNativeName_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_LanguagePackInfo_GetNativeName")]
    private static extern IntPtr ovr_LanguagePackInfo_GetNativeName_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_LanguagePackInfo_GetTag(IntPtr obj) {
      var result = StringFromNative(ovr_LanguagePackInfo_GetTag_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_LanguagePackInfo_GetTag")]
    private static extern IntPtr ovr_LanguagePackInfo_GetTag_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern bool ovr_LaunchBlockFlowResult_GetDidBlock(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern bool ovr_LaunchBlockFlowResult_GetDidCancel(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_LaunchDetails_GetDeeplinkMessage(IntPtr obj) {
      var result = StringFromNative(ovr_LaunchDetails_GetDeeplinkMessage_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_LaunchDetails_GetDeeplinkMessage")]
    private static extern IntPtr ovr_LaunchDetails_GetDeeplinkMessage_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_LaunchDetails_GetDestinationApiName(IntPtr obj) {
      var result = StringFromNative(ovr_LaunchDetails_GetDestinationApiName_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_LaunchDetails_GetDestinationApiName")]
    private static extern IntPtr ovr_LaunchDetails_GetDestinationApiName_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_LaunchDetails_GetLaunchSource(IntPtr obj) {
      var result = StringFromNative(ovr_LaunchDetails_GetLaunchSource_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_LaunchDetails_GetLaunchSource")]
    private static extern IntPtr ovr_LaunchDetails_GetLaunchSource_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern LaunchType ovr_LaunchDetails_GetLaunchType(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_LaunchDetails_GetLobbySessionID(IntPtr obj) {
      var result = StringFromNative(ovr_LaunchDetails_GetLobbySessionID_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_LaunchDetails_GetLobbySessionID")]
    private static extern IntPtr ovr_LaunchDetails_GetLobbySessionID_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_LaunchDetails_GetMatchSessionID(IntPtr obj) {
      var result = StringFromNative(ovr_LaunchDetails_GetMatchSessionID_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_LaunchDetails_GetMatchSessionID")]
    private static extern IntPtr ovr_LaunchDetails_GetMatchSessionID_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_LaunchDetails_GetTrackingID(IntPtr obj) {
      var result = StringFromNative(ovr_LaunchDetails_GetTrackingID_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_LaunchDetails_GetTrackingID")]
    private static extern IntPtr ovr_LaunchDetails_GetTrackingID_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_LaunchDetails_GetUsers(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern bool ovr_LaunchFriendRequestFlowResult_GetDidCancel(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern bool ovr_LaunchFriendRequestFlowResult_GetDidSendRequest(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_LaunchInvitePanelFlowResult_GetInvitedUsers(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern bool ovr_LaunchReportFlowResult_GetDidCancel(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UInt64 ovr_LaunchReportFlowResult_GetUserReportId(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern bool ovr_LaunchUnblockFlowResult_GetDidCancel(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern bool ovr_LaunchUnblockFlowResult_GetDidUnblock(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_Leaderboard_GetApiName(IntPtr obj) {
      var result = StringFromNative(ovr_Leaderboard_GetApiName_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_Leaderboard_GetApiName")]
    private static extern IntPtr ovr_Leaderboard_GetApiName_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Leaderboard_GetDestination(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UInt64 ovr_Leaderboard_GetID(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_LeaderboardArray_GetElement(IntPtr obj, UIntPtr index);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_LeaderboardArray_GetNextUrl(IntPtr obj) {
      var result = StringFromNative(ovr_LeaderboardArray_GetNextUrl_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_LeaderboardArray_GetNextUrl")]
    private static extern IntPtr ovr_LeaderboardArray_GetNextUrl_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UIntPtr ovr_LeaderboardArray_GetSize(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern bool ovr_LeaderboardArray_HasNextPage(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_LeaderboardEntry_GetDisplayScore(IntPtr obj) {
      var result = StringFromNative(ovr_LeaderboardEntry_GetDisplayScore_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_LeaderboardEntry_GetDisplayScore")]
    private static extern IntPtr ovr_LeaderboardEntry_GetDisplayScore_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static byte[] ovr_LeaderboardEntry_GetExtraData(IntPtr obj) {
      var result = BlobFromNative(ovr_LeaderboardEntry_GetExtraDataLength(obj), ovr_LeaderboardEntry_GetExtraData_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_LeaderboardEntry_GetExtraData")]
    private static extern IntPtr ovr_LeaderboardEntry_GetExtraData_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern uint ovr_LeaderboardEntry_GetExtraDataLength(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UInt64 ovr_LeaderboardEntry_GetID(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern int ovr_LeaderboardEntry_GetRank(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern long ovr_LeaderboardEntry_GetScore(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_LeaderboardEntry_GetSupplementaryMetric(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static DateTime ovr_LeaderboardEntry_GetTimestamp(IntPtr obj) {
      var result = DateTimeFromNative(ovr_LeaderboardEntry_GetTimestamp_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_LeaderboardEntry_GetTimestamp")]
    private static extern ulong ovr_LeaderboardEntry_GetTimestamp_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_LeaderboardEntry_GetUser(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_LeaderboardEntryArray_GetElement(IntPtr obj, UIntPtr index);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_LeaderboardEntryArray_GetNextUrl(IntPtr obj) {
      var result = StringFromNative(ovr_LeaderboardEntryArray_GetNextUrl_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_LeaderboardEntryArray_GetNextUrl")]
    private static extern IntPtr ovr_LeaderboardEntryArray_GetNextUrl_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_LeaderboardEntryArray_GetPreviousUrl(IntPtr obj) {
      var result = StringFromNative(ovr_LeaderboardEntryArray_GetPreviousUrl_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_LeaderboardEntryArray_GetPreviousUrl")]
    private static extern IntPtr ovr_LeaderboardEntryArray_GetPreviousUrl_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UIntPtr ovr_LeaderboardEntryArray_GetSize(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_LeaderboardEntryArray_GetTotalCount(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern bool ovr_LeaderboardEntryArray_HasNextPage(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern bool ovr_LeaderboardEntryArray_HasPreviousPage(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern bool ovr_LeaderboardUpdateStatus_GetDidUpdate(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UInt64 ovr_LeaderboardUpdateStatus_GetUpdatedChallengeId(IntPtr obj, uint index);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern uint ovr_LeaderboardUpdateStatus_GetUpdatedChallengeIdsSize(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_LinkedAccount_GetAccessToken(IntPtr obj) {
      var result = StringFromNative(ovr_LinkedAccount_GetAccessToken_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_LinkedAccount_GetAccessToken")]
    private static extern IntPtr ovr_LinkedAccount_GetAccessToken_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ServiceProvider ovr_LinkedAccount_GetServiceProvider(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_LinkedAccount_GetUserId(IntPtr obj) {
      var result = StringFromNative(ovr_LinkedAccount_GetUserId_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_LinkedAccount_GetUserId")]
    private static extern IntPtr ovr_LinkedAccount_GetUserId_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_LinkedAccountArray_GetElement(IntPtr obj, UIntPtr index);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UIntPtr ovr_LinkedAccountArray_GetSize(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern bool ovr_LivestreamingApplicationStatus_GetStreamingEnabled(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern LivestreamingStartStatus ovr_LivestreamingStartResult_GetStreamingResult(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern bool ovr_LivestreamingStatus_GetCommentsVisible(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern bool ovr_LivestreamingStatus_GetIsPaused(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern bool ovr_LivestreamingStatus_GetLivestreamingEnabled(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern int ovr_LivestreamingStatus_GetLivestreamingType(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern bool ovr_LivestreamingStatus_GetMicEnabled(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern int ovr_LivestreamingVideoStats_GetCommentCount(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern int ovr_LivestreamingVideoStats_GetReactionCount(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_LivestreamingVideoStats_GetTotalViews(IntPtr obj) {
      var result = StringFromNative(ovr_LivestreamingVideoStats_GetTotalViews_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_LivestreamingVideoStats_GetTotalViews")]
    private static extern IntPtr ovr_LivestreamingVideoStats_GetTotalViews_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_ManagedInfo_GetDepartment(IntPtr obj) {
      var result = StringFromNative(ovr_ManagedInfo_GetDepartment_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_ManagedInfo_GetDepartment")]
    private static extern IntPtr ovr_ManagedInfo_GetDepartment_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_ManagedInfo_GetEmail(IntPtr obj) {
      var result = StringFromNative(ovr_ManagedInfo_GetEmail_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_ManagedInfo_GetEmail")]
    private static extern IntPtr ovr_ManagedInfo_GetEmail_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_ManagedInfo_GetEmployeeNumber(IntPtr obj) {
      var result = StringFromNative(ovr_ManagedInfo_GetEmployeeNumber_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_ManagedInfo_GetEmployeeNumber")]
    private static extern IntPtr ovr_ManagedInfo_GetEmployeeNumber_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_ManagedInfo_GetExternalId(IntPtr obj) {
      var result = StringFromNative(ovr_ManagedInfo_GetExternalId_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_ManagedInfo_GetExternalId")]
    private static extern IntPtr ovr_ManagedInfo_GetExternalId_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_ManagedInfo_GetLocation(IntPtr obj) {
      var result = StringFromNative(ovr_ManagedInfo_GetLocation_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_ManagedInfo_GetLocation")]
    private static extern IntPtr ovr_ManagedInfo_GetLocation_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_ManagedInfo_GetManager(IntPtr obj) {
      var result = StringFromNative(ovr_ManagedInfo_GetManager_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_ManagedInfo_GetManager")]
    private static extern IntPtr ovr_ManagedInfo_GetManager_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_ManagedInfo_GetName(IntPtr obj) {
      var result = StringFromNative(ovr_ManagedInfo_GetName_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_ManagedInfo_GetName")]
    private static extern IntPtr ovr_ManagedInfo_GetName_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_ManagedInfo_GetOrganizationId(IntPtr obj) {
      var result = StringFromNative(ovr_ManagedInfo_GetOrganizationId_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_ManagedInfo_GetOrganizationId")]
    private static extern IntPtr ovr_ManagedInfo_GetOrganizationId_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_ManagedInfo_GetOrganizationName(IntPtr obj) {
      var result = StringFromNative(ovr_ManagedInfo_GetOrganizationName_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_ManagedInfo_GetOrganizationName")]
    private static extern IntPtr ovr_ManagedInfo_GetOrganizationName_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_ManagedInfo_GetPosition(IntPtr obj) {
      var result = StringFromNative(ovr_ManagedInfo_GetPosition_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_ManagedInfo_GetPosition")]
    private static extern IntPtr ovr_ManagedInfo_GetPosition_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetAbuseReportRecording(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetAchievementDefinitionArray(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetAchievementProgressArray(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetAchievementUpdate(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetAppDownloadProgressResult(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetAppDownloadResult(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetApplicationInviteArray(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetApplicationVersion(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetAssetDetails(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetAssetDetailsArray(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetAssetFileDeleteResult(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetAssetFileDownloadCancelResult(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetAssetFileDownloadResult(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetAssetFileDownloadUpdate(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetAvatarEditorResult(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetBlockedUserArray(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetChallenge(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetChallengeArray(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetChallengeEntryArray(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetCowatchViewerArray(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetCowatchViewerUpdate(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetCowatchingState(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetDataStore(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetDestinationArray(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetError(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetGroupPresenceJoinIntent(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetGroupPresenceLeaveIntent(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetHttpTransferUpdate(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetInstalledApplicationArray(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetInvitePanelResultInfo(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetLaunchBlockFlowResult(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetLaunchFriendRequestFlowResult(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetLaunchInvitePanelFlowResult(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetLaunchReportFlowResult(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetLaunchUnblockFlowResult(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetLeaderboardArray(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetLeaderboardEntryArray(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetLeaderboardUpdateStatus(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetLinkedAccountArray(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetLivestreamingApplicationStatus(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetLivestreamingStartResult(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetLivestreamingStatus(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetLivestreamingVideoStats(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetMicrophoneAvailabilityState(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetNativeMessage(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetNetSyncConnection(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetNetSyncSessionArray(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetNetSyncSessionsChangedNotification(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetNetSyncSetSessionPropertyResult(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetNetSyncVoipAttenuationValueArray(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetOrgScopedID(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetParty(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetPartyID(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetPartyUpdateNotification(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetPidArray(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetPlatformInitialize(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetProductArray(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetPurchase(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetPurchaseArray(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetPushNotificationResult(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetRejoinDialogResult(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ulong ovr_Message_GetRequestID(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetSdkAccountArray(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetSendInvitesResult(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetShareMediaResult(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_Message_GetString(IntPtr obj) {
      var result = StringFromNative(ovr_Message_GetString_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_Message_GetString")]
    private static extern IntPtr ovr_Message_GetString_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetSystemVoipState(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern Message.MessageType ovr_Message_GetType(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetUser(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetUserAccountAgeCategory(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetUserArray(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetUserCapabilityArray(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetUserDataStoreUpdateResponse(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetUserProof(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Message_GetUserReportID(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern bool ovr_Message_IsError(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UIntPtr ovr_Microphone_GetNumSamplesAvailable(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UIntPtr ovr_Microphone_GetOutputBufferMaxSize(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UIntPtr ovr_Microphone_GetPCM(IntPtr obj, Int16[] outputBuffer, UIntPtr outputBufferNumElements);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UIntPtr ovr_Microphone_GetPCMFloat(IntPtr obj, float[] outputBuffer, UIntPtr outputBufferNumElements);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UIntPtr ovr_Microphone_ReadData(IntPtr obj, float[] outputBuffer, UIntPtr outputBufferSize);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_Microphone_SetAcceptableRecordingDelayHint(IntPtr obj, UIntPtr delayMs);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_Microphone_Start(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_Microphone_Stop(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern bool ovr_MicrophoneAvailabilityState_GetMicrophoneAvailable(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern long ovr_NetSyncConnection_GetConnectionId(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern NetSyncDisconnectReason ovr_NetSyncConnection_GetDisconnectReason(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UInt64 ovr_NetSyncConnection_GetSessionId(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern NetSyncConnectionStatus ovr_NetSyncConnection_GetStatus(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_NetSyncConnection_GetZoneId(IntPtr obj) {
      var result = StringFromNative(ovr_NetSyncConnection_GetZoneId_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_NetSyncConnection_GetZoneId")]
    private static extern IntPtr ovr_NetSyncConnection_GetZoneId_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern long ovr_NetSyncSession_GetConnectionId(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern bool ovr_NetSyncSession_GetMuted(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UInt64 ovr_NetSyncSession_GetSessionId(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UInt64 ovr_NetSyncSession_GetUserId(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_NetSyncSession_GetVoipGroup(IntPtr obj) {
      var result = StringFromNative(ovr_NetSyncSession_GetVoipGroup_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_NetSyncSession_GetVoipGroup")]
    private static extern IntPtr ovr_NetSyncSession_GetVoipGroup_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_NetSyncSessionArray_GetElement(IntPtr obj, UIntPtr index);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UIntPtr ovr_NetSyncSessionArray_GetSize(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern long ovr_NetSyncSessionsChangedNotification_GetConnectionId(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_NetSyncSessionsChangedNotification_GetSessions(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_NetSyncSetSessionPropertyResult_GetSession(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern float ovr_NetSyncVoipAttenuationValue_GetDecibels(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern float ovr_NetSyncVoipAttenuationValue_GetDistance(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_NetSyncVoipAttenuationValueArray_GetElement(IntPtr obj, UIntPtr index);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UIntPtr ovr_NetSyncVoipAttenuationValueArray_GetSize(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UInt64 ovr_OrgScopedID_GetID(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_Packet_Free(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Packet_GetBytes(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UInt64 ovr_Packet_GetSenderID(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UIntPtr ovr_Packet_GetSize(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_PaidOffer_GetPrice(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern OfferTerm ovr_PaidOffer_GetSubscriptionTerm(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UInt64 ovr_Party_GetID(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Party_GetInvitedUsers(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Party_GetLeader(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Party_GetUsers(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UInt64 ovr_PartyID_GetID(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern PartyUpdateAction ovr_PartyUpdateNotification_GetAction(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UInt64 ovr_PartyUpdateNotification_GetPartyId(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UInt64 ovr_PartyUpdateNotification_GetSenderId(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_PartyUpdateNotification_GetUpdateTimestamp(IntPtr obj) {
      var result = StringFromNative(ovr_PartyUpdateNotification_GetUpdateTimestamp_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_PartyUpdateNotification_GetUpdateTimestamp")]
    private static extern IntPtr ovr_PartyUpdateNotification_GetUpdateTimestamp_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_PartyUpdateNotification_GetUserAlias(IntPtr obj) {
      var result = StringFromNative(ovr_PartyUpdateNotification_GetUserAlias_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_PartyUpdateNotification_GetUserAlias")]
    private static extern IntPtr ovr_PartyUpdateNotification_GetUserAlias_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UInt64 ovr_PartyUpdateNotification_GetUserId(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_PartyUpdateNotification_GetUserName(IntPtr obj) {
      var result = StringFromNative(ovr_PartyUpdateNotification_GetUserName_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_PartyUpdateNotification_GetUserName")]
    private static extern IntPtr ovr_PartyUpdateNotification_GetUserName_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_Pid_GetId(IntPtr obj) {
      var result = StringFromNative(ovr_Pid_GetId_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_Pid_GetId")]
    private static extern IntPtr ovr_Pid_GetId_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_PidArray_GetElement(IntPtr obj, UIntPtr index);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UIntPtr ovr_PidArray_GetSize(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern PlatformInitializeResult ovr_PlatformInitialize_GetResult(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern uint ovr_Price_GetAmountInHundredths(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_Price_GetCurrency(IntPtr obj) {
      var result = StringFromNative(ovr_Price_GetCurrency_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_Price_GetCurrency")]
    private static extern IntPtr ovr_Price_GetCurrency_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_Price_GetFormatted(IntPtr obj) {
      var result = StringFromNative(ovr_Price_GetFormatted_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_Price_GetFormatted")]
    private static extern IntPtr ovr_Price_GetFormatted_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Product_GetBillingPlans(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Product_GetContentRating(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_Product_GetCoverUrl(IntPtr obj) {
      var result = StringFromNative(ovr_Product_GetCoverUrl_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_Product_GetCoverUrl")]
    private static extern IntPtr ovr_Product_GetCoverUrl_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_Product_GetDescription(IntPtr obj) {
      var result = StringFromNative(ovr_Product_GetDescription_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_Product_GetDescription")]
    private static extern IntPtr ovr_Product_GetDescription_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_Product_GetFormattedPrice(IntPtr obj) {
      var result = StringFromNative(ovr_Product_GetFormattedPrice_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_Product_GetFormattedPrice")]
    private static extern IntPtr ovr_Product_GetFormattedPrice_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_Product_GetIconUrl(IntPtr obj) {
      var result = StringFromNative(ovr_Product_GetIconUrl_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_Product_GetIconUrl")]
    private static extern IntPtr ovr_Product_GetIconUrl_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_Product_GetName(IntPtr obj) {
      var result = StringFromNative(ovr_Product_GetName_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_Product_GetName")]
    private static extern IntPtr ovr_Product_GetName_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_Product_GetPrice(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_Product_GetShortDescription(IntPtr obj) {
      var result = StringFromNative(ovr_Product_GetShortDescription_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_Product_GetShortDescription")]
    private static extern IntPtr ovr_Product_GetShortDescription_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_Product_GetSKU(IntPtr obj) {
      var result = StringFromNative(ovr_Product_GetSKU_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_Product_GetSKU")]
    private static extern IntPtr ovr_Product_GetSKU_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ProductType ovr_Product_GetType(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_ProductArray_GetElement(IntPtr obj, UIntPtr index);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_ProductArray_GetNextUrl(IntPtr obj) {
      var result = StringFromNative(ovr_ProductArray_GetNextUrl_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_ProductArray_GetNextUrl")]
    private static extern IntPtr ovr_ProductArray_GetNextUrl_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UIntPtr ovr_ProductArray_GetSize(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern bool ovr_ProductArray_HasNextPage(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_Purchase_GetDeveloperPayload(IntPtr obj) {
      var result = StringFromNative(ovr_Purchase_GetDeveloperPayload_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_Purchase_GetDeveloperPayload")]
    private static extern IntPtr ovr_Purchase_GetDeveloperPayload_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static DateTime ovr_Purchase_GetExpirationTime(IntPtr obj) {
      var result = DateTimeFromNative(ovr_Purchase_GetExpirationTime_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_Purchase_GetExpirationTime")]
    private static extern ulong ovr_Purchase_GetExpirationTime_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static DateTime ovr_Purchase_GetGrantTime(IntPtr obj) {
      var result = DateTimeFromNative(ovr_Purchase_GetGrantTime_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_Purchase_GetGrantTime")]
    private static extern ulong ovr_Purchase_GetGrantTime_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UInt64 ovr_Purchase_GetPurchaseID(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_Purchase_GetPurchaseStrID(IntPtr obj) {
      var result = StringFromNative(ovr_Purchase_GetPurchaseStrID_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_Purchase_GetPurchaseStrID")]
    private static extern IntPtr ovr_Purchase_GetPurchaseStrID_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_Purchase_GetReportingId(IntPtr obj) {
      var result = StringFromNative(ovr_Purchase_GetReportingId_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_Purchase_GetReportingId")]
    private static extern IntPtr ovr_Purchase_GetReportingId_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_Purchase_GetSKU(IntPtr obj) {
      var result = StringFromNative(ovr_Purchase_GetSKU_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_Purchase_GetSKU")]
    private static extern IntPtr ovr_Purchase_GetSKU_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ProductType ovr_Purchase_GetType(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_PurchaseArray_GetElement(IntPtr obj, UIntPtr index);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_PurchaseArray_GetNextUrl(IntPtr obj) {
      var result = StringFromNative(ovr_PurchaseArray_GetNextUrl_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_PurchaseArray_GetNextUrl")]
    private static extern IntPtr ovr_PurchaseArray_GetNextUrl_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UIntPtr ovr_PurchaseArray_GetSize(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern bool ovr_PurchaseArray_HasNextPage(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_PushNotificationResult_GetId(IntPtr obj) {
      var result = StringFromNative(ovr_PushNotificationResult_GetId_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_PushNotificationResult_GetId")]
    private static extern IntPtr ovr_PushNotificationResult_GetId_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern bool ovr_RejoinDialogResult_GetRejoinSelected(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern SdkAccountType ovr_SdkAccount_GetAccountType(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UInt64 ovr_SdkAccount_GetUserId(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_SdkAccountArray_GetElement(IntPtr obj, UIntPtr index);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UIntPtr ovr_SdkAccountArray_GetSize(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_SendInvitesResult_GetInvites(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern ShareMediaStatus ovr_ShareMediaResult_GetStatus(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UInt64 ovr_SupplementaryMetric_GetID(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern long ovr_SupplementaryMetric_GetMetric(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern VoipMuteState ovr_SystemVoipState_GetMicrophoneMuted(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern SystemVoipStatus ovr_SystemVoipState_GetStatus(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_TestUser_GetAccessToken(IntPtr obj) {
      var result = StringFromNative(ovr_TestUser_GetAccessToken_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_TestUser_GetAccessToken")]
    private static extern IntPtr ovr_TestUser_GetAccessToken_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_TestUser_GetAppAccessArray(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_TestUser_GetFbAppAccessArray(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_TestUser_GetFriendAccessToken(IntPtr obj) {
      var result = StringFromNative(ovr_TestUser_GetFriendAccessToken_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_TestUser_GetFriendAccessToken")]
    private static extern IntPtr ovr_TestUser_GetFriendAccessToken_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_TestUser_GetFriendAppAccessArray(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_TestUser_GetUserAlias(IntPtr obj) {
      var result = StringFromNative(ovr_TestUser_GetUserAlias_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_TestUser_GetUserAlias")]
    private static extern IntPtr ovr_TestUser_GetUserAlias_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UInt64 ovr_TestUser_GetUserFbid(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UInt64 ovr_TestUser_GetUserId(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_TestUserAppAccess_GetAccessToken(IntPtr obj) {
      var result = StringFromNative(ovr_TestUserAppAccess_GetAccessToken_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_TestUserAppAccess_GetAccessToken")]
    private static extern IntPtr ovr_TestUserAppAccess_GetAccessToken_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UInt64 ovr_TestUserAppAccess_GetAppId(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UInt64 ovr_TestUserAppAccess_GetUserId(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_TestUserAppAccessArray_GetElement(IntPtr obj, UIntPtr index);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UIntPtr ovr_TestUserAppAccessArray_GetSize(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern int ovr_TrialOffer_GetMaxTermCount(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_TrialOffer_GetPrice(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern OfferTerm ovr_TrialOffer_GetTrialTerm(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern OfferType ovr_TrialOffer_GetTrialType(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_TrialOfferArray_GetElement(IntPtr obj, UIntPtr index);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UIntPtr ovr_TrialOfferArray_GetSize(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_User_GetDisplayName(IntPtr obj) {
      var result = StringFromNative(ovr_User_GetDisplayName_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_User_GetDisplayName")]
    private static extern IntPtr ovr_User_GetDisplayName_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UInt64 ovr_User_GetID(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_User_GetImageUrl(IntPtr obj) {
      var result = StringFromNative(ovr_User_GetImageUrl_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_User_GetImageUrl")]
    private static extern IntPtr ovr_User_GetImageUrl_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_User_GetManagedInfo(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_User_GetOculusID(IntPtr obj) {
      var result = StringFromNative(ovr_User_GetOculusID_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_User_GetOculusID")]
    private static extern IntPtr ovr_User_GetOculusID_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_User_GetPresence(IntPtr obj) {
      var result = StringFromNative(ovr_User_GetPresence_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_User_GetPresence")]
    private static extern IntPtr ovr_User_GetPresence_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_User_GetPresenceDeeplinkMessage(IntPtr obj) {
      var result = StringFromNative(ovr_User_GetPresenceDeeplinkMessage_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_User_GetPresenceDeeplinkMessage")]
    private static extern IntPtr ovr_User_GetPresenceDeeplinkMessage_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_User_GetPresenceDestinationApiName(IntPtr obj) {
      var result = StringFromNative(ovr_User_GetPresenceDestinationApiName_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_User_GetPresenceDestinationApiName")]
    private static extern IntPtr ovr_User_GetPresenceDestinationApiName_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_User_GetPresenceLobbySessionId(IntPtr obj) {
      var result = StringFromNative(ovr_User_GetPresenceLobbySessionId_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_User_GetPresenceLobbySessionId")]
    private static extern IntPtr ovr_User_GetPresenceLobbySessionId_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_User_GetPresenceMatchSessionId(IntPtr obj) {
      var result = StringFromNative(ovr_User_GetPresenceMatchSessionId_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_User_GetPresenceMatchSessionId")]
    private static extern IntPtr ovr_User_GetPresenceMatchSessionId_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UserPresenceStatus ovr_User_GetPresenceStatus(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_User_GetSmallImageUrl(IntPtr obj) {
      var result = StringFromNative(ovr_User_GetSmallImageUrl_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_User_GetSmallImageUrl")]
    private static extern IntPtr ovr_User_GetSmallImageUrl_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern AccountAgeCategory ovr_UserAccountAgeCategory_GetAgeCategory(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_UserArray_GetElement(IntPtr obj, UIntPtr index);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_UserArray_GetNextUrl(IntPtr obj) {
      var result = StringFromNative(ovr_UserArray_GetNextUrl_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_UserArray_GetNextUrl")]
    private static extern IntPtr ovr_UserArray_GetNextUrl_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UIntPtr ovr_UserArray_GetSize(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern bool ovr_UserArray_HasNextPage(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_UserCapability_GetDescription(IntPtr obj) {
      var result = StringFromNative(ovr_UserCapability_GetDescription_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_UserCapability_GetDescription")]
    private static extern IntPtr ovr_UserCapability_GetDescription_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern bool ovr_UserCapability_GetIsEnabled(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_UserCapability_GetName(IntPtr obj) {
      var result = StringFromNative(ovr_UserCapability_GetName_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_UserCapability_GetName")]
    private static extern IntPtr ovr_UserCapability_GetName_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_UserCapability_GetReasonCode(IntPtr obj) {
      var result = StringFromNative(ovr_UserCapability_GetReasonCode_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_UserCapability_GetReasonCode")]
    private static extern IntPtr ovr_UserCapability_GetReasonCode_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_UserCapabilityArray_GetElement(IntPtr obj, UIntPtr index);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_UserCapabilityArray_GetNextUrl(IntPtr obj) {
      var result = StringFromNative(ovr_UserCapabilityArray_GetNextUrl_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_UserCapabilityArray_GetNextUrl")]
    private static extern IntPtr ovr_UserCapabilityArray_GetNextUrl_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UIntPtr ovr_UserCapabilityArray_GetSize(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern bool ovr_UserCapabilityArray_HasNextPage(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern bool ovr_UserDataStoreUpdateResponse_GetSuccess(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static string ovr_UserProof_GetNonce(IntPtr obj) {
      var result = StringFromNative(ovr_UserProof_GetNonce_Native(obj));
      return result;
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_UserProof_GetNonce")]
    private static extern IntPtr ovr_UserProof_GetNonce_Native(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern bool ovr_UserReportID_GetDidCancel(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UInt64 ovr_UserReportID_GetID(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_VoipDecoder_Decode(IntPtr obj, byte[] compressedData, UIntPtr compressedSize);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UIntPtr ovr_VoipDecoder_GetDecodedPCM(IntPtr obj, float[] outputBuffer, UIntPtr outputBufferSize);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_VoipEncoder_AddPCM(IntPtr obj, float[] inputData, uint inputSize);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UIntPtr ovr_VoipEncoder_GetCompressedData(IntPtr obj, byte[] outputBuffer, UIntPtr intputSize);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern UIntPtr ovr_VoipEncoder_GetCompressedDataSize(IntPtr obj);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_AbuseReportOptions_Create();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_AbuseReportOptions_Destroy(IntPtr handle);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_AbuseReportOptions_SetPreventPeopleChooser(IntPtr handle, bool value);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_AbuseReportOptions_SetReportType(IntPtr handle, AbuseReportType value);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_AdvancedAbuseReportOptions_Create();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_AdvancedAbuseReportOptions_Destroy(IntPtr handle);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static void ovr_AdvancedAbuseReportOptions_SetDeveloperDefinedContextString(IntPtr handle, string key, string value) {
      IntPtr key_native = StringToNative(key);
      IntPtr value_native = StringToNative(value);
      ovr_AdvancedAbuseReportOptions_SetDeveloperDefinedContextString_Native(handle, key_native, value_native);
      Marshal.FreeCoTaskMem(key_native);
      Marshal.FreeCoTaskMem(value_native);
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_AdvancedAbuseReportOptions_SetDeveloperDefinedContextString")]
    private static extern void ovr_AdvancedAbuseReportOptions_SetDeveloperDefinedContextString_Native(IntPtr handle, IntPtr key, IntPtr value);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_AdvancedAbuseReportOptions_ClearDeveloperDefinedContext(IntPtr handle);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static void ovr_AdvancedAbuseReportOptions_SetObjectType(IntPtr handle, string value) {
      IntPtr value_native = StringToNative(value);
      ovr_AdvancedAbuseReportOptions_SetObjectType_Native(handle, value_native);
      Marshal.FreeCoTaskMem(value_native);
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_AdvancedAbuseReportOptions_SetObjectType")]
    private static extern void ovr_AdvancedAbuseReportOptions_SetObjectType_Native(IntPtr handle, IntPtr value);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_AdvancedAbuseReportOptions_SetReportType(IntPtr handle, AbuseReportType value);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_AdvancedAbuseReportOptions_AddSuggestedUser(IntPtr handle, UInt64 value);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_AdvancedAbuseReportOptions_ClearSuggestedUsers(IntPtr handle);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_AdvancedAbuseReportOptions_SetVideoMode(IntPtr handle, AbuseReportVideoMode value);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_ApplicationOptions_Create();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_ApplicationOptions_Destroy(IntPtr handle);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static void ovr_ApplicationOptions_SetDeeplinkMessage(IntPtr handle, string value) {
      IntPtr value_native = StringToNative(value);
      ovr_ApplicationOptions_SetDeeplinkMessage_Native(handle, value_native);
      Marshal.FreeCoTaskMem(value_native);
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_ApplicationOptions_SetDeeplinkMessage")]
    private static extern void ovr_ApplicationOptions_SetDeeplinkMessage_Native(IntPtr handle, IntPtr value);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static void ovr_ApplicationOptions_SetDestinationApiName(IntPtr handle, string value) {
      IntPtr value_native = StringToNative(value);
      ovr_ApplicationOptions_SetDestinationApiName_Native(handle, value_native);
      Marshal.FreeCoTaskMem(value_native);
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_ApplicationOptions_SetDestinationApiName")]
    private static extern void ovr_ApplicationOptions_SetDestinationApiName_Native(IntPtr handle, IntPtr value);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static void ovr_ApplicationOptions_SetLobbySessionId(IntPtr handle, string value) {
      IntPtr value_native = StringToNative(value);
      ovr_ApplicationOptions_SetLobbySessionId_Native(handle, value_native);
      Marshal.FreeCoTaskMem(value_native);
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_ApplicationOptions_SetLobbySessionId")]
    private static extern void ovr_ApplicationOptions_SetLobbySessionId_Native(IntPtr handle, IntPtr value);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static void ovr_ApplicationOptions_SetMatchSessionId(IntPtr handle, string value) {
      IntPtr value_native = StringToNative(value);
      ovr_ApplicationOptions_SetMatchSessionId_Native(handle, value_native);
      Marshal.FreeCoTaskMem(value_native);
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_ApplicationOptions_SetMatchSessionId")]
    private static extern void ovr_ApplicationOptions_SetMatchSessionId_Native(IntPtr handle, IntPtr value);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_ApplicationOptions_SetRoomId(IntPtr handle, UInt64 value);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_AvatarEditorOptions_Create();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_AvatarEditorOptions_Destroy(IntPtr handle);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static void ovr_AvatarEditorOptions_SetSourceOverride(IntPtr handle, string value) {
      IntPtr value_native = StringToNative(value);
      ovr_AvatarEditorOptions_SetSourceOverride_Native(handle, value_native);
      Marshal.FreeCoTaskMem(value_native);
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_AvatarEditorOptions_SetSourceOverride")]
    private static extern void ovr_AvatarEditorOptions_SetSourceOverride_Native(IntPtr handle, IntPtr value);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_ChallengeOptions_Create();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_ChallengeOptions_Destroy(IntPtr handle);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static void ovr_ChallengeOptions_SetDescription(IntPtr handle, string value) {
      IntPtr value_native = StringToNative(value);
      ovr_ChallengeOptions_SetDescription_Native(handle, value_native);
      Marshal.FreeCoTaskMem(value_native);
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_ChallengeOptions_SetDescription")]
    private static extern void ovr_ChallengeOptions_SetDescription_Native(IntPtr handle, IntPtr value);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static void ovr_ChallengeOptions_SetEndDate(IntPtr handle, DateTime value) {
      ulong value_native = DateTimeToNative(value);
      ovr_ChallengeOptions_SetEndDate_Native(handle, value_native);
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_ChallengeOptions_SetEndDate")]
    private static extern void ovr_ChallengeOptions_SetEndDate_Native(IntPtr handle, ulong value);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_ChallengeOptions_SetIncludeActiveChallenges(IntPtr handle, bool value);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_ChallengeOptions_SetIncludeFutureChallenges(IntPtr handle, bool value);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_ChallengeOptions_SetIncludePastChallenges(IntPtr handle, bool value);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static void ovr_ChallengeOptions_SetLeaderboardName(IntPtr handle, string value) {
      IntPtr value_native = StringToNative(value);
      ovr_ChallengeOptions_SetLeaderboardName_Native(handle, value_native);
      Marshal.FreeCoTaskMem(value_native);
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_ChallengeOptions_SetLeaderboardName")]
    private static extern void ovr_ChallengeOptions_SetLeaderboardName_Native(IntPtr handle, IntPtr value);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static void ovr_ChallengeOptions_SetStartDate(IntPtr handle, DateTime value) {
      ulong value_native = DateTimeToNative(value);
      ovr_ChallengeOptions_SetStartDate_Native(handle, value_native);
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_ChallengeOptions_SetStartDate")]
    private static extern void ovr_ChallengeOptions_SetStartDate_Native(IntPtr handle, ulong value);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static void ovr_ChallengeOptions_SetTitle(IntPtr handle, string value) {
      IntPtr value_native = StringToNative(value);
      ovr_ChallengeOptions_SetTitle_Native(handle, value_native);
      Marshal.FreeCoTaskMem(value_native);
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_ChallengeOptions_SetTitle")]
    private static extern void ovr_ChallengeOptions_SetTitle_Native(IntPtr handle, IntPtr value);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_ChallengeOptions_SetViewerFilter(IntPtr handle, ChallengeViewerFilter value);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_ChallengeOptions_SetVisibility(IntPtr handle, ChallengeVisibility value);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_GroupPresenceOptions_Create();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_GroupPresenceOptions_Destroy(IntPtr handle);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static void ovr_GroupPresenceOptions_SetDeeplinkMessageOverride(IntPtr handle, string value) {
      IntPtr value_native = StringToNative(value);
      ovr_GroupPresenceOptions_SetDeeplinkMessageOverride_Native(handle, value_native);
      Marshal.FreeCoTaskMem(value_native);
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_GroupPresenceOptions_SetDeeplinkMessageOverride")]
    private static extern void ovr_GroupPresenceOptions_SetDeeplinkMessageOverride_Native(IntPtr handle, IntPtr value);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static void ovr_GroupPresenceOptions_SetDestinationApiName(IntPtr handle, string value) {
      IntPtr value_native = StringToNative(value);
      ovr_GroupPresenceOptions_SetDestinationApiName_Native(handle, value_native);
      Marshal.FreeCoTaskMem(value_native);
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_GroupPresenceOptions_SetDestinationApiName")]
    private static extern void ovr_GroupPresenceOptions_SetDestinationApiName_Native(IntPtr handle, IntPtr value);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_GroupPresenceOptions_SetIsJoinable(IntPtr handle, bool value);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static void ovr_GroupPresenceOptions_SetLobbySessionId(IntPtr handle, string value) {
      IntPtr value_native = StringToNative(value);
      ovr_GroupPresenceOptions_SetLobbySessionId_Native(handle, value_native);
      Marshal.FreeCoTaskMem(value_native);
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_GroupPresenceOptions_SetLobbySessionId")]
    private static extern void ovr_GroupPresenceOptions_SetLobbySessionId_Native(IntPtr handle, IntPtr value);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static void ovr_GroupPresenceOptions_SetMatchSessionId(IntPtr handle, string value) {
      IntPtr value_native = StringToNative(value);
      ovr_GroupPresenceOptions_SetMatchSessionId_Native(handle, value_native);
      Marshal.FreeCoTaskMem(value_native);
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_GroupPresenceOptions_SetMatchSessionId")]
    private static extern void ovr_GroupPresenceOptions_SetMatchSessionId_Native(IntPtr handle, IntPtr value);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_InviteOptions_Create();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_InviteOptions_Destroy(IntPtr handle);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_InviteOptions_AddSuggestedUser(IntPtr handle, UInt64 value);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_InviteOptions_ClearSuggestedUsers(IntPtr handle);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_MultiplayerErrorOptions_Create();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_MultiplayerErrorOptions_Destroy(IntPtr handle);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_MultiplayerErrorOptions_SetErrorKey(IntPtr handle, MultiplayerErrorErrorKey value);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_NetSyncOptions_Create();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_NetSyncOptions_Destroy(IntPtr handle);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static void ovr_NetSyncOptions_SetVoipGroup(IntPtr handle, string value) {
      IntPtr value_native = StringToNative(value);
      ovr_NetSyncOptions_SetVoipGroup_Native(handle, value_native);
      Marshal.FreeCoTaskMem(value_native);
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_NetSyncOptions_SetVoipGroup")]
    private static extern void ovr_NetSyncOptions_SetVoipGroup_Native(IntPtr handle, IntPtr value);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_NetSyncOptions_SetVoipStreamDefault(IntPtr handle, NetSyncVoipStreamMode value);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static void ovr_NetSyncOptions_SetZoneId(IntPtr handle, string value) {
      IntPtr value_native = StringToNative(value);
      ovr_NetSyncOptions_SetZoneId_Native(handle, value_native);
      Marshal.FreeCoTaskMem(value_native);
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_NetSyncOptions_SetZoneId")]
    private static extern void ovr_NetSyncOptions_SetZoneId_Native(IntPtr handle, IntPtr value);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_RichPresenceOptions_Create();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_RichPresenceOptions_Destroy(IntPtr handle);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static void ovr_RichPresenceOptions_SetApiName(IntPtr handle, string value) {
      IntPtr value_native = StringToNative(value);
      ovr_RichPresenceOptions_SetApiName_Native(handle, value_native);
      Marshal.FreeCoTaskMem(value_native);
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_RichPresenceOptions_SetApiName")]
    private static extern void ovr_RichPresenceOptions_SetApiName_Native(IntPtr handle, IntPtr value);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    public static void ovr_RichPresenceOptions_SetDeeplinkMessageOverride(IntPtr handle, string value) {
      IntPtr value_native = StringToNative(value);
      ovr_RichPresenceOptions_SetDeeplinkMessageOverride_Native(handle, value_native);
      Marshal.FreeCoTaskMem(value_native);
    }

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl, EntryPoint="ovr_RichPresenceOptions_SetDeeplinkMessageOverride")]
    private static extern void ovr_RichPresenceOptions_SetDeeplinkMessageOverride_Native(IntPtr handle, IntPtr value);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_RichPresenceOptions_SetIsJoinable(IntPtr handle, bool value);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_RosterOptions_Create();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_RosterOptions_Destroy(IntPtr handle);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_RosterOptions_AddSuggestedUser(IntPtr handle, UInt64 value);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_RosterOptions_ClearSuggestedUsers(IntPtr handle);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_UserOptions_Create();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_UserOptions_Destroy(IntPtr handle);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_UserOptions_SetMaxUsers(IntPtr handle, uint value);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_UserOptions_AddServiceProvider(IntPtr handle, ServiceProvider value);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_UserOptions_ClearServiceProviders(IntPtr handle);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_UserOptions_SetTimeWindow(IntPtr handle, TimeWindow value);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern IntPtr ovr_VoipOptions_Create();

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_VoipOptions_Destroy(IntPtr handle);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_VoipOptions_SetBitrateForNewConnections(IntPtr handle, VoipBitrate value);

    /// The Meta Quest Platform offers features that you can use to create engaging and social game experiences.
    /// Supporting objectives: Identity, Social and Community, Engagement, Revenue, and Security.
    /// See more details at [here](https://developer.oculus.com/reference/platform-unity/latest/).
    [DllImport(DLL_NAME, CallingConvention=CallingConvention.Cdecl)]
    public static extern void ovr_VoipOptions_SetCreateNewConnectionUseDtx(IntPtr handle, VoipDtxState value);
  }
}

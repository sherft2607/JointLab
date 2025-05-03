namespace Oculus.Platform.Models
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using Oculus.Platform.Models;
    using UnityEngine;
    /// Represents an update to an HTTP transfer, which is a process of transferring data over the internet using the HTTP protocol.
    /// It provides a way for developers to monitor the progress of an HTTP transfer and can be retrieved using Message::MessageType::Notification_HTTP_Transfer.
    public class HttpTransferUpdate
    {
        /// It's a unique identifier for the HTTP transfer.
        /// It is used to track the progress of the transfer and can be retrieved using Message#RequestID.
        public readonly UInt64 ID;
        /// An array of bytes that represents the data being transferred.
        public readonly byte[] Payload;
        /// This field is a `boolean` value that indicates whether the HTTP transfer has been completed or not.
        public readonly bool IsCompleted;

        public HttpTransferUpdate(IntPtr o)
        {
            ID = CAPI.ovr_HttpTransferUpdate_GetID(o);
            IsCompleted = CAPI.ovr_HttpTransferUpdate_IsCompleted(o);

            long size = (long)CAPI.ovr_HttpTransferUpdate_GetSize(o);

            Payload = new byte[size];
            Marshal.Copy(CAPI.ovr_Packet_GetBytes(o), Payload, 0, (int)size);
        }
    }
}

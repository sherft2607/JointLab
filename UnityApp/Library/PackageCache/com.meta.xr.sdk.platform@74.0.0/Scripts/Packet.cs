namespace Oculus.Platform
{
    using System;
    using System.Runtime.InteropServices;

    /// This class provides a way to encapsulate and transmit data across a network. It contains methods for reading and writing data,
    /// as well as properties for accessing metadata about the packet.
    /// See more info about Platform Solutions [here](https://developer.oculus.com/documentation/{{url.platform}}/ps-platform-intro/).
    public sealed class Packet : IDisposable
    {
        /// The size of the packet in bytes. This value is set when the packet is created and remains constant throughout its lifetime.
        private readonly ulong size;
        /// A pointer to the packet handle. This is an opaque value that represents the underlying native resource that stores the packet data.
        private readonly IntPtr packetHandle;

        /// Initializes a new instance of the <see cref="Packet"/> class with a specified packet handle.
        /// The param packetHandle must be a valid handle to a packet created by the underlying native API.
        public Packet(IntPtr packetHandle)
        {
            this.packetHandle = packetHandle;
            size = (ulong)CAPI.ovr_Packet_GetSize(packetHandle);
        }

        /// Copies all the bytes in the payload into a byte array. This method is used to retrieve the raw data contained within the packet.
        /// <param name="destination">The destination byte array. This array must be large enough to hold the entire packet payload, which is specified by the <see cref="Size"/> property.</param>
        /// <returns>The number of bytes read from the packet. This value will always be equal to the <see cref="Size"/> property.</returns>
        public ulong ReadBytes(byte[] destination)
        {
            if ((ulong)destination.LongLength < size)
            {
                throw new System.ArgumentException(
                    String.Format("Destination array was not big enough to hold {0} bytes", size));
            }

            Marshal.Copy(CAPI.ovr_Packet_GetBytes(packetHandle), destination, 0, (int)size);
            return size;
        }

        /// Gets the sender ID of the packet. This value identifies the source of the packet and can be used to track the origin of the data.
        public UInt64 SenderID
        {
            get { return CAPI.ovr_Packet_GetSenderID(packetHandle); }
        }


        /// Gets the size of the packet in bytes. This value represents the total amount of data contained within the packet.
        public ulong Size
        {
            get { return size; }
        }

        #region IDisposable

        ~Packet()
        {
            Dispose();
        }

        /// This method is called when the <see cref="Packet"/> is being garbage collected or when it is explicitly disposed of.
        /// It is responsible for releasing any unmanaged resources, such as the native packet handle, that are held by the <see cref="Packet"/>.
        public void Dispose()
        {
            CAPI.ovr_Packet_Free(packetHandle);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}

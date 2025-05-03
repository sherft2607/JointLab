namespace Oculus.Platform
{
    /// This is the interface for VoipPCMSource subclasses to implement which represent a PCM(pulse code modulation) object.
    /// An example class that implements this interface is VoipPCMSourceNative. Read more about Voip [here](https://developer.oculus.com/documentation/unity/ps-parties/#voip-options).
    public interface IVoipPCMSource
    {
        /// Retrieves the PCM float as an int.
        int GetPCM(float[] dest, int length);

        /// Sets the sender ID associated with this Voip source. The ID will belong to a Models.User.
        void SetSenderID(ulong senderID);

        /// Updates the Voip PCM source.
        void Update();

        /// Uses the sender ID and will return an int that represents the size of the current PCM data stack.
        int PeekSizeElements();
    }
}

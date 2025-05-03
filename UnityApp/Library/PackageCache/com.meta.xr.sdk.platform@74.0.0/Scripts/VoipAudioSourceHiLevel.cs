namespace Oculus.Platform
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    /// A class that is used in Oculus Platform SDK to provide audio source helper functions and fields for high-level audio processing.
    /// Is used to hold a PCM source for receiving and decoding VoIP data. Read more about VoIP options [here](https://developer.oculus.com/documentation/unity/ps-parties/#voip-options).
    public class VoipAudioSourceHiLevel : MonoBehaviour
    {
        /// This is a delegate that exists as a surface for OnAudioFilterRead
        /// It will be called on unity's audio thread and has a custom filter to determine whether the audio system is able to handle the ring buffer size. It is a subclass of VoipAudioSourceHiLevel.
        public class FilterReadDelegate : MonoBehaviour
        {
            /// This field is used to hold a reference to the parent object which is a class type of VoipAudioSourceHiLevel.
            public VoipAudioSourceHiLevel parent;
            float[] scratchBuffer;

            void Awake()
            {
                int bufferSizeElements = (int)CAPI.ovr_Voip_GetOutputBufferMaxSize();
                scratchBuffer = new float[bufferSizeElements];
            }

            void OnAudioFilterRead(float[] data, int channels)
            {
                int sizeToFetch = data.Length / channels;
                int sourceBufferSize = sizeToFetch;
                if (sourceBufferSize > scratchBuffer.Length)
                {
                    Array.Clear(data, 0, data.Length);
                    throw new Exception(string.Format(
                        "Audio system tried to pull {0} bytes, max voip internal ring buffer size {1}", sizeToFetch,
                        scratchBuffer.Length));
                }

                int available = parent.pcmSource.PeekSizeElements();
                if (available < sourceBufferSize)
                {
                    if (verboseLogging)
                    {
                        Debug.LogFormat(
                            "Voip starved! Want {0}, but only have {1} available",
                            sourceBufferSize,
                            available);
                    }

                    return;
                }

                int copied = parent.pcmSource.GetPCM(scratchBuffer, sourceBufferSize);
                if (copied < sourceBufferSize)
                {
                    Debug.LogWarningFormat(
                        "GetPCM() returned {0} samples, expected {1}",
                        copied,
                        sourceBufferSize);

                    return;
                }

                int dest = 0;
                float tmpPeakAmp = -1;
                for (int i = 0; i < sizeToFetch; i++)
                {
                    float val = scratchBuffer[i];

                    for (int j = 0; j < channels; j++)
                    {
                        data[dest++] = val;
                        if (val > tmpPeakAmp)
                        {
                            tmpPeakAmp = val;
                        }
                    }
                }

                parent.peakAmplitude = tmpPeakAmp;
            }
        }


        int initialPlaybackDelayMS;

        /// The ID of the sender of the PCM source. This is the User who is the source for the audio.
        public UInt64 senderID
        {
            set { pcmSource.SetSenderID(value); }
        }

        /// The AudioSource object that will be used to control the audio source such as muting and unmuting.
        public AudioSource audioSource;
        /// This field is used to indicate the peak ampltitude value of the audio source data.
        public float peakAmplitude;

        /// Represents the PCM source that is used to read analogue data. It is of type IVoipPCMSource.
        protected IVoipPCMSource pcmSource;

        static int audioSystemPlaybackFrequency;
        static bool verboseLogging = false;

        protected void Stop()
        {
        }

        VoipSampleRate SampleRateToEnum(int rate)
        {
            switch (rate)
            {
                case 48000:
                    return VoipSampleRate.HZ48000;
                case 44100:
                    return VoipSampleRate.HZ44100;
                case 24000:
                    return VoipSampleRate.HZ24000;
                default:
                    return VoipSampleRate.Unknown;
            }
        }

        /// This function is used to initialize the VoipAudioSourceHiLevel. It is used to set up the PCM source and the audio source component attached to the game object.
        protected void Awake()
        {
            CreatePCMSource();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            audioSource.gameObject.AddComponent<FilterReadDelegate>();
            var filterDelegate = audioSource.gameObject.GetComponent<FilterReadDelegate>();
            filterDelegate.parent = this;

            initialPlaybackDelayMS = 40;

            audioSystemPlaybackFrequency = AudioSettings.outputSampleRate;
            CAPI.ovr_Voip_SetOutputSampleRate(SampleRateToEnum(audioSystemPlaybackFrequency));
            if (verboseLogging)
            {
                Debug.LogFormat("freq {0}", audioSystemPlaybackFrequency);
            }
        }

        void Start()
        {
            audioSource.Stop();
        }

        protected virtual void CreatePCMSource()
        {
            pcmSource = new VoipPCMSourceNative();
        }

        /// Converts delay in milliseconds to number of PCM elements to compare against PCM buffer to determine when to initiate playback.
        protected static int MSToElements(int ms)
        {
            return ms * audioSystemPlaybackFrequency / 1000;
        }

        void Update()
        {
            pcmSource.Update();

            if (!audioSource.isPlaying && pcmSource.PeekSizeElements() >= MSToElements(initialPlaybackDelayMS))
            {
                if (verboseLogging)
                {
                    Debug.LogFormat("buffered {0} elements, starting playback", pcmSource.PeekSizeElements());
                }

                audioSource.Play();
            }
        }
    }
}

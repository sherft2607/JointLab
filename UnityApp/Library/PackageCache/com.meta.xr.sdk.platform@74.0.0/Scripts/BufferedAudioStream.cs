//#define VERBOSE_LOGGING

using UnityEngine;
using System.Collections;
using System;
using Oculus.Platform;

/// BufferedAudioStream provides a way to stream audio data from an AudioSource object.
/// It maintains an internal buffer and offers methods for updating and adding new data.
/// Update() updates playback based on remaining buffer time, while AddData() adds new data to the buffer,
/// wrapping around if necessary. This class is useful for real-time audio streaming, such as playing back
/// audio from a network connection or generating audio dynamically.
/// This class is used in VoipAudioSource, which has been deprecated. See details [here](https://developers.meta.com/horizon/blog/deprecating-oculus-rooms-api-in-march-2023/).
public class BufferedAudioStream
{
    const bool VerboseLogging = false;
    AudioSource audio;

    float[] audioBuffer;
    int writePos;

    const float bufferLengthSeconds = 0.25f;
    const int sampleRate = 48000;
    const int bufferSize = (int)(sampleRate * bufferLengthSeconds);
    const float playbackDelayTimeSeconds = 0.05f;

    float playbackDelayRemaining;
    float remainingBufferTime;

    /// The constructor of BufferedAudioStream takes an AudioSource object as a parameter and initializes the audio buffer with a specified size.
    /// It also sets the loop property of the AudioSource object to true and creates an AudioClip object with the specified buffer size,
    /// sample rate, and number of channels. This class is used in VoipAudioSource, which has been deprecated. See details [here](https://developers.meta.com/horizon/blog/deprecating-oculus-rooms-api-in-march-2023/).
    public BufferedAudioStream(AudioSource audio)
    {
        audioBuffer = new float[bufferSize];
        this.audio = audio;

        audio.loop = true;
        audio.clip = AudioClip.Create("", bufferSize, 1, sampleRate, false);

        Stop();
    }

    /// This method updates the audio playback based on the remaining buffer time. If there is still buffer time remaining,
    /// it checks if the audio should start playing and updates the playback delay timer. If the audio is currently playing,
    /// it decrements the remaining buffer time. If the buffer is empty, it stops the audio playback and logs a message.
    /// This method is used in VoipAudioSource which got drecated, which has been deprecated. See details [here](https://developers.meta.com/horizon/blog/deprecating-oculus-rooms-api-in-march-2023/).
    public void Update()
    {
        if (remainingBufferTime > 0)
        {
#if VERBOSE_LOGGING
      Debug.Log(string.Format("current time: {0}, remainingBufferTime: {1}", Time.time, remainingBufferTime));
#endif

            if (!audio.isPlaying && remainingBufferTime > playbackDelayTimeSeconds)
            {
                playbackDelayRemaining -= Time.deltaTime;
                if (playbackDelayRemaining <= 0)
                {
#if VERBOSE_LOGGING
          Debug.Log("Starting playback");
#endif
                    audio.Play();
                }
            }

            if (audio.isPlaying)
            {
                remainingBufferTime -= Time.deltaTime;
                if (remainingBufferTime < 0)
                {
                    remainingBufferTime = 0;
                }
            }
        }

        if (remainingBufferTime <= 0)
        {
            if (audio.isPlaying)
            {
                Debug.Log("Buffer empty, stopping " + DateTime.Now);
                Stop();
            }
            else
            {
                if (writePos != 0)
                {
                    Debug.LogError("writePos non zero while not playing, how did this happen?");
                }
            }
        }
    }

    void Stop()
    {
        audio.Stop();
        audio.time = 0;
        writePos = 0;
        playbackDelayRemaining = playbackDelayTimeSeconds;
    }

    /// This method adds new audio data to the buffer. It takes an array of float samples as input and writes them to the internal audio buffer.
    /// If there is not enough space in the buffer, it will wrap around to the beginning of the buffer. If the write position exceeds the buffer length,
    /// an exception is thrown. The method returns the number of samples written to the buffer.
    /// This method is used in VoipAudioSource which got drecated, which has been deprecated. See details [here](https://developers.meta.com/horizon/blog/deprecating-oculus-rooms-api-in-march-2023/).
    public void AddData(float[] samples)
    {
        int remainingWriteLength = samples.Length;

        if (writePos > audioBuffer.Length)
        {
            throw new Exception();
        }

        do
        {
            int writeLength = remainingWriteLength;
            int remainingSpace = audioBuffer.Length - writePos;

            if (writeLength > remainingSpace)
            {
                writeLength = remainingSpace;
            }

            Array.Copy(samples, 0, audioBuffer, writePos, writeLength);

            remainingWriteLength -= writeLength;
            writePos += writeLength;
            if (writePos > audioBuffer.Length)
            {
                throw new Exception();
            }

            if (writePos == audioBuffer.Length)
            {
                writePos = 0;
            }
        } while (remainingWriteLength > 0);

#if VERBOSE_LOGGING
    float prev = remainingBufferTime;
#endif
        remainingBufferTime += (float)samples.Length / sampleRate;
#if VERBOSE_LOGGING
    Debug.Log(string.Format("previous remaining: {0}, new remaining: {1}, added {2} samples", prev, remainingBufferTime, samples.Length));
#endif
        audio.clip.SetData(audioBuffer, 0);
    }
}

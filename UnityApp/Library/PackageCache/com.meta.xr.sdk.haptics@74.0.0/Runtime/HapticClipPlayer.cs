/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
 * you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https://developer.oculus.com/licenses/oculussdk/
 *
 * Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

// @lint-ignore-every LICENSELINT

using System;
using UnityEngine;

namespace Oculus.Haptics
{
    /// <summary>
    /// <c>HapticClipPlayer</c> provides controls for playing a <c>HapticClip</c>.
    /// </summary>
    ///
    /// <remarks>
    /// <para>
    /// A <c>HapticClipPlayer</c> only plays valid <c>HapticClip</c>s. You can start and
    /// stop a <c>HapticClip</c> assigned to a <c>HapticClipPlayer</c> as often as required.
    /// </para>
    ///
    /// <para>
    /// A <c>HapticClipPlayer</c> can be in a stopped, playing, or paused state.
    /// <br />
    /// By default a <c>HapticClipPlayer</c> is in a stopped state. A player returns to the stopped state when the loaded clip reaches its end during playback,
    /// or by explicitly calling <see cref="Stop()"/>.
    /// <br />
    /// When calling <see cref="Play(Controller)"/> the player enters a playing state.
    /// <br />
    /// A <c>HapticClipPlayer</c> in the playing state can enter a paused state by calling <see cref="Pause()"/>.
    /// Playback can be unpaused (i.e. playing) from the current paused playback position by calling
    /// <see cref="Play(Controller)"/> or <see cref="Resume()"/>.
    /// <br />
    /// Calling <see cref="Resume()"/> on a playing player has no effect.
    /// <br />
    /// Calling <see cref="Play(Controller)"/> on a playing player makes it play again from the start.
    /// <br />
    /// Calling <see cref="Seek(float)"/> on a stopped player will move into a paused state. The playback location defaults to both controllers in this case,
    /// making it possible to call <see cref="Resume()"/>. To deliberately start playback from the seeked playback location, use <see cref="Play(Controller)"/>.
    /// </para>
    ///
    /// <para>
    /// The rendered amplitude and frequency can be modulated during runtime using the <see cref="amplitude"/>
    /// and <see cref="frequencyShift"/> properties respectively.
    /// <br />
    /// You can also loop a clip using the <see cref="isLooping"/> property.
    /// </para>
    ///
    /// <para>
    /// It is possible to release <c>HapticClipPlayer</c> objects as needed to free up memory using the <see cref="Dispose()"/> method.
    /// Of course, calling any method on a released <c>HapticClipPlayer</c> will cause a runtime error.
    /// </para>
    /// </remarks>
    public class HapticClipPlayer : IDisposable
    {
        /// The internal ID of the <see cref="HapticClip"/> associated with the <c>HapticClipPlayer</c>. This ID is used internally to identify the
        /// clip.
        ///
        /// The <c>_clipID</c> is set when creating a new <c>HapticClipPlayer</c> instance with a <c>HapticClip</c>, typically through
        /// the <see cref="HapticClipPlayer(HapticClip)"/> constructor and when assigning the clip via the <c>clip</c> property
        private int _clipId = Ffi.InvalidId;

        /// The internal ID of the <c>HapticClipPlayer</c>. As long as the player has an ID, it is considered to be in a valid state and can play.
        /// If the player is explicitly disposed, its ID is invalidated.
        private int _playerId = Ffi.InvalidId;

        /// <summary>
        /// The implementation of <see cref="Haptics"/> for <c>HapticClipPlayer</c> to use. This field is protected to allow derived
        /// classes to provide a custom implementation.
        /// The <c>HapticClipPlayer</c> uses this instance to play haptic clips and access haptic-related functionality.
        /// </summary>
        protected Haptics _haptics;

        /// <summary>
        /// Creates a <c>HapticClipPlayer</c> with no <see cref="HapticClip"/> assigned to it.
        /// </summary>
        ///
        /// <remarks>
        /// You must assign a <c>HapticClip</c> before you can play this <c>HapticClipPlayer</c>.
        /// You can either call the overloaded version of this constructor that accepts
        /// a <c>HapticClip</c> or assign it with <see cref="clip"/>.
        /// </remarks>
        public HapticClipPlayer()
        {
            SetHaptics();

            // Create player and check if that succeeded.
            int playerReturnValue = _haptics.CreateHapticPlayer();

            if (Ffi.InvalidId != playerReturnValue)
            {
                _playerId = playerReturnValue;
            }
        }

        /// <summary>
        /// Creates a <c>HapticClipPlayer</c> and assigns the given <see cref="HapticClip"/> to it. You can use
        /// this player to play, stop, and generally control the haptic clip's playback properties.
        /// </summary>
        ///
        /// <param name="clip">The <c>HapticClip</c> to be played by this <c>HapticClipPlayer</c>.
        /// Providing invalid clip data (e.g., null or empty) will throw an <c>ArgumentNullException</c>.
        /// </param>
        public HapticClipPlayer(HapticClip clip)
        {
            SetHaptics();

            // Create player and check if that succeeded.
            int playerReturnValue = _haptics.CreateHapticPlayer();

            if (Ffi.InvalidId != playerReturnValue)
            {
                _playerId = playerReturnValue;

                this.clip = clip;
            }
        }

        /// <summary>
        /// Sets the <see cref="Haptics"/> implementation that <c>HapticClipPlayer</c> will call
        /// into for all haptics operations.
        /// See also: <see cref="Haptics.Instance"/> for more information on <c>Haptics</c>.
        /// </summary>
        protected virtual void SetHaptics()
        {
            _haptics = Haptics.Instance;
        }

        /// <summary>
        /// Starts playing the assigned haptic clip on the specified controller.
        /// </summary>
        ///
        /// <param name="controller">The controller(s) to play back on. See <see cref="Haptics.Controller"/>
        /// </param>
        public void Play(Controller controller)
        {
            _haptics.PlayHapticPlayer(_playerId, controller);
        }

        /// <summary>
        /// Pauses playback on the <c>HapticClipPlayer</c>.
        /// If a haptic clip is currently playing, it will be paused immediately and maintain it's current playback position.
        /// You can call this method at any time to pause playback, regardless of whether a clip is currently playing or not.
        /// </summary>
        public void Pause()
        {
            _haptics.PauseHapticPlayer(_playerId);
        }

        /// <summary>
        /// Resumes playback on the <c>HapticClipPlayer</c>.
        /// If the playback of a haptic clip is currently paused, playback will be resumed immediately on the controller previously defined by <see cref="Play(Controller)"/>.
        /// If the clip player is currently stopped or already playing, calling this method has no effect.
        /// If playback was previously seeked on a stopped clip player, playback will resume on both controllers by default from the seeked playback position.
        /// </summary>
        public void Resume()
        {
            _haptics.ResumeHapticPlayer(_playerId);
        }

        /// <summary>
        /// Stops playback of the <c>HapticClipPlayer</c>.
        /// If a haptic clip is currently playing, it will be stopped immediately.
        /// You can call this method at any time to stop playback, regardless of whether a clip is currently playing or not.
        /// </summary>
        public void Stop()
        {
            _haptics.StopHapticPlayer(_playerId);
        }

        /// <summary>
        /// Moves the current playback position of the <c>HapticClipPlayer</c> to the provided time in seconds.
        /// If a haptic clip is currently playing, the playback position will jump to the provided time immediately.
        /// If the player is currently paused or stopped, it will require a deliberate call to <see cref="Resume"/> or
        /// <see cref="Play"/> to start playback from the seeked playback position.
        /// </summary>
        ///
        /// <param name="time">The target time in seconds to move the current playback position to.</param>
        public void Seek(float time)
        {
            _haptics.SeekPlaybackPositionHapticPlayer(_playerId, time);
        }

        /// <summary>
        /// Whether looping is enabled or not. When set to <c>true</c>, the haptic clip will loop continuously
        /// until stopped.
        /// </summary>
        ///
        /// <value><c>true</c> if looping is enabled.</value>
        public bool isLooping
        {
            get => _haptics.IsHapticPlayerLooping(_playerId);
            set => _haptics.LoopHapticPlayer(_playerId, value);
        }

        /// <summary>
        /// Gets the duration of the loaded haptic clip of this <c>HapticClipPlayer</c>'s instance.
        /// </summary>
        ///
        /// <value>The duration of the haptic clip in seconds.</value>
        /// <remarks>
        /// This property returns the length of the haptic clip in seconds. If no haptic clip is loaded, this property will return 0.
        /// </remarks>
        public float clipDuration => _haptics.GetClipDuration(_clipId);

        /// <summary>
        /// Sets/gets the <c>HapticClipPlayer</c>'s amplitude.
        ///
        /// During playback, the individual amplitudes in the clip will be multiplied by the player's amplitude.
        /// This changes how strong the vibration is. Amplitude values in a clip range from 0.0 to 1.0,
        /// and the result after applying the amplitude scale will be clipped to that range.
        ///
        /// An amplitude of 0.0 means that no vibration will be triggered, and an amplitude of 0.5 will
        /// result in the clip being played back at half of its amplitude.
        ///
        /// Example: if you apply amplitude of 5.0 to a haptic clip and the following amplitudes are in the
        /// clip: [0.2, 0.5, 0.1], the initial amplitude calculation would produce these values: [1.0, 2.5, 0.5]
        /// which will then be clamped like this: [1.0, 1.0, 0.5]
        ///
        /// This method can be called during active playback, in which case the amplitude is applied
        /// immediately, with a small delay in the tens of milliseconds.
        /// </summary>
        ///
        /// <value>A value of zero or greater. Negative values will
        /// cause an exception.</value>
        public float amplitude
        {
            get => _haptics.GetAmplitudeHapticPlayer(_playerId);
            set => _haptics.SetAmplitudeHapticPlayer(_playerId, value);
        }

        /// <summary>
        /// Sets/gets the <c>HapticClipPlayer</c>'s frequency shift.
        ///
        /// The frequencies in a haptic clip are in the range zero to one. This property shifts the
        /// individual frequencies up or down. The acceptable range of values is -1.0 to 1.0 inclusive.
        ///
        /// Once the frequencies in a clip have been shifted, they will be clamped to the playable
        /// range of frequency values, i.e. zero to one.
        ///
        /// Setting this property to 0.0 means that the frequencies will not be changed.
        ///
        /// Example: if you apply a frequency shift of 0.8 to a haptic clip and the following frequencies
        /// are in the clip: [0.1, 0.5, 0.0], the initial frequency shift calculation will produce these
        /// frequencies: [0.9, 1.3, 0.8] which will then be clamped like this: [0.9, 1.0, 0.8]
        ///
        /// This method can be called during active playback, in which case the frequency shift is applied
        /// immediately, with a small delay in the tens of milliseconds.
        /// </summary>
        ///
        /// <value>A value between -1.0 and 1.0. Values outside this range will cause an exception.</value>
        public float frequencyShift
        {
            get => _haptics.GetFrequencyShiftHapticPlayer(_playerId);
            set => _haptics.SetFrequencyShiftHapticPlayer(_playerId, value);
        }

        /// <summary>
        /// Sets/gets the <c>HapticClipPlayer</c>'s playback priority.
        ///
        /// The playback engine of the Haptics SDK only ever renders the vibrations of a single <c>HapticClipPlayer</c>
        /// clip on the same controller at the same time. Meaning, haptic clips are not "mixed" when played back.
        /// If you have multiple players playing on the same controller at the same time, then only the player with the
        /// highest priority will trigger vibrations.
        ///
        /// Given the same priority value, the engine will always play the most recent clip that was started. All other
        /// players are muted, but continue tracking playback on their respective timeline (i.e. they are not paused).
        /// If a clip finishes (or is stopped), the engine will resume playback of the second most recent clip with an
        /// equal or higher priority level and so on.
        ///
        /// Example: Setting priority can be helpful if some haptic clips are more important than others, and allow us to
        /// design a hierarchy of haptic feedback based on context or the overall importance. For example, we could want
        /// a user to always receive a distinct haptic feedback if they are hit. Setting this "hit" clips priority higher
        /// compared to other haptic clips will ensure that the user always receives this haptic feedback.
        ///
        /// Priority values can be on the range of 0 (high priority) to 255 (low priority).
        /// By default, the priority value is set to 128 for every <c>HapticClipPlayer</c>.
        /// The player's priority can be changed before and during playback.
        /// </summary>
        ///
        /// <value>An integer value within the range 0 to 255 (inclusive).
        /// Values outside this range will cause an exception.</value>
        public uint priority
        {
            get => _haptics.GetPriorityHapticPlayer(_playerId);
            set => _haptics.SetPriorityHapticPlayer(_playerId, value);
        }

        /// <summary>
        /// Sets the <c>HapticClipPlayer</c>'s current haptic clip.
        ///
        /// This feature allows you to change the clip loaded in a clip player.
        /// If the player is currently playing it will be stopped. All other properties like amplitude, frequency
        /// shift, looping and priority are kept.
        /// </summary>
        ///
        /// <value>A valid, JSON formatted, UTF-8 encoded haptic clip.
        /// Providing invalid clip data will cause an exception.</value>
        public HapticClip clip
        {
            set
            {
                int returnValue = _haptics.LoadClip(value.json);

                if (Ffi.InvalidId != returnValue)
                {
                    _haptics.SetHapticPlayerClip(_playerId, returnValue);

                    // Remove previously assigned haptic clip.
                    if (_clipId != Ffi.InvalidId)
                    {
                        _haptics.ReleaseClip(_clipId);
                    }

                    _clipId = returnValue;
                }
            }
        }

        /// <summary>
        /// Call this method to explicitly/deterministically release a <c>HapticClipPlayer</c> object, otherwise
        /// the garbage collector will release it. Of course, any calls to a disposed <c>HapticClipPlayer</c>
        /// will result in runtime errors.
        /// </summary>
        ///
        /// <remarks>
        /// A given <c>HapticClip</c> will not be freed until all <c>HapticClipPlayer</c>s to which
        /// it is assigned have also been freed.
        /// </remarks>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the assigned clip and clip player from memory.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (_playerId != Ffi.InvalidId)
            {
                if (!_haptics.ReleaseClip(_clipId) & _haptics.ReleaseHapticPlayer(_playerId))
                {
                    Debug.LogError($"Error: HapticClipPlayer or HapticClip could not be released");
                }

                _clipId = Ffi.InvalidId;
                _playerId = Ffi.InvalidId;
            }
        }

        ~HapticClipPlayer()
        {
            Dispose(false);
        }
    }
}

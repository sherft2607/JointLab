// (c) Meta Platforms, Inc. and affiliates. Confidential and proprietary.

using System;
using UnityEngine;

namespace Oculus.Haptics
{
    /// <summary>
    /// Provides playback functionality for a single haptic clip by wrapping <see cref="HapticClipPlayer"/>.
    /// </summary>
    ///
    /// <remarks>
    /// If you don't need a MonoBehaviour and a C# class would be more suitable, use <c>HapticClipPlayer</c>
    /// instead. It has the same functionality.
    /// <br />
    ///
    /// Once you have assigned a <see cref="HapticClip"/> to the <see cref="clip"/> property you can play
    /// the <c>HapticSource</c>, enable or disable <c>loop</c>ing, and modulate the amplitude and frequency.
    /// <br />
    ///
    /// You can place multiple <c>HapticSource</c> components in your scene, with a different
    /// <c>HapticClip</c> assigned to each.
    /// </remarks>
    public class HapticSource : MonoBehaviour, ISerializationCallbackReceiver
    {
        // The <c>HapticClipPlayer</c> that <c>HapticSource</c> wraps.
        private HapticClipPlayer _player;

        // The following are serializable fields that Unity uses to persist the various
        // properties and that <c>HapticSourceEditor</c> uses as the PropertyField. The
        // default values set here are the ones that the sliders etc. are set to in the
        // Unity editor.

        // A serializable field for <see href="clip"/>.
        [SerializeField]
        private HapticClip _clip;

        // A serializable field for <see href="controller"/>.
        [SerializeField]
        Controller _controller = Controller.Both;

        // A serializable field for <see href="loop"/>.
        [SerializeField]
        private bool _loop = false;

        // A serializable field for <see href="amplitude"/>.
        [SerializeField]
        [Range(0.0f, float.MaxValue)]
        private float _amplitude = 1.0f;

        // A serializable field for <see href="frequencyShift"/>.
        [SerializeField]
        [Range(-1.0f, 1.0f)]
        private float _frequencyShift = 0.0f;

        // A serializable field for <see href="priority"/>.
        [SerializeField]
        [Range(0, 255)]
        private uint _priority = 128;

        /// <summary>
        /// <c>Awake()</c> creates a new <c>HapticClipPlayer</c>, assigns the
        /// serialized <c>clip</c> to it, and applies all serialized properties.
        /// </summary>
        void Awake()
        {
            _player = new HapticClipPlayer();
            _player.clip = _clip;
            SyncSerializedFieldsToPlayer();
        }

        /// <summary>
        /// Starts playback of the <c>HapticClip</c> that has been assigned with the <c>clip</c>
        /// property from the start of the clip.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        /// All properties applied to this <see cref="HapticSource"/> will be effective during playback.<br />
        /// Although multiple <c>HapticSource</c>s can play simultaneously, the output of only one will be felt at any given moment on
        /// a given controller.<br />
        /// See <see cref="HapticClipPlayer.priority"/> for a description of how this works.
        /// </para>
        ///
        /// <para>
        /// For further information on <c>Play()</c> see <see cref="HapticClipPlayer.Play(Controller)"/>.<br />
        /// For more details on playback states and playback behaviour, see <see cref="HapticClipPlayer"/>.
        /// </para>
        /// </remarks>
        public void Play()
        {
            _player.Play(_controller);
        }

        /// <summary>
        /// Starts playback of the <c>HapticClip</c> that has been assigned with the <c>clip</c>
        /// property on the controller passed by argument.
        /// </summary>
        ///
        /// <remarks>
        /// The controller assigned to this <c>HapticSource</c> will be reassigned to the one
        /// passed to this method.
        ///
        /// <para>
        /// For more information on playback behaviour see <see cref="HapticSource.Play()"/> and
        /// <see cref="HapticClipPlayer"/>.
        /// </para>
        /// </remarks>
        /// <param name="controller">The physical controller to play haptics on.</param>
        public void Play(Controller controller)
        {
            this.controller = controller;
            _player.Play(_controller);
        }

        /// <summary>
        /// Pause playback of the <c>HapticSource</c>.
        /// </summary>
        ///
        /// <remarks>
        /// See also: <see cref="HapticClipPlayer.Pause()"/>.<br />
        /// For details on playback states and playback behaviour, see <see cref="HapticClipPlayer"/>.
        /// </remarks>
        public void Pause()
        {
            _player.Pause();
        }

        /// <summary>
        /// Resume playback of the <c>HapticSource</c>.
        /// </summary>
        ///
        /// <remarks>
        /// See also: <see cref="HapticClipPlayer.Resume()"/>.<br />
        /// For details on playback states and playback behaviour, see <see cref="HapticClipPlayer"/>.
        /// </remarks>
        public void Resume()
        {
            _player.Resume();
        }

        /// <summary>
        /// Stops playback of the <c>HapticSource</c>.
        /// </summary>
        ///
        /// <remarks>
        /// See also: <see cref="HapticClipPlayer.Stop()"/>.<br />
        /// For details on playback states and playback behaviour, see <see cref="HapticClipPlayer"/>.
        /// </remarks>
        public void Stop()
        {
            _player.Stop();
        }

        /// <summary>
        /// Seeks the current playback position of the <c>HapticSource</c>.
        /// </summary>
        ///
        /// <remarks>
        /// See also: <see cref="HapticClipPlayer.Seek()"/>.<br />
        /// For details on playback states and playback behaviour, see <see cref="HapticClipPlayer"/>.
        /// </remarks>
        public void Seek(float time)
        {
            _player.Seek(time);
        }

        /// <summary>
        /// Assigns a <c>HapticClip</c> to this <c>HapticSource</c>.
        /// </summary>
        ///
        /// <remarks>
        /// See also: <see cref="HapticClipPlayer.Play(Controller)"/>.<br />
        /// For details on playback states and playback behaviour, see <see cref="HapticClipPlayer"/>.
        /// </remarks>
        public HapticClip clip
        {
            set
            {
                _clip = value;
                if (_player != null)
                {
                    _player.clip = _clip;
                }
            }
        }

        /// <summary>
        /// The duration of the assigned <c>HapticClip</c> in seconds.
        /// </summary>
        public float clipDuration => _player.clipDuration;

        /// <summary>
        /// Assigns a physical controller to this <c>HapticSource</c> that
        /// haptics should output to.
        /// </summary>
        ///
        /// <remarks>
        /// The controller will only be applied the next time <see cref="PlayFromStart()"/>
        /// is called. It can also be assigned as an argument to <see cref="PlayFromStart"/>
        /// if preferred.
        /// </remarks>
        public Controller controller
        {
            set
            {
                _controller = value;
            }
        }

        /// <inheritdoc cref="HapticClipPlayer.isLooping" />
        [System.ComponentModel.DefaultValue(false)]
        public bool loop
        {
            get => _loop;
            set
            {

                _loop = value;
                _player.isLooping = _loop;
            }
        }

        /// <summary>
        /// Sets/gets the <c>HapticSource</c>'s amplitude.
        /// </summary>
        ///
        /// <remarks>
        /// See <see cref="HapticClipPlayer.amplitude"/> for details.
        /// </remarks>
        /// <inheritdoc cref="HapticClipPlayer.amplitude" />
        [System.ComponentModel.DefaultValue(1.0)]
        public float amplitude
        {
            get => _amplitude;
            set
            {

                _amplitude = value;
                _player.amplitude = _amplitude;
            }
        }

        /// <summary>
        /// Sets/gets the <c>HapticSource</c>'s frequency shift.
        ///
        /// See <see cref="HapticClipPlayer.frequencyShift"/> for details.
        /// </summary>
        /// <inheritdoc cref="HapticClipPlayer.frequencyShift" />
        [System.ComponentModel.DefaultValue(0.0)]
        public float frequencyShift
        {
            get => _frequencyShift;
            set
            {

                _frequencyShift = value;
                _player.frequencyShift = _frequencyShift;
            }
        }

        /// <summary>
        /// Sets/gets the <c>HapticSource</c>'s playback priority.
        /// </summary>
        ///
        /// <remarks>
        /// See <see cref="HapticClipPlayer.priority"/> for details.
        /// </remarks>
        /// <inheritdoc cref="HapticClipPlayer.priority" />
        [System.ComponentModel.DefaultValue(128)]
        public uint priority
        {
            get => _priority;
            set
            {

                _priority = value;
                _player.priority = _priority;
            }
        }

        private void SyncSerializedFieldsToPlayer()
        {
            if (_player is null)
            {
                return;
            }

            // Send properties to the HapticClipPlayer.
            _player.isLooping = _loop;
            _player.amplitude = _amplitude;
            _player.frequencyShift = _frequencyShift;
            _player.priority = _priority;
        }

        /// <summary>
        /// Serialization callback from the <c>ISerializationCallbackReceiver</c> interface.
        /// Unused for now but declared to fulfill the interface.
        /// </summary>
        public void OnBeforeSerialize()
        {
        }

        /// <summary>
        /// Serialization callback from the <c>ISerializationCallbackReceiver</c> interface.
        /// <c>HapticSource</c> uses it to synchronize the properties of the wrapped
        /// <c>HapticClipPlayer</c> to the serializable fields on this <c>HapticSource</c>.
        /// This occurs whenever a user updates any of the values on the custom inspector or
        /// whenever Unity loads the persisted data.
        /// </summary>
        public void OnAfterDeserialize()
        {
            if (_player != null)
            {
                SyncSerializedFieldsToPlayer();
            }
        }

        /// <summary>
        /// Ensures that the lifetime of the wrapped <c>HapticClipPlayer</c> matches that of
        /// the <c>HapticSource</c>.
        /// </summary>
        protected virtual void OnDestroy()
        {
            _player.Dispose();
        }
    }
}

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

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.Throw
{
    /// <summary>
    /// Transform information used to derive velocities.
    /// </summary>
    [Obsolete]
    public struct TransformSample
    {
        public TransformSample(Vector3 position, Quaternion rotation, float time,
          int frameIndex)
        {
            Position = position;
            Rotation = rotation;
            SampleTime = time;
            FrameIndex = frameIndex;
        }

        public static TransformSample Interpolate(TransformSample start,
          TransformSample fin, float time)
        {
            float alpha = Mathf.Clamp01(Mathf.InverseLerp(start.SampleTime,
              fin.SampleTime, time));

            return new TransformSample(Vector3.Lerp(start.Position, fin.Position, alpha),
              Quaternion.Slerp(start.Rotation, fin.Rotation, alpha),
              time, (int)Mathf.Lerp((float)start.FrameIndex, (float)fin.FrameIndex, alpha));
        }

        public readonly Vector3 Position;
        public readonly Quaternion Rotation;
        public readonly float SampleTime;
        public readonly int FrameIndex;
    }

    /// <summary>
    /// Information related to release velocities such as linear and
    /// angular.
    /// </summary>
    public struct ReleaseVelocityInformation
    {
        public Vector3 LinearVelocity;
        public Vector3 AngularVelocity;
        public Vector3 Origin;
        public bool IsSelectedVelocity;

        public ReleaseVelocityInformation(Vector3 linearVelocity,
            Vector3 angularVelocity,
            Vector3 origin,
            bool isSelectedVelocity = false)
        {
            LinearVelocity = linearVelocity;
            AngularVelocity = angularVelocity;
            Origin = origin;
            IsSelectedVelocity = isSelectedVelocity;
        }
    }

    /// <summary>
    /// Defines a velocity calculation system for throwing mechanics in the Interaction SDK.
    /// This interface provides methods and events for calculating, tracking, and notifying about throw velocities,
    /// particularly useful for implementing realistic throwing behaviors for virtual objects.
    /// </summary>
    /// <remarks>
    /// This interface extends <see cref="IThrowVelocityCalculator"/> and adds additional functionality for:
    /// <list type="bullet">
    /// <item><description>Real-time velocity updates and notifications</description></item>
    /// <item><description>Historical velocity tracking</description></item>
    /// <item><description>Configurable update frequency for velocity calculations</description></item>
    /// </list>
    /// Note: This interface is marked as obsolete. Use <see cref="IThrowVelocityCalculator"/> directly instead.
    /// </remarks>
    [Obsolete("Use " + nameof(IThrowVelocityCalculator) + " directly instead")]
    public interface IVelocityCalculator : IThrowVelocityCalculator
    {
        /// <summary>
        /// Gets the frequency at which the velocity calculations are updated. The frequency is calculated in updates per second.
        /// </summary>
        float UpdateFrequency { get; }

        /// <summary>
        /// Event triggered when the throw velocities are recalculated, providing a list of all current <see cref="ReleaseVelocityInformation"/>.
        /// </summary>
        event Action<List<ReleaseVelocityInformation>> WhenThrowVelocitiesChanged;

        /// <summary>
        /// Event triggered when a new velocity sample becomes available. This is used for real-time velocity updates.
        /// </summary>
        event Action<ReleaseVelocityInformation> WhenNewSampleAvailable;

        /// <summary>
        /// Retrieves the most recently calculated <see cref="ReleaseVelocityInformation"/>.
        /// </summary>
        /// <returns>A read-only list of velocity information from the last calculation.</returns>
        IReadOnlyList<ReleaseVelocityInformation> LastThrowVelocities();

        /// <summary>
        /// Configures the frequency at which velocity calculations are updated.
        /// </summary>
        /// <param name="frequency">The desired update frequency in updates per second.</param>
        void SetUpdateFrequency(float frequency);
    }
}

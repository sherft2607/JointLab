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
using UnityEngine;

namespace Oculus.Interaction.Throw
{
    [Obsolete("Use " + nameof(Grabbable) + " instead")]
    public class RANSACVelocityCalculator : MonoBehaviour,
        IThrowVelocityCalculator, ITimeConsumer
    {
        [SerializeField, Interface(typeof(IPoseInputDevice))]
        private UnityEngine.Object _poseInputDevice;
        public IPoseInputDevice PoseInputDevice { get; private set; }

        private Func<float> _timeProvider = () => Time.time;
        public void SetTimeProvider(Func<float> timeProvider)
        {
            _timeProvider = timeProvider;
        }

        private float _previousPositionId;

        private RANSACOffsettedVelocity _ransac = new RANSACOffsettedVelocity(8, 2);

        protected virtual void Awake()
        {
            PoseInputDevice = _poseInputDevice as IPoseInputDevice;
        }

        protected virtual void Start()
        {
            this.AssertField(PoseInputDevice, nameof(PoseInputDevice));

            _ransac.Initialize();
        }

        private void Update()
        {
            ProcessInput();
        }

        public ReleaseVelocityInformation CalculateThrowVelocity(Transform objectThrown)
        {
            ProcessInput();

            return GetThrowInformation(objectThrown.GetPose());
        }

        private void ProcessInput()
        {
            PoseInputDevice.GetRootPose(out Pose rootPose);

            bool isHighConfidence = PoseInputDevice.IsInputValid
                && PoseInputDevice.IsHighConfidence
                && rootPose.position.sqrMagnitude != _previousPositionId;

            _ransac.Process(rootPose, _timeProvider(), isHighConfidence);
            _previousPositionId = rootPose.position.sqrMagnitude;
        }

        private ReleaseVelocityInformation GetThrowInformation(Pose grabPoint)
        {
            Vector3 position = grabPoint.position;
            PoseInputDevice.GetRootPose(out Pose rootPose);
            Pose offset = PoseUtils.Delta(rootPose, grabPoint);

            _ransac.GetOffsettedVelocities(offset, out Vector3 velocity, out Vector3 torque);

            return new ReleaseVelocityInformation(velocity, torque, position, true);
        }

        #region Inject

        public void InjectAllRANSACVelocityCalculator(IPoseInputDevice poseInputDevice)
        {
            InjectPoseInputDevice(poseInputDevice);
        }

        public void InjectPoseInputDevice(IPoseInputDevice poseInputDevice)
        {
            PoseInputDevice = poseInputDevice;
            _poseInputDevice = poseInputDevice as UnityEngine.Object;
        }

        #endregion

        private class RANSACOffsettedVelocity : RANSACVelocity
        {
            private Pose _offset = Pose.identity;

            [Obsolete("The minHighConfidenceSamples parameter will be ignored. Use the constructor without it")]
            public RANSACOffsettedVelocity(int samplesCount = 10, int samplesDeadZone = 2, int minHighConfidenceSamples = 2)
                : base(samplesCount, samplesDeadZone)
            {
            }

            public RANSACOffsettedVelocity(int samplesCount = 10, int samplesDeadZone = 2)
                : base(samplesCount, samplesDeadZone)
            {
            }

            public void GetOffsettedVelocities(Pose offset, out Vector3 velocity, out Vector3 torque)
            {
                _offset = offset;
                this.GetVelocities(out velocity, out torque);
                _offset = Pose.identity;
            }

            protected override Vector3 PositionOffset(Pose youngerPose, Pose olderPose)
            {
                return PoseUtils.Multiply(youngerPose, _offset).position - PoseUtils.Multiply(olderPose, _offset).position;
            }
        }
    }
}

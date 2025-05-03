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

using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Serialization;

using static Oculus.Interaction.TransformerUtils;

namespace Oculus.Interaction
{
    /// <summary>
    /// A Transformer that translates, rotates and scales the target on a plane.
    /// </summary>
    public class TwoGrabPlaneTransformer : MonoBehaviour, ITransformer
    {
        [SerializeField, Optional]
        private Transform _planeTransform = null;

        [SerializeField, Optional]
        private Vector3 _localPlaneNormal = new Vector3(0, 1, 0);

        [Serializable]
        public class TwoGrabPlaneConstraints
        {
            public FloatConstraint MaxScale;
            public FloatConstraint MinScale;
            public FloatConstraint MaxY;
            public FloatConstraint MinY;
        }

        [SerializeField]
        private TwoGrabPlaneConstraints _constraints;

        public TwoGrabPlaneConstraints Constraints
        {
            get => _constraints;
            set => _constraints = value;
        }

        public struct TwoGrabPlaneState
        {
            public Pose Center;
            public float PlanarDistance;
        }

        private IGrabbable _grabbable;

        public void Initialize(IGrabbable grabbable)
        {
            _grabbable = grabbable;
        }

        private Pose _localToTarget;
        private float _localMagnitudeToTarget;

        private Vector3 WorldPlaneNormal()
        {
            Transform t = _planeTransform != null ? _planeTransform : _grabbable.Transform;
            return t.TransformDirection(_localPlaneNormal).normalized;
        }

        public void BeginTransform()
        {
            var target = _grabbable.Transform;
            var grabA = _grabbable.GrabPoints[0];
            var grabB = _grabbable.GrabPoints[1];
            var planeNormal = WorldPlaneNormal();

            var twoGrabPlaneState = TwoGrabPlane(grabA.position, grabB.position, planeNormal);
            _localToTarget = WorldToLocalPose(twoGrabPlaneState.Center, target.worldToLocalMatrix);
            _localMagnitudeToTarget = WorldToLocalMagnitude(twoGrabPlaneState.PlanarDistance, target.worldToLocalMatrix);
        }

        public void UpdateTransform()
        {
            var target = _grabbable.Transform;
            var grabA = _grabbable.GrabPoints[0];
            var grabB = _grabbable.GrabPoints[1];
            var planeNormal = WorldPlaneNormal();

            var twoGrabPlaneState = TwoGrabPlane(grabA.position, grabB.position, planeNormal);

            float prevDistInWorld = LocalToWorldMagnitude(_localMagnitudeToTarget, target.localToWorldMatrix);
            float scaleDelta = prevDistInWorld != 0 ? twoGrabPlaneState.PlanarDistance / prevDistInWorld : 1f;

            float targetScale = scaleDelta * target.localScale.x;
            if (_constraints.MinScale.Constrain)
            {
                targetScale = Mathf.Max(_constraints.MinScale.Value, targetScale);
            }
            if (_constraints.MaxScale.Constrain)
            {
                targetScale = Mathf.Min(_constraints.MaxScale.Value, targetScale);
            }
            target.localScale = (targetScale / target.localScale.x) * target.localScale;

            Pose result = AlignLocalToWorldPose(target.localToWorldMatrix, _localToTarget, twoGrabPlaneState.Center);
            target.position = result.position;
            target.rotation = result.rotation;

            target.position = ConstrainAlongDirection(
                target.position, target.parent != null ? target.parent.position : Vector3.zero,
                planeNormal, _constraints.MinY, _constraints.MaxY);
        }

        public void EndTransform() { }

        public static TwoGrabPlaneState TwoGrabPlane(Vector3 p0, Vector3 p1, Vector3 planeNormal)
        {
            Vector3 centroid = p0 * 0.5f + p1 * 0.5f;

            Vector3 p0planar = Vector3.ProjectOnPlane(p0, planeNormal);
            Vector3 p1planar = Vector3.ProjectOnPlane(p1, planeNormal);

            Vector3 planarDelta = p1planar - p0planar;
            Quaternion poseDir = Quaternion.LookRotation(planarDelta, planeNormal);

            return new TwoGrabPlaneState()
            {
                Center = new Pose(centroid, poseDir),
                PlanarDistance = planarDelta.magnitude
            };
        }

        #region Inject

        public void InjectOptionalPlaneTransform(Transform planeTransform)
        {
            _planeTransform = planeTransform;
        }

        public void InjectOptionalConstraints(TwoGrabPlaneConstraints constraints)
        {
            _constraints = constraints;
        }

        #endregion
    }
}

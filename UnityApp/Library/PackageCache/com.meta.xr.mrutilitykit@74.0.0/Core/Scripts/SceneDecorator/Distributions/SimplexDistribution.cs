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
using Meta.XR.Util;
using UnityEngine;

namespace Meta.XR.MRUtilityKit.SceneDecorator
{
    /// <summary>
    ///    A distribution that generates points on a plane using simplex noise.
    /// </summary>
    [Serializable]
    [Feature(Feature.Scene)]
    public class SimplexDistribution : SceneDecorator.IDistribution
    {
        /// <summary>
        ///   Configuration for the point sampling.
        /// </summary>
        [Serializable]
        public struct PointSamplingConfig
        {
            public float pointsPerUnitX;
            public float pointsPerUnitY;
            public float noiseOffsetRadius;

            public static PointSamplingConfig DefaultConfig = new PointSamplingConfig()
            {
                pointsPerUnitX = 1.0f,
                pointsPerUnitY = 1.0f,
                noiseOffsetRadius = 0.1f
            };
        }

        [SerializeField]
        public PointSamplingConfig pointSamplingConfig;

        /// <summary>
        /// Generates uniform sampling points with simplex noise applied for a given plane in localspace.
        /// The points will be generated based on the specified resolution within the interior of the plane, with no points on the borders.
        /// If the plane is too small, at least one point will still be generated.
        /// </summary>
        /// <param name="sceneAnchor">The MRUKAnchor representing the plane.</param>
        /// <param name="config">The configuration for point sampling.</param>
        /// <returns>A tuple containing two arrays: the first array contains the sampling points in local space, and the second array contains the normalized sampling points.</returns>
        public static (Vector2[], Vector2[]) GeneratePointsLocal(MRUKAnchor sceneAnchor, PointSamplingConfig config)
        {
            if (!sceneAnchor.PlaneRect.HasValue)
            {
                return (Array.Empty<Vector2>(), Array.Empty<Vector2>());
            }

            var planeSize = sceneAnchor.PlaneRect.Value.size;

            var pointsX = Mathf.Max(Mathf.CeilToInt(config.pointsPerUnitX * planeSize.x), 1);
            var pointsY = Mathf.Max(Mathf.CeilToInt(config.pointsPerUnitY * planeSize.y), 1);

            var stride = new Vector2(1f / (pointsX + 1), 1f / (pointsY + 1));

            var points = new Vector2[pointsX * pointsY];
            var pointsNormalized = new Vector2[pointsX * pointsY];
            for (int iy = 0; iy < pointsY; ++iy)
            {
                for (int ix = 0; ix < pointsX; ++ix)
                {
                    float dx = (ix + 1) * stride.x;
                    float dy = (iy + 1) * stride.y;

                    var noise = SimplexNoise.srdnoise(new(dx, dy), 0);
                    dx += noise.x * config.noiseOffsetRadius;
                    dy += noise.y * config.noiseOffsetRadius;

                    Vector2 point = new(dx * planeSize.x - planeSize.x / 2, dy * planeSize.y - planeSize.y / 2);
                    Vector2 pointNormalized = new(dx, dy);
                    points[ix + iy * pointsX] = point;
                    pointsNormalized[ix + iy * pointsX] = pointNormalized;
                }
            }

            return (points, pointsNormalized);
        }

        /// <summary>
        /// Distributes a scene decoration across a surface defined by an MRUKAnchor using the generated sampling points.
        /// </summary>
        /// <param name="sceneDecorator">The scene decorator used to generate the decoration.</param>
        /// <param name="sceneAnchor">The MRUKAnchor that defines the surface on which to distribute the decoration.</param>
        /// <param name="sceneDecoration">The scene decoration to be distributed.</param>
        public void Distribute(SceneDecorator sceneDecorator, MRUKAnchor sceneAnchor, SceneDecoration sceneDecoration)
        {
            var positions = GeneratePointsLocal(sceneAnchor, pointSamplingConfig);
            for (int i = 0; i < positions.Item1.Length - 1; i++)
            {
                var pos = positions.Item1[i];
                var posNormalized = positions.Item2[i];
                sceneDecorator.GenerateOn(pos, posNormalized, sceneAnchor, sceneDecoration);
            }
        }
    }
}

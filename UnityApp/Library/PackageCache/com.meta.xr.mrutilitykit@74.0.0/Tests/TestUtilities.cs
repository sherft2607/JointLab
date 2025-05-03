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
using Newtonsoft.Json;
using NUnit.Framework;
using UnityEngine;

namespace Meta.XR.MRUtilityKit.Tests
{
    public static class TestUtilities
    {
        // Use the triangulated area as a proxy to ensure the triangulation worked as expected
        internal static float CalculateTriangulatedArea(Vector2[] vertices, int[] indices)
        {
            float area = 0f;
            for (int i = 0; i < indices.Length; i += 3)
            {
                var p1 = vertices[indices[i]];
                var p2 = vertices[indices[i + 1]];
                var p3 = vertices[indices[i + 2]];
                var triangleArea = CalculateTriangleArea(p1, p2, p3);
                Assert.GreaterOrEqual(triangleArea, 0f);
                area += triangleArea;
            }

            return area;
        }

        private static float CalculateTriangleArea(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            return (p1.x * (p2.y - p3.y) + p2.x * (p3.y - p1.y) + p3.x * (p1.y - p2.y)) / 2.0f;
        }

        /// <summary>
        /// Calculates the total area of a triangulated mesh using the vertices and indices.
        /// </summary>
        /// <param name="vertices">An array of <see cref="Vector3"/> representing the vertices of the mesh.</param>
        /// <param name="indices">An array of integers representing the indices of the mesh triangles.</param>
        /// <returns>The total area of the mesh.</returns>
        internal static float CalculateTriangulatedArea(Vector3[] vertices, int[] indices)
        {
            float area = 0f;
            for (int i = 0; i < indices.Length; i += 3)
            {
                var p1 = vertices[indices[i]];
                var p2 = vertices[indices[i + 1]];
                var p3 = vertices[indices[i + 2]];
                var triangleArea = CalculateTriangleArea(p1, p2, p3);
                Assert.GreaterOrEqual(triangleArea, 0f);
                area += triangleArea;
            }
            return area;
        }
        /// <summary>
        /// Calculates the area of a triangle given three vertices in 3D space.
        /// </summary>
        /// <param name="p1">The first vertex of the triangle.</param>
        /// <param name="p2">The second vertex of the triangle.</param>
        /// <param name="p3">The third vertex of the triangle.</param>
        /// <returns>The area of the triangle.</returns>
        private static float CalculateTriangleArea(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            // Calculate the vectors for two sides of the triangle
            Vector3 u = p2 - p1;
            Vector3 v = p3 - p1;
            // Calculate the cross product of the two vectors
            Vector3 crossProduct = Vector3.Cross(u, v);
            // The area of the triangle is half the magnitude of the cross product
            float area = crossProduct.magnitude * 0.5f;
            return area;
        }
    }

    #region COMPARERS

    internal class Vector3ArrayComparer : IEqualityComparer<Vector3[]>
    {
        public bool Equals(Vector3[] x, Vector3[] y)
        {
            if (x == null || y == null)
            {
                return x == y;
            }

            if (x.Length != y.Length)
            {
                return false;
            }

            var setX = new HashSet<Vector3>(x);

            return setX.SetEquals(y);
        }

        public int GetHashCode(Vector3[] obj)
        {
            return 0;
        }
    }

    internal class IntArrayComparer : IEqualityComparer<int[]>
    {
        public bool Equals(int[] x, int[] y)
        {
            if (x == null || y == null)
            {
                return x == y;
            }

            if (x.Length != y.Length)
            {
                return false;
            }

            var setX = new HashSet<int>(x);
            return setX.SetEquals(y);
        }

        public int GetHashCode(int[] obj)
        {
            return 0;
        }
    }

    #endregion COMPARERS

    #region JSON_CONVERTERS

    internal class IntArrayConverter : JsonConverter<int[]>
    {
        public override void WriteJson(JsonWriter writer, int[] value, JsonSerializer serializer)
        {
            var array = value;
            writer.WriteStartArray();
            foreach (var item in array)
            {
                writer.WriteValue(item);
            }

            writer.WriteEndArray();
        }

        public override int[] ReadJson(JsonReader reader, Type objectType, int[] existingValue, bool hasExistingValue,
            JsonSerializer serial)
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
                var list = new List<int>();
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.EndArray)
                    {
                        return list.ToArray();
                    }

                    list.Add((int)(long)reader.Value);
                }
            }

            throw new JsonReaderException("Expected start of array.");
        }
    }

    public class Vector3Converter : JsonConverter<Vector3>
    {
        public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
        {
            writer.WriteStartArray();
            // Disable indentation to make it more compact
            var prevFormatting = writer.Formatting;
            writer.Formatting = Formatting.None;
            writer.WriteValue(value.x);
            writer.WriteValue(value.y);
            writer.WriteValue(value.z);
            writer.WriteEndArray();
            writer.Formatting = prevFormatting;
        }

        public override Vector3 ReadJson(JsonReader reader, Type objectType, Vector3 existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            Vector3 result = new()
            {
                x = (float)reader.ReadAsDouble(),
                y = (float)reader.ReadAsDouble(),
                z = (float)reader.ReadAsDouble()
            };
            reader.Read();
            if (reader.TokenType != JsonToken.EndArray)
            {
                throw new Exception("Expected end of array");
            }

            return result;
        }
    }

    internal class Vector3ArrayConverter : JsonConverter<Vector3[]>
    {
        public override void WriteJson(JsonWriter writer, Vector3[] value, JsonSerializer serializer)
        {
            var array = value;
            writer.WriteStartArray();
            foreach (var item in array)
            {
                var prevFormatting = writer.Formatting;
                writer.WriteStartArray();
                writer.Formatting = Formatting.None;
                writer.WriteValue(item.x);
                writer.WriteValue(item.y);
                writer.WriteValue(item.z);
                writer.WriteEndArray();
                writer.Formatting = prevFormatting;
            }

            writer.WriteEndArray();
        }

        public override Vector3[] ReadJson(JsonReader reader, Type objectType, Vector3[] existingValue,
            bool hasExistingValue, JsonSerializer serial)
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
                var list = new List<Vector3>();
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.EndArray)
                    {
                        return list.ToArray();
                    }

                    Vector3 result = new()
                    {
                        x = (float)reader.ReadAsDouble(),
                        y = (float)reader.ReadAsDouble(),
                        z = (float)reader.ReadAsDouble()
                    };
                    reader.Read();
                    if (reader.TokenType != JsonToken.EndArray)
                    {
                        throw new Exception("Expected end of array");
                    }

                    list.Add(result);
                }
            }

            throw new JsonReaderException("Expected start of array.");
        }
    }

    internal class SceneNavigationConverter : JsonConverter<SceneNavigation>
    {
        public override void WriteJson(JsonWriter writer, SceneNavigation value, JsonSerializer serializer)
        {
            var sceneNavigation = value;
            writer.WriteStartObject();
            writer.WritePropertyName("BuildOnSceneLoad");
            serializer.Serialize(writer, sceneNavigation.BuildOnSceneLoaded);
            writer.WritePropertyName("CollectGeometry");
            writer.WriteValue(sceneNavigation.CollectGeometry);
            writer.WritePropertyName("CollectObjects");
            serializer.Serialize(writer, sceneNavigation.CollectObjects);
            writer.WritePropertyName("AgentRadius");
            writer.WriteValue(sceneNavigation.AgentRadius);
            writer.WritePropertyName("AgentHeight");
            serializer.Serialize(writer, sceneNavigation.AgentHeight);
            writer.WritePropertyName("AgentClimb");
            writer.WriteValue(sceneNavigation.AgentClimb);
            writer.WritePropertyName("AgentMaxSlope");
            serializer.Serialize(writer, sceneNavigation.AgentMaxSlope);
            writer.WritePropertyName("NavigableSurfaces");
            writer.WriteValue(sceneNavigation.NavigableSurfaces);
            writer.WritePropertyName("SceneObstacles");
            serializer.Serialize(writer, sceneNavigation.SceneObstacles);
            writer.WritePropertyName("Layers");
            writer.WriteValue(sceneNavigation.Layers);
            writer.WritePropertyName("AgentIndex");
            writer.WriteValue(sceneNavigation.AgentIndex);
            writer.WritePropertyName("UseSceneData");
            writer.WriteValue(sceneNavigation.UseSceneData);
            writer.WritePropertyName("CustomAgent");
            writer.WriteValue(sceneNavigation.CustomAgent);
            writer.WritePropertyName("OverrideVoxelSize");
            writer.WriteValue(sceneNavigation.OverrideVoxelSize);
            writer.WritePropertyName("VoxelSize");
            writer.WriteValue(sceneNavigation.VoxelSize);
            writer.WritePropertyName("OverrideTileSize");
            writer.WriteValue(sceneNavigation.OverrideTileSize);
            writer.WritePropertyName("TileSize");
            writer.WriteValue(sceneNavigation.TileSize);
            writer.WriteEndObject();
        }

        public override SceneNavigation ReadJson(JsonReader reader, Type objectType,
            SceneNavigation existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            var sceneNavigation = new SceneNavigation();
            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.PropertyName)
                {
                    if (reader.Value != null)
                    {
                        var propertyName = reader.Value.ToString();
                        switch (propertyName)
                        {
                            case "BuildOnSceneLoaded":
                                reader.Read();
                                var BuildOnSceneLoaded = reader.Value.ToString();
                                sceneNavigation.BuildOnSceneLoaded = Enum.Parse<MRUK.RoomFilter>(BuildOnSceneLoaded);
                                break;
                            case "CollectGeometry":
                                reader.Read();
                                var CollectGeometry = reader.Value.ToString();
                                sceneNavigation.BuildOnSceneLoaded = Enum.Parse<MRUK.RoomFilter>(CollectGeometry);
                                break;
                            case "CollectObjects":
                                reader.Read();
                                var CollectObjects = reader.Value.ToString();
                                sceneNavigation.BuildOnSceneLoaded = Enum.Parse<MRUK.RoomFilter>(CollectObjects);
                                break;
                            case "AgentRadius":
                                reader.Read();
                                sceneNavigation.AgentRadius = serializer.Deserialize<float>(reader);
                                break;
                            case "AgentHeight":
                                reader.Read();
                                sceneNavigation.AgentHeight = serializer.Deserialize<float>(reader);
                                break;
                            case "AgentClimb":
                                reader.Read();
                                sceneNavigation.AgentClimb = serializer.Deserialize<float>(reader);
                                break;
                            case "AgentMaxSlope":
                                reader.Read();
                                sceneNavigation.AgentMaxSlope = serializer.Deserialize<float>(reader);
                                break;
                            case "NavigableSurfaces":
                                reader.Read();
                                sceneNavigation.NavigableSurfaces =
                                    serializer.Deserialize<MRUKAnchor.SceneLabels>(reader);
                                break;
                            case "SceneObstacles":
                                reader.Read();
                                sceneNavigation.SceneObstacles = serializer.Deserialize<MRUKAnchor.SceneLabels>(reader);
                                break;
                            case "Layers":
                                reader.Read();
                                sceneNavigation.Layers.value = serializer.Deserialize<int>(reader);
                                break;
                            case "AgentIndex":
                                reader.Read();
                                sceneNavigation.AgentIndex = serializer.Deserialize<int>(reader);
                                break;
                            case "UseSceneData":
                                reader.Read();
                                sceneNavigation.UseSceneData = serializer.Deserialize<bool>(reader);
                                break;
                            case "CustomAgent":
                                reader.Read();
                                sceneNavigation.CustomAgent = serializer.Deserialize<bool>(reader);
                                break;
                            case "OverrideVoxelSize":
                                reader.Read();
                                sceneNavigation.OverrideVoxelSize = serializer.Deserialize<bool>(reader);
                                break;
                            case "VoxelSize":
                                reader.Read();
                                sceneNavigation.VoxelSize = serializer.Deserialize<float>(reader);
                                break;
                            case "OverrideTileSize":
                                reader.Read();
                                sceneNavigation.OverrideTileSize = serializer.Deserialize<bool>(reader);
                                break;
                            case "TileSize":
                                reader.Read();
                                sceneNavigation.TileSize = serializer.Deserialize<int>(reader);
                                break;
                        }
                    }
                }
                else if (reader.TokenType == JsonToken.EndObject)
                {
                    break;
                }
            }

            return sceneNavigation;
        }
    }

    internal class GridSliceResizerConverter : JsonConverter<GridSliceResizer>
    {
        public override void WriteJson(JsonWriter writer, GridSliceResizer value, JsonSerializer serializer)
        {
            var gridSliceResizer = value;
            writer.WriteStartObject();
            writer.WritePropertyName("PivotOffset");
            serializer.Serialize(writer, gridSliceResizer.PivotOffset);
            writer.WritePropertyName("ScalingX");
            writer.WriteValue(gridSliceResizer.ScalingX.ToString());
            writer.WritePropertyName("BorderXNegative");
            serializer.Serialize(writer, gridSliceResizer.BorderXNegative);
            writer.WritePropertyName("BorderXPositive");
            serializer.Serialize(writer, gridSliceResizer.BorderXPositive);
            writer.WritePropertyName("ScalingY");
            writer.WriteValue(gridSliceResizer.ScalingY.ToString());
            writer.WritePropertyName("BorderYNegative");
            serializer.Serialize(writer, gridSliceResizer.BorderYNegative);
            writer.WritePropertyName("BorderYPositive");
            serializer.Serialize(writer, gridSliceResizer.BorderYPositive);
            writer.WritePropertyName("ScalingZ");
            writer.WriteValue(gridSliceResizer.ScalingZ.ToString());
            writer.WritePropertyName("BorderZNegative");
            serializer.Serialize(writer, gridSliceResizer.BorderZNegative);
            writer.WritePropertyName("BorderZPositive");
            serializer.Serialize(writer, gridSliceResizer.BorderZPositive);
            writer.WritePropertyName("StretchCenter");
            writer.WriteValue((int)gridSliceResizer.StretchCenter);
            writer.WriteEndObject();
        }

        public override GridSliceResizer ReadJson(JsonReader reader, Type objectType, GridSliceResizer existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            var gridSliceResizer = new GridSliceResizer();
            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.PropertyName)
                {
                    var propertyName = reader.Value.ToString();
                    switch (propertyName)
                    {
                        case "PivotOffset":
                            reader.Read();
                            gridSliceResizer.PivotOffset = serializer.Deserialize<Vector3>(reader);
                            break;
                        case "ScalingX":
                            reader.Read();
                            var scalingXString = reader.Value.ToString();
                            gridSliceResizer.ScalingX = Enum.Parse<GridSliceResizer.Method>(scalingXString);
                            break;
                        case "BorderXNegative":
                            reader.Read();
                            gridSliceResizer.BorderXNegative = serializer.Deserialize<float>(reader);
                            break;
                        case "BorderXPositive":
                            reader.Read();
                            gridSliceResizer.BorderXPositive = serializer.Deserialize<float>(reader);
                            break;
                        case "ScalingY":
                            reader.Read();
                            var scalingYString = reader.Value.ToString();
                            gridSliceResizer.ScalingY = Enum.Parse<GridSliceResizer.Method>(scalingYString);
                            break;
                        case "BorderYNegative":
                            reader.Read();
                            gridSliceResizer.BorderYNegative = serializer.Deserialize<float>(reader);
                            break;
                        case "BorderYPositive":
                            reader.Read();
                            gridSliceResizer.BorderYPositive = serializer.Deserialize<float>(reader);
                            break;
                        case "ScalingZ":
                            reader.Read();
                            var scalingZString = reader.Value.ToString();
                            gridSliceResizer.ScalingZ = Enum.Parse<GridSliceResizer.Method>(scalingZString);
                            break;
                        case "BorderZNegative":
                            reader.Read();
                            gridSliceResizer.BorderZNegative = serializer.Deserialize<float>(reader);
                            break;
                        case "BorderZPositive":
                            reader.Read();
                            gridSliceResizer.BorderZPositive = serializer.Deserialize<float>(reader);
                            break;
                        case "StretchCenter":
                            reader.Read();
                            var stretchCenter = reader.Value.ToString();
                            gridSliceResizer.StretchCenter =
                                Enum.Parse<GridSliceResizer.StretchCenterAxis>(stretchCenter);
                            break;
                    }
                }
                else if (reader.TokenType == JsonToken.EndObject)
                {
                    break;
                }
            }

            return gridSliceResizer;
        }
    }

    #endregion JSON_CONVERTERS
}

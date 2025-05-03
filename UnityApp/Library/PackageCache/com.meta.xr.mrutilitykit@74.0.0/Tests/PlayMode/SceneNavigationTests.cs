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


using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using NUnit.Framework;
using UnityEngine.AI;
using UnityEngine.TestTools;

namespace Meta.XR.MRUtilityKit.Tests
{
    public class SceneNavigationTests : MRUKTestBase
    {
        /// <summary>
        /// A struct that can serialized to json to quickly load/save preconfigured SceneNavigation and it's results.
        /// </summary>
        internal struct SceneNavigationTestConfig
        {
            [JsonProperty("SceneNavigation")] public SceneNavigation sceneNavigation;
        }

        private readonly JsonSerializerSettings _serializerSettings = new()
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.None,
            Converters = new List<JsonConverter>
            {
                new IntArrayConverter(),
                new Vector3ArrayConverter(),
                new Vector3Converter(),
                new SceneNavigationConverter()
            }
        };

        private SceneNavigation _sceneNav;
        private GameObject _sceneNavGameObject;
        private EffectMesh _effectMesh;

        private const float expectedCustomAgent = 24.29f;
        private const float expectedDefaultAgent = 17.09f;
        private const float expectedWithin = 0.01f;
        private const float expectedSceneDataCustomAgentGlobalMesh = 5.74f;
        private const float expectedBuiltInCustomAgentGlobalMesh = 5.55f;


        private const string _sceneDataCustomAgent =
            @"{""SceneNavigation"":{""BuildOnSceneLoad"":0,""CollectGeometry"":1,""CollectObjects"":2,""AgentRadius"":0.05,""AgentHeight"":2.0,""AgentClimb"":0.2,""AgentMaxSlope"":14.0,""NavigableSurfaces"":921,""SceneObstacles"":73624,""Layers"":-1,""AgentIndex"":0,""connectRoomsInNavMesh"":true,""UseSceneData"":false,""CustomAgent"":true,""OverrideVoxelSize"":false,""VoxelSize"":0.05,""OverrideTileSize"":false,""TileSize"":5}}";
        private const string _sceneDataDefaultAgent =
            @"{""SceneNavigation"":{""BuildOnSceneLoad"":0,""CollectGeometry"":1,""CollectObjects"":2,""AgentRadius"":0.05,""AgentHeight"":2.0,""AgentClimb"":0.2,""AgentMaxSlope"":14.0,""NavigableSurfaces"":921,""SceneObstacles"":73624,""Layers"":-1,""AgentIndex"":0,""connectRoomsInNavMesh"":true,""UseSceneData"":false,""CustomAgent"":false,""OverrideVoxelSize"":false,""VoxelSize"":0.05,""OverrideTileSize"":false,""TileSize"":5}}";
        private const string _builtInDefaultAgent =
            @"{""SceneNavigation"":{""BuildOnSceneLoad"":0,""CollectGeometry"":1,""CollectObjects"":2,""AgentRadius"":0.05,""AgentHeight"":2.0,""AgentClimb"":0.2,""AgentMaxSlope"":14.0,""NavigableSurfaces"":921,""SceneObstacles"":73624,""Layers"":-1,""AgentIndex"":0,""connectRoomsInNavMesh"":true,""UseSceneData"":false,""CustomAgent"":false,""OverrideVoxelSize"":false,""VoxelSize"":0.05,""OverrideTileSize"":false,""TileSize"":5}}";
        private const string _builtInCustomAgent =
            @"{""SceneNavigation"":{""BuildOnSceneLoad"":0,""CollectGeometry"":1,""CollectObjects"":2,""AgentRadius"":0.05,""AgentHeight"":2.0,""AgentClimb"":0.2,""AgentMaxSlope"":14.0,""NavigableSurfaces"":921,""SceneObstacles"":73624,""Layers"":-1,""AgentIndex"":0,""connectRoomsInNavMesh"":true,""UseSceneData"":false,""CustomAgent"":true,""OverrideVoxelSize"":false,""VoxelSize"":0.05,""OverrideTileSize"":false,""TileSize"":5}}";
        private const string _sceneDataCustomAgentGlobalMesh =
            @"{""SceneNavigation"":{""BuildOnSceneLoad"":0,""CollectGeometry"":1,""CollectObjects"":2,""AgentRadius"":0.05,""AgentHeight"":2.0,""AgentClimb"":0.2,""AgentMaxSlope"":14.0,""NavigableSurfaces"":16384,""SceneObstacles"":0,""Layers"":-1,""AgentIndex"":0,""connectRoomsInNavMesh"":true,""UseSceneData"":true,""CustomAgent"":true,""OverrideVoxelSize"":false,""VoxelSize"":0.05,""OverrideTileSize"":false,""TileSize"":5}}";

        private const string _builtInCustomAgentGlobalMesh =
            @"{""SceneNavigation"":{""BuildOnSceneLoad"":0,""CollectGeometry"":1,""CollectObjects"":2,""AgentRadius"":0.05,""AgentHeight"":2.0,""AgentClimb"":0.2,""AgentMaxSlope"":14.0,""NavigableSurfaces"":921,""SceneObstacles"":73624,""Layers"":-1,""AgentIndex"":0,""connectRoomsInNavMesh"":true,""UseSceneData"":false,""CustomAgent"":true,""OverrideVoxelSize"":false,""VoxelSize"":0.05,""OverrideTileSize"":false,""TileSize"":5}}";


        /// <summary>
        /// Utility method to quickly export a SceneNavigationTestConfig from a scene.
        /// Set up the SceneNavigationTests scene and serialize the scenarion that needs to be tested.
        /// </summary>
        /// <param name="jsonFileName">The name given to the exported json file.</param>
        private void SerializeSceneNavigationTestConfig(string jsonFileName)
        {
            var sceneNavigationTestConfig = new SceneNavigationTestConfig()
            {
                sceneNavigation = _sceneNav
            };
            var json = JsonConvert.SerializeObject(sceneNavigationTestConfig, _serializerSettings);
            using var writer =
                new StreamWriter(Path.Combine(Application.persistentDataPath, jsonFileName + ".json"));
            {
                writer.WriteLine(json);
            }
        }

        /// <summary>
        /// Sets up the SceneNavigation instance so that it matches an exported conviguration
        /// </summary>
        /// <param name="sceneNavigationTestConfigJson">The serialized SceneNavigationTestConfig. </param>
        /// <returns>The resized mesh to verify.</returns>
        private void SetUpSceneNavigationTest(string sceneNavigationTestConfigJson)
        {
            var sceneNavMeshConfig =
                JsonConvert.DeserializeObject<SceneNavigationTestConfig>(sceneNavigationTestConfigJson,
                    _serializerSettings);

            _sceneNav.BuildOnSceneLoaded = sceneNavMeshConfig.sceneNavigation.BuildOnSceneLoaded;
            _sceneNav.CollectGeometry = sceneNavMeshConfig.sceneNavigation.CollectGeometry;
            _sceneNav.CollectObjects = sceneNavMeshConfig.sceneNavigation.CollectObjects;
            _sceneNav.AgentRadius = sceneNavMeshConfig.sceneNavigation.AgentRadius;
            _sceneNav.AgentHeight = sceneNavMeshConfig.sceneNavigation.AgentHeight;
            _sceneNav.AgentClimb = sceneNavMeshConfig.sceneNavigation.AgentClimb;
            _sceneNav.AgentMaxSlope = sceneNavMeshConfig.sceneNavigation.AgentMaxSlope;
            _sceneNav.NavigableSurfaces = sceneNavMeshConfig.sceneNavigation.NavigableSurfaces;
            _sceneNav.SceneObstacles = sceneNavMeshConfig.sceneNavigation.SceneObstacles;
            _sceneNav.AgentIndex = sceneNavMeshConfig.sceneNavigation.AgentIndex;
            _sceneNav.UseSceneData = sceneNavMeshConfig.sceneNavigation.UseSceneData;
            _sceneNav.CustomAgent = sceneNavMeshConfig.sceneNavigation.CustomAgent;
            _sceneNav.OverrideVoxelSize = sceneNavMeshConfig.sceneNavigation.OverrideVoxelSize;
            _sceneNav.VoxelSize = sceneNavMeshConfig.sceneNavigation.VoxelSize;
            _sceneNav.OverrideTileSize = sceneNavMeshConfig.sceneNavigation.OverrideTileSize;
            _sceneNav.TileSize = sceneNavMeshConfig.sceneNavigation.TileSize;
        }

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            yield return LoadScene("Packages/com.meta.xr.mrutilitykit/Tests/SceneNavigationTests.unity", false);
            _sceneNav = FindObjectOfType<SceneNavigation>();
            _effectMesh = FindObjectOfType<EffectMesh>();
        }


        [UnityTearDown]
        public IEnumerator TearDown()
        {
            yield return UnloadScene();
        }


        [UnityTest]
        public IEnumerator NavMeshBuild_BuiltIn_CustomAgent()
        {
            _effectMesh.DestroyMesh();
            _sceneNav.RemoveNavMeshData();
            MRUK.Instance.LoadSceneFromPrefab(MRUK.Instance.SceneSettings.RoomPrefabs[0]);
            yield return null;
            SetUpSceneNavigationTest(_builtInCustomAgent);
            _sceneNav.BuildSceneNavMesh();
            yield return null;
            var triangulation = NavMesh.CalculateTriangulation();
            yield return null;
            Assert.IsTrue(triangulation.vertices.Length > 0, "NavMesh should have vertices.");
            var triangulatedArea =
              TestUtilities.CalculateTriangulatedArea(triangulation.vertices, triangulation.indices);
            Assert.That(expectedCustomAgent, Is.EqualTo(triangulatedArea).Within(expectedWithin),
              "Expected triangulated area should be be equal to the actual NavMesh triangulated area within a tolerance of 0.001");
        }

        [UnityTest]
        public IEnumerator NavMeshBuild_BuiltIn_DefaultAgent()
        {
            _effectMesh.DestroyMesh();
            _sceneNav.RemoveNavMeshData();
            MRUK.Instance.LoadSceneFromPrefab(MRUK.Instance.SceneSettings.RoomPrefabs[0]);
            yield return null;
            SetUpSceneNavigationTest(_builtInDefaultAgent);
            _sceneNav.BuildSceneNavMesh();
            yield return null;
            var triangulation = NavMesh.CalculateTriangulation();
            yield return null;
            Assert.IsTrue(NavMesh.CalculateTriangulation().vertices.Length > 0, "NavMesh should have vertices.");
            var triangulatedArea =
              TestUtilities.CalculateTriangulatedArea(triangulation.vertices, triangulation.indices);
            Assert.That(expectedDefaultAgent, Is.EqualTo(triangulatedArea).Within(expectedWithin),
              "Expected triangulated area should be be equal to the actual NavMesh triangulated area within a tolerance of 0.001");
        }

        [UnityTest]
        public IEnumerator NavMeshBuild_UseSceneData_CustomAgent()
        {
            _effectMesh.DestroyMesh();
            _sceneNav.RemoveNavMeshData();
            MRUK.Instance.LoadSceneFromPrefab(MRUK.Instance.SceneSettings.RoomPrefabs[0]);
            yield return null;
            SetUpSceneNavigationTest(_sceneDataCustomAgent);
            _sceneNav.BuildSceneNavMesh();
            yield return null;
            var triangulation = NavMesh.CalculateTriangulation();
            Assert.IsTrue(triangulation.vertices.Length > 0, "NavMesh should have vertices.");
            var triangulatedArea =
              TestUtilities.CalculateTriangulatedArea(triangulation.vertices, triangulation.indices);
            Assert.That(expectedCustomAgent, Is.EqualTo(triangulatedArea).Within(expectedWithin),
              "Expected triangulated area should be be equal to the actual NavMesh triangulated area within a tolerance of 0.001");
        }

        [UnityTest]
        public IEnumerator NavMeshBuild_UseSceneData_DefaultAgent()
        {
            _effectMesh.DestroyMesh();
            _sceneNav.RemoveNavMeshData();
            MRUK.Instance.LoadSceneFromPrefab(MRUK.Instance.SceneSettings.RoomPrefabs[0]);
            yield return null;
            SetUpSceneNavigationTest(_sceneDataDefaultAgent);
            _sceneNav.BuildSceneNavMesh();
            yield return null;
            var triangulation = NavMesh.CalculateTriangulation();
            Assert.IsTrue(NavMesh.CalculateTriangulation().vertices.Length > 0, "NavMesh should have vertices.");
            var triangulatedArea =
              TestUtilities.CalculateTriangulatedArea(triangulation.vertices, triangulation.indices);
            Assert.That(expectedDefaultAgent, Is.EqualTo(triangulatedArea).Within(expectedWithin),
              "Expected triangulated area should be be equal to the actual NavMesh triangulated area within a tolerance of 0.001");
        }

        [UnityTest]
        public IEnumerator NavMeshBuild_UseSceneData_CustomAgent_GlobalMesh()
        {
            _effectMesh.Labels = MRUKAnchor.SceneLabels.GLOBAL_MESH;
            _effectMesh.DestroyMesh();
            _sceneNav.RemoveNavMeshData();
            MRUK.Instance.LoadSceneFromJsonString(MRUK.Instance.SceneSettings.SceneJsons[0].ToString());
            yield return null;
            SetUpSceneNavigationTest(_sceneDataCustomAgentGlobalMesh);
            _sceneNav.BuildSceneNavMesh();
            yield return null;
            var triangulation = NavMesh.CalculateTriangulation();
            Assert.IsTrue(triangulation.vertices.Length > 0, "NavMesh should have vertices.");
            var triangulatedArea =
              TestUtilities.CalculateTriangulatedArea(triangulation.vertices, triangulation.indices);
            Assert.That(expectedSceneDataCustomAgentGlobalMesh, Is.EqualTo(triangulatedArea).Within(expectedWithin),
              "Expected triangulated area should be be equal to the actual NavMesh triangulated area within a tolerance of 0.001");
        }


        [UnityTest]
        public IEnumerator NavMeshBuild_BuiltIn_CustomAgent_GlobalMesh()
        {
            _effectMesh.Labels = MRUKAnchor.SceneLabels.GLOBAL_MESH;
            _effectMesh.DestroyMesh();
            _sceneNav.RemoveNavMeshData();
            MRUK.Instance.LoadSceneFromJsonString(MRUK.Instance.SceneSettings.SceneJsons[0].ToString());
            yield return null;
            SetUpSceneNavigationTest(_builtInCustomAgentGlobalMesh);
            _sceneNav.BuildSceneNavMesh();
            yield return null;
            var triangulation = NavMesh.CalculateTriangulation();
            Assert.IsTrue(NavMesh.CalculateTriangulation().vertices.Length > 0, "NavMesh should have vertices.");
            var triangulatedArea =
              TestUtilities.CalculateTriangulatedArea(triangulation.vertices, triangulation.indices);
            Assert.That(expectedBuiltInCustomAgentGlobalMesh, Is.EqualTo(triangulatedArea).Within(expectedWithin),
              "Expected triangulated area should be be equal to the actual NavMesh triangulated area within a tolerance of 0.001");
        }
    }
}

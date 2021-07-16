/*
© Siemens AG, 2018
Author: Suzannah Smith (suzannah.smith@siemens.com)
Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at
<http://www.apache.org/licenses/LICENSE-2.0>.
Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using UnityEngine;
using UnityMeshImporter;

namespace Unity.Robotics.UrdfImporter
{
    public class UrdfGeometryVisual : UrdfGeometry
    {
        public static void Create(Transform parent, GeometryTypes geometryType, Link.Geometry geometry = null)
        {
            GameObject geometryGameObject = null;

            switch (geometryType)
            {
                case GeometryTypes.Box:
                    geometryGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    geometryGameObject.transform.DestroyImmediateIfExists<BoxCollider>();
                    break;
                case GeometryTypes.Cylinder:
                    geometryGameObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    geometryGameObject.transform.DestroyImmediateIfExists<CapsuleCollider>();
                    break;
                case GeometryTypes.Sphere:
                    geometryGameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    geometryGameObject.transform.DestroyImmediateIfExists<SphereCollider>();
                    break;
                case GeometryTypes.Mesh:
                        if (geometry != null)
                        {
                            geometryGameObject = CreateMeshVisual(geometry.mesh);
                        }
                        //else, let user add their own mesh gameObject
                    break;
            }

            if (geometryGameObject != null)
            {
                geometryGameObject.transform.SetParentAndAlign(parent);
                if (geometry != null)
                {
                    SetScale(parent, geometry, geometryType);
                }
            }
        }

        private static GameObject CreateMeshVisual(Link.Geometry.Mesh mesh)
        {
#if UNITY_EDITOR
            if (!RuntimeUrdf.IsRuntimeMode())
            {
                GameObject meshObject = LocateAssetHandler.FindUrdfAsset<GameObject>(mesh.filename);
                return meshObject == null ? null : (GameObject)RuntimeUrdf.PrefabUtility_InstantiatePrefab(meshObject);
            }
#endif
            return CreateMeshVisualRuntime(mesh);
        }

        private static GameObject CreateMeshVisualRuntime(Link.Geometry.Mesh mesh)
        {
            GameObject meshObject = null;
            if (!string.IsNullOrEmpty(mesh.filename))
            {
                try 
                {
                    string meshFilePath = UrdfAssetPathHandler.GetRelativeAssetPathFromUrdfPath(mesh.filename, false);
                    if (meshFilePath.ToLower().EndsWith(".stl"))
                    {
                        meshObject = StlAssetPostProcessor.CreateStlGameObjectRuntime(meshFilePath);
                    }
                    else if (meshFilePath.ToLower().EndsWith(".dae"))
                    {
                        float globalScale = ColladaAssetPostProcessor.ReadGlobalScale(meshFilePath);
                        meshObject = MeshImporter.Load(meshFilePath, globalScale, globalScale, globalScale);
                        if (meshObject != null) 
                        {
                            ColladaAssetPostProcessor.ApplyColladaOrientation(meshObject, meshFilePath);
                        }
                    }
                    else if (meshFilePath.ToLower().EndsWith(".obj"))
                    {
                        meshObject = MeshImporter.Load(meshFilePath);
                    }
                }
                catch (Exception ex) 
                {
                    Debug.LogAssertion(ex);
                }
                
                if (meshObject == null) 
                {
                    Debug.LogError("Unable to load visual mesh: " + mesh.filename);
                }
            }
            return meshObject;
        }
    }
}

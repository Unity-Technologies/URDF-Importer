/*
© Siemens AG, 2017-2019
Author: Dr. Martin Bischoff (martin.bischoff@siemens.com)
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

using UnityEngine;
using System.Linq;
using System.IO;

namespace Unity.Robotics.UrdfImporter
{
    /// <summary>
    /// Utility functions for processing STL mesh files.
    /// Note that no post processing is done on STL files anymore when they are added to the project
    /// As such StlAssetPostProcessor no longer drives from AssetPostProcessor and the OnPostprocessAllAssets()
    /// function is removed. 
    /// </summary>
    public class StlAssetPostProcessor
    {
        private static Material s_DefaultDiffuse = null;

        public static void PostprocessStlFile(string stlFile)
        {
            var stlFileLowercase = stlFile.ToLower();
            if (stlFileLowercase.StartsWith("assets"))
            {
                Debug.Log($"Detected an stl file at {stlFile} - creating a mesh prefab.");
                CreateStlPrefab(stlFile);
            }
            else if (stlFileLowercase.StartsWith("packages"))
            {
                Debug.Log($"Found an stl file at {stlFile} - " + 
                          "skipping post-processing because it's a Package asset");
            }
            else
            {
                Debug.LogWarning($"Found an stl file at {stlFile} - " + 
                                 "skipping post-processing because we don't know how to handle an asset in this location.");
            }
        }

        private static void CreateStlPrefab(string stlFile)
        {
            GameObject gameObject = CreateStlParent(stlFile);
            if (!gameObject)
            {
                Debug.LogWarning($"Could not create a mesh prefab for {stlFile}");
                return;
            }

            RuntimeUrdf.PrefabUtility_SaveAsPrefabAsset(gameObject, GetPrefabAssetPath(stlFile));
            Object.DestroyImmediate(gameObject);
        }

        private static Material GetDefaultDiffuseMaterial() 
        {
#if UNITY_EDITOR
            // also save the material in the Assets
            if (!RuntimeUrdf.IsRuntimeMode() && MaterialExtensions.GetRenderPipelineType() == MaterialExtensions.RenderPipelineType.Standard)
            {
                s_DefaultDiffuse = RuntimeUrdf.AssetDatabase_GetBuiltinExtraResource<Material>("Default-Diffuse.mat");
            }
#endif
            if (s_DefaultDiffuse) 
            {   // Could't use the "Default-Diffuse.mat", either because of HDRP or runtime. so let's create one.
                s_DefaultDiffuse = MaterialExtensions.CreateBasicMaterial();
            }
            return s_DefaultDiffuse;
        }

        private static GameObject CreateStlParent(string stlFile)
        {
            Mesh[] meshes = StlImporter.ImportMesh(stlFile);
            if (meshes == null)
                return null;

            GameObject parent = new GameObject(Path.GetFileNameWithoutExtension(stlFile));
            Material material = GetDefaultDiffuseMaterial();

            for (int i = 0; i < meshes.Length; i++)
            {
                string meshAssetPath = GetMeshAssetPath(stlFile, i);
                RuntimeUrdf.AssetDatabase_CreateAsset(meshes[i], meshAssetPath);
                GameObject gameObject = CreateStlGameObject(meshAssetPath, material);
                gameObject.transform.SetParent(parent.transform, false);
            }
            return parent;
        }
        
        private static GameObject CreateStlGameObject(string meshAssetPath, Material material)
        {
            GameObject gameObject = new GameObject(Path.GetFileNameWithoutExtension(meshAssetPath));
            gameObject.AddComponent<MeshFilter>().sharedMesh = RuntimeUrdf.AssetDatabase_LoadAssetAtPath<Mesh>(meshAssetPath);
            gameObject.AddComponent<MeshRenderer>().sharedMaterial = material;
            return gameObject;
        }
        
        public static string GetMeshAssetPath(string stlFile, int i)
        {
            return stlFile.Substring(0, stlFile.Length - 4) + "_" + i.ToString() + ".asset";
        }

        public static string GetPrefabAssetPath(string stlFile)
        {
            return stlFile.Substring(0, stlFile.Length - 4) + ".prefab";
        }
        
        public static GameObject CreateStlGameObjectRuntime(string stlFile)
        {
            Mesh[] meshes = StlImporter.ImportMesh(stlFile);
            if (meshes == null)
            {
                return null;
            }
            
            GameObject parent = new GameObject(Path.GetFileNameWithoutExtension(stlFile));

            Material material = GetDefaultDiffuseMaterial();
            
            for (int i = 0; i < meshes.Length; i++)
            {
                GameObject gameObject = new GameObject(Path.GetFileNameWithoutExtension(GetMeshAssetPath(stlFile, i)));
                gameObject.AddComponent<MeshFilter>().sharedMesh = meshes[i];
                gameObject.AddComponent<MeshRenderer>().sharedMaterial = material;
                gameObject.transform.SetParent(parent.transform, false);
            }
            return parent;
        }

    }
}
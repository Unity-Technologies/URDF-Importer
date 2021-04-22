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

namespace RosSharp
{
    using Unity.Robotics;
#if UNITY_EDITOR
    using UnityEditor;
    public class StlAssetPostProcessor : AssetPostprocessor
#else
    public class StlAssetPostProcessor
#endif    
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromPath)
        {
#if UNITY_EDITOR
            if (!RuntimeURDF.IsRuntimeMode())
            {
                foreach (string stlFile in importedAssets.Where(x => x.ToLowerInvariant().EndsWith(".stl")))
                {
                    createStlPrefab(stlFile);
                }
            }
#endif
        }

        private static void createStlPrefab(string stlFile)
        {
            GameObject gameObject = CreateStlParent(stlFile);
            if (gameObject == null)
            {
                return;
            }

            RuntimeURDF.PrefabUtility_SaveAsPrefabAsset(gameObject, getPrefabAssetPath(stlFile));
            Object.DestroyImmediate(gameObject);
        }

        private static Material defaultDiffuse = null;
        private static Material GetDefaultDiffuseMaterial() 
        {
#if UNITY_EDITOR
            // also save the material in the Assets
            if (!RuntimeURDF.IsRuntimeMode() && MaterialExtensions.GetRenderPipelineType() == MaterialExtensions.RenderPipelineType.Standard)
            {
                defaultDiffuse = RuntimeURDF.AssetDatabase_GetBuiltinExtraResource<Material>("Default-Diffuse.mat");
            }
#endif
            if (defaultDiffuse == null) 
            {   // Could't use the "Default-Diffuse.mat", either because of HDRP or runtime. so let's create one.
                defaultDiffuse = MaterialExtensions.CreateBasicMaterial();
            }
            return defaultDiffuse;
        }

        private static GameObject CreateStlParent(string stlFile)
        {
            Mesh[] meshes = Urdf.StlImporter.ImportMesh(stlFile);
            if (meshes == null)
                return null;

            GameObject parent = new GameObject(Path.GetFileNameWithoutExtension(stlFile));
            Material material = GetDefaultDiffuseMaterial();

            for (int i = 0; i < meshes.Length; i++)
            {
                string meshAssetPath = getMeshAssetPath(stlFile, i);
                RuntimeURDF.AssetDatabase_CreateAsset(meshes[i], meshAssetPath);
                GameObject gameObject = CreateStlGameObject(meshAssetPath, material);
                gameObject.transform.SetParent(parent.transform, false);
            }
            return parent;
        }
        
        private static GameObject CreateStlGameObject(string meshAssetPath, Material material)
        {
            GameObject gameObject = new GameObject(Path.GetFileNameWithoutExtension(meshAssetPath));
            gameObject.AddComponent<MeshFilter>().sharedMesh = RuntimeURDF.AssetDatabase_LoadAssetAtPath<Mesh>(meshAssetPath);
            gameObject.AddComponent<MeshRenderer>().sharedMaterial = material;
            return gameObject;
        }
        
        private static string getMeshAssetPath(string stlFile, int i)
        {
            return stlFile.Substring(0, stlFile.Length - 4) + "_" + i.ToString() + ".asset";
        }

        private static string getPrefabAssetPath(string stlFile)
        {
            return stlFile.Substring(0, stlFile.Length - 4) + ".prefab";
        }
        
        public static GameObject CreateStlGameObjectRuntime(string stlFile)
        {
            Mesh[] meshes = Urdf.StlImporter.ImportMesh(stlFile);
            if (meshes == null)
            {
                return null;
            }
            
            GameObject parent = new GameObject(Path.GetFileNameWithoutExtension(stlFile));

            Material material = GetDefaultDiffuseMaterial();
            
            for (int i = 0; i < meshes.Length; i++)
            {
                GameObject gameObject = new GameObject(Path.GetFileNameWithoutExtension(getMeshAssetPath(stlFile, i)));
                gameObject.AddComponent<MeshFilter>().sharedMesh = meshes[i];
                gameObject.AddComponent<MeshRenderer>().sharedMaterial = material;
                gameObject.transform.SetParent(parent.transform, false);
            }
            return parent;
        }

    }
}
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
using System.Collections.Generic;
using System.IO;
using Unity.Robotics;
using UnityEngine;
using UnityEngine.Rendering;

namespace RosSharp.Urdf
{
    public static class UrdfMaterial
    {
        private const string DefaultMaterialName = "Default";
        private const int RoundDigits = 4;

        public static Dictionary<string, Link.Visual.Material> Materials =
            new Dictionary<string, Link.Visual.Material>();
        
        #region Import
        private static Material CreateMaterial(this Link.Visual.Material urdfMaterial)
        {
            if (urdfMaterial.name == "")
            {
                urdfMaterial.name = GenerateMaterialName(urdfMaterial);
            }

            var material = RuntimeURDF.AssetDatabase_LoadAssetAtPath<Material>(UrdfAssetPathHandler.GetMaterialAssetPath(urdfMaterial.name));
            if (material != null)
            {   //material already exists
                return material;
            }

            material = MaterialExtensions.CreateBasicMaterial();
            if (urdfMaterial.color != null)
            {
                MaterialExtensions.SetMaterialColor(material, CreateColor(urdfMaterial.color));
            }
            else if (urdfMaterial.texture != null)
            {
                material.mainTexture = LoadTexture(urdfMaterial.texture.filename);
            }

            RuntimeURDF.AssetDatabase_CreateAsset(material, UrdfAssetPathHandler.GetMaterialAssetPath(urdfMaterial.name));
            return material;
        }

        static Material defaultMaterial = null; // used RuntimeURDF
        private static void CreateDefaultMaterial()
        {
            Material material = defaultMaterial;
#if UNITY_EDITOR
            if (!RuntimeURDF.IsRuntimeMode())
            {
                material = RuntimeURDF.AssetDatabase_LoadAssetAtPath<Material>(UrdfAssetPathHandler.GetMaterialAssetPath(DefaultMaterialName));
            }
#endif
            if (material != null)
            {
                return;
            }

            material = MaterialExtensions.CreateBasicMaterial();
            MaterialExtensions.SetMaterialColor(material, new Color(0.33f, 0.33f, 0.33f, 0.0f));
            
            // just keep it in memory while the app is running.
            defaultMaterial = material;
#if UNITY_EDITOR
            if (!RuntimeURDF.IsRuntimeMode())
            {
                // create the material to be reused
                RuntimeURDF.AssetDatabase_CreateAsset(material, UrdfAssetPathHandler.GetMaterialAssetPath(DefaultMaterialName));
            }
#endif
        }

        private static string GenerateMaterialName(Link.Visual.Material urdfMaterial)
        {
            var materialName = "";
            if (urdfMaterial.color != null)
            {
                materialName = "rgba-";
                for (var i = 0; i < urdfMaterial.color.rgba.Length; i++)
                {
                    materialName += urdfMaterial.color.rgba[i];
                    if (i != urdfMaterial.color.rgba.Length - 1)
                        materialName += "-";
                }
            }
            else if (urdfMaterial.texture != null)
                materialName = "texture-" + Path.GetFileName(urdfMaterial.texture.filename);

            return materialName;
        }

        private static Color CreateColor(Link.Visual.Material.Color urdfColor)
        {
            return new Color(
                (float)urdfColor.rgba[0],
                (float)urdfColor.rgba[1],
                (float)urdfColor.rgba[2],
                (float)urdfColor.rgba[3]);
        }

        private static Texture LoadTexture(string filename)
        {
            return filename == "" ? null : LocateAssetHandler.FindUrdfAsset<Texture>(filename);
        }


        public static void InitializeRobotMaterials(Robot robot)
        {
            CreateDefaultMaterial();
            foreach (var material in robot.materials)
                CreateMaterial(material);
        }
        
        public static void SetUrdfMaterial(GameObject gameObject, Link.Visual.Material urdfMaterial)
        {
            if (urdfMaterial != null)
            {
                var material = CreateMaterial(urdfMaterial);
                SetMaterial(gameObject, material);
            }
            else
            {
                //If the URDF material is not defined, and the renderer is missing
                //a material, assign the default material.
                Renderer renderer = gameObject.GetComponentInChildren<Renderer>();
                if (renderer != null && renderer.sharedMaterial == null)
                {
                    Material material = defaultMaterial;
#if UNITY_EDITOR
                    if (!RuntimeURDF.IsRuntimeMode())
                    {
                        material = RuntimeURDF.AssetDatabase_LoadAssetAtPath<Material>(UrdfAssetPathHandler.GetMaterialAssetPath(DefaultMaterialName));
                    }
#endif
                    SetMaterial(gameObject, material);
                }
            }
        }

        private static void SetMaterial(GameObject gameObject, Material material)
        {
            var renderers = gameObject.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                renderer.sharedMaterial = material;
            }
        }

        #endregion

        #region Export

        public static Link.Visual.Material ExportMaterialData(Material material)
        {
            if (material == null)
            {
                return null;
            }

            if (!Materials.ContainsKey(material.name))
            {
                if (material.mainTexture != null)
                {
                    Link.Visual.Material.Texture texture = ExportTextureData(material.mainTexture);
                    Materials[material.name] = new Link.Visual.Material(material.name, null, texture);
                }
                else if (!material.color.Equals(Color.clear))
                {
                    Link.Visual.Material.Color color = new Link.Visual.Material.Color(ExportRgbaData(material));
                    Materials[material.name] = new Link.Visual.Material(material.name, color);
                }
                else
                {
                    return null;
                }
            }

            return Materials[material.name];
        }

        private static double[] ExportRgbaData(Material material)
        {
            return new double[]
            {
                Math.Round(material.color.r, RoundDigits),
                Math.Round(material.color.g, RoundDigits),
                Math.Round(material.color.b, RoundDigits),
                Math.Round(material.color.a, RoundDigits)
            };
        }

        private static Link.Visual.Material.Texture ExportTextureData(Texture texture)
        {
            string oldTexturePath = UrdfAssetPathHandler.GetFullAssetPath(RuntimeURDF.AssetDatabase_GetAssetPath(texture));
            string newTexturePath = UrdfExportPathHandler.GetNewResourcePath(Path.GetFileName(oldTexturePath));
            if (oldTexturePath != newTexturePath)
            {
                File.Copy(oldTexturePath, newTexturePath, true);
            }

            string packagePath = UrdfExportPathHandler.GetPackagePathForResource(newTexturePath);
            return new Link.Visual.Material.Texture(packagePath);
        }

        #endregion
    }
}
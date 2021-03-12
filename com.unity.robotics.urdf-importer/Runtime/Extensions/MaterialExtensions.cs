using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Unity.Robotics
{
    public static class MaterialExtensions
    {
        private static string[] standardShaders = { "Standard" };//, "UI/Default" };
        private static string[] hdrpShaders = { "HDRP/Lit" };//, "UI/Default" };
        public static Material CreateBasicMaterial()
        {
            try
            {
                string[] shadersToTry = IsHDRP() ?  hdrpShaders : standardShaders;
                foreach (var shaderName in shadersToTry)
                {
                    Shader shader = Shader.Find(shaderName);
                    if (shader != null)
                    {
                        //Debug.Log("Found shader: " + shaderName);
                        var material = new Material(shader);
                        material.SetFloat("_Metallic", 0.75f);
                        material.SetFloat("_Glossiness", 0.75f);
                        return material;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogAssertion(ex.ToString());
            }
            return null;
        }

        /// Checks if current render pipeline is HDRP 
        /// Used for creating the proper default material.
        public static bool IsHDRP()
        {
            //TODO: should it also return true for Universal Render pipeline?
            return GraphicsSettings.renderPipelineAsset != null && GraphicsSettings.renderPipelineAsset.GetType().ToString().Contains("HighDefinition");
        }

        public static void SetMaterialColor(Material material, Color color)
        {
            material.SetColor(IsHDRP() ? "_BaseColor" : "_Color", color);
        }

        public static void SetMaterialEmissionColor(Material material, Color color)
        {
            // Assuming both shaders use the _EmissionColor property. Not tested for HDRP. 
            material.SetColor(IsHDRP() ? "_EmissionColor" : "_EmissionColor", color);
            material.EnableKeyword("_EMISSION");
        }

        public static Color GetMaterialColor(Renderer renderer)
        {
            if (IsHDRP())
            {
                return renderer.material.GetColor("_BaseColor");
            }
            else
            {
                return renderer.sharedMaterial.GetColor("_Color");
            }
        }
    }
}
using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Unity.Robotics
{
    public static class MaterialExtensions
    {
        public enum RenderPipelineType 
        {
            Standard,
            URP,
            HDRP,
        }
        
        private static string[] standardShaders = { "Standard" };
        private static string[] hdrpShaders = { "HDRP/Lit" };
        private static string[] urpShaders = { "Universal Render Pipeline/Lit" };
        public static Material CreateBasicMaterial()
        {
            try
            {
                string[] shadersToTry = standardShaders;
                if (GetRenderPipelineType() == RenderPipelineType.HDRP)
                {
                    shadersToTry = hdrpShaders;
                }
                else if (GetRenderPipelineType() == RenderPipelineType.URP)
                {
                    shadersToTry = urpShaders;
                }

                foreach (var shaderName in shadersToTry)
                {
                    Shader shader = Shader.Find(shaderName);
                    if (shader != null)
                    {
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
        public static RenderPipelineType GetRenderPipelineType()
        {
            if (GraphicsSettings.renderPipelineAsset != null) 
            {
                if (GraphicsSettings.renderPipelineAsset.GetType().ToString().Contains("HighDefinition"))
                {
                    return RenderPipelineType.HDRP;
                }
                else if (GraphicsSettings.renderPipelineAsset.GetType().ToString().Contains("Universal"))
                {
                    return RenderPipelineType.URP;
                }
            }
            return RenderPipelineType.Standard;
        }

        public static void SetMaterialColor(Material material, Color color)
        {
            material.SetColor(GetRenderPipelineType() != RenderPipelineType.Standard ? "_BaseColor" : "_Color", color);
        }

        public static void SetMaterialEmissionColor(Material material, Color color)
        {
            // Assuming both shaders use the _EmissionColor property. Not tested for HDRP and URP.
            //Library/PackageCache/com.unity.render-pipelines.universal@10.3.2/Shaders/Lit.shader 
            material.SetColor(GetRenderPipelineType() != RenderPipelineType.Standard ? "_EmissionColor" : "_EmissionColor", color);
            material.EnableKeyword("_EMISSION");
        }

        public static Color GetMaterialColor(Renderer renderer)
        {
            if (GetRenderPipelineType() != RenderPipelineType.Standard)
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
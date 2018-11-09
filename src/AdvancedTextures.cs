using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AdvancedTextures
{
    public class AdvancedTextures : PartModule
    {
        [KSPField(isPersistant = false)]
        public string newShader = null;
        [KSPField(isPersistant = false)]
        public string transforms = "Any";
        [KSPField(isPersistant = false)]
        public string newMaterial = "";
        [KSPField(isPersistant = false)]
        public string BuiltinIndex = "0";
        [KSPField(isPersistant = false)]
        public string _MainTex = null;          // texture
        [KSPField(isPersistant = false)]
        public string _BumpMap = null;          // normal map
        [KSPField(isPersistant = false)]
        public string _ParallaxMap = null;      // height map
        [KSPField(isPersistant = false)]
        public string _Emissive = null;         // legacy shader  U4 name for emissive map
        [KSPField(isPersistant = false)]
        public string _EmissionMap = null;      // U5 std shader name for emissive map
        [KSPField(isPersistant = false)]
        public string _MetallicGlossMap = null; // U5 metallic (standard shader)
        [KSPField(isPersistant = false)]
        public string _OcclusionMap = null;     // ambient occlusion
        [KSPField(isPersistant = false)]
        public string _SpecGlossMap = null;     // U5 metallic (standard shader - spec gloss setup)
        
        private int textureIndex = 0;
        private List<string> targetTransforms = new List<string>();

        private string[] seperators = new string[] { " ", ",", ";" };



        public void Start()
        {

//            Log.Normal("called on " + part.name);

            if (!int.TryParse(BuiltinIndex, out textureIndex))
            {
                Log.UserError("AdvancedTextures: could not parse BuiltinIndex " + BuiltinIndex);
            }

            targetTransforms = transforms.Split(seperators, StringSplitOptions.RemoveEmptyEntries).ToList();
//            Log.Normal("Transforms: " + transforms);

            foreach (MeshRenderer renderer in gameObject.GetComponentsInChildren<MeshRenderer>(true))
            {
                if (!transforms.Equals("Any", StringComparison.CurrentCultureIgnoreCase) && !targetTransforms.Contains(renderer.transform.name))
                {
                    continue;
                }

                // Log.Normal("Processing Transform: " + renderer.transform.name);

                if (newMaterial != "")
                {
                    ReplaceMaterial(renderer, newMaterial);
                    continue;
                }

                if (!string.IsNullOrEmpty(newShader))
                {
                    ReplaceShader(renderer, newShader);
                }

                SetTexture(renderer, _MainTex, "_MainTex");
                SetTexture(renderer, _ParallaxMap, "_ParallaxMap");
                SetTexture(renderer, _Emissive, "_Emissive");
                SetTexture(renderer, _EmissionMap, "_EmissionMap");
                SetTexture(renderer, _MetallicGlossMap, "_MetallicGlossMap");
                SetTexture(renderer, _OcclusionMap, "_OcclusionMap");
                SetTexture(renderer, _SpecGlossMap, "_SpecGlossMap");
                SetTexture(renderer, _BumpMap, "_BumpMap", true);
            }
        }


        private void SetTexture(MeshRenderer renderer, string texturename, string targetname, bool isNormal = false)
        {
            if (!String.IsNullOrEmpty(texturename))
            {
                Texture2D newTexture = KKGraphics.GetTexture(texturename, isNormal, textureIndex);
                if (newTexture != null)
                {
                    //Log.Normal("setting Texture: " + texturename +  " on: " + targetname ) ;
                    renderer.material.SetTexture(targetname, newTexture);

                }
                else
                {
                    Log.UserWarning("cannot set Texture: " + texturename + " as " + targetname + " on: " + part.name);
                }
            }
        }


        private void ReplaceMaterial(MeshRenderer renderer, string materialName)
        {
            //Log.Normal("Material replaceder called");
            Material foundMaterial = KKGraphics.GetMaterial(materialName);
            if (foundMaterial != null)
            {
                Log.Normal("Material replaced: " + foundMaterial.name);
                renderer.material = Instantiate(foundMaterial);
            }

        }


        private void ReplaceShader(MeshRenderer renderer, string newShaderName)
        {
            if (!KKGraphics.HasShader(newShaderName))
            {
                Log.UserError("No Shader like this found: " + newShaderName);
                return;
            }

            Shader newShader = KKGraphics.GetShader(newShaderName);
            renderer.material.shader = newShader;
            //Log.Normal("Applied Shader: " + newShader.name);

        }

    }
}

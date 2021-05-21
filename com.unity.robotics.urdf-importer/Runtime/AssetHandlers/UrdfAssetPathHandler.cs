/*
© Siemens AG, 2017-2018
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

using System.IO;
using UnityEngine;

namespace RosSharp.Urdf
{
    public static class UrdfAssetPathHandler
    {
        //Relative to Assets folder
        private static string packageRoot;
        private const string MaterialFolderName = "Materials";

        #region SetAssetRootFolder
        public static void SetPackageRoot(string newPath, bool correctingIncorrectPackageRoot = false)
        {
            string oldPackagePath = packageRoot;

            packageRoot = GetRelativeAssetPath(newPath);

            if (!RuntimeURDF.AssetDatabase_IsValidFolder(Path.Combine(packageRoot, MaterialFolderName)))
            {
                RuntimeURDF.AssetDatabase_CreateFolder(packageRoot, MaterialFolderName);
            }

            if (correctingIncorrectPackageRoot)
            {
                MoveMaterialsToNewLocation(oldPackagePath);
            }
        }
        #endregion

        #region GetPaths
        public static string GetPackageRoot()
        {
            return packageRoot;
        }
        
        public static string GetRelativeAssetPath(string absolutePath)
        {
            var absolutePathUnityFormat = absolutePath.SetSeparatorChar();
            if (!absolutePathUnityFormat.StartsWith(Application.dataPath.SetSeparatorChar()))
            {
#if UNITY_EDITOR
                if (!RuntimeURDF.IsRuntimeMode())
                {
                    return null;
                }
#endif
                return absolutePath; // so that it works in runtime
            }

            var assetPath = "Assets" + absolutePath.Substring(Application.dataPath.Length);
            return assetPath.SetSeparatorChar();
        }

        public static string GetFullAssetPath(string relativePath)
        {
            string fullPath = Application.dataPath;
            if (relativePath.Substring(0, "Assets".Length) == "Assets")
            {
                fullPath += relativePath.Substring("Assets".Length);
            }
            else 
            {
                fullPath = fullPath.Substring(0, fullPath.Length - "Assets".Length) + relativePath;
            }
            return fullPath.SetSeparatorChar();
        }

        public static string GetRelativeAssetPathFromUrdfPath(string urdfPath, bool convertToPrefab=true)
        {
            if (!urdfPath.StartsWith(@"package://"))
            {
               Debug.LogWarning(@$"{urdfPath} is not a valid URDF package file path. Path should start with package://, and URDF file should be in the directory root.");
               if (urdfPath.Substring(0, 3) == "../")
               {
                   Debug.LogWarning("Attempting to replace file path's starting instance of `../` with standard package notation `package://` to prevent manual path traversal at root of directory!");
                   urdfPath = $@"package://{urdfPath.Substring(3)}";
               }
               else
               {
                   return null;
               }
            }
            string path;
            if (urdfPath.StartsWith(@"package://"))
            {
                path = urdfPath.Substring(10).SetSeparatorChar();
            }
            else
            {
                path = urdfPath.SetSeparatorChar();
            }

            if (convertToPrefab) 
            {
                if (Path.GetExtension(path)?.ToLowerInvariant() == ".stl")
                    path = path.Substring(0, path.Length - 3) + "prefab";

            }
            return Path.Combine(packageRoot, path);
        }
        #endregion

        public static bool IsValidAssetPath(string path)
        {
#if UNITY_EDITOR
            if (!RuntimeURDF.IsRuntimeMode())
            {
                return GetRelativeAssetPath(path) != null;
            }
#endif
            //RuntimeImporter. TODO: check if the path really exists
            return true;
        }

        #region Materials

        private static void MoveMaterialsToNewLocation(string oldPackageRoot)
        {
            if (RuntimeURDF.AssetDatabase_IsValidFolder(Path.Combine(oldPackageRoot, MaterialFolderName)))
            {
                RuntimeURDF.AssetDatabase_MoveAsset(
                    Path.Combine(oldPackageRoot, MaterialFolderName),
                    Path.Combine(UrdfAssetPathHandler.GetPackageRoot(), MaterialFolderName));
            }
            else
            {
                RuntimeURDF.AssetDatabase_CreateFolder(UrdfAssetPathHandler.GetPackageRoot(), MaterialFolderName);
            }
        }

        public static string GetMaterialAssetPath(string materialName)
        {
            return Path.Combine(packageRoot, MaterialFolderName, Path.GetFileName(materialName) + ".mat");
        }

        #endregion
    }

}
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Gamla.Editor
{
    public class ScriptableObjectUtility
    {
        private const string AssetFolder = "Assets";
        private const string ResourcesFolder = "Resources";
        private static string PathResourcesFolder => Path.Combine(AssetFolder, ResourcesFolder);
        private const string AssetExt = ".asset";
        
        public static T Find<T>() where T : ScriptableObject
        {
            var loadedInstances = Resources.FindObjectsOfTypeAll<T>();

            if (loadedInstances.Length > 0)
            {
                return loadedInstances[0];
            }
            else
            {
                return LoadAsset<T>();
            }
        }

        static T LoadAsset<T>() where T : ScriptableObject
        {
            var guids = AssetDatabase.FindAssets($"t: {typeof(T)}");

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);

                var asset = AssetDatabase.LoadAssetAtPath<T>(path);

                if (asset != null)
                {
                    return asset;
                }
            }

            return null;
        }

        public static T Create<T>(string assetName) where T : ScriptableObject
        {
            var gamlaConfig = ScriptableObject.CreateInstance<T>();
            
            if (!AssetDatabase.IsValidFolder(PathResourcesFolder))
            {
                var baseResourcesFolder = AssetDatabase.GUIDToAssetPath(AssetDatabase.CreateFolder(AssetFolder, ResourcesFolder));
                Debug.Log($"Gamla created asset folder '{baseResourcesFolder}'");
            }

            AssetDatabase.CreateAsset(gamlaConfig, Path.Combine(PathResourcesFolder, assetName + AssetExt));
            return gamlaConfig;
        }
    }
}
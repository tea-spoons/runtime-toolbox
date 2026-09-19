
namespace TeaSpoons.RuntimeToolbox.Editor
{
    using System.IO;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;
    using JetBrains.Annotations;

    /// <summary>
    /// Creates and keeps in sync every <see cref="CachedTypeList"/> in the project.
    /// </summary>
    [InitializeOnLoad]
    internal static class CachedTypeListUpdater
    {
        private const string Folder = "Assets/Resources/Generated";

        static CachedTypeListUpdater()
        {
            OnReload();
        }

        private static void OnReload()
        {
            var soTypes = TypeCache.GetTypesDerivedFrom<CachedTypeList>().Where(t => !t.IsAbstract);

            foreach (var t in soTypes)
            {
                GetLoadedOrCreateAsset(t)?.Refresh();
            }

            AssetDatabase.SaveAssets();
        }

        [CanBeNull]
        private static CachedTypeList GetLoadedOrCreateAsset(System.Type type)
        {
            var path = $"{Folder}/{type.Name}.asset";
            var loadedAsset = AssetDatabase.LoadAssetAtPath<CachedTypeList>(path);
            if (loadedAsset)
            {
                return loadedAsset;
            }

            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            loadedAsset = ScriptableObject.CreateInstance(type) as CachedTypeList;
            loadedAsset.hideFlags = HideFlags.NotEditable;
            AssetDatabase.CreateAsset(loadedAsset, path);

            return loadedAsset;
        }
    }
}

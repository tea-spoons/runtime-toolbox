
namespace TeaSpoons.RuntimeToolbox
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class CachedTypeList : ScriptableObject
    {
        /// <summary>
        /// Convenience loader for any <see cref="CachedTypeList"/> that
        /// lives under the project’s generated-cache folder inside Resources folder.
        /// </summary>
        /// <returns>
        /// The requested asset instance or <c>null</c> if no matching file exists
        /// under <c>Resources/Generated/</c>.
        /// </returns>
        public static T Load<T>() where T : CachedTypeList
        {
            return Resources.Load<T>($"Generated/{typeof(T).Name}");
        }

        [SerializeField]
        private string[] typeNames = Array.Empty<string>();

        public IReadOnlyList<string> TypeNames => typeNames;

        protected abstract bool IsValid(Type t);
        protected abstract IEnumerable<Type> GetTypes();

        protected IEnumerable<Type> FindDerivedTypes<T>()
        {
#if UNITY_EDITOR
            return UnityEditor.TypeCache.GetTypesDerivedFrom<T>().Where(IsValid);
#else
            return Array.Empty<Type>();
#endif
        }

#if UNITY_EDITOR
        internal void Refresh()
        {
            var newNames = GetTypes()
                .Select(t => t.AssemblyQualifiedName)
                .OrderBy(n => n)
                .ToArray();

            if (typeNames.SequenceEqual(newNames))
            {
                return;
            }

            typeNames = newNames;
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}

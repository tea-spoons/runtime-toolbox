namespace TeaSpoons.RuntimeToolbox
{
    using System;
    using UnityEngine;
    using Object = UnityEngine.Object;

    /// <summary>
    /// Allows serialization of interfaces. Works with fields and lists in the inspector.
    /// </summary>
    /// <typeparam name="TInterface"></typeparam>
	[Serializable]
    public struct InterfaceField<TInterface> : ISerializationCallbackReceiver where TInterface : class
    {
        [SerializeField]
        private Object target;
        private TInterface cache;

        public TInterface Value
        {
            get
            {
                if (cache == null && target != null)
                {
                    cache = target as TInterface;
                }
                return cache;
            }
            set
            {
                cache = value;
                target = value as Object;
            }
        }

        public void OnBeforeSerialize()
        {
            // prevent the assignment of objects not implementing the interface
            if (target != null && !(target is TInterface))
            {
                Debug.LogError($"[InterfaceField] Object '{target.name}' doesn't implement interface '{typeof(TInterface).Name}'. Assignment denied.");
                target = null;
            }
        }

        public void OnAfterDeserialize() { }
    }
}

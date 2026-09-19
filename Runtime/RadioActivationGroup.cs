
namespace TeaSpoons.RuntimeToolbox
{
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.Serialization;
    using System.Collections.Generic;

    /// <inheritdoc/>
    public class RadioActivationGroup : RadioActivationGroupBase
    {
        [SerializeField, FormerlySerializedAs(nameof(targets))]
        private List<GameObject> _targets;
        protected override IEnumerable<GameObject> targets => _targets;

        [Space]
        [SerializeField]
        private GameObject initiallyActivatedTarget;
        [Space]
        [SerializeField]
        private UnityEvent<GameObject> onActiveTargetChanged;

        private void Start()
        {
            ActivateInitialTarget();
        }

        public new void Activate(GameObject target)
        {
            base.Activate(target);
        }

        protected override void InvokeOnActiveTargetChanged(GameObject target)
        {
            onActiveTargetChanged.Invoke(target);
            base.InvokeOnActiveTargetChanged(target);
        }

        private void ActivateInitialTarget()
        {
            if (hasActivatedBefore) return;

            if (initiallyActivatedTarget)
            {
                Activate(initiallyActivatedTarget);
            }
#if UNITY_EDITOR
            else if (!Application.isPlaying && targets != null)
            {
                var enumerator = targets.GetEnumerator();
                if (enumerator.MoveNext())
                {
                    Activate(enumerator.Current);
                }
            }
#endif
            else
            {
                Activate(null);
            }
        }
    }
}

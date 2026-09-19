
namespace TeaSpoons.RuntimeToolbox
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    /// <summary>
    /// A component that has a list of GameObjects and a method form activating one of them, while deactivating all the others.
    /// </summary>
    /// <remarks>
    /// Use <see cref="RadioActivationGroup"/> for free inspector support
    /// or extend this class for a more restrictive implementation.
    /// </remarks>
#if UNITY_EDITOR
    [ExecuteAlways]
#endif
    public abstract class RadioActivationGroupBase : MonoBehaviour
    {
        public event Action<GameObject> OnActiveTargetChanged = delegate { };

        protected abstract IEnumerable<GameObject> targets { get; }

        /// <summary>
        /// <c>true</c> if <see cref="Activate(GameObject)"/> has been called at least once.
        /// If so, it's probably better to avoid any initial activation in <c>Start()</c>.
        /// </summary>
        protected bool hasActivatedBefore { get; private set; } = false;

#if UNITY_EDITOR
        protected virtual void OnEnable()
        {
            Selection.selectionChanged += UpdateTargetVisiblity;
        }

        protected virtual void OnDisable()
        {
            Selection.selectionChanged -= UpdateTargetVisiblity;
        }
#endif

        protected void Activate(GameObject target)
        {
            var activatedGivenTarget = false;
            var oneOrMoreTargetsChanged = false;
            foreach (var targetInList in targets)
            {
                var isTargetToActivate = targetInList == target;
                if (isTargetToActivate)
                {
                    activatedGivenTarget = true;
                }

                oneOrMoreTargetsChanged = targetInList != null &&
                    targetInList.activeSelf != isTargetToActivate;

                SetActive(targetInList, isTargetToActivate);
            }

            if (oneOrMoreTargetsChanged)
            {
                if (!activatedGivenTarget)
                {
                    target = null;
                }
                InvokeOnActiveTargetChanged(target);
            }

            hasActivatedBefore = true;
        }

        protected static void SetActive(GameObject target, bool active)
        {
            if (target == null) return;

#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                target.SetActive(active);
            }
            else
            {
                if (active)
                {
                    SceneVisibilityManager.instance.Show(target, true);
                }
                else
                {
                    SceneVisibilityManager.instance.Hide(target, true);
                }
            }
#else
            target.SetActive(active);
#endif
        }

        protected virtual void InvokeOnActiveTargetChanged(GameObject target)
        {
            OnActiveTargetChanged(target);
        }

#if UNITY_EDITOR
        private void UpdateTargetVisiblity()
        {
            if (targets == null || Application.isPlaying) return;

            var transform = Selection.activeTransform;

            while (transform != null)
            {
                if (TargetsContain(transform.gameObject))
                {
                    Activate(transform.gameObject);
                    return;
                }

                transform = transform.parent;
            }
        }

        private bool TargetsContain(GameObject target)
        {
            foreach (var item in targets)
            {
                if (item == target)
                {
                    return true;
                }
            }

            return false;
        }
#endif
    }
}

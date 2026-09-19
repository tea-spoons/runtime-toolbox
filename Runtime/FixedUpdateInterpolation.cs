
namespace TeaSpoons.RuntimeToolbox
{
    using static TeaSpoons.PackageCore.PlayerLoopUtility;
    using UnityEngine;
    using UnityEngine.LowLevel;
    using UnityEngine.PlayerLoop;
    using System;
    using System.Collections.Generic;
    using UnityObject = UnityEngine.Object;

    /// <summary>
    /// Interpolates any values in Update between two FixedUpdates.
    /// </summary>
    public static class FixedUpdateInterpolation
    {
        #region Nested Type Definitions
        private interface ICallback
        {
            void ReadCurrent();
            void FinishInterpolation();
            void Teleport();
            void Apply(float t);
        }

        private struct Callback<T> : ICallback
            where T : struct, IEquatable<T>
        {
            private readonly Func<T, T, float, T> interpolate;
            private readonly Func<T> read;
            private readonly Action<T> write;

            private T previous;
            private T lastKnown;
            private T next;

            public Callback(Func<T, T, float, T> interpolate,
                Func<T> read,
                Action<T> write)
            {
                this.interpolate = interpolate;
                this.read = read;
                this.write = write;

                lastKnown = read();
                previous = lastKnown;
                next = lastKnown;
            }

            // In LateFixedUpdate, reads the current value that was set in FixedUpdate.
            public void ReadCurrent()
            {
                previous = next;
                next = read();
                lastKnown = next;
            }

            public void Teleport()
            {
                Teleport(read());
            }

            // In EarlyFixedUpdate, sets the final interpolation value,
            // so the following FixedUpdate can work with the result of the last one.
            public void FinishInterpolation()
            {
                CheckTeleport();
                write(next);
                lastKnown = read();
            }

            // Applies the interpolated value in Update.
            public void Apply(float t)
            {
                CheckTeleport();
                var current = interpolate(previous, next, t);
                write(current);
                lastKnown = read();
            }

            /// <summary>
            /// Detects if something changed the value outside of FixedUpdate
            /// and "accepts" that new value, stopping interpolation.
            /// </summary>
            private void CheckTeleport()
            {
                var current = read();
                if (!current.Equals(lastKnown))
                {
                    Teleport(current);
                }
            }

            private void Teleport(T value)
            {
                next = value;
                previous = value;
                lastKnown = value;
            }
        }

        private struct EarlyFixedUpdateLoop { }
        private struct LateFixedUpdateLoop { }
        private struct UpdateLoop { }
        #endregion

        private static readonly Dictionary<UnityObject, List<ICallback>> callbacks = new();
        private static readonly HashSet<UnityObject> teleportBuffer = new();
        private static readonly HashSet<UnityObject> removalBuffer = new();
        private static bool initialized;

        private static void InitializeIfNeeded()
        {
            if (initialized) return;

            initialized = true;

            var earlyFixedUpdate = new PlayerLoopSystem
            {
                updateDelegate = EarlyFixedUpdate,
                type = typeof(EarlyFixedUpdateLoop)
            };

            var lateFixedUpdate = new PlayerLoopSystem
            {
                updateDelegate = LateFixedUpdate,
                type = typeof(LateFixedUpdateLoop)
            };

            var update = new PlayerLoopSystem
            {
                updateDelegate = Update,
                type = typeof(UpdateLoop)
            };

            AddPlayerLoopSystem(new Path<FixedUpdate>(),
                new Before<FixedUpdate.ScriptRunBehaviourFixedUpdate>(),
                earlyFixedUpdate);
            AddPlayerLoopSystem(new Path<FixedUpdate>(),
                new After<FixedUpdate.ScriptRunDelayedFixedFrameRate>(),
                lateFixedUpdate);
            AddPlayerLoopSystem(new Path<Update>(),
                new Append(),
                update);
        }

        /// <summary>
        /// Adds an entry to interpolation.
        /// </summary>
        /// <param name="owner">The object the interpolation is done for.</param>
        /// <param name="interpolate">The interpolation function.</param>
        /// <param name="read">The function to read the current value.</param>
        /// <param name="write">The function to overwrite the value.</param>
        public static void Interpolate<T>(in UnityObject owner,
            in Func<T, T, float, T> interpolate,
            in Func<T> read,
            in Action<T> write)
            where T : struct, IEquatable<T>
        {
            if (!callbacks.TryGetValue(owner, out var ownerCallbacks))
            {
                ownerCallbacks = new();
                callbacks.Add(owner, ownerCallbacks);
            }
            ownerCallbacks.Add(new Callback<T>(interpolate, read, write));

            InitializeIfNeeded();
        }

        #region Common Interpolate Methods
        public static void Interpolate(in UnityObject owner,
            in Func<float> read,
            in Action<float> write)
        {
            Interpolate(owner, Mathf.Lerp, read, write);
        }

        public static void Interpolate(in UnityObject owner,
            in Func<Vector2> read,
            in Action<Vector2> write)
        {
            Interpolate(owner, Vector2.Lerp, read, write);
        }

        public static void Interpolate(in UnityObject owner,
            in Func<Vector3> read,
            in Action<Vector3> write)
        {
            Interpolate(owner, Vector3.Lerp, read, write);
        }

        public static void Interpolate(in UnityObject owner,
            in Func<Quaternion> read,
            in Action<Quaternion> write)
        {
            Interpolate(owner, Quaternion.Slerp, read, write);
        }

        public static void InterpolatePositionAndRotation(Transform transform)
        {
            Interpolate(transform, Vector3.Lerp, () => transform.position, v => transform.position = v);
            Interpolate(transform, Quaternion.Slerp, () => transform.rotation, q => transform.rotation = q);
        }
        #endregion

        /// <summary>
        /// "Accepts" the current values that are interpolated for the given <paramref name="owner"/>.
        /// Interpolation for those values is skipped until the next FixedUpdate.
        /// </summary>
        /// <param name="owner"></param>
        public static void Teleport(in UnityObject owner)
        {
            teleportBuffer.Add(owner);
        }

        public static void Unregister(in UnityObject owner)
        {
            callbacks.Remove(owner);
        }

        #region PlayerLoop Callback Methods
        /// <summary>
        /// Sets all values to their final values of the current interpolation,
        /// so reading them in the following FixedUpdate returns the same values that were set in the previous one.
        /// </summary>
        private static void EarlyFixedUpdate()
        {
            foreach (var kvp in callbacks)
            {
                if (kvp.Key != null)
                {
                    foreach (var callback in kvp.Value)
                    {
                        callback.FinishInterpolation();
                    }
                }
                else
                {
                    removalBuffer.Add(kvp.Key);
                }
            }
            ClearRemovalBuffer();
        }

        /// <summary>
        /// Reads all current values in order to interpolate towards them.
        /// </summary>
        private static void LateFixedUpdate()
        {
            foreach (var kvp in callbacks)
            {
                if (kvp.Key != null)
                {
                    var shouldTeleport = teleportBuffer.Contains(kvp.Key);
                    if (shouldTeleport)
                    {
                        foreach (var callback in kvp.Value)
                        {
                            callback.Teleport();
                        }
                    }
                    else
                    {
                        foreach (var callback in kvp.Value)
                        {
                            callback.ReadCurrent();
                        }
                    }
                }
                else
                {
                    removalBuffer.Add(kvp.Key);
                }
            }

            teleportBuffer.Clear();
            ClearRemovalBuffer();
        }

        /// <summary>
        /// Applies all interpolated values in the Update phase.
        /// </summary>
        private static void Update()
        {
            var currentT = (Time.time - Time.fixedTime) / Time.fixedDeltaTime;

            foreach (var kvp in callbacks)
            {
                if (kvp.Key != null)
                {
                    foreach (var callback in kvp.Value)
                    {
                        callback.Apply(currentT);
                    }
                }
                else
                {
                    removalBuffer.Add(kvp.Key);
                }
            }
            ClearRemovalBuffer();
        }
        #endregion

        private static void ClearRemovalBuffer()
        {
            if (removalBuffer.Count > 0)
            {
                foreach (var owner in removalBuffer)
                {
                    callbacks.Remove(owner);
                }
                removalBuffer.Clear();
            }
        }
    }
}

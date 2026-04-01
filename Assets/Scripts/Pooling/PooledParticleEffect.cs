using UnityEngine;

namespace Pooling
{
    public sealed class PooledParticleEffect : MonoBehaviour, IPoolable
    {
        [Header("References")]
        [SerializeField] private ParticleSystem[] systems;

        [Header("Fallback")]
        [SerializeField, Min(0.1f)] private float fallbackReturnDelay = 5f;

        private PoolMember _poolMember;
        private float _returnAtTime;
        private bool _isActive;

        private void Awake()
        {
            _poolMember = GetComponent<PoolMember>();

            if (systems == null || systems.Length == 0)
            {
                systems = GetComponentsInChildren<ParticleSystem>(true);
            }

            for (int i = 0; i < systems.Length; i++)
            {
                if (systems[i] == null)
                {
                    continue;
                }

                ParticleSystem.MainModule main = systems[i].main;
                main.stopAction = ParticleSystemStopAction.Callback;
            }
        }

        private void Update()
        {
            if (!_isActive)
            {
                return;
            }

            if (Time.time >= _returnAtTime)
            {
                ReturnToPool();
            }
        }

        public void OnTakenFromPool()
        {
            _isActive = true;
            _returnAtTime = Time.time + GetLongestDuration();

            for (int i = 0; i < systems.Length; i++)
            {
                if (systems[i] == null)
                {
                    continue;
                }

                systems[i].Clear(true);
                systems[i].Play(true);
            }
        }

        public void OnReturnedToPool()
        {
            _isActive = false;

            for (int i = 0; i < systems.Length; i++)
            {
                if (systems[i] == null)
                {
                    continue;
                }

                systems[i].Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                systems[i].Clear(true);
            }
        }

        private void OnParticleSystemStopped()
        {
            if (!_isActive)
            {
                return;
            }

            if (AllSystemsStopped())
            {
                ReturnToPool();
            }
        }

        private bool AllSystemsStopped()
        {
            for (int i = 0; i < systems.Length; i++)
            {
                if (systems[i] == null)
                {
                    continue;
                }

                if (systems[i].IsAlive(true))
                {
                    return false;
                }
            }

            return true;
        }

        private float GetLongestDuration()
        {
            float longest = 0f;

            for (int i = 0; i < systems.Length; i++)
            {
                ParticleSystem ps = systems[i];
                if (ps == null)
                {
                    continue;
                }

                ParticleSystem.MainModule main = ps.main;

                float duration = main.duration;

                if (main.startLifetime.mode == ParticleSystemCurveMode.TwoConstants)
                {
                    duration += main.startLifetime.constantMax;
                }
                else if (main.startLifetime.mode == ParticleSystemCurveMode.Constant)
                {
                    duration += main.startLifetime.constant;
                }
                else
                {
                    duration += fallbackReturnDelay;
                }

                if (main.loop)
                {
                    duration = fallbackReturnDelay;
                }

                if (duration > longest)
                {
                    longest = duration;
                }
            }

            return Mathf.Max(longest, fallbackReturnDelay);
        }

        private void ReturnToPool()
        {
            if (!_isActive)
            {
                return;
            }

            _isActive = false;

            if (_poolMember != null)
            {
                _poolMember.ReturnToPool();
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}
using System.Collections;
using UnityEngine;

namespace Pooling
{
    [RequireComponent(typeof(PoolMember))]
    public sealed class PooledTrail : MonoBehaviour, IPoolable
    {
        [SerializeField] private TrailRenderer trail;
        [SerializeField] private float speed = 200f;
        [SerializeField] private float minDistanceToTarget = 0.05f;
        [SerializeField] private float extraLifetime = 0.02f;

        private PoolMember _poolMember;
        private Coroutine _playRoutine;
        private bool _isReturning;

        private void Awake()
        {
            _poolMember = GetComponent<PoolMember>();

            if (trail == null)
            {
                trail = GetComponentInChildren<TrailRenderer>(true);
            }
        }

        public void Play(Vector3 start, Vector3 end)
        {
            transform.position = start;

            if (_playRoutine != null)
            {
                StopCoroutine(_playRoutine);
            }

            _playRoutine = StartCoroutine(PlayRoutine(end));
        }

        public void OnTakenFromPool()
        {
            _isReturning = false;

            if (trail != null)
            {
                trail.emitting = false;
                trail.Clear();
                trail.emitting = true;
            }
        }

        public void OnReturnedToPool()
        {
            _isReturning = true;

            if (_playRoutine != null)
            {
                StopCoroutine(_playRoutine);
                _playRoutine = null;
            }

            if (trail != null)
            {
                trail.emitting = false;
                trail.Clear();
            }
        }

        private IEnumerator PlayRoutine(Vector3 target)
        {
            while (!_isReturning)
            {
                Vector3 next = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
                transform.position = next;

                if ((next - target).sqrMagnitude <= minDistanceToTarget * minDistanceToTarget)
                {
                    transform.position = target;
                    break;
                }

                yield return null;
            }

            if (_isReturning)
            {
                yield break;
            }

            if (trail != null)
            {
                trail.emitting = false;
            }

            float waitTime = trail != null ? trail.time + extraLifetime : extraLifetime;
            if (waitTime > 0f)
            {
                yield return new WaitForSeconds(waitTime);
            }

            if (!_isReturning && _poolMember != null)
            {
                _isReturning = true;
                _poolMember.ReturnToPool();
            }

            _playRoutine = null;
        }
    }
}
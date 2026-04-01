using Combat;
using Pooling;
using ScriptableObjects;
using UnityEngine;

namespace Weapons
{
    public sealed class HitscanWeapon : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform muzzle;
        [SerializeField] private ParticleSystem muzzleFlash;
        [SerializeField] private PooledTrail trailPrefab;
        [SerializeField] private PooledParticleEffect hitEffectPrefab;

        [Header("Debug")]
        [SerializeField] private bool logDebugInfo;

        private WeaponDefinition _definition;
        private PoolManager _poolManager;
        private float _nextFireTime;

        public WeaponDefinition Definition => _definition;

        public void Initialize(WeaponDefinition definition, PoolManager poolManager)
        {
            _definition = definition;
            _poolManager = poolManager;
        }

        public bool CanFire()
        {
            return _definition != null
                   && _poolManager != null
                   && muzzle != null
                   && Time.time >= _nextFireTime;
        }

        public bool TryFire(Camera aimCamera, bool isAiming)
        {
            if (_definition == null)
            {
                DebugLog("TryFire failed: definition is null.");
                return false;
            }

            if (_poolManager == null)
            {
                DebugLog("TryFire failed: pool manager is null.");
                return false;
            }

            if (aimCamera == null)
            {
                DebugLog("TryFire failed: aim camera is null.");
                return false;
            }

            if (muzzle == null)
            {
                DebugLog("TryFire failed: muzzle is null.");
                return false;
            }

            if (Time.time < _nextFireTime)
            {
                return false;
            }

            float fireRate = Mathf.Max(0.01f, _definition.FireRate);
            _nextFireTime = Time.time + 1f / fireRate;

            Ray aimRay = aimCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            Vector3 aimDirection = ApplySpread(
                aimRay.direction,
                aimCamera.transform.right,
                aimCamera.transform.up,
                isAiming ? _definition.AimSpread : _definition.HipSpread);

            Vector3 targetPoint = aimRay.origin + aimDirection * _definition.Range;

            if (Physics.Raycast(
                    aimRay.origin,
                    aimDirection,
                    out RaycastHit cameraHit,
                    _definition.Range,
                    _definition.HitMask,
                    QueryTriggerInteraction.Ignore))
            {
                targetPoint = cameraHit.point;
            }

            Vector3 muzzleOrigin = muzzle.position;
            Vector3 shotDirection = (targetPoint - muzzleOrigin).normalized;

            PlayMuzzleFlash();

            if (Physics.Raycast(
                    muzzleOrigin,
                    shotDirection,
                    out RaycastHit hit,
                    _definition.Range,
                    _definition.HitMask,
                    QueryTriggerInteraction.Ignore))
            {
                SpawnTrail(muzzleOrigin, hit.point);
                SpawnHitEffect(hit.point, hit.normal);
                ApplyDamage(hit.collider);
            }
            else
            {
                Vector3 missPoint = muzzleOrigin + shotDirection * _definition.Range;
                SpawnTrail(muzzleOrigin, missPoint);
            }

            return true;
        }

        private void ApplyDamage(Collider hitCollider)
        {
            if (hitCollider == null)
            {
                return;
            }

            if (hitCollider.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(_definition.Damage);
                return;
            }

            IDamageable damageableInParent = hitCollider.GetComponentInParent<IDamageable>();
            if (damageableInParent != null)
            {
                damageableInParent.TakeDamage(_definition.Damage);
            }
        }

        private void PlayMuzzleFlash()
        {
            if (muzzleFlash != null)
            {
                muzzleFlash.Play();
            }
        }

        private Vector3 ApplySpread(Vector3 direction, Vector3 right, Vector3 up, float spreadAngle)
        {
            if (spreadAngle <= 0f)
            {
                return direction.normalized;
            }

            float spreadRadius = Mathf.Tan(spreadAngle * Mathf.Deg2Rad);
            Vector2 offset = Random.insideUnitCircle * spreadRadius;

            Vector3 spreadDirection = direction + right * offset.x + up * offset.y;
            return spreadDirection.normalized;
        }

        private void SpawnTrail(Vector3 from, Vector3 to)
        {
            PooledTrail prefab = trailPrefab != null ? trailPrefab : _definition.TrailPrefab;
            if (prefab == null)
            {
                return;
            }

            PooledTrail trail = _poolManager.Spawn<PooledTrail>(prefab.gameObject, from, Quaternion.identity);
            if (trail == null)
            {
                return;
            }

            trail.Play(from, to);
        }

        private void SpawnHitEffect(Vector3 position, Vector3 normal)
        {
            PooledParticleEffect prefab = hitEffectPrefab != null ? hitEffectPrefab : _definition.HitEffectPrefab;
            if (prefab == null)
            {
                return;
            }

            _poolManager.Spawn<PooledParticleEffect>(prefab.gameObject, position, Quaternion.LookRotation(normal));
        }

        private void DebugLog(string message)
        {
            if (!logDebugInfo)
            {
                return;
            }

            Debug.Log($"[HitscanWeapon] {message}", this);
        }
    }
}
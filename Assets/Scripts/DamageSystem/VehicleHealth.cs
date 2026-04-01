using Combat;
using Pooling;
using UnityEngine;

namespace Vehicles
{
    public sealed class VehicleHealth : MonoBehaviour, IDamageable
    {
        [Header("Health")]
        [SerializeField, Min(1f)] private float maxHp = 100f;

        [Header("Effects")]
        [SerializeField] private GameObject fireEffect;
        [SerializeField] private GameObject explosionPrefab;
        [SerializeField] private Transform explosionPoint;

        [Header("References")]
        [SerializeField] private PoolManager poolManager;

        [Header("Debug")]
        [SerializeField] private bool logDebugInfo;

        private float _currentHp;
        private bool _fireActivated;
        private bool _isDestroyed;

        public float CurrentHp => _currentHp;
        public float MaxHp => maxHp;
        public bool IsDestroyed => _isDestroyed;

        private void Awake()
        {
            _currentHp = maxHp;

            if (fireEffect != null)
            {
                fireEffect.SetActive(false);
            }
        }

        public void TakeDamage(float damage)
        {
            if (_isDestroyed)
            {
                return;
            }

            if (damage <= 0f)
            {
                return;
            }

            _currentHp -= damage;

            DebugLog($"Damage received: {damage}. Current HP: {_currentHp}/{maxHp}");

            if (!_fireActivated && _currentHp <= maxHp * 0.5f)
            {
                ActivateFire();
            }

            if (_currentHp <= 0f)
            {
                DestroyVehicle();
            }
        }

        private void ActivateFire()
        {
            _fireActivated = true;

            if (fireEffect != null)
            {
                fireEffect.SetActive(true);
            }

            DebugLog("Fire effect activated.");
        }

        private void DestroyVehicle()
        {
            if (_isDestroyed)
            {
                return;
            }

            _isDestroyed = true;

            SpawnExplosion();
            DebugLog("Vehicle destroyed.");

            Destroy(gameObject);
        }

        private void SpawnExplosion()
        {
            if (explosionPrefab == null)
            {
                DebugLog("Explosion prefab is missing.");
                return;
            }

            if (poolManager == null)
            {
                DebugLog("PoolManager is missing.");
                return;
            }

            Vector3 spawnPosition = explosionPoint != null
                ? explosionPoint.position
                : transform.position;

            Quaternion spawnRotation = explosionPoint != null
                ? explosionPoint.rotation
                : Quaternion.identity;

            PooledParticleEffect explosionInstance = poolManager.Spawn<PooledParticleEffect>(explosionPrefab.gameObject, spawnPosition, spawnRotation);
            
            if (explosionInstance == null)
            {
                DebugLog("Failed to spawn explosion from pool.");
                return;
            }

            DebugLog("Explosion spawned from pool.");
        }

        private void DebugLog(string message)
        {
            if (!logDebugInfo)
            {
                return;
            }

            Debug.Log($"[VehicleHealth] {message}", this);
        }
    }
}
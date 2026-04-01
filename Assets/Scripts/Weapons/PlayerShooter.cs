using Effects;
using Pooling;
using ScriptableObjects;
using UnityEngine;

namespace Weapons
{
    public sealed class PlayerShooter : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Camera aimCamera;
        [SerializeField] private WeaponHolder weaponHolder;
        [SerializeField] private PoolManager poolManager;
        [SerializeField] private CameraShake cameraShake;

        [Header("Debug")]
        [SerializeField] private bool logDebugInfo;

        private HitscanWeapon _cachedWeapon;
        private bool _isFiringHeld;

        public bool IsAiming { get; private set; }
        public bool HasWeapon => weaponHolder != null && weaponHolder.HasWeapon;

        public void SetAiming(bool value)
        {
            IsAiming = value;
        }

        public void PressFire()
        {
            WeaponDefinition definition = weaponHolder != null
                ? weaponHolder.CurrentDefinition
                : null;

            if (definition == null)
            {
                DebugLog("PressFire skipped: no weapon equipped.");
                return;
            }

            if (definition.IsAutomatic)
            {
                _isFiringHeld = true;
            }
            else
            {
                HitscanWeapon weapon = GetCurrentWeapon();
                if (weapon == null)
                {
                    return;
                }

                if (weapon.TryFire(aimCamera, IsAiming))
                {
                    PlayCameraShake(definition);
                }
            }
        }

        public void ReleaseFire()
        {
            _isFiringHeld = false;
        }

        private void Update()
        {
            if (!_isFiringHeld)
            {
                return;
            }

            WeaponDefinition definition = weaponHolder != null
                ? weaponHolder.CurrentDefinition
                : null;

            if (definition == null || !definition.IsAutomatic)
            {
                return;
            }

            HitscanWeapon weapon = GetCurrentWeapon();
            if (weapon == null)
            {
                return;
            }

            if (weapon.TryFire(aimCamera, IsAiming))
            {
                PlayCameraShake(definition);
            }
        }

        private HitscanWeapon GetCurrentWeapon()
        {
            if (weaponHolder == null)
            {
                return null;
            }

            GameObject currentInstance = weaponHolder.CurrentInstance;
            if (currentInstance == null)
            {
                _cachedWeapon = null;
                return null;
            }

            if (_cachedWeapon != null && _cachedWeapon.gameObject == currentInstance)
            {
                return _cachedWeapon;
            }

            _cachedWeapon = currentInstance.GetComponentInChildren<HitscanWeapon>();

            if (_cachedWeapon == null)
            {
                DebugLog($"Current weapon instance '{currentInstance.name}' has no HitscanWeapon.");
                return null;
            }

            if (_cachedWeapon.Definition == null && weaponHolder.CurrentDefinition != null)
            {
                _cachedWeapon.Initialize(weaponHolder.CurrentDefinition, poolManager);
            }

            return _cachedWeapon;
        }

        private void PlayCameraShake(WeaponDefinition definition)
        {
            if (cameraShake == null || definition == null)
            {
                return;
            }

            cameraShake.PlayShotShake(
                definition.ShakePosition,
                definition.ShakeRotation);
        }

        private void DebugLog(string message)
        {
            if (!logDebugInfo)
            {
                return;
            }

            Debug.Log($"[PlayerShooter] {message}", this);
        }
    }
}
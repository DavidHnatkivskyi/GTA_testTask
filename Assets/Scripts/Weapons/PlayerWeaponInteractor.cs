using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;

namespace Weapons
{
    public sealed class PlayerWeaponInteractor : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private WeaponHolder weaponHolder;
        [SerializeField] private Animator animator;
        [SerializeField] private Transform dropPoint;

        [Header("Animator Triggers")]
        [SerializeField] private string emptyTrigger = "Empty";
        [SerializeField] private string pistolTrigger = "Pistol";
        [SerializeField] private string rifleTrigger = "Rifle";

        [Header("Debug")]
        [SerializeField] private bool logDebugInfo = true;

        private readonly List<WeaponPickup> _availablePickups = new();
        private WeaponPickup _currentPickup;

        public WeaponPickup CurrentPickup => _currentPickup;
        public bool CanPickUp => _currentPickup != null && _currentPickup.Definition != null;
        public string CurrentPickupName => CanPickUp ? _currentPickup.Definition.DisplayName : string.Empty;

        public void RegisterPickup(WeaponPickup pickup)
        {
            if (pickup == null)
            {
                return;
            }

            if (_availablePickups.Contains(pickup))
            {
                RefreshCurrentPickup();
                return;
            }

            _availablePickups.Add(pickup);
            RefreshCurrentPickup();

            DebugLog($"Registered pickup: {(pickup.Definition != null ? pickup.Definition.DisplayName : pickup.name)}");
        }

        public void UnregisterPickup(WeaponPickup pickup)
        {
            if (pickup == null)
            {
                return;
            }

            _availablePickups.Remove(pickup);
            RefreshCurrentPickup();

            DebugLog($"Unregistered pickup: {(pickup.Definition != null ? pickup.Definition.DisplayName : pickup.name)}");
        }

        public void PickUpCurrentWeapon()
        {
            if (!CanPickUp)
            {
                DebugLog("PickUpCurrentWeapon skipped: no available pickup.");
                return;
            }

            if (weaponHolder == null)
            {
                DebugLog("PickUpCurrentWeapon failed: weaponHolder is not assigned.");
                return;
            }

            WeaponPickup pickedUpPickup = _currentPickup;
            WeaponDefinition newWeapon = pickedUpPickup.Definition;
            WeaponDefinition oldWeapon = weaponHolder.CurrentDefinition;

            bool equipped = weaponHolder.Equip(newWeapon);
            if (!equipped)
            {
                DebugLog("PickUpCurrentWeapon failed: could not equip new weapon.");
                return;
            }

            ApplyAnimation(newWeapon.WeaponType);

            if (oldWeapon != null)
            {
                SpawnDroppedWeapon(oldWeapon);
            }

            _availablePickups.Remove(pickedUpPickup);
            _currentPickup = null;

            pickedUpPickup.OnPickedUp();

            RefreshCurrentPickup();

            DebugLog($"Picked up weapon: {newWeapon.DisplayName}");
        }

        public void DropCurrentWeapon()
        {
            if (weaponHolder == null)
            {
                DebugLog("DropCurrentWeapon failed: weaponHolder is not assigned.");
                return;
            }

            if (!weaponHolder.HasWeapon)
            {
                DebugLog("DropCurrentWeapon skipped: no weapon equipped.");
                return;
            }

            WeaponDefinition oldWeapon = weaponHolder.Unequip();
            ApplyAnimation(WeaponType.None);

            if (oldWeapon != null)
            {
                SpawnDroppedWeapon(oldWeapon);
                DebugLog($"Dropped weapon: {oldWeapon.DisplayName}");
            }
        }

        public void ForceEquip(WeaponDefinition definition)
        {
            if (weaponHolder == null)
            {
                DebugLog("ForceEquip failed: weaponHolder is not assigned.");
                return;
            }

            if (definition == null)
            {
                DebugLog("ForceEquip failed: definition is null.");
                return;
            }

            bool equipped = weaponHolder.Equip(definition);
            if (!equipped)
            {
                DebugLog("ForceEquip failed: could not equip weapon.");
                return;
            }

            ApplyAnimation(definition.WeaponType);
            DebugLog($"Force equipped weapon: {definition.DisplayName}");
        }

        private void RefreshCurrentPickup()
        {
            CleanupNullPickups();

            WeaponPickup bestPickup = null;
            float bestDistanceSqr = float.MaxValue;
            Vector3 origin = transform.position;

            for (int i = 0; i < _availablePickups.Count; i++)
            {
                WeaponPickup pickup = _availablePickups[i];
                if (pickup == null || pickup.Definition == null)
                {
                    continue;
                }

                float distanceSqr = (pickup.transform.position - origin).sqrMagnitude;
                if (distanceSqr < bestDistanceSqr)
                {
                    bestDistanceSqr = distanceSqr;
                    bestPickup = pickup;
                }
            }

            _currentPickup = bestPickup;
        }

        private void CleanupNullPickups()
        {
            for (int i = _availablePickups.Count - 1; i >= 0; i--)
            {
                if (_availablePickups[i] == null)
                {
                    _availablePickups.RemoveAt(i);
                }
            }
        }

        private void SpawnDroppedWeapon(WeaponDefinition definition)
        {
            if (definition == null)
            {
                DebugLog("SpawnDroppedWeapon skipped: definition is null.");
                return;
            }

            if (definition.PickupPrefab == null)
            {
                DebugLog($"SpawnDroppedWeapon skipped: pickup prefab is missing for '{definition.name}'.");
                return;
            }

            Vector3 spawnPosition = dropPoint != null
                ? dropPoint.position
                : transform.position + transform.forward * 0.75f;

            Quaternion spawnRotation = Quaternion.identity;
            Instantiate(definition.PickupPrefab, spawnPosition, spawnRotation);
        }

        private void ApplyAnimation(WeaponType weaponType)
        {
            if (animator == null)
            {
                DebugLog("ApplyAnimation skipped: animator is not assigned.");
                return;
            }

            ResetWeaponTriggers();

            if (weaponType == WeaponType.None)
            {
                animator.SetTrigger(emptyTrigger);
            }
            else if (weaponType == WeaponType.Pistol)
            {
                animator.SetTrigger(pistolTrigger);
            }
            else if (weaponType == WeaponType.Rifle)
            {
                animator.SetTrigger(rifleTrigger);
            }
            else
            {
                DebugLog($"ApplyAnimation: unsupported weapon type {weaponType}.");
            }
        }

        private void ResetWeaponTriggers()
        {
            if (animator == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(emptyTrigger))
            {
                animator.ResetTrigger(emptyTrigger);
            }

            if (!string.IsNullOrEmpty(pistolTrigger))
            {
                animator.ResetTrigger(pistolTrigger);
            }

            if (!string.IsNullOrEmpty(rifleTrigger))
            {
                animator.ResetTrigger(rifleTrigger);
            }
        }

        private void DebugLog(string message)
        {
            if (!logDebugInfo)
            {
                return;
            }

            Debug.Log($"[PlayerWeaponInteractor] {message}", this);
        }
    }
}
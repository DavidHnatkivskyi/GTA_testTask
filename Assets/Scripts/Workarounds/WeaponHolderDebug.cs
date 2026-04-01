using ScriptableObjects;
using UnityEngine;
using Weapons;

namespace Workarounds
{
    public sealed class WeaponHolderDebug : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private WeaponHolder weaponHolder;
        [SerializeField] private PlayerWeaponInteractor weaponInteractor;

        [Header("Definitions")]
        [SerializeField] private WeaponDefinition pistol;
        [SerializeField] private WeaponDefinition carbine;

        [Header("Debug")]
        [SerializeField] private bool logDebugInfo = true;

        [ContextMenu("Equip Pistol")]
        private void EquipPistol()
        {
            if (weaponHolder == null)
            {
                DebugLog("Equip Pistol failed: weaponHolder is null.");
                return;
            }

            weaponHolder.Equip(pistol);
            DebugLog("Equipped pistol directly.");
        }

        [ContextMenu("Equip Carbine")]
        private void EquipCarbine()
        {
            if (weaponHolder == null)
            {
                DebugLog("Equip Carbine failed: weaponHolder is null.");
                return;
            }

            weaponHolder.Equip(carbine);
            DebugLog("Equipped carbine directly.");
        }

        [ContextMenu("Unequip")]
        private void Unequip()
        {
            if (weaponHolder == null)
            {
                DebugLog("Unequip failed: weaponHolder is null.");
                return;
            }

            WeaponDefinition removed = weaponHolder.Unequip();
            DebugLog(removed != null
                ? $"Unequipped: {removed.DisplayName}"
                : "Unequip skipped: no weapon.");
        }

        [ContextMenu("Drop Current Weapon")]
        private void DropCurrentWeapon()
        {
            if (weaponInteractor == null)
            {
                DebugLog("Drop Current Weapon failed: weaponInteractor is null.");
                return;
            }

            weaponInteractor.DropCurrentWeapon();
            DebugLog("DropCurrentWeapon called.");
        }

        [ContextMenu("Pick Up Current Weapon")]
        private void PickUpCurrentWeapon()
        {
            if (weaponInteractor == null)
            {
                DebugLog("Pick Up Current Weapon failed: weaponInteractor is null.");
                return;
            }

            weaponInteractor.PickUpCurrentWeapon();
            DebugLog("PickUpCurrentWeapon called.");
        }

        [ContextMenu("Log Current Weapon")]
        private void LogCurrentWeapon()
        {
            if (weaponHolder == null)
            {
                DebugLog("Log Current Weapon failed: weaponHolder is null.");
                return;
            }

            if (!weaponHolder.HasWeapon || weaponHolder.CurrentDefinition == null)
            {
                DebugLog("Current weapon: none.");
                return;
            }

            DebugLog($"Current weapon: {weaponHolder.CurrentDefinition.DisplayName}");
        }

        [ContextMenu("Log Current Pickup")]
        private void LogCurrentPickup()
        {
            if (weaponInteractor == null)
            {
                DebugLog("Log Current Pickup failed: weaponInteractor is null.");
                return;
            }

            if (!weaponInteractor.CanPickUp)
            {
                DebugLog("Current pickup: none.");
                return;
            }

            DebugLog($"Current pickup: {weaponInteractor.CurrentPickupName}");
        }

        private void DebugLog(string message)
        {
            if (!logDebugInfo)
            {
                return;
            }

            Debug.Log($"[WeaponHolderDebug] {message}", this);
        }
    }
}
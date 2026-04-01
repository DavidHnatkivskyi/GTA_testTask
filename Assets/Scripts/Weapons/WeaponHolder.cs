using System;
using ScriptableObjects;
using UnityEngine;

namespace Weapons
{
    public sealed class WeaponHolder : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform weaponSocket;

        [Header("Debug")]
        [SerializeField] private bool logDebugInfo = true;

        private WeaponDefinition _currentDefinition;
        private GameObject _currentInstance;

        public WeaponDefinition CurrentDefinition => _currentDefinition;
        public GameObject CurrentInstance => _currentInstance;
        public bool HasWeapon => _currentDefinition != null;

        public event Action<WeaponDefinition> WeaponEquipped;
        public event Action WeaponUnequipped;
        public event Action<WeaponDefinition> WeaponChanged;

        public bool Equip(WeaponDefinition definition)
        {
            if (definition == null)
            {
                DebugLog("Equip failed: definition is null.");
                return false;
            }

            if (definition.EquippedPrefab == null)
            {
                DebugLog($"Equip failed: equipped prefab is missing for '{definition.name}'.");
                return false;
            }

            if (weaponSocket == null)
            {
                DebugLog("Equip failed: weapon socket is not assigned.");
                return false;
            }

            ClearCurrentInternal();

            _currentDefinition = definition;
            _currentInstance = Instantiate(definition.EquippedPrefab, weaponSocket);

            DebugLog($"Equipped weapon: {_currentDefinition.DisplayName}");

            WeaponEquipped?.Invoke(_currentDefinition);
            WeaponChanged?.Invoke(_currentDefinition);

            return true;
        }

        public WeaponDefinition Unequip()
        {
            if (_currentDefinition == null)
            {
                DebugLog("Unequip skipped: no weapon equipped.");
                return null;
            }

            WeaponDefinition oldDefinition = _currentDefinition;

            ClearCurrentInternal();

            DebugLog($"Unequipped weapon: {oldDefinition.DisplayName}");

            WeaponUnequipped?.Invoke();
            WeaponChanged?.Invoke(null);

            return oldDefinition;
        }

        public WeaponDefinition Replace(WeaponDefinition newDefinition)
        {
            WeaponDefinition oldDefinition = _currentDefinition;

            bool equipped = Equip(newDefinition);
            if (!equipped)
            {
                return oldDefinition;
            }

            return oldDefinition;
        }

        public void Clear()
        {
            if (_currentDefinition == null && _currentInstance == null)
            {
                return;
            }

            ClearCurrentInternal();
            DebugLog("Weapon holder cleared.");

            WeaponUnequipped?.Invoke();
            WeaponChanged?.Invoke(null);
        }

        private void ClearCurrentInternal()
        {
            if (_currentInstance != null)
            {
                Destroy(_currentInstance);
            }

            _currentInstance = null;
            _currentDefinition = null;
        }

        private void DebugLog(string message)
        {
            if (!logDebugInfo)
            {
                return;
            }

            Debug.Log($"[WeaponHolder] {message}", this);
        }

        private void Reset()
        {
            if (weaponSocket == null)
            {
                weaponSocket = transform;
            }
        }
    }
}
using UnityEngine;

namespace Weapons
{
    public sealed class PlayerWeaponPickupDetector : MonoBehaviour
    {
        [SerializeField] private PlayerWeaponInteractor weaponInteractor;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out WeaponPickup pickup))
            {
                weaponInteractor.RegisterPickup(pickup);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out WeaponPickup pickup))
            {
                weaponInteractor.UnregisterPickup(pickup);
            }
        }
    }
}
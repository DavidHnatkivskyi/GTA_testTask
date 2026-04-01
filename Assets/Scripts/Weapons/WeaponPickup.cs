using ScriptableObjects;
using UnityEngine;

namespace Weapons
{
    [RequireComponent(typeof(Collider))]
    public sealed class WeaponPickup : MonoBehaviour
    {
        [SerializeField] private WeaponDefinition definition;

        public WeaponDefinition Definition => definition;

        public void Init(WeaponDefinition newDefinition)
        {
            definition = newDefinition;
        }

        public void OnPickedUp()
        {
            Destroy(gameObject);
        }

        private void Reset()
        {
            Collider col = GetComponent<Collider>();
            col.isTrigger = true;
        }
    }
}
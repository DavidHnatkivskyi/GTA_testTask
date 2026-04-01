using Pooling;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(
        fileName = "WeaponDefinition",
        menuName = "Game/Weapons/Weapon Definition")]
    public sealed class WeaponDefinition : ScriptableObject
    {
        [Header("Info")]
        [SerializeField] private string id;
        [SerializeField] private string displayName;

        [Header("Type")]
        [SerializeField] private WeaponType weaponType;

        [Header("Prefabs")]
        [SerializeField] private GameObject equippedPrefab;
        [SerializeField] private GameObject pickupPrefab;

        [Header("Fire")]
        [SerializeField] private FireMode fireMode = FireMode.SemiAuto;
        [SerializeField] [Min(0.01f)] private float fireRate = 4f;
        [SerializeField] [Min(0f)] private float damage = 20f;
        [SerializeField] [Min(0.1f)] private float range = 100f;
        [SerializeField] private LayerMask hitMask = ~0;

        [Header("Spread")]
        [SerializeField] [Min(0f)] private float hipSpread = 1.5f;
        [SerializeField] [Min(0f)] private float aimSpread = 0.2f;

        [Header("Ammo")]
        [SerializeField] [Min(1)] private int magazineSize = 12;
        [SerializeField] [Min(0f)] private float reloadTime = 1.2f;
        [SerializeField] private bool infiniteAmmo = true;

        [Header("FX")]
        [SerializeField] private AudioClip shotSfx;
        [SerializeField] private AudioClip reloadSfx;
        [SerializeField] private PooledParticleEffect hitEffectPrefab;
        [SerializeField] private GameObject muzzleFlashPrefab;
        [SerializeField] private PooledTrail trailPrefab;

        [Header("Camera Shake")]
        [SerializeField] [Min(0f)] private float shakePosition = 0.025f;
        [SerializeField] [Min(0f)] private float shakeRotation = 1.0f;

        public string Id => id;
        public string DisplayName => displayName;
        public WeaponType WeaponType => weaponType;

        public GameObject EquippedPrefab => equippedPrefab;
        public GameObject PickupPrefab => pickupPrefab;

        public FireMode FireMode => fireMode;
        public bool IsAutomatic => fireMode == FireMode.Auto;
        public float FireRate => fireRate;
        public float Damage => damage;
        public float Range => range;
        public LayerMask HitMask => hitMask;

        public float HipSpread => hipSpread;
        public float AimSpread => aimSpread;

        public int MagazineSize => magazineSize;
        public float ReloadTime => reloadTime;
        public bool InfiniteAmmo => infiniteAmmo;

        public AudioClip ShotSfx => shotSfx;
        public AudioClip ReloadSfx => reloadSfx;
        public PooledParticleEffect HitEffectPrefab => hitEffectPrefab;
        public GameObject MuzzleFlashPrefab => muzzleFlashPrefab;
        public PooledTrail TrailPrefab => trailPrefab;

        public float ShakePosition => shakePosition;
        public float ShakeRotation => shakeRotation;
    }

    public enum WeaponType
    {
        None = 0,
        Pistol = 1,
        Rifle = 2
    }

    public enum FireMode
    {
        SemiAuto = 0,
        Auto = 1
    }
}
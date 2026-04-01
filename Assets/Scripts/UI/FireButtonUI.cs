using ScriptableObjects;
using UnityEngine;
using UnityEngine.UIElements;
using Weapons;

namespace UI
{
    [RequireComponent(typeof(UIDocument))]
    public sealed class FireButtonUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerShooter shooter;
        [SerializeField] private WeaponHolder weaponHolder;

        private UIDocument _document;
        private VisualElement _button;

        private void Awake()
        {
            _document = GetComponent<UIDocument>();
        }

        private void OnEnable()
        {
            VisualElement root = _document.rootVisualElement;

            _button = root.Q<VisualElement>("fire-button");

            if (_button == null)
            {
                Debug.LogError("[FireButtonUI] fire-button not found.", this);
                return;
            }

            _button.RegisterCallback<PointerDownEvent>(OnDown);
            _button.RegisterCallback<PointerUpEvent>(OnUp);
            _button.RegisterCallback<PointerCancelEvent>(OnCancel);

            if (weaponHolder != null)
            {
                weaponHolder.WeaponChanged += OnWeaponChanged;
            }

            RefreshVisibility();
        }

        private void OnDisable()
        {
            if (_button != null)
            {
                _button.UnregisterCallback<PointerDownEvent>(OnDown);
                _button.UnregisterCallback<PointerUpEvent>(OnUp);
                _button.UnregisterCallback<PointerCancelEvent>(OnCancel);
            }

            if (weaponHolder != null)
            {
                weaponHolder.WeaponChanged -= OnWeaponChanged;
            }
        }

        private void OnWeaponChanged(WeaponDefinition _)
        {
            RefreshVisibility();
        }

        private void RefreshVisibility()
        {
            if (_button == null)
            {
                return;
            }

            bool shouldShow = shooter != null && shooter.HasWeapon;
            _button.style.display = shouldShow ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void OnDown(PointerDownEvent evt)
        {
            if (shooter == null || !shooter.HasWeapon)
            {
                return;
            }

            shooter.PressFire();
            evt.StopPropagation();
        }

        private void OnUp(PointerUpEvent evt)
        {
            shooter?.ReleaseFire();
            evt.StopPropagation();
        }

        private void OnCancel(PointerCancelEvent evt)
        {
            shooter?.ReleaseFire();
            evt.StopPropagation();
        }
    }
}
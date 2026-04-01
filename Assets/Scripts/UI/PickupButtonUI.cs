using UnityEngine;
using UnityEngine.UIElements;
using Weapons;

namespace UI
{
    [RequireComponent(typeof(UIDocument))]
    public sealed class PickupButtonUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerWeaponInteractor weaponInteractor;

        private UIDocument _document;
        private Button _button;

        private void Awake()
        {
            _document = GetComponent<UIDocument>();
        }

        private void OnEnable()
        {
            var root = _document.rootVisualElement;

            _button = root.Q<Button>("pickup-button");

            if (_button == null)
            {
                Debug.LogError("[PickupButtonUI] Button not found.");
                return;
            }

            _button.clicked += OnClick;
        }

        private void OnDisable()
        {
            if (_button != null)
            {
                _button.clicked -= OnClick;
            }
        }

        private void Update()
        {
            if (_button == null || weaponInteractor == null)
            {
                return;
            }

            if (weaponInteractor.CanPickUp)
            {
                _button.style.display = DisplayStyle.Flex;
                _button.text = $"Take {weaponInteractor.CurrentPickupName}";
            }
            else
            {
                _button.style.display = DisplayStyle.None;
            }
        }

        private void OnClick()
        {
            if (weaponInteractor == null)
            {
                return;
            }

            weaponInteractor.PickUpCurrentWeapon();
        }
    }
}
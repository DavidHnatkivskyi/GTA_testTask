using StarterAssets;
using UnityEngine;
using UI;

namespace InputBridges
{
    public sealed class JoystickToStarterAssetsBridge : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private OnScreenJoystick joystick;
        [SerializeField] private StarterAssetsInputs starterAssetsInputs;

        [Header("Settings")]
        [SerializeField] private bool normalizeInput = true;
        [SerializeField] private bool invertY = false;

        private void Reset()
        {
            starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        }

        private void Update()
        {
            if (joystick == null || starterAssetsInputs == null)
            {
                return;
            }

            Vector2 input = joystick.Value;

            if (invertY)
            {
                input.y = -input.y;
            }

            if (normalizeInput && input.sqrMagnitude > 1f)
            {
                input.Normalize();
            }

            starterAssetsInputs.move = input;
        }
    }
}
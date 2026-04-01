using Character;
using UnityEngine;

namespace UI
{
    public sealed class JoystickLookBridge : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private OnScreenJoystick lookJoystick;
        [SerializeField] private ThirdPersonCameraLook cameraLook;

        [Header("Settings")]
        [SerializeField] private bool invertY = true;

        private void Update()
        {
            if (lookJoystick == null || cameraLook == null)
            {
                return;
            }

            Vector2 input = lookJoystick.IsPressed
                ? lookJoystick.Value
                : Vector2.zero;

            if (invertY)
            {
                input.y = -input.y;
            }

            cameraLook.SetLookInput(input);
        }
    }
}
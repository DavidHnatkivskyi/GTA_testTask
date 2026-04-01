using UnityEngine;

namespace Character
{
    public sealed class ThirdPersonCameraLook : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform yawRoot;
        [SerializeField] private Transform pitchRoot;
        [SerializeField] private Transform characterRoot;

        [Header("Settings")]
        [SerializeField] private float sensitivityX = 180f;
        [SerializeField] private float sensitivityY = 120f;
        [SerializeField] private float minPitch = -35f;
        [SerializeField] private float maxPitch = 70f;
        [SerializeField] private bool rotateCharacterWithCamera = true;
        [SerializeField] private float characterTurnSpeed = 720f;

        [Header("Input")]
        [SerializeField] private bool useMouseInput = false;

        private float _yaw;
        private float _pitch;
        private Vector2 _lookInput;

        private void Start()
        {
            Vector3 yawEuler = yawRoot.rotation.eulerAngles;
            _yaw = yawEuler.y;

            float rawPitch = pitchRoot.localEulerAngles.x;
            if (rawPitch > 180f)
            {
                rawPitch -= 360f;
            }

            _pitch = rawPitch;
        }

        public void SetLookInput(Vector2 input)
        {
            _lookInput = input;
        }

        private void Update()
        {
            Vector2 lookInput = _lookInput;

            if (useMouseInput)
            {
                lookInput.x = Input.GetAxisRaw("Mouse X");
                lookInput.y = Input.GetAxisRaw("Mouse Y");
            }

            _yaw += lookInput.x * sensitivityX * Time.deltaTime;
            _pitch -= lookInput.y * sensitivityY * Time.deltaTime;
            _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);

            yawRoot.rotation = Quaternion.Euler(0f, _yaw, 0f);
            pitchRoot.localRotation = Quaternion.Euler(_pitch, 0f, 0f);

            RotateCharacter();
        }

        private void RotateCharacter()
        {
            if (!rotateCharacterWithCamera || characterRoot == null)
            {
                return;
            }

            Vector3 flatForward = yawRoot.forward;
            flatForward.y = 0f;

            if (flatForward.sqrMagnitude <= 0.0001f)
            {
                return;
            }

            Quaternion targetRotation = Quaternion.LookRotation(flatForward.normalized, Vector3.up);

            characterRoot.rotation = Quaternion.RotateTowards(
                characterRoot.rotation,
                targetRotation,
                characterTurnSpeed * Time.deltaTime);
        }
    }
}
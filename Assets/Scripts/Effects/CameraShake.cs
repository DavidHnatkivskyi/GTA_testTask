using UnityEngine;

namespace Effects
{
    public sealed class CameraShake : MonoBehaviour
    {
        [SerializeField] private Transform shakeTarget;
        [SerializeField] private float positionalDamping = 30f;
        [SerializeField] private float rotationalDamping = 35f;

        private Vector3 _currentPositionOffset;
        private Vector3 _currentRotationOffset;

        private Vector3 _positionVelocity;
        private Vector3 _rotationVelocity;

        private void Awake()
        {
            if (shakeTarget == null)
            {
                shakeTarget = transform;
            }
        }

        private void LateUpdate()
        {
            _currentPositionOffset = Vector3.SmoothDamp(
                _currentPositionOffset,
                Vector3.zero,
                ref _positionVelocity,
                1f / positionalDamping);

            _currentRotationOffset = Vector3.SmoothDamp(
                _currentRotationOffset,
                Vector3.zero,
                ref _rotationVelocity,
                1f / rotationalDamping);

            shakeTarget.localPosition = _currentPositionOffset;
            shakeTarget.localRotation = Quaternion.Euler(_currentRotationOffset);
        }

        public void PlayShotShake(float positionStrength, float rotationStrength)
        {
            Vector3 positionKick = new Vector3(
                Random.Range(-0.5f, 0.5f) * positionStrength,
                Random.Range(-0.5f, 0.5f) * positionStrength,
                -positionStrength);

            Vector3 rotationKick = new Vector3(
                -rotationStrength,
                Random.Range(-0.5f, 0.5f) * rotationStrength,
                Random.Range(-0.25f, 0.25f) * rotationStrength);

            _currentPositionOffset += positionKick;
            _currentRotationOffset += rotationKick;
        }
    }
}
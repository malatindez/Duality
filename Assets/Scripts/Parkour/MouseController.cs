using UnityEngine;

namespace Parkour
{
    public class MouseController : MonoBehaviour
    {
        private Transform _playerTransform;
        private Transform _cameraTransform;

        private Quaternion _characterTargetRot;
        private Quaternion _cameraTargetRot;

#pragma warning disable S1104 // Unity inspector
        public float XSensitivity = 2f;
        public float YSensitivity = 2f;
        public float MinimumX = -90F;
        public float MaximumX = 90F;
        public bool Smooth;
        public float SmoothTime = 5f;
#pragma warning restore S1104 // Unity inspector

        void Start()
        {
            _playerTransform = gameObject.transform;
            _cameraTransform = gameObject.GetComponentInChildren<Camera>().transform;
            if (_cameraTransform == null)
            {
                Debug.LogError("MouseController: Camera was not found");
            }

            _characterTargetRot = _playerTransform.localRotation;
            _cameraTargetRot = _cameraTransform.localRotation;

            Cursor.lockState = CursorLockMode.Locked;
        }

        /// <summary>
        /// Simply clamps rotation.
        /// </summary>
        private Quaternion ClampRotationAroundXAxis(Quaternion q)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

            angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);

            q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

            return q;
        }

        /// <summary>
        /// Rotates camera corresponding to input.
        /// </summary>
        private void InputRotation()
        {
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                return;
            }

            float yRot = Input.GetAxis("Mouse X") * XSensitivity;
            float xRot = Input.GetAxis("Mouse Y") * YSensitivity;

            _characterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
            _cameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

            _cameraTargetRot = ClampRotationAroundXAxis(_cameraTargetRot);

            if (Smooth)
            {
                _playerTransform.localRotation = Quaternion.Slerp(_playerTransform.localRotation, _characterTargetRot,
                    SmoothTime * Time.deltaTime);
                _cameraTransform.localRotation = Quaternion.Slerp(_cameraTransform.localRotation, _cameraTargetRot,
                    SmoothTime * Time.deltaTime);
            }
            else
            {
                _playerTransform.localRotation = _characterTargetRot;
                _cameraTransform.localRotation = _cameraTargetRot;
            }
        }

        void FixedUpdate()
        {
            InputRotation();
        }
    }
}

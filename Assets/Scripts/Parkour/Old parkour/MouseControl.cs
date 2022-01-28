// Class responsible for mouse controls.

using System;
using UnityEngine;

namespace Player.Controls
{
    /// <summary>
    /// Contains mouse settings and functions to allow mouse controls.
    /// </summary>
    [Serializable]
    public class MouseControl
    {
        private Quaternion _characterTargetRot;
        private Quaternion _cameraTargetRot;

#pragma warning disable S1104 // Unity can't serialize properties.
        public float XSensitivity = 2f;
        public float YSensitivity = 2f;
        public bool ClampVerticalRotation = true;
        public float MinimumX = -90F;
        public float MaximumX = 90F;
        public bool Smooth;
        public float SmoothTime = 5f;
#pragma warning restore S1104 // Unity can't serialize properties.

        public void Init(Transform character, Transform camera)
        {
            _characterTargetRot = character.localRotation;
            _cameraTargetRot = camera.localRotation;
        }

        public void LookRotation(Transform character, Transform camera)
        {
            Cursor.lockState = CursorLockMode.Locked;

            float yRot = Input.GetAxis("Mouse X") * XSensitivity;
            float xRot = Input.GetAxis("Mouse Y") * YSensitivity;

            _characterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
            _cameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

            if (ClampVerticalRotation)
                _cameraTargetRot = ClampRotationAroundXAxis(_cameraTargetRot);

            if (Smooth)
            {
                character.localRotation = Quaternion.Slerp(character.localRotation, _characterTargetRot,
                    SmoothTime * Time.deltaTime);
                camera.localRotation = Quaternion.Slerp(camera.localRotation, _cameraTargetRot,
                    SmoothTime * Time.deltaTime);
            }
            else
            {
                character.localRotation = _characterTargetRot;
                camera.localRotation = _cameraTargetRot;
            }
        }

        public void LookOveride(Transform character, Transform camera)
        {
            _characterTargetRot = character.localRotation;
            _cameraTargetRot = camera.localRotation;

            _characterTargetRot.x = 0f;
            _characterTargetRot.z = 0f;
            _cameraTargetRot.z = 0f;
            _cameraTargetRot.y = 0f;
        }

        public void CamGoBackAll(Transform character, Transform camera)
        {
            _cameraTargetRot.x = 0f;
            _cameraTargetRot.z = 0f;
            _cameraTargetRot.y = 0f;

            camera.localRotation = _cameraTargetRot;
        }
        public void CamGoBack(Transform character, Transform camera, float speed)
        {
            if (_cameraTargetRot.x > 0)
            {
                _cameraTargetRot.x -= 1f * Time.deltaTime * speed;

            }
            if (_cameraTargetRot.x < 0)
            {
                _cameraTargetRot.x += 1f * Time.deltaTime * speed;

            }

            if (_cameraTargetRot.y > 0)
            {
                _cameraTargetRot.y -= 1f * Time.deltaTime * speed;

            }
            if (_cameraTargetRot.y < 0)
            {
                _cameraTargetRot.y += 1f * Time.deltaTime * speed;

            }

            if (_cameraTargetRot.z > 0)
            {
                _cameraTargetRot.z -= 1f * Time.deltaTime * speed;

            }
            if (_cameraTargetRot.z < 0)
            {
                _cameraTargetRot.z += 1f * Time.deltaTime * speed;

            }

            camera.localRotation = _cameraTargetRot;
        }

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

    }
}

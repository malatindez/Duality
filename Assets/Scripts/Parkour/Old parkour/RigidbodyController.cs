using System;
using UnityEngine;

namespace Player.Controls
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class RigidbodyController : MonoBehaviour
    {
        private Rigidbody _rigidBody;
        private bool _isGrounded;

#pragma warning disable S1104 // Unity can't serialize properties.
        /// <summary>
        /// Enables or disabled mouse controls.
        /// </summary>
        public bool EnableMouseControls = true;

        /// <summary>
        /// Camera attached to player.
        /// </summary>
        public Camera CameraReference;

        /// <summary>
        /// Contains player speed and jump settings.
        /// </summary>
        public PlayerMovementSettings MovementSettings = new PlayerMovementSettings();

        /// <summary>
        /// Used to rotate camera when mouse input deteceted.
        /// </summary>
        public MouseControl MouseSettings = new MouseControl();

        /// <summary>
        /// 
        /// </summary>
        public DetectObstacles DetectGround;
#pragma warning restore S1104 // Unity can't serialize properties.

        public Vector3 RelativeVelocity;// { get; set; }

        public bool WallRunning { get; set; }

        public Vector3 Velocity
        {
            get { return _rigidBody.velocity; }
        }

        public bool Grounded
        {
            get { return _isGrounded; }
        }

        private void Awake()
        {
            _rigidBody = gameObject.GetComponent<Rigidbody>();

            MouseSettings.Init(gameObject.transform, CameraReference.transform);
        }

        private void Update()
        {
            RelativeVelocity = transform.InverseTransformDirection(_rigidBody.velocity);

            if (_isGrounded && Input.GetKeyDown(KeyCode.Space))
            {
                NormalJump();
            }
        }

        private void LateUpdate()
        {
            if (EnableMouseControls)
            {
                RotateView();
            }
            else
            {
                MouseSettings.LookOveride(transform, CameraReference.transform);
            }
        }

        private void FixedUpdate()
        {
            GroundCheck();
            Vector2 input = GetInput();

            float h = input.x;
            float v = input.y;
            Vector3 inputVector = new Vector3(h, 0, v);
            inputVector = Vector3.ClampMagnitude(inputVector, 1);

            //grounded
            if ((Mathf.Abs(input.x) > float.Epsilon || Mathf.Abs(input.y) > float.Epsilon) && _isGrounded && !WallRunning)
            {
                if (Input.GetAxisRaw("Vertical") > 0.3f)
                {
                    _rigidBody.AddRelativeForce(0, 0, Time.deltaTime * 1000f * MovementSettings.ForwardSpeed * Mathf.Abs(inputVector.z));
                }
                if (Input.GetAxisRaw("Vertical") < -0.3f)
                {
                    _rigidBody.AddRelativeForce(0, 0, Time.deltaTime * 1000f * -MovementSettings.BackwardSpeed * Mathf.Abs(inputVector.z));
                }
                if (Input.GetAxisRaw("Horizontal") > 0.5f)
                {
                    _rigidBody.AddRelativeForce(Time.deltaTime * 1000f * MovementSettings.StrafeSpeed * Mathf.Abs(inputVector.x), 0, 0);
                }
                if (Input.GetAxisRaw("Horizontal") < -0.5f)
                {
                    _rigidBody.AddRelativeForce(Time.deltaTime * 1000f * -MovementSettings.StrafeSpeed * Mathf.Abs(inputVector.x), 0, 0);
                }

            }
            //inair
            if ((Mathf.Abs(input.x) > float.Epsilon || Mathf.Abs(input.y) > float.Epsilon) && !_isGrounded  && !WallRunning)
            {
                if (Input.GetAxisRaw("Vertical") > 0.3f)
                {
                    _rigidBody.AddRelativeForce(0, 0, Time.deltaTime * 1000f * MovementSettings.SpeedInAir * Mathf.Abs(inputVector.z));
                }
                if (Input.GetAxisRaw("Vertical") < -0.3f)
                {
                    _rigidBody.AddRelativeForce(0, 0, Time.deltaTime * 1000f * -MovementSettings.SpeedInAir * Mathf.Abs(inputVector.z));
                }
                if (Input.GetAxisRaw("Horizontal") > 0.5f)
                {
                    _rigidBody.AddRelativeForce(Time.deltaTime * 1000f * MovementSettings.SpeedInAir * Mathf.Abs(inputVector.x), 0, 0);
                }
                if (Input.GetAxisRaw("Horizontal") < -0.5f)
                {
                    _rigidBody.AddRelativeForce(Time.deltaTime * 1000f * -MovementSettings.SpeedInAir * Mathf.Abs(inputVector.x), 0, 0);
                }

            }
        }

        public void CamGoBack(float speed)
        {
            MouseSettings.CamGoBack(transform, CameraReference.transform, speed);

        }

        public void CamGoBackAll()
        {
            MouseSettings.CamGoBackAll(transform, CameraReference.transform);

        }
        public void NormalJump()
        {
            _rigidBody.velocity = new Vector3(_rigidBody.velocity.x, 0f, _rigidBody.velocity.z);
            _rigidBody.AddForce(new Vector3(0f, MovementSettings.JumpForce, 0f), ForceMode.Impulse);
        }

        public void SwitchDirectionJump()
        {
            _rigidBody.velocity = transform.forward * _rigidBody.velocity.magnitude;
            _rigidBody.AddForce(new Vector3(0f, MovementSettings.JumpForce, 0f), ForceMode.Impulse);
        }

        private Vector2 GetInput()
        {
            Vector2 input = new Vector2
            {
                x = Input.GetAxisRaw("Horizontal"),
                y = Input.GetAxisRaw("Vertical")
            };

            MovementSettings.UpdateDesiredTargetSpeed(input);
            return input;
        }


        private void RotateView()
        {
            // Avoids the mouse looking if the game is effectively paused.
            if (Mathf.Abs(Time.timeScale) < float.Epsilon) return;

            MouseSettings.LookRotation(transform, CameraReference.transform);
        }


        // Checks if 
        private void GroundCheck()
        {
            if (DetectGround.IsColliding)
            {
                _isGrounded = true;
            }
            else
            {
                _isGrounded = false;
            }
        }
    }
}

using UnityEngine;

namespace Parkour
{
    [RequireComponent(typeof(Rigidbody))]
    public class ParkourController : MonoBehaviour
    {
        // I take assumption that mass of the rigidbody is 1.0f to simplify calculations.
        // In case, if you need to have runtime mass. Just replace this with _rigidbody.mass
        private const float Mass = 1.0f;

#pragma warning disable S1104 // Unity inspector
        [Space(10)]
        public Animator CameraAnimator;

        [Space(10)]

        public float WalkStartUpTime = 10.0f;
        public float WalkSpeed = 1.0f;
        public float StrafeSpeed = 2.0f;

        [UnityEngine.Header("Call RecalculateDrag() to change on run-time")]
        public float WalkFalloutTime = 0.1f;

        [Space(10)]

        public float RunStartUpTime = 1.0f;
        public float RunSpeed = 2.0f;

        [UnityEngine.Header("Call RecalculateDrag() to change on run-time")]
        public float RunFalloutTime = 0.1f;

#pragma warning restore S1104 // Unity inspector

        private Rigidbody _rigidbody;
        private CollisionManager _collisionManager;

        // Contains user input 
        private Vector3 _moveInput = new Vector3();

        // Local space velocity
        private Vector3 _relativeVelocity = new Vector3();

        // Timer for increasing startup speed
        private float _tWalkStartUp = 0.0f;
        private float _tRunStartUp = 0.0f;

        private float _currentDrag;
        private float _walkDrag;
        private float _runDrag;
        private float _crouchDrag;

        // TODO : implement this.
        // private bool _onMove = false
        // private float _startSpeed = 0.0f

        /// <summary>
        /// Drag is calculated at start. In case you need to change XXXFalloutTime in run-time, call this procedure.
        /// </summary>
        public void RecalculateDrag()
        {
            _walkDrag = (-1 * Mathf.Pow(0.1f / WalkSpeed, Time.fixedDeltaTime / WalkFalloutTime) + 1) / Time.fixedDeltaTime;
            _runDrag = (-1 * Mathf.Pow(0.1f / RunSpeed, Time.fixedDeltaTime / RunFalloutTime) + 1) / Time.fixedDeltaTime;
        }

        private void Start()
        {
            _rigidbody = gameObject.GetComponent<Rigidbody>();

            _collisionManager = gameObject.GetComponentInChildren<CollisionManager>();
            if (_collisionManager == null)
            {
                Debug.LogError("ParkourController: CollisionManager was not found");
            }

            RecalculateDrag();
        }

        private void Update()
        {
            _moveInput.x = Input.GetAxisRaw("Horizontal");
            _moveInput.y = Input.GetAxisRaw("Vertical");
            _moveInput.Normalize();

            bool jumpPressed = Input.GetKey(KeyCode.Space);
            bool shiftPressed = Input.GetKey(KeyCode.LeftShift);
            bool ctrlPressed = Input.GetKey(KeyCode.LeftControl);

            if (Input.GetKeyDown(KeyCode.K))
            {
                Debug.Log("OnDebug()");
            }




            /* Physics */
            _relativeVelocity = transform.InverseTransformDirection(_rigidbody.velocity);
            CameraAnimator.SetFloat("Velocity", (float)System.Math.Round(_relativeVelocity.z, 3));

            if (_collisionManager.Ground.IsColliding)
            {
                _rigidbody.useGravity = false;
                float wantedSpeed = 0.0f;

                if (_moveInput.y != 0.0f)
                {
                    if (shiftPressed && _relativeVelocity.magnitude >= WalkSpeed - 0.5f && _moveInput.y > 0.0f)
                    {
                        wantedSpeed = RunSpeed;
                        _currentDrag = _runDrag;
                    }
                    else
                    {
                        _tRunStartUp = 0.0f;

                        wantedSpeed = WalkSpeed;
                        _currentDrag = _walkDrag;
                    }
                }
                else
                {
                    _tWalkStartUp = 0.0f;
                }

                if (_relativeVelocity.magnitude - 0.1f <= wantedSpeed)
                {
                    if (wantedSpeed == RunSpeed)
                    {
                        _relativeVelocity.z = Mathf.Lerp(WalkSpeed, RunSpeed, _tRunStartUp / RunStartUpTime);
                        _tRunStartUp += Time.deltaTime;
                    }
                    else
                    {
                        _relativeVelocity.z = _moveInput.y * Mathf.Lerp(0.0f, WalkSpeed, _tWalkStartUp / WalkStartUpTime);
                        _tWalkStartUp += Time.deltaTime;
                    }
                }
                else
                {
                    // Custom drag.
                    _relativeVelocity.z *= Mathf.Clamp01(1.0f - _currentDrag * Time.deltaTime);
                }

                // It's important to always set it so no drifting occurs.
                _relativeVelocity.x = _moveInput.x * StrafeSpeed;
                _relativeVelocity.y = 0.0f;
            }

            _rigidbody.velocity = transform.TransformDirection(_relativeVelocity);
        }
    }
}

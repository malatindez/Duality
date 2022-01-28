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

            if (_collisionManager.Ground.IsColliding)
            {
                if (_moveInput.y != 0.0f)
                {
                    // Interpolate speed so it's accumulates during startup.
                    _relativeVelocity.z = _moveInput.y * Mathf.Lerp(0.0f, WalkSpeed, _tWalkStartUp / WalkStartUpTime);

                    // if(User pressed shift && Walk startup already finished and walks forwards)
                    if (shiftPressed && _relativeVelocity.z >= WalkSpeed)
                    {
                        _relativeVelocity.z = Mathf.Lerp(WalkSpeed, RunSpeed, _tRunStartUp / RunStartUpTime);
                        _tRunStartUp += Time.deltaTime;
                        _currentDrag = _runDrag;

                        CameraAnimator.SetBool("Run", true);
                    }
                    else
                    {
                        _tRunStartUp = 0.0f;
                        _currentDrag = _walkDrag;

                        CameraAnimator.SetBool("Run", false);
                    }

                    _tWalkStartUp += Time.deltaTime;

                    CameraAnimator.SetBool("Walk", true);
                }
                else
                {
                    _tWalkStartUp = 0.0f;

                    // Custom drag.
                    _relativeVelocity.z *= Mathf.Clamp01(1.0f - _currentDrag * Time.deltaTime);

                    // In case of no input, simply disable animation.
                    if (_moveInput.x == 0.0f)
                    {
                        CameraAnimator.SetBool("Walk", false);
                    }
                }

                if (_moveInput.x != 0)
                {
                    CameraAnimator.SetBool("Walk", true);
                }

                // It's important to always set it so no drifting occurs.
                _relativeVelocity.x = _moveInput.x * StrafeSpeed;
            }

            _rigidbody.velocity = transform.TransformDirection(_relativeVelocity);
        }
    }
}

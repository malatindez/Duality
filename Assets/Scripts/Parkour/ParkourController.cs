// Written by BerezkovN
// https://github.com/BerezkovN

using UnityEngine;

namespace Parkour
{
    [RequireComponent(typeof(Rigidbody))]
    public class ParkourController : MonoBehaviour
    {
        // I take an assumption that mass of the rigidbody is 1.0f to simplify calculations.
        // In case, if you need to have runtime mass. Just replace this with _rigidbody.mass
        private const float Mass = 1.0f;

#pragma warning disable S1104 // Unity inspector
        [Space()]
        [UnityEngine.Header("REFERENCES")]
        public Animator CameraAnimator;

        [Space(10)]
        [UnityEngine.Header("WALK")]

        public float WalkStartUpTime = 0.4f;
        public float WalkSpeed = 7.0f;
        public float StrafeSpeed = 7.0f;

        [Space(-10)]
        [UnityEngine.Header("   !- call RecalculateDrag() to change on run-time")]
        public float WalkFalloutTime = 0.5f;

        [Space(10)]
        [UnityEngine.Header("RUN")]

        public float RunStartUpTime = 1.0f;
        public float RunSpeed = 20.0f;

        [Space(-10)]
        [UnityEngine.Header("   !- call RecalculateDrag() to change on run-time")]
        public float RunFalloutTime = 2.0f;

        [Space(10)]

        public float AirAdditionalForce = 3.0f;

        [Space(10)]
        [UnityEngine.Header("VAULT AND CLIMB")]

        public float VaultTime = 0.6f;
        public float ClimbTime = 1.0f;

#pragma warning restore S1104 // Unity inspector

        private Rigidbody _rigidbody;
        private CollisionManager _collisionManager;
        private Transform _vaultEnd;
        private Transform _climbEnd;

        // Contains user input 
        private Vector3 _moveInput = new Vector3();

        // Local space velocity
        private Vector3 _relativeVelocity = new Vector3();

        // Timer for increasing startup speed
        private float _tWalkStartUp = 0.0f;
        private float _tRunStartUp = 0.0f;
        private float _tParkour = 0.0f;

        private float _currentDrag;
        private float _walkDrag;
        private float _runDrag;
        private float _crouchDrag;

        private Vector3 _parkourStartPosition = new Vector3();
        private Vector3 _parkourEndPosition = new Vector3();

        private readonly Utils.InvokeOnce _parkourOnce = new Utils.InvokeOnce();

        private readonly Utils.InvokeOnce _jumpOnce = new Utils.InvokeOnce();
        private float _onAirVelocity;

        private readonly Utils.InvokeOnce _startOnce = new Utils.InvokeOnce();
        private float _startSpeed = 0.0f;

        /// <summary>
        /// Drag is calculated at start. In case you need to change XXXFalloutTime in run-time, call this procedure.
        /// </summary>
        [ContextMenu("Recalculate Drag")]
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
                Debug.LogError("ParkourController: CollisionManager component was not found.");
            }

            _vaultEnd = gameObject.transform.RecursiveFind("VaultEnd");
            if (_vaultEnd == null)
            {
                Debug.LogError("ParkourController: VaultEnd gameobject was not found.");
            }
            else
            {
                _vaultEnd.position -= _collisionManager.Ground.gameObject.transform.localPosition;
            }

            _climbEnd = gameObject.transform.RecursiveFind("ClimbEnd");
            if (_climbEnd == null)
            {
                Debug.LogError("ParkourController: climbEnd gameobject was not found.");
            }
            else
            {
                _climbEnd.position -= _collisionManager.Ground.gameObject.transform.localPosition;
            }

            RecalculateDrag();
        }

        private void Update()
        {
            /* Get conrols */
            _moveInput.x = Input.GetAxisRaw("Horizontal");
            _moveInput.y = Input.GetAxisRaw("Vertical");

            bool jumpPressed = Input.GetKey(KeyCode.Space);
            bool shiftPressed = Input.GetKey(KeyCode.LeftShift);
            bool ctrlPressed = Input.GetKey(KeyCode.LeftControl);

            if (Input.GetKeyDown(KeyCode.K))
            {
                Debug.Log("OnDebug()");
            }

            /* Vault */
            if ((_collisionManager.VaultObject.IsColliding && !_collisionManager.VaultObstruction.IsColliding && jumpPressed && _moveInput.y > 0.0f)
                || (_parkourOnce.WasInvoked && _tParkour / VaultTime <= 1.0f))
            {
                _parkourOnce.Invoke(() =>
                {
                    _rigidbody.isKinematic = true;
                    CameraAnimator.CrossFade("Vault", 0.1f);

                    _parkourStartPosition = gameObject.transform.position;
                    _parkourEndPosition = _vaultEnd.position;
                });

                _tParkour += Time.deltaTime;
                gameObject.transform.position = Vector3.Lerp(_parkourStartPosition, _parkourEndPosition, _tParkour / VaultTime);
            }
            else
            {
                _parkourOnce.Reset();
                _tParkour = 0.0f;
                _rigidbody.isKinematic = false;
            }

            /* Physics */
            // Convert rigidbody's velocity to local velocity (it means that it takes rotation into account)
            // Temporarly not inside if statement.
            _relativeVelocity = transform.InverseTransformDirection(_rigidbody.velocity);
            Debug.Log(_relativeVelocity);

            // Camera FOV animation that depends on speed.
            if (!_collisionManager.VaultObject.IsColliding)
            {
                float interpolatedVelocity = _relativeVelocity.z * Mathf.Pow(_relativeVelocity.z / RunSpeed, 1.5f);
                CameraAnimator.SetFloat("Velocity", (float)System.Math.Round(interpolatedVelocity, 3));
            }

            if (_collisionManager.Ground.IsColliding)
            {
                /* Movement */
                _rigidbody.useGravity = false;
                float wantedSpeed = 0.0f;

                if (_moveInput.y != 0.0f)
                {
                    // If shift pressed AND already reached walk speed AND moving forwards
                    if (shiftPressed && _relativeVelocity.magnitude >= WalkSpeed - 0.5f && _moveInput.y > 0.0f)
                    {
                        _startSpeed = WalkSpeed;

                        wantedSpeed = RunSpeed;
                        _currentDrag = _runDrag;
                    }
                    else
                    {
                        _tRunStartUp = 0.0f;

                        _startOnce.Invoke(() =>
                        {
                            _startSpeed = -Mathf.Abs(_relativeVelocity.z);
                            _tWalkStartUp = 0.0f;
                        });

                        wantedSpeed = WalkSpeed;
                        _currentDrag = _walkDrag;
                    }
                }
                else
                {
                    _startOnce.Reset();
                    _tWalkStartUp = 0.0f;
                }

                // Constantly checking if we reached wanted speed
                if (Mathf.Abs(_relativeVelocity.z) - 0.1f <= wantedSpeed)
                {

                    if (wantedSpeed == RunSpeed)
                    {
                        _relativeVelocity.z = Mathf.Lerp(WalkSpeed, RunSpeed, _tRunStartUp / RunStartUpTime);
                        _tRunStartUp += Time.deltaTime;
                    }
                    else if (wantedSpeed == WalkSpeed)
                    {
                        _relativeVelocity.z = _moveInput.y * Mathf.Lerp(_startSpeed, WalkSpeed, _tWalkStartUp / WalkStartUpTime);
                        _tWalkStartUp += Time.deltaTime;
                    }
                }
                // If we exceeded wanted speed, we apply drag.
                else
                {
                    // I don't use rigibody's drag because I need to apply only to one z axis.
                    _relativeVelocity.z *= Mathf.Clamp01(1.0f - _currentDrag * Time.deltaTime);
                }

                // It's important to always set it so no drifting occurs.
                _relativeVelocity.x = _moveInput.x * StrafeSpeed;

                // In case something unexpected happens (like little bump) we don't start flying.
                _relativeVelocity.y = 0.0f;

                /* Jump */
                if (jumpPressed)
                {
                    _rigidbody.AddRelativeForce(Vector3.up * 4.0f, ForceMode.Impulse);
                }

                _jumpOnce.Reset();
            }
            else
            {
                _rigidbody.useGravity = true;

                if (!_collisionManager.Player.IsColliding)
                {
                    _jumpOnce.Invoke(() =>
                    {
                        _onAirVelocity = _relativeVelocity.z;
                    });

                    _relativeVelocity.z = _onAirVelocity;
                }
            }

            _rigidbody.velocity = transform.TransformDirection(_relativeVelocity);
        }
    }
}

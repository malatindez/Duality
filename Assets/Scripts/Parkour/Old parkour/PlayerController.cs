//

using UnityEngine;

namespace Player.Controls
{
    public class PlayerController : MonoBehaviour
    {
        public float drag_grounded;
        public float drag_inair;

        public DetectObstacles detectVaultObject; //checks for vault object
        public DetectObstacles detectVaultObstruction; //checks if theres somthing in front of the object e.g walls that will not allow the player to vault
        public DetectObstacles detectClimbObject; //checks for climb object
        public DetectObstacles detectClimbObstruction; //checks if theres somthing in front of the object e.g walls that will not allow the player to climb


        public DetectObstacles DetectWallL; //detects for a wall on the left
        public DetectObstacles DetectWallR; //detects for a wall on the right

        public Animator cameraAnimator;

        public float WallRunUpForce;
        public float WallRunUpForce_DecreaseRate;

        private float upforce;

        public float WallJumpUpVelocity;
        public float WallJumpForwardVelocity;
        public float drag_wallrun;
        public bool WallRunning;
        public bool WallrunningLeft;
        public bool WallrunningRight;
        private bool canwallrun; // ensure that player can only wallrun once before needing to hit the ground again, can be modified for double wallruns

        public bool IsParkour;
        private float t_parkour;
        private float chosenParkourMoveTime;

        private bool CanVault;
        public float VaultTime; //how long the vault takes
        public Transform VaultEndPoint;

        private bool CanClimb;
        public float ClimbTime; //how long the vault takes
        public Transform ClimbEndPoint;

        private RigidbodyController _rbfps;
        private Rigidbody _rbody;
        private Vector3 _recordedMoveToPosition; //the position of the vault end point in world space to move the player to
        private Vector3 _recordedStartPosition; // position of player right before vault

        void Start()
        {
            _rbfps = gameObject.GetComponent<RigidbodyController>();
            _rbody = gameObject.GetComponent<Rigidbody>();
        }

        void Update()
        {
            if (_rbfps.Grounded)
            {
                _rbody.drag = drag_grounded;
                canwallrun = true;
            }
            else
            {
                _rbody.drag = drag_inair;
            }
            if (WallRunning)
            {
                _rbody.drag = drag_wallrun;

            }
            //vault
            if (detectVaultObject.IsColliding && !detectVaultObstruction.IsColliding && !CanVault && !IsParkour && !WallRunning
                && (Input.GetKey(KeyCode.Space) || !_rbfps.Grounded) && Input.GetAxisRaw("Vertical") > 0f)
            // if detects a vault object and there is no wall in front then player can pressing space or in air and pressing forward
            {
                CanVault = true;
            }

            if (CanVault)
            {
                CanVault = false; // so this is only called once
                _rbody.isKinematic = true; //ensure physics do not interrupt the vault
                _recordedMoveToPosition = VaultEndPoint.position;
                _recordedStartPosition = transform.position;
                IsParkour = true;
                chosenParkourMoveTime = VaultTime;

                cameraAnimator.CrossFade("Vault", 0.1f);
            }

            //climb
            if (detectClimbObject.IsColliding && !detectClimbObstruction.IsColliding && !CanClimb && !IsParkour && !WallRunning
                && (Input.GetKey(KeyCode.Space) || !_rbfps.Grounded) && Input.GetAxisRaw("Vertical") > 0f)
            {
                CanClimb = true;
            }

            if (CanClimb)
            {
                CanClimb = false; // so this is only called once
                _rbody.isKinematic = true; //ensure physics do not interrupt the vault
                _recordedMoveToPosition = ClimbEndPoint.position;
                _recordedStartPosition = transform.position;
                IsParkour = true;
                chosenParkourMoveTime = ClimbTime;

                cameraAnimator.CrossFade("Climb", 0.1f);
            }


            //Parkour movement
            if (IsParkour && t_parkour < 1f)
            {
                t_parkour += Time.deltaTime / chosenParkourMoveTime;
                transform.position = Vector3.Lerp(_recordedStartPosition, _recordedMoveToPosition, t_parkour);

                if (t_parkour >= 1f)
                {
                    IsParkour = false;
                    t_parkour = 0f;
                    _rbody.isKinematic = false;

                }
            }


            //Wallrun
            if (DetectWallL.IsColliding && !_rbfps.Grounded && !IsParkour && canwallrun) // if detect wall on the left and is not on the ground and not doing parkour(climb/vault)
            {
                WallrunningLeft = true;
                canwallrun = false;
                upforce = WallRunUpForce; //refer to line 186
            }

            if (DetectWallR.IsColliding && !_rbfps.Grounded && !IsParkour && canwallrun) // if detect wall on thr right and is not on the ground
            {
                WallrunningRight = true;
                canwallrun = false;
                upforce = WallRunUpForce;
            }
            if (WallrunningLeft && !DetectWallL.IsColliding || Input.GetAxisRaw("Vertical") <= 0f || _rbfps.RelativeVelocity.magnitude < 1f) // if there is no wall on the lef tor pressing forward or forward speed < 1 (refer to fpscontroller script)
            {
                WallrunningLeft = false;
                WallrunningRight = false;
            }
            if (WallrunningRight && !DetectWallR.IsColliding || Input.GetAxisRaw("Vertical") <= 0f || _rbfps.RelativeVelocity.magnitude < 1f) // same as above
            {
                WallrunningLeft = false;
                WallrunningRight = false;
            }

            if (WallrunningLeft || WallrunningRight)
            {
                WallRunning = true;
                _rbfps.WallRunning = true; // this stops the playermovement (refer to fpscontroller script)
            }
            else
            {
                WallRunning = false;
                _rbfps.WallRunning = false;
            }

            if (WallrunningLeft)
            {
                cameraAnimator.SetBool("WallLeft", true); //Wallrun camera tilt
            }
            else
            {
                cameraAnimator.SetBool("WallLeft", false);
            }
            if (WallrunningRight)
            {
                cameraAnimator.SetBool("WallRight", true);
            }
            else
            {
                cameraAnimator.SetBool("WallRight", false);
            }

            if (WallRunning)
            {

                _rbody.velocity = new Vector3(_rbody.velocity.x, upforce, _rbody.velocity.z); //set the y velocity while wallrunning
                upforce -= WallRunUpForce_DecreaseRate * Time.deltaTime; //so the player will have a curve like wallrun, upforce from line 136

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    _rbody.velocity = transform.forward * WallJumpForwardVelocity + transform.up * WallJumpUpVelocity; //walljump
                    WallrunningLeft = false;
                    WallrunningRight = false;
                }
                if (_rbfps.Grounded)
                {
                    WallrunningLeft = false;
                    WallrunningRight = false;
                }
            }


        }

    }
}

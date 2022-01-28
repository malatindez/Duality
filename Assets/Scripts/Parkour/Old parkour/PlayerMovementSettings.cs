using UnityEngine;

namespace Player.Controls
{
    [System.Serializable]
    public class PlayerMovementSettings
    {
#pragma warning disable S1104 // Unity can't serialize properties.
        /// <summary>
        /// Speed when walking forwards.
        /// </summary>
        public float ForwardSpeed = 2.0f;

        /// <summary>
        /// Speed when walking backwards.
        /// </summary>
        public float BackwardSpeed = 2.0f;

        /// <summary>
        /// Speed when you strafe (walking left and right).
        /// </summary>
        public float StrafeSpeed = 3.0f;

        /// <summary>
        /// Speed when player doesn't touch ground.
        /// </summary>
        public float SpeedInAir = 0.1f;

        /// <summary>
        /// Force applied when jump occurs.
        /// </summary>
        public float JumpForce = 7f;
#pragma warning restore S1104 // Unity can't serialize properties.

        /// <summary>
        /// Unkown purpose.
        /// TODO: remove this property.
        /// </summary>
        public float CurrentTargetSpeed { get; set; } = 8f;

        public void UpdateDesiredTargetSpeed(Vector2 input)
        {
            if (input == Vector2.zero)
            {
                return;
            }

            // Strafe.
            if (input.x > 0 || input.x < 0)
            {
                CurrentTargetSpeed = StrafeSpeed;
            }

            // Backwards.
            if (input.y < 0)
            {
                CurrentTargetSpeed = BackwardSpeed;
            }

            // Forwards.
            // Handled last as if strafing and moving forward at the same time forwards speed should take precedence.
            if (input.y > 0)
            {
                CurrentTargetSpeed = ForwardSpeed;
            }
        }
    }
}

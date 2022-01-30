using UnityEngine;

namespace Parkour
{
    [RequireComponent(typeof(Collider))]
    public class CollisionDetector : MonoBehaviour
    {
#pragma warning disable S1104 // Unity inspector

        /// <summary>
        /// Tag which collision should be detected with.
        /// </summary>
        public string TagToDetect;

#pragma warning restore S1104 // Unity inspector

        /// <summary>
        /// Property is true whenever collider stays.
        /// </summary>
        public bool IsColliding { get; private set; }

        /// <summary>
        /// Detected collider.
        /// </summary>
        public Collider DetectedCollider { get; private set; }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(TagToDetect))
            {
                IsColliding = true;
                DetectedCollider = other;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(TagToDetect))
            {
                IsColliding = false;
                DetectedCollider = null;
            }
        }
    }
}

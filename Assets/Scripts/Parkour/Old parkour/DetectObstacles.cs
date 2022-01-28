using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player.Controls
{
    public class DetectObstacles : MonoBehaviour
    {
        private Collider _collider;

#pragma warning disable S1104 // Unity can't serialize properties.
        public string ObjectTagName = "";
#pragma warning restore S1104 // Unity can't serialize properties.

        /// <summary>
        /// Set to true when 
        /// </summary>
        public bool IsColliding { get; set; }
        public GameObject Object { get; set; }

        void OnTriggerStay(Collider collider)
        {
            if (!IsColliding && ObjectTagName != "")
            {
                var customTag = collider.GetComponent<CustomTag>();
                if (!collider.isTrigger && customTag != null && customTag.IsEnabled && customTag.HasTag(ObjectTagName))
                {
                    IsColliding = true;
                    Object = collider.gameObject;
                    _collider = collider;
                }

                // Next if check definetly will not happen.
                return;
            }

            if (ObjectTagName == "" && !IsColliding && collider != null && !collider.isTrigger)
            {
                IsColliding = true;
                Object = collider.gameObject;
                _collider = collider;
            }
        }

        void OnTriggerExit(Collider col)
        {
            if (col == _collider)
            {
                IsColliding = false;
            }

        }
    }
}

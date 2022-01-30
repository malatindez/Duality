using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Parkour
{
    public class CollisionManager : MonoBehaviour
    {
        public CollisionDetector Player;

        public CollisionDetector Ground { get; set; }
        public CollisionDetector VaultObject { get; set; }
        public CollisionDetector VaultObstruction { get; set; }
        public CollisionDetector ClimbObject { get; set; }
        public CollisionDetector ClimbObstruction { get; set; }

        /// <summary>
        /// Finds child GameObject with name <paramref name="gameObjectName"/> and returns component's reference.
        /// </summary>
        /// <param name="gameObjectName">Name of child GameObject</param>
        private CollisionDetector InitDetector(string gameObjectName)
        {
            CollisionDetector detector = null;

            Transform groundTransform = gameObject.transform.RecursiveFind(gameObjectName);
            if (groundTransform != null)
            {
                detector = groundTransform.GetComponent<CollisionDetector>();

                if (detector == null)
                {
                    Debug.LogError($"CollisionManager: {groundTransform.gameObject.GetFullName()} is missing component CollisionDetector.");
                }
            }
            else
            {
                Debug.LogError($"CollisionManager: {gameObject.GetFullName()} is missing child { gameObjectName }.");
            }

            return detector;
        }

        void Start()
        {
            // Initializing all detectors.
            // I decided not to get references of detectors through inspector.
            // So it's just finds them by names.

            Ground = InitDetector(nameof(Ground));

            VaultObject = InitDetector(nameof(VaultObject));
            VaultObstruction = InitDetector(nameof(VaultObstruction));

            ClimbObject = InitDetector(nameof(ClimbObject));
            ClimbObstruction = InitDetector(nameof(ClimbObstruction));
        }
    }

}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Parkour
{
    public class CollisionManager : MonoBehaviour
    {
        public CollisionDetector Ground { get; set; }

        /// <summary>
        /// Finds child GameObject with name <paramref name="gameObjectName"/> and returns component's reference.
        /// </summary>
        /// <param name="gameObjectName">Name of child GameObject</param>
        private CollisionDetector InitDetector(string gameObjectName)
        {
            CollisionDetector detector = null;

            Transform groundTransform = gameObject.transform.Find(gameObjectName);
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
        }
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Abilities
{
    public class Teleportation : Ability
    {
        [SerializeField] private float _range = 1000;
        [SerializeField] private AudioClip _teleportationSound;

        private RaycastHit _lastRaycastHit;
        private Camera _camera;

        protected override void StartInternal()
        {
            _camera = Camera.main;
        }

        protected override void UpdateInternal()
        {
            // Intentionally unimplemented
        }

        public override void Use(AbilityContext context)
        {
            if (GetLookedAtObject() != null)
                TeleportToLookAt();
        }

        private GameObject GetLookedAtObject()
        {
            Vector3 origin = transform.position;
            Vector3 direaction = _camera.transform.forward;

            if (Physics.Raycast(origin, direaction, out _lastRaycastHit, _range))
            {
                return _lastRaycastHit.collider.gameObject;
            }
            else
            {
                return null;
            }
        }

        private void TeleportToLookAt()
        {
            transform.position = _lastRaycastHit.point + _lastRaycastHit.normal * 1.5f;
            if (_teleportationSound != null)
                AudioSource.PlayClipAtPoint(_teleportationSound, transform.position);
        }
    }
}

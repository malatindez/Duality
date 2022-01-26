using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Abilities
{
    public class Timescape : Ability
    {
        private CameraController _controller;
        protected override void StartInternal()
        {
            _controller = GetComponent<CameraController>();
        }

        protected override void UpdateInternal()
        {
            // Intentionally unimplemented
        }

        public override void Use(AbilityContext context)
        {
            _controller.SwitchCameras();
        }
    }
}
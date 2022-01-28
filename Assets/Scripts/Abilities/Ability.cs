using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Abilities
{
    public abstract class Ability : MonoBehaviour
    {
        private Sprite _sprite;
        public Sprite Sprite { get => _sprite; }
        private void Awake()
        {
            var texture = Resources.Load("sprites/" + GetType().Name) as Texture2D;
            _sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }

        private void Start()
        {
            StartInternal();
        }

        private void Update()
        {
            UpdateInternal();
        }
        protected abstract void StartInternal();
        protected abstract void UpdateInternal();
        public abstract void Use(AbilityContext context);
    }
}
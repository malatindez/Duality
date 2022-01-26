using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Abilities
{
    public class AbilityController : MonoBehaviour
    {
        [SerializeField] private Image _selectedAbilitySprite;

        private List<Ability> _abilities;
        private Ability? _selectedAbility;
        private Sprite _noneAbility;

        private Ability? SelectedAbility 
        { 
            get => _selectedAbility; 
            set 
            { 
                _selectedAbility = value;
                if (value == null) { _selectedAbilitySprite.sprite = _noneAbility; }
                else { _selectedAbilitySprite.sprite = value.Sprite; }
            }
        }


        private void Start()
        {
            var texture = Resources.Load("sprites/none") as Texture2D;
            _noneAbility = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            _abilities = new List<Ability>
            {
                gameObject.AddComponent<Teleportation>(),
                gameObject.AddComponent<Timescape>()
            };
            SelectedAbility = null;
        }

        private int i = 0;
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (i == 2)
                {
                    i = 0;
                    SelectedAbility = null;
                    return;
                }
                SelectedAbility = _abilities[i];
                i++;
            }
        }
    }
}

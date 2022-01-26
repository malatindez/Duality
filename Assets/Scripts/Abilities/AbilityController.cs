using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Abilities
{
    public class AbilityController : MonoBehaviour
    {
        [SerializeField] private Image _selectedAbilitySprite;
        [SerializeField] private Image _abilityImage;
        [SerializeField] private RawImage _arrowImage;
        [SerializeField] private GameObject _wheel;

        private List<GameObject> _abilitiesWheel = new List<GameObject>();
        private List<Ability> _abilities = new List<Ability>();
        private Ability? _selectedAbility;

        private Ability? SelectedAbility 
        { 
            get => _selectedAbility; 
            set 
            { 
                _selectedAbility = value;
                if (value == null) 
                { 
                    _selectedAbilitySprite.transform.gameObject.SetActive(false); 
                }
                else 
                {
                    _selectedAbilitySprite.transform.gameObject.SetActive(true);
                    _selectedAbilitySprite.sprite = value.Sprite; 
                }
            }
        }


        private void Start()
        {
            _abilities = new List<Ability>
            {
                gameObject.AddComponent<Teleportation>(),
                gameObject.AddComponent<Timescape>()
            };
            SelectedAbility = null;
            float angle = 2 * Mathf.PI / _abilities.Count;
            for(int i = 0; i < _abilities.Count; i++)
            {
                GameObject ability = Instantiate(_abilityImage.transform.gameObject, _wheel.transform);
                ability.SetActive(true);
                ability.GetComponent<Image>().sprite = _abilities[i].Sprite;
                ability.transform.localPosition =
                    new Vector3(Mathf.Cos(angle * i), Mathf.Sin(angle * i)) * ability.transform.localPosition.magnitude;
                _abilitiesWheel.Add(ability);
            }

        }

        private float _timeScaleBuf;
        private float _mmbDelay = 0; // 50 ms before window will be active
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse2))
            {
                _timeScaleBuf = Time.timeScale;
                _wheel.SetActive(true);
                Time.timeScale = 0;
                Cursor.lockState = CursorLockMode.Confined;
                SelectedAbility = null;
                _mmbDelay = Time.realtimeSinceStartup + 0.05f;
            }
            // So that if the button was just clicked — selected ability will remain null
            else if (Input.GetKey(KeyCode.Mouse2) && Time.realtimeSinceStartup > _mmbDelay)
            {
                Vector2 vec = new Vector2(Screen.width / 2, Screen.height / 2);
                vec -= new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                vec = vec.normalized;
                float angle = Vector2.SignedAngle(new Vector2(-1, 0), vec);
                _arrowImage.transform.eulerAngles = new Vector3(0, 0, angle);
                angle += 180;
                angle = 360 - angle;
                if (angle > 90)
                {
                    angle -= 90;
                } else
                {
                    angle += 270;
                }
                float range = 360.0f / _abilities.Count;
                int i = -1;
                angle -= range / 2;
                float t = -range / 2;
                do
                {
                    i++; t += range;
                } while (angle > t);
                SelectedAbility = _abilities[i];
            }
            else if (Input.GetKeyUp(KeyCode.Mouse2))
            {
                Time.timeScale  = _timeScaleBuf;
                _wheel.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
            }


            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                if(SelectedAbility != null)
                {
                    SelectedAbility.Use(new AbilityContext());
                }
            }
        }
    }
}

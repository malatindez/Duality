using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fadeout : MonoBehaviour
{
    [SerializeField] Image _image;
    // Start is called before the first frame update

    float _beginning = 0f;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _beginning = Time.realtimeSinceStartup;
        }
        else if (Input.GetKey(KeyCode.Escape))
        {
            float t = Time.realtimeSinceStartup - _beginning;
            _image.color = new Color(0, 0, 0, 255 * (1 - Mathf.Log10(1 / (0.1f + t / 2.5f))));
            if(t > 2.5f)
            {
                Application.Quit();
            }
        }
        else if (Input.GetKeyUp(KeyCode.Escape))
        {
            _beginning = 0f;
        }
    }
}

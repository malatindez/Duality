using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Camera _camera;
    Camera _alternateCamera;

    private void Awake()
    {
        // Create alternate camera
        _alternateCamera = Instantiate(_camera, _camera.transform.parent);

        // Bind alternate camera to render texture
        var AlternateView = new RenderTexture(Screen.width, Screen.height, 24);
        Shader.SetGlobalTexture("AlternateCamera", AlternateView);
        _alternateCamera.targetTexture = AlternateView;

        // Change culling mask so it renders only the main universe
        _camera.cullingMask |= 1 << LayerMask.NameToLayer("Main Universe");
        _camera.cullingMask &= ~(1 << LayerMask.NameToLayer("Alternate Universe"));

        // Change culling mask so it renders only the alternate universe
        _alternateCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("Main Universe"));
        _alternateCamera.cullingMask |= 1 << LayerMask.NameToLayer("Alternate Universe");
    }

    [ContextMenu("SwitchCameras")]
    public void SwitchCameras()
    {
        var buf = _camera.cullingMask;
        _camera.cullingMask = _alternateCamera.cullingMask;
        _alternateCamera.cullingMask = buf;
    }
}

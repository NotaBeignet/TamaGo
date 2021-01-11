using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class EnvironmentInteraction : MonoBehaviour 
{

    [SerializeField]
    RenderTexture m_interactionRT;

#if UNITY_EDITOR
    [SerializeField]
    Transform m_lockToTransform;
#endif
    private void Start()
    {
        Shader.SetGlobalFloat("_environmentInteractionCaptureSize", GetComponent<Camera>().orthographicSize);
    }

    void Update () 
    {
#if UNITY_EDITOR
        transform.position = m_lockToTransform.position;
#else
        transform.position = CameraManager.Instance.Camera.transform.position;
#endif
        if (m_interactionRT != null)
        {
            m_interactionRT.SetGlobalShaderProperty("_interactionRT");

            Shader.SetGlobalVector("_interactionCameraPos", transform.position);
        }
	}
}

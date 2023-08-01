using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameManager
{
    [Header("First person view Streaming")]
    public GameObject m_webcamCapture;
    public GameObject m_firstPersonStreamingSettings;

    [HideInInspector] public WebCamTexture webCamTexture;

    public void StartStreaming()
    {
        m_firstPersonStreamingSettings.SetActive(true);

        virtualObjectsCamera.gameObject.SetActive(true);
        virtualObjectsCamera.fieldOfView = ARCamera.GetComponent<Camera>().fieldOfView;
        virtualObjectsCamera.clearFlags = CameraClearFlags.SolidColor;


        realObjectsCamera.gameObject.SetActive(true);
        realObjectsCamera.aspect = (32f / 9f);
        //m_webcamCapture.transform.localScale = new Vector3(4f, 2.25f, 1);
        //m_webcamCapture.transform.localPosition = new Vector3(0, 0, 4);
        //m_webcamCapture.transform.localEulerAngles = new Vector3(0, 0, 0);

        webCamTexture = new WebCamTexture(500, 282, 60);
        Renderer videoBackgroundRenderer = m_webcamCapture.GetComponent<Renderer>();
        videoBackgroundRenderer.material.mainTexture = webCamTexture;


        webCamTexture.Play();

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameManager
{
    [Header("First person view Streaming")]
    public Camera m_ARCamera;
    public Camera m_webcamTextureCaptureCamera;
    public GameObject ViewQuadCapture;
    public GameObject firstPersonStreamingSettings;

    public WebCamTexture webCamTexture;

    public void StartStreaming()
    {
        firstPersonStreamingSettings.SetActive(true);
        firstPersonViewStreamCamera.gameObject.SetActive(true);

        //Create Quad
        GameObject quadCapture = GameObject.Instantiate(ViewQuadCapture);
        // GameObject quadCapture1 = GameObject.Instantiate(ViewQuadCapture);

        quadCapture.transform.parent = m_ARCamera.transform;
        quadCapture.transform.localScale = new Vector3(4, 3f, -1);
        quadCapture.transform.localPosition = new Vector3(0, 0, 4);
        // videoBackground.transform.localEulerAngles = videoBackgroundEuler;
        quadCapture.transform.localEulerAngles = new Vector3(0, 0, 0);

        firstPersonViewStreamCamera.fieldOfView = m_ARCamera.fieldOfView;
        firstPersonViewStreamCamera.clearFlags = CameraClearFlags.SolidColor;

        m_webcamTextureCaptureCamera.transform.SetParent(null);
        // quadCapture1.transform.parent = m_webcamTextureCaptureCamera.transform;
        // quadCapture1.transform.localScale = new Vector3(4, 3f, -1);
        // quadCapture1.transform.localPosition = new Vector3(0, 0, 4);
        // // videoBackground.transform.localEulerAngles = videoBackgroundEuler;
        // quadCapture1.transform.localEulerAngles = new Vector3(0, 0, 0);



        webCamTexture = new WebCamTexture(500, 282, 60);
        //WebCamTexture webcamTexture = new WebCamTexture();

        Renderer videoBackgroundRenderer = quadCapture.GetComponent<Renderer>();
        // Renderer videoBackgroundRenderer1 = quadCapture1.GetComponent<Renderer>();

        videoBackgroundRenderer.material.mainTexture = webCamTexture;
        // videoBackgroundRenderer1.material.mainTexture = webCamTexture;

        webCamTexture.Play();
        // }

    }



}

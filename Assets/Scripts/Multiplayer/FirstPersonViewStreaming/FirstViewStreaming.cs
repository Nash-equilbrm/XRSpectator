using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstViewStreaming : MonoBehaviour
{
    public Camera m_ARCamera;
    public GameObject ViewQuadCapture;
    //public RenderTexture m_RenderTexture;
    //public Material m_FirstPersonStreamMat;

    public void StartStreaming()
    {
        // GameObject videoBackground = m_ARCamera.transform.Find("VideoBackground").gameObject;
        // if (videoBackground)
        // {
        // Vector3 videoBackgroundEuler = videoBackground.transform.localEulerAngles;
        // videoBackgroundEuler.x *= -1;
        // videoBackgroundEuler.z *= -1;

        //Create Quad
        GameObject quadCapture = Instantiate(ViewQuadCapture);
        quadCapture.transform.parent = m_ARCamera.transform;
        quadCapture.transform.localScale = new Vector3(4, 2.5f, -1);
        quadCapture.transform.localPosition = new Vector3(0, 0, 4);
        // videoBackground.transform.localEulerAngles = videoBackgroundEuler;
        quadCapture.transform.localEulerAngles = new Vector3(-2, 0, 0);

        Camera streamCamera = GetComponent<Camera>();
        streamCamera.fieldOfView = m_ARCamera.fieldOfView;
        streamCamera.clearFlags = CameraClearFlags.SolidColor;


        WebCamTexture webcamTexture = new WebCamTexture();
        Renderer videoBackgroundRenderer = quadCapture.GetComponent<Renderer>();
        videoBackgroundRenderer.enabled = true;
        videoBackgroundRenderer.material.mainTexture = webcamTexture;
        webcamTexture.Play();
        // }

    }



}

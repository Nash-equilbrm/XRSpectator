using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstViewStreaming : MonoBehaviour
{
    public Camera m_ARCamera;
    //public RenderTexture m_RenderTexture;
    //public Material m_FirstPersonStreamMat;

    public void StartStreaming()
    {
        GameObject videoBackground = m_ARCamera.transform.Find("VideoBackground").gameObject;
        if (videoBackground)
        {
            Vector3 videoBackgroundEuler = videoBackground.transform.localEulerAngles;
            videoBackgroundEuler.x *= -1;
            videoBackgroundEuler.z *= -1;
            videoBackground.transform.localEulerAngles = videoBackgroundEuler;

            Camera streamCamera = GetComponent<Camera>();
            streamCamera.fieldOfView = m_ARCamera.fieldOfView;
            streamCamera.clearFlags = CameraClearFlags.SolidColor;


            WebCamTexture webcamTexture = new WebCamTexture();
            Renderer videoBackgroundRenderer = videoBackground.GetComponent<Renderer>();
            videoBackgroundRenderer.enabled = true;
            videoBackgroundRenderer.material.mainTexture = webcamTexture;
            webcamTexture.Play();
            

        }

    }



}

using Microsoft.MixedReality.Toolkit;
using UnityEngine;
using Vuforia;

public partial class GameManager
{
    [Header("ZedCamera")]
    public Transform zedCameraTransform;
    public GameObject marker;
    public GameObject zedRigStereo;
    public GameObject zedCaptureToOpenCV;
    public GameObject ArUcoDetectManager;
    public Transform leftEye;
    public float retrackMarkerInterval = 10f;
    private float m_retrackTimer = 0f;
    private void InitZED()
    {
        ARCamera.GetComponent<VuforiaBehaviour>().enabled = false;
        MixedRealityToolkit.Instance.enabled = false;
        ARCamera.SetActive(false);

        zedRigStereo.SetActive(true);

        leftEye.gameObject.tag = "MainCamera";

        Instantiate(zedCaptureToOpenCV);
        Instantiate(ArUcoDetectManager);

        m_userRoleInitialized = true;
    }

    private void UpdateZed()
    {
        if(m_retrackTimer >= retrackMarkerInterval){
            if (marker.activeSelf)
            {
                zedCameraTransform.position = leftEye.transform.position;
                zedCameraTransform.rotation = leftEye.transform.rotation;
                zedCameraTransform.SetParent(marker.transform);


                marker.transform.position = Vector3.zero;
                marker.transform.eulerAngles = new Vector3(-90, 180, 0);
                leftEye.position = zedCameraTransform.position;
                leftEye.rotation = zedCameraTransform.rotation;
                m_retrackTimer = 0f;
            }
            else{
                m_retrackTimer = retrackMarkerInterval;
            }

        }
        else{
            m_retrackTimer += Time.deltaTime;
        }
        
    }




}

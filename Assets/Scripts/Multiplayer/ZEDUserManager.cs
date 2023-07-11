using Microsoft.MixedReality.Toolkit;
using UnityEngine;

public partial class GameManager
{
    public Transform leftEye;

    public float retrackMarkerInterval = 10f;
    private float m_retrackTimer = 0f;




    [Header("Sound effects")]
    public AudioSource m_monsterDestroySFX;

    private void InitZED()
    {
        //ARCamera.GetComponent<VuforiaBehaviour>().enabled = false;
        //MixedRealityToolkit.Instance.enabled = false;
        //ARCamera.SetActive(false);

        zedRigStereo.SetActive(true);
        //GameObject zedRigStereo = Instantiate(zedRigStereo);
        leftEye = zedRigStereo.transform.Find("Camera_eyes").Find("Left_eye");
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

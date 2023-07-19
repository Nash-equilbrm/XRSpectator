using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.UnityUtils;
using OpenCVForUnity.ImgprocModule;
using System;

public class VirtualAndRealSceneBlend : MonoBehaviour
{
    private Texture2D receivedTexture;

    private Mat receivedMat;

    private Mat dstMat;
    public Texture2D DstTexture { get => dstTexture; }
    private Texture2D dstTexture;


    //public Renderer blendViewRenderer;


    private BlendVirtualAndRealMatHandler frameHandler;


    private void Update()
    {
        BlendVirtualAndRealScene();
    }

    public void BlendVirtualAndRealScene()
    {
        if(receivedTexture)
        {
            if (receivedMat == null)
            {
                receivedMat = new Mat(receivedTexture.height, receivedTexture.width, CvType.CV_8UC4);
                dstMat = new Mat(receivedTexture.height, receivedTexture.width / 2, CvType.CV_8UC4);
                frameHandler = new BlendVirtualAndRealMatHandler(receivedTexture.height, receivedTexture.width);
            }

            // convert received texture to mat
            Utils.texture2DToMat(receivedTexture, receivedMat);

            // proccess images
            frameHandler.BlendVirtualAndRealMat(receivedMat, ref dstMat);

            // put new output mat on renderer main texture
            if(dstMat != null)
            {
                if(DstTexture == null)
                {
                    dstTexture = new Texture2D(dstMat.cols(), dstMat.rows(), TextureFormat.RGBA32, false);
                }

                Utils.matToTexture2D(dstMat, DstTexture);
                //blendViewRenderer.material.mainTexture = dstTexture;
            }
           

        }
    }


    public class BlendVirtualAndRealMatHandler
    {
        private Mat img2gray;
        private Mat mask;


        public Mat leftImg;
        public Mat rightImg;


        public BlendVirtualAndRealMatHandler(int height, int width)
        {
            img2gray = new Mat(height, width, CvType.CV_8UC4);
            mask = new Mat(height, width, CvType.CV_8UC4);

        }
        public void BlendVirtualAndRealMat(Mat inputMat, ref Mat dst)
        {
            SplitMat(inputMat);
            Imgproc.cvtColor(leftImg, img2gray, Imgproc.COLOR_RGBA2GRAY);

            // Create a mask for black pixels in imgMat1
            Core.inRange(img2gray, new Scalar(0), new Scalar(10), mask);

            // Copy pixels from imgMat2 to imgMat1 where mask is white
            rightImg.copyTo(leftImg, mask);
            leftImg.copyTo(dst);
        }


        public void SplitMat(Mat inputMat)
        {
            int halfWidth = inputMat.width() / 2;
            int height = inputMat.height();

            // Define the ROI for the left half of the image
            OpenCVForUnity.CoreModule.Rect leftRect = new OpenCVForUnity.CoreModule.Rect(0, 0, halfWidth, height);
            OpenCVForUnity.CoreModule.Rect rightRect = new OpenCVForUnity.CoreModule.Rect(halfWidth, 0, halfWidth, height);

            // Create a new Mat for the output image
            if(leftImg != null)
            {
                leftImg.Dispose();
            }
            if (rightImg != null)
            {
                rightImg.Dispose();
            }
            leftImg = new Mat(inputMat, leftRect);
            rightImg = new Mat(inputMat, rightRect);

        }



        ~BlendVirtualAndRealMatHandler()
        {
            img2gray.Dispose();
            mask.Dispose();
        }

    }
    public void SetTexture(Texture2D texture)
    {
        receivedTexture = texture;
    }

   
    private void OnDestroy()
    {
        if(receivedMat != null)
        {
            receivedMat.Dispose();
        }
    }

}

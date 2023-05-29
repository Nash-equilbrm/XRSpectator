using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

public class MakeClickable : MonoBehaviour, IMixedRealityPointerHandler
{
    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
        print("Click");
    }

    public void OnPointerDown(MixedRealityPointerEventData eventData)
    {
        // print("Click");
    }

    public void OnPointerDragged(MixedRealityPointerEventData eventData)
    {
        // print("Click");
    }

    public void OnPointerUp(MixedRealityPointerEventData eventData)
    {
        // print("Click");
    }

}

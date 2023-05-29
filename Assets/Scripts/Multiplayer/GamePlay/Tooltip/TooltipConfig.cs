using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tooltip Collider Config", menuName = "TOOLTIP CONFIG")]
public class TooltipConfig : ScriptableObject
{
    public TooltipColliderInfo[] tooltipColliderInfos;

}

[System.Serializable]
public struct TooltipColliderInfo{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 size;
}

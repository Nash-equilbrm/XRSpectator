using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinResult : MonoBehaviour
{
    public bool LandOnThis { get => landOnThis; set => landOnThis = value; }
    private bool landOnThis = false;


    private void OnTriggerStay(Collider other)
    {
        LandOnThis = true;
    }

    private void OnTriggerExit(Collider other)
    {
        LandOnThis = false;
    }
}

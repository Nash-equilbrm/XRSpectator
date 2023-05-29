using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinToss : MonoBehaviour
{
    public Rigidbody rb;
    public CoinResult heads;
    public CoinResult tails;

    public CoinTossResult CoinTossResult { get => coinTossResult; set => coinTossResult = value; }
    private CoinTossResult coinTossResult = CoinTossResult.NONE;


    
    public void Toss()
    {
        // jump
        rb.AddForce(0, UnityEngine.Random.Range(200f, 350f), 0);
        // spin
        rb.AddTorque(UnityEngine.Random.Range(0.2f, 1f), 0, 0);
    }

    public void GetTossResult()
    {
        if(!heads.LandOnThis && !tails.LandOnThis)
        {
            CoinTossResult = CoinTossResult.NONE;
        }
        else if (heads.LandOnThis)
        {
            CoinTossResult = CoinTossResult.HEAD;
        }
        else if (tails.LandOnThis)
        {
            CoinTossResult = CoinTossResult.TAIL;
        }
        //Debug.Log(CoinTossResult);
    }
}

public enum CoinTossResult
{
    HEAD,
    TAIL,
    NONE
}

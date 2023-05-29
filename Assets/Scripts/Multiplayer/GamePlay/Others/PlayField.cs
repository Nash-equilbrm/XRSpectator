using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayField : MonoBehaviour
{
    public GameObject normalField;
    public GameObject occupiedField;

    public Card PlayFieldCard { get => playFieldCard; set => playFieldCard = value; }
    private Card playFieldCard = null;

    private void Start()
    {
        TurnOnNormalField();
    }

    public void TurnOnNormalField()
    {
        normalField.SetActive(true);
        occupiedField.SetActive(false);
    }

    public void TurnOnOccupiedField()
    {
        normalField.SetActive(false);
        occupiedField.SetActive(true);
    }
}
 
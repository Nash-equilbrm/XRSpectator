using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayField : MonoBehaviour
{
    public GameObject normalField;
    public GameObject monsterField;
    public GameObject attackPhaseField;

    public Card PlayFieldCard { get => playFieldCard; set => playFieldCard = value; }
    private Card playFieldCard = null;

  
    private void Update()
    {
        if(PlayFieldCard == null)
        {
            normalField.SetActive(true);
            monsterField.SetActive(false);
            attackPhaseField.SetActive(false);
        }
        else if(playFieldCard.Monster.OnAttack)
        {
            normalField.SetActive(false);
            monsterField.SetActive(false);
            attackPhaseField.SetActive(true);
        }
        else
        {
            normalField.SetActive(false);
            monsterField.SetActive(true);
            attackPhaseField.SetActive(false);
        }


    }

}
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card Config", menuName = "CardConfig")]
public class CardConfig : ScriptableObject
{
    public int configID;
    public GameObject model;
    public Sprite avatarImg;

    public int HP;
    public float attackDuration;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Monster
{
    private int m_HP;


    public void TakeDamage(int damage)
    {
        m_HP -= damage;
        if (m_HP < 0) m_HP = 0;
    }
}

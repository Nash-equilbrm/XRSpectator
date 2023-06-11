using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Monster
{
    public ParticleSystem m_attackEffect;
    public ParticleSystem m_takeDamageEffect;

    private void UpdateAnimation()
    {
        m_animator.SetBool("Attacking", OnAttack);
    }

}

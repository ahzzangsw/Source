using CharacterDefines;
using UnityEngine;

public class Explosion : EffectBase
{
    protected override void OnTriggerEnter(Collider collision)
    {
        MonsterBase monsterObject = collision.GetComponent<MonsterBase>();
        if (monsterObject == null)
            return;

        if (m_MasterObject)
        {
            if (m_MasterObject.HitMonster(monsterObject, HitParticleType.NONE) == false)
                return;
        }
    }
}

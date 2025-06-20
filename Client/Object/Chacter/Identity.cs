using CharacterDefines;
using GameDefines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Identity : MonoBehaviour
{
    [SerializeField] public SpeciesType m_eSpeciesType = SpeciesType.NONE;
    public AttributeType m_eAttributeType { get; set; } = AttributeType.NONE;

    protected virtual void SetBasicInfo()
    {
        //Renderer renderer = transform.GetChild(0).GetComponent<Renderer>();
        //SpriteRenderer spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        //if (renderer != null)
        //{
        //    renderer.material.color = Oracle.GetColor(m_eAttributeType);
        //}
        //else if (spriteRenderer != null)        // 2D SpriteRenderer를 사용하는 경우
        //{
        //    spriteRenderer.color = Oracle.GetColor(m_eAttributeType);
        //}
        //else
        //{

        //}
    }


    public virtual void ReduceHP(int Damage, HitParticleType eHitParticleType = HitParticleType.NONE)
    {

    }
}

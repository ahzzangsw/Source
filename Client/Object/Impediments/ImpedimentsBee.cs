using CharacterDefines;
using DG.Tweening.Plugins.Core.PathCore;
using GameDefines;
using OptionDefines;
using System;
using System.Collections;
using UnityEngine;

public class ImpedimentsBee : ImpedimentsBase
{
    [SerializeField] private float controlPointOffset = 10f;

    private bool bEnabled = false;
    private float moveSlowSpeed = 0f;

    private Vector3 arrivedPosition = Vector3.zero;

    private enum MoveStepType
    { 
        NONE,
        SLOW,
        LINE,
    };
    MoveStepType eMoveStepType = MoveStepType.NONE;

    void Update()
    {
        if (m_Target == null || bEnabled == false)
            return;

        if (eMoveStepType == MoveStepType.NONE)
        {
            StartCoroutine(Move());
        }
        else if (eMoveStepType == MoveStepType.SLOW) 
        {
            transform.position += (m_Target.position - transform.position).normalized * moveSlowSpeed * Time.deltaTime;
        }
        else
        {
            if (arrivedPosition.x != 0f)
            {
                Quaternion rotation = transform.localRotation;
                rotation.y = arrivedPosition.x > 0f ? 180 : rotation.x;
                transform.localRotation = rotation;
            }

            transform.position += arrivedPosition * moveSpeed * 2f * Time.deltaTime;

            if (transform.position.y < -9.5f || transform.position.x < -18.5f || transform.position.x > 18.5f)
            {
                bEnabled = false;
                DestroyPool();
            }
        }
    }

    public override void SetInfo(Transform player, ADVLayerType eADVLayerType, int ilevel)
    {
        if (player == null)
        {
            Debug.Log("ImpedimentsBase set error =" + m_eAdventurePrefabsType);
        }

        m_Target = player;
        ImpedimentLevel = (float)ilevel;
        Damage = 2;
        moveSlowSpeed = 2f;
        if (ilevel > 2)
        {
            moveSpeed = 9;
        }
        else if (ilevel > 1)
        {
            moveSpeed = 7;
        }
        else
        {
            moveSpeed = 5;
        }

        arrivedPosition = Vector3.zero;

        eMoveStepType = MoveStepType.NONE;
        SoundManager.Instance.PlaySfx(SFXType.SFX_BEE);
        bEnabled = true;
    }

    private IEnumerator Move()
    {
        eMoveStepType = MoveStepType.SLOW;
        yield return new WaitForSeconds(3f);

        if (m_Target == null)
            yield break;

        eMoveStepType = MoveStepType.LINE;
        arrivedPosition = (m_Target.position - transform.position).normalized;
    }
}

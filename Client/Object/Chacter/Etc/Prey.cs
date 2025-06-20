using System.Collections;
using UnityEngine;
using CharacterDefines;
using DG.Tweening;
using OptionDefines;
using System.Collections.Generic;
using DG.Tweening.Plugins.Core.PathCore;

public class Prey : Character
{
    [SerializeField] private Vector3[] StayPositionList = null;

    private int index = -1;
    private Vector3 vTargetPosition = Vector3.zero;
    
    private bool m_bBosstransform = false;

    void Awake()
    {
        SetComponent(this);
        SetFirstLayerFloor(ADVLayerType.ADVLayerType_1);

        index = -1;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            ADVLayerType elayerType = (ADVLayerType)(collision.gameObject.layer);
            if (elayerType > ADVLayerType.ADVLayerType_None && elayerType < ADVLayerType.ADVLayerType_Max)
            {
                if (m_CurrentLayerFloor != elayerType)
                {
                    m_CurrentLayerFloor = elayerType;
                }
            }
        }
    }

    public void ChangePosition(bool bEnd)
    {
        if (StayPositionList == null || StayPositionList.Length == 0)
            return;

        int RandomIndex = Oracle.RandomDice(0, StayPositionList.Length);
        if (index == RandomIndex)
            return;

        index = RandomIndex;
        StartCoroutine(CallGo(bEnd));
    }

    private IEnumerator CallGo(bool bEnd)
    {
        Vector3 vecDestination = StayPositionList[index];
        Vector3 initialPosition = transform.position;

        if (bEnd)
        {
            vecDestination.x = vecDestination.x > 0 ? 22f : -22f;
        }

        float journeyFraction = 0f;
        float startTime = Time.time;
        float journeyLength = Vector2.Distance(initialPosition, vecDestination);

        if (journeyLength == 0)
            yield break;

        while (journeyFraction < 1.0f)
        {
            float distanceCovered = (Time.time - startTime) * 10f;
            journeyFraction = distanceCovered / journeyLength;

            Vector2 LookVector = (vecDestination - transform.position).normalized;
            if (LookVector.x != 0)
            {
                Turn(LookVector.x);
                SetAnimationState(LAnimationState.Running);
            }

            transform.position = Vector2.Lerp(initialPosition, vecDestination, journeyFraction);
            yield return null;
        }

        if (bEnd)
        {
            gameObject.SetActive(false);
        }
    }

    public Vector3 GetLastPosition()
    {
        if (index < 0 || index >= StayPositionList.Length)
            return transform.position;

        return StayPositionList[index];
    }
}
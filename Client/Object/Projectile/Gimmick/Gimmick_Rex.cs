using DG.Tweening;
using GameDefines;
using OptionDefines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_Rex : Gimmick
{
    private List<Vector3> StartPositionList;
    //FireSpeed 여기선 이동속도로 치환
    //FireCount 여기선 박치기 횟수로 치환
    protected override void Awake()
    {
        base.Awake();
        StartPositionList = new List<Vector3>
        {
            new Vector3(-20f, 11f, 0f),
            new Vector3(20f, 11f, 0f),
            new Vector3(-20f, -12f, 0f),
            new Vector3(20f, -12f, 0f)
        };
    }

    protected override void Clear()
    {
        base.Clear();
    }
    protected override IEnumerator PhaseStep0()
    {
        FireSpeed = 10f;
        FireCount = 1;
        SubAttackState("HeadButt");
        yield return null;
    }
    protected override IEnumerator PhaseStep1()
    {
        FireSpeed = 15f;
        FireCount = 2;
        SubAttackState("HeadButt");
        yield return null;
    }
    protected override IEnumerator PhaseStep2()
    {
        FireSpeed = 20f;
        FireCount = 4;
        SubAttackState("HeadButt");
        yield return null;
    }

    private IEnumerator HeadButt()
    {
        int ProcessCount = 0;
        while (ProcessCount < FireCount)
        {
            if (m_Target == null)
                break;

            Vector3 vecTargetPosition = m_Target.transform.position;
            bool bLeftX = transform.position.x < 0;
            bool bDownY = transform.position.y < 0;

            SoundManager.Instance.PlayUISound(UISoundType.ENEMYREX);

            Vector3 direction = (vecTargetPosition - transform.position).normalized;
            while (true)
            {
                if (m_Target == null)
                    break;

                if (m_OwnerBoss)
                    m_OwnerBoss.Running();

                float angleX = 0f;
                float angleZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                if (!bLeftX)
                {
                    angleX = 180f;
                    angleZ *= -1f;
                }

                transform.rotation = Quaternion.Euler(new Vector3(angleX, 0f, angleZ));
                transform.position += direction * FireSpeed * Time.deltaTime;

                if (bLeftX)
                {
                    if (transform.position.x > 20f)
                        break;
                }
                else
                {
                    if (transform.position.x < -20f)
                        break;
                }

                if (bDownY)
                {
                    if (transform.position.y > 12f)
                        break;
                }
                else
                {
                    if (transform.position.y < -13f)
                        break;
                }

                yield return null;
            }

            transform.position = StartPositionList[Oracle.RandomDice(0, StartPositionList.Count)];
            ++ProcessCount;
        }

        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider collision)
    {
        if (gameObject == null)
            return;

        if (collision.transform != m_Target.transform)
            return;

        Player MyPlayer = GameManager.Instance.GetPlayer();
        if (MyPlayer == null)
            return;

        if (BossAdventure_Last_Manager.Instance.m_bDie)
            return;

        MyPlayer.ReduceHP(m_OwnerBoss.m_Damage);
        MyPlayer.AddBuffActor(BuffType.INCAPACITATE, true);
    }
}
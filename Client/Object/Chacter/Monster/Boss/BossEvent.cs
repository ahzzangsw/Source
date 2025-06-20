using CharacterDefines;
using OptionDefines;
using System.Collections;
using System.Collections.Generic;
using UIDefines;
using UnityEngine;

public class BossEvent : MonoBehaviour
{
    private BossBase m_Owner = null;
    private EventState m_eEventState = EventState.FOOTPRINT;
    private bool m_bWait = false;
    private Vector3 targetPosition = Vector3.zero;
    private UI_Complete UIComplete;


    private enum EventState
    {
        //<Enter>//
        FOOTPRINT,          // 발자국 소리가 나고 카메라가 흔들린다
        FIRSTAPPEARANCE,    // 첫등장 카메라 앞에 붙은것처럼 출력후 두리번
        DROP,               // 떨어짐
        ENTEREND,           // 입장 종료

        //<Out>//
        DIE,                // 사망하고 캔버스가 생긴다
        LEFTRIGHT,          // 왔다갔다
        FURY,              // 공격 모션
        END,                // 종료
    }

    private void Clear()
    {
        m_eEventState = EventState.FOOTPRINT;
        targetPosition = Vector3.zero;
        m_bWait = false;
    }

    public void Set(BossBase Owner)
    {
        if (Owner == null)
            return;

        Clear();

        transform.localScale = new Vector3(3f, 3f, 3f);
        if (Oracle.RandomDice(0, 2) > 0)
            transform.position = new Vector3(-24f, -9f, 0);
        else
            transform.position = new Vector3(-9f, -9f, 0);

        m_Owner = Owner;
    }

    public void SetEnd()
    {
        if (m_Owner == null)
            return;

        Clear();

        m_eEventState = EventState.DIE;
        UIManager.Instance.ShowUI(UIIndexType.COMPLETE);

        UIComplete = UIManager.Instance.GetUI(UIIndexType.COMPLETE) as UI_Complete;
        if (UIComplete)
        {
            Canvas canvasComponent = UIComplete.GetComponent<Canvas>();
            m_Owner.ChangeRanderOrder(canvasComponent.sortingOrder);

            UIComplete.OnEventTitleDown += HandleDropBoss;
        }

        enabled = true;
    }

    void Update()
    {
        if (m_bWait)
            return;

        m_bWait = true;
        switch (m_eEventState)
        {
            //<Enter>//
            case EventState.FOOTPRINT:
                StartCoroutine(CallFootprint(5));
                break;
            case EventState.FIRSTAPPEARANCE:
                StartCoroutine(CallFirstappearance());
                break;
            case EventState.DROP:
                m_bWait = false;
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, 15f * Time.deltaTime);
                if (transform.position == targetPosition)
                {
                    CameraManager.Instance.CameraShake(0.2f, true);
                    SoundManager.Instance.PlayBossSfx(BossState.FOOTPRINT);

                    UI_TargetBar BossTargetHPBar = UIManager.Instance.GetUI(UIIndexType.TARGETBAR) as UI_TargetBar;
                    if (BossTargetHPBar)
                    {
                        BossTargetHPBar.SetUp(m_Owner);
                        UIManager.Instance.ShowUI(UIIndexType.TARGETBAR);
                    }

                    m_eEventState = EventState.ENTEREND;
                }
                break;
            case EventState.ENTEREND:
                StartCoroutine(CallEnterEnd());
                break;
            //<Out>//
            case EventState.DIE:
                StartCoroutine(CallDie());
                break;
            case EventState.LEFTRIGHT:
                if(Oracle.RandomDice(0,2) > 0)
                    StartCoroutine(CallLeftright());
                else
                    StartCoroutine(CallRightleft());
                break;
            case EventState.FURY:
                StartCoroutine(CallFury());
                break;
            case EventState.END:
                StartCoroutine(CallEnd());
                break;
        }
    }

    //<Enter>//
    private IEnumerator CallFootprint(int count)
    {
        int cur = 0;
        while (cur < count)
        {
            ++cur;
            CameraManager.Instance.CameraShake(0.5f, true);
            SoundManager.Instance.PlayBossSfx(BossState.FOOTPRINT);
            yield return new WaitForSeconds(1.5f);
        }

        m_eEventState = EventState.FIRSTAPPEARANCE;
        m_bWait = false;
    }

    private IEnumerator CallFirstappearance()
    {
        float speed = 1f;
        int count = 3;

        while (true)
        {
            while (transform.position.y < -4.5f)
            {
                float newY = Mathf.MoveTowards(transform.position.y, -4.5f, Time.deltaTime * speed);
                transform.position = new Vector3(transform.position.x, newY, transform.position.z);
                yield return null;
            }

            yield return new WaitForSeconds(1.5f);

            while (count > 0)
            {
                m_Owner.Turn(count % 2 == 0 ? 1 : -1);
                --count;
                yield return new WaitForSeconds(1.5f);
            }

            while (transform.position.y > -9)
            {
                float newY = Mathf.MoveTowards(transform.position.y, -9f, Time.deltaTime * speed);
                transform.position = new Vector3(transform.position.x, newY, transform.position.z);
                yield return null;
            }

            m_eEventState = EventState.DROP;
            Vector2 newPosition = targetPosition = m_Owner.GetMovePosition(0);
            newPosition.y += 10;
            transform.position = newPosition;
            transform.localScale = new Vector3(2f, 2f, 1f);
            m_bWait = false;
            yield break;
        }
    }

    private IEnumerator CallEnterEnd()
    {
        yield return new WaitForSeconds(3f);
        CameraManager.Instance.EndCameraShake();
        SoundManager.Instance.PlayBGM(BGMType.LASTBOSS);
        m_Owner.StartProductionEnd();
        enabled = false;
    }

    //<Out>//
    private IEnumerator CallDie()
    {
        yield return new WaitForSeconds(5f);
        m_Owner.SetAnimationState(LAnimationState.Idle);
        yield return new WaitForSeconds(2f);
        UIComplete.EnableQuestionmark(true);
        yield return new WaitForSeconds(1f);
        UIComplete.EnableQuestionmark(false);
        m_eEventState = EventState.LEFTRIGHT;
        m_bWait = false;
    }

    private IEnumerator CallLeftright()
    {
        float speed = 1f;

        while (true)
        {
            float leftPosX = transform.position.x - 5f;
            while (transform.position.x > leftPosX)
            {
                m_Owner.SetAnimationState(LAnimationState.Running);
                float newX = Mathf.MoveTowards(transform.position.x, leftPosX, Time.deltaTime * speed);
                transform.position = new Vector3(newX, transform.position.y, transform.position.z);
                m_Owner.Turn(-1);
                yield return null;
            }

            m_Owner.SetAnimationState(LAnimationState.Idle);
            yield return new WaitForSeconds(1f);

            float rightPosX = transform.position.x + 5f;
            while (transform.position.x < rightPosX)
            {
                m_Owner.SetAnimationState(LAnimationState.Running);
                float newX = Mathf.MoveTowards(transform.position.x, rightPosX, Time.deltaTime * speed);
                transform.position = new Vector3(newX, transform.position.y, transform.position.z);
                m_Owner.Turn(1);
                yield return null;
            }

            m_Owner.SetAnimationState(LAnimationState.Idle);
            m_eEventState = EventState.FURY;
            m_bWait = false;
            yield break;
        }
    }
    private IEnumerator CallRightleft()
    {
        float speed = 1f;

        while (true)
        {
            float rightPosX = transform.position.x + 5f;
            while (transform.position.x < rightPosX)
            {
                m_Owner.SetAnimationState(LAnimationState.Running);
                float newX = Mathf.MoveTowards(transform.position.x, rightPosX, Time.deltaTime * speed);
                transform.position = new Vector3(newX, transform.position.y, transform.position.z);
                m_Owner.Turn(1);
                yield return null;
            }

            m_Owner.SetAnimationState(LAnimationState.Idle);
            yield return new WaitForSeconds(1f);

            float leftPosX = transform.position.x - 5f;
            while (transform.position.x > leftPosX)
            {
                m_Owner.SetAnimationState(LAnimationState.Running);
                float newX = Mathf.MoveTowards(transform.position.x, leftPosX, Time.deltaTime * speed);
                transform.position = new Vector3(newX, transform.position.y, transform.position.z);
                m_Owner.Turn(-1);
                yield return null;
            }

            m_Owner.SetAnimationState(LAnimationState.Idle);
            m_eEventState = EventState.FURY;
            m_bWait = false;
            yield break;
        }
    }

    private IEnumerator CallFury()
    {
        yield return new WaitForSeconds(2f);
        m_Owner.SetAnimationActionState(LAnimationState.Attack);
        yield return new WaitForSeconds(1f);

        if (UIComplete)
        {
            UIComplete.NextStep();
        }
    }

    private IEnumerator CallEnd()
    {
        float speed = 8f;
        while (transform.position.y > -30f)
        {
            float newY = Mathf.MoveTowards(transform.position.y, -30, Time.deltaTime * speed);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            yield return null;
        }
    }

    private void HandleDropBoss()
    {
        m_eEventState = EventState.END;
        m_bWait = false;
        SoundManager.Instance.PlayBossSfx(BossState.HEADSHOT);
        SoundManager.Instance.PlayBGM(BGMType.ENDING);
    }
}

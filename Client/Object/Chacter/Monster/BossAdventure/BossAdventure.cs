using GameDefines;
using OptionDefines;
using System.Collections;
using System.Collections.Generic;
using UIDefines;
using UnityEngine;
using CharacterDefines;

public class BossAdventure : MonsterAdventure
{
    public int m_Damage { get; private set; } = 0;

    protected bool bInterrupt = false;
    protected int[] iBossSkillPercent = null;

    protected float fDelay = 1f;

    protected Player MyPlayer = null;

    private UI_Interrupt UIInterrupt = null;
    private int currentSplitCount = 10;
    private float splitScaleOffset = 0.85f;
    private enum Boss8State { NORMAL, ANGRY, MAD, FURY, FIRE };
    private Boss8State eBoss8State = Boss8State.NORMAL;
    private List<KeyValuePair<int, bool>> StatusAboutHP = null;
    private MonsterPathfinder MonsterController = null;

    protected enum DieActionState
    {
        NONE,
        SCALEDOWN,          // 수축
        FALL,               // 떨어지다
    }
    protected DieActionState eRandomDieAction = DieActionState.NONE;

    protected override void Awake()
    {
        base.Awake();

        MyPlayer = GameManager.Instance.GetPlayer();
        if (MyPlayer == null)
            return;

        m_eClickTargetType = ClickTargetType.FINAL;
        eRandomDieAction = DieActionState.NONE;
        m_AttackAnimState = "Attack";

        MonsterPool.Instance.OnUpdateBossPatternEvent += HandleUpdateBossPatternEvent;

        UIInterrupt = UIManager.Instance.GetUI(UIIndexType.INTERRUPT) as UI_Interrupt;

        iBossSkillPercent = new int[10];
        {
            iBossSkillPercent[0] = 0;
            iBossSkillPercent[1] = 20;
            iBossSkillPercent[2] = 5;
            iBossSkillPercent[3] = 10;
            iBossSkillPercent[4] = 0;
            iBossSkillPercent[5] = 10;
            iBossSkillPercent[6] = 10;
            iBossSkillPercent[7] = 100;
            iBossSkillPercent[8] = 0;
            iBossSkillPercent[9] = 0;
        }
    }
    protected override void OnEnable()
    {
        base.OnEnable();

        if (ID < 0 || ID >= iBossSkillPercent.Length)
        {
            bEnabled = false;
            gameObject.SetActive(false);
            return;
        }

        switch (ID)
        {
            case 3:
                Range = 2f;
                break;
            case 4:
                Range = 5f;
                break;
            case 8:
                {
                    eBoss8State = Boss8State.NORMAL;

                    float Maxhp = (float)Hp;
                    StatusAboutHP = new List<KeyValuePair<int, bool>>();
                    StatusAboutHP.Add(new KeyValuePair<int, bool>((int)(Maxhp * 0.7f), true));
                    StatusAboutHP.Add(new KeyValuePair<int, bool>((int)(Maxhp * 0.5f), true));
                    StatusAboutHP.Add(new KeyValuePair<int, bool>((int)(Maxhp * 0.1f), true));

                    MonsterController = GetComponent<MonsterPathfinder>();
                }
                break;
        }
    }

    protected override void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.H))
        {
            bInterrupt = true;
        }
#endif

        if (bInterrupt)
        {
            DoInterrupt();
            bInterrupt = false;
        }

        base.Update();
    }

    protected override void FixedUpdate()
    {
        if (eRandomDieAction != DieActionState.NONE)
        {
            switch (eRandomDieAction)
            {
                case DieActionState.SCALEDOWN:
                    transform.Rotate(Vector3.forward * 20f);
                    transform.localScale *= 0.9f;
                    break;
            }
        }

        base.FixedUpdate();
    }
    public override void ReduceHP(int Damage, HitParticleType eHitParticleType = HitParticleType.NONE)
    {
        base.ReduceHP(Damage, eHitParticleType);

        if (Oracle.PercentSuccess(iBossSkillPercent[ID-1]))
        {
            bInterrupt = true;
        }
    }

    protected override void OnDie(bool beKilled)
    {
        Player MyPlayer = GameManager.Instance.GetPlayer();
        if (MyPlayer == null)
            return;

        if (ID == 5)
        {
            if (currentSplitCount > 0 && beKilled)
            {
                --currentSplitCount;
                SetSplitInfo();
                return;
            }
        }

        if (ID == 1 || ID == 3 || ID == 5 || ID == 7)
        {
            MyPlayer.SetSelectedPick(AdventureLevelUpItemType.UTIL, 2, true);
        }
        else if (ID == 6 || ID == 8)
        {
            int iValue = Oracle.RandomDice(1, (int)(SpeciesType.MAX));
            MyPlayer.SetSelectedPick(AdventureLevelUpItemType.CHARACTER, iValue, true);
        }

        if (m_Animator)
            m_Animator.SetTrigger("Die");

        base.OnDie(beKilled);
        RemoveBoss();
    }

    private void RemoveBoss()
    {
        int iRandomDieAction = 0;// Oracle.RandomDice(0, 2);
        if (iRandomDieAction == 0)
        {
            // 돌면서 스케일값 줄어들면서 사라짐
            eRandomDieAction = DieActionState.SCALEDOWN;
            fDelay = 1f;
        }
        else
        {
            eRandomDieAction = DieActionState.FALL;
            fDelay = 3f;

            Rigidbody Rb = gameObject.AddComponent<Rigidbody>();
            if (Rb)
            {
                Vector3 Direction = new Vector3(-1f, 1f, 0f).normalized;
                Rb.AddForce(Direction * 500f);
            }
        }

        BossState eBossState = (BossState)((int)(BossState.DIE0) + iRandomDieAction);
        SoundManager.Instance.PlayBossSfx(eBossState);

        StartCoroutine(CallRemoveBossObject());
    }

    protected IEnumerator CallRemoveBossObject()
    {
        yield return new WaitForSeconds(fDelay);

        Rigidbody Rb = GetComponent<Rigidbody>();
        if (Rb)
        {
            Destroy(Rb);
        }

        ClearInterrupt();
        eRandomDieAction = DieActionState.NONE;
        gameObject.SetActive(false);

        MonsterPool.Instance.RemoveBoss(poolIndex);
        yield break;
    }

    protected virtual void DoInterrupt()
    {
        switch (ID)
        {
            case 1:
                Interrupt1();
                break;
            case 2:
                Interrupt2();
                break;
            case 3:
                Interrupt3();
                break;
            case 4:
                Interrupt4();
                break;
            case 5:
                Interrupt5();
                break;
            case 6:
                Interrupt6();
                break;
            case 7:
                Interrupt7();
                break;
            case 8:
                Interrupt8();
                break;
            case 9:
                Interrupt9();
                break;
            case 10:
                Interrupt10();
                break;
        };
    }

    protected void ClearInterrupt()
    {
        bInterrupt = false;

        if (ID == 2)
        {
            if (UIInterrupt != null)
            {
                UIInterrupt.Clear();
                if (UIInterrupt.IsShow() == true)
                {
                    UIInterrupt.Hide();
                }
            }
        }
        else if (ID == 8)
        {
            eBoss8State = Boss8State.NORMAL;
        }
    }

    protected virtual void HandleUpdateBossPatternEvent()
    {
        if (ID == 2)
        {
            moveSpeed *= 5f;
            m_BuffContainer.UpdateMonsterInfo(moveSpeed);
        }

        ClearInterrupt();
    }

    private void Interrupt1() 
    {
        // X
    }
    private void Interrupt2() 
    {
        if (UIInterrupt == null)
            return;

        if (UIInterrupt.IsShow() == false)
        {
            UIManager.Instance.ShowUI(UIIndexType.INTERRUPT);
        }

        UIInterrupt.Process();
    }
    private void Interrupt3() 
    {
        MyPlayer.AddBuffActor(BuffType.INCAPACITATE, transform.position, Range);
        Range += 0.1f;
    }
    private void Interrupt4() 
    {
        MyPlayer.AddBuffActor(BuffType.REDUCING, transform.position, Range);
        Range += 0.2f;
    }
    private void Interrupt5() 
    {
        // Split
    }
    private void Interrupt6() 
    {
        Defense += 1;
        if (Defense > 95)
            Defense = 95;
    }
    private void Interrupt7()
    {
        moveSpeed += 0.1f;
        m_BuffContainer.UpdateMonsterInfo(moveSpeed);
    }
    private void Interrupt8() 
    {
        if (eBoss8State == Boss8State.NORMAL || eBoss8State == Boss8State.MAD || eBoss8State == Boss8State.ANGRY)
        {
            if (currentHP <= StatusAboutHP[2].Key && StatusAboutHP[1].Value)
            {
                moveSpeed += 3f;

                StatusAboutHP[2] = new KeyValuePair<int, bool>(StatusAboutHP[2].Key, false);
            }
            else if (currentHP <= StatusAboutHP[1].Key && StatusAboutHP[1].Value)
            {
                moveSpeed += 2f;

                StatusAboutHP[1] = new KeyValuePair<int, bool>(StatusAboutHP[1].Key, false);
            }
            else if (currentHP <= StatusAboutHP[0].Key && StatusAboutHP[0].Value)
            {
                moveSpeed += 1f;

                StatusAboutHP[0] = new KeyValuePair<int, bool>(StatusAboutHP[0].Key, false);
            }
            else
                return;

            m_BuffContainer.UpdateMonsterInfo(moveSpeed);
        }
        else if (eBoss8State == Boss8State.FURY)
        {
            eBoss8State = Boss8State.FIRE;

            Blink();
            SoundManager.Instance.PlayBossSfx(BossState.WARP);

            MonsterController.TeleportationEndPosition();
        }
    }
    private void Interrupt9() 
    {
        // X
    }
    private void Interrupt10() 
    {

    }
    private void SetSplitInfo()
    {
        int tempHP = Hp / 2;
        if (currentSplitCount > 5)
            tempHP += 2000;

        currentHP = tempHP;

        transform.localScale = transform.localScale;
        transform.localScale *= splitScaleOffset;

        bEnabled = true;
    }
}
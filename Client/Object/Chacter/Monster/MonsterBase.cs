using CharacterDefines;
using GameDefines;
using OptionDefines;
using System.Collections;
using UIDefines;
using UnityEngine;

public class MonsterBase : Character
{
    public int ID { get; protected set; } = -1;
    public int Hp { get; protected set; } = 0;
    public int Defense { get; protected set; } = 0;
    public int currentHP { get; set; } = 0;
    public float moveSpeed { get; set; } = 0;
    public int moveIndex { get; protected set; } = 1;
    public int poolIndex { get; set; } = -1;

    public bool bStun = false;
    public bool bSlow = false;
    public bool bKnockback = false;
    public bool bOnForceRemove = false;

    public bool bOnBuff { get; protected set; } = true;

    protected short giveMoney = 0;
    protected Vector2[] movePositions;
    protected Vector2 LookVector = Vector2.zero;

    protected bool bEnabled = false;
    // Move
    protected int currentMoveIndex = -1;
    protected Vector2 initialPosition;
    protected float journeyLength;
    protected float startTime;
    // Knockback
    protected Vector2 beforeKnockbackPosition = Vector2.zero;
    protected int KnockbackIndex = 1;
    protected float totalKnockbackMoved = 0f;

    //Paricle
    protected Color originalParticleColor_Hit;

    //Ruby
    protected int RubyCount = 1;

    private bool bSpawnPointRandom = false;

    protected virtual void Awake()
    {
        m_eClickTargetType = ClickTargetType.MONSTER;

        moveIndex = 1;
        MapBase pMapBase = MapManager.Instance.GetCurrentMapInfo();
        if (pMapBase == null)
            return;

        int WayPointCount = pMapBase.GetWayPointCount();
        movePositions = new Vector2[WayPointCount+1];

        Transform SpawnPosition = pMapBase.spawnPoint;
        if (pMapBase.spawnPoint == null)
            return;

        bSpawnPointRandom = false;
        if (pMapBase.spawnPoint1 != null)
            bSpawnPointRandom = true;

        if (bSpawnPointRandom && Oracle.RandomDice(0, 2) > 0)
        {
            SpawnPosition = pMapBase.spawnPoint1;
            UnlockManager.Instance.AddLobbyDropCharacterCount();
        }

        movePositions[0] = (Vector2)(SpawnPosition.position);
        for (int i = 0; i < WayPointCount; ++i)
        {
            Vector2 wayPoint = pMapBase.GetWayPointByVector2(i);
            if (wayPoint == null)
                continue;

            movePositions[i+1] = wayPoint;
        }

        transform.position = SpawnPosition.position;
    }
    protected virtual void OnEnable()
    {
        bEnabled = true;

        currentMoveIndex = -1;
    }
    protected virtual void Update()
    {
        m_BuffContainer.Tick();
    }
    protected override void FixedUpdate()
    {
        if (!bEnabled)
            return;

        if (currentHP <= 0)
        {
            OnDie(true); 
            return;
        }

        if (moveIndex >= movePositions.Length)
        {
            bEnabled = false;
            return;
        }

        if (bKnockback)
        {
            bool bReset = true;
            if (moveIndex - KnockbackIndex >= 0)
            {
                if (beforeKnockbackPosition == Vector2.zero)
                {
                    beforeKnockbackPosition = transform.position;
                }

                Vector2 vecArrived = movePositions[moveIndex - KnockbackIndex];
                Vector3 knockbackDirection = ((Vector2)transform.position - vecArrived).normalized;
                transform.position += -knockbackDirection * Time.deltaTime * 2f;

                float fKnockbackMoved = Vector3.Distance(beforeKnockbackPosition, transform.position);
                totalKnockbackMoved += fKnockbackMoved;
                if (totalKnockbackMoved < 1f)
                {
                    bReset = false;
                }

                if (fKnockbackMoved == 0)
                {
                    ++KnockbackIndex;
                }

                SetAnimationState(LAnimationState.Crawling);
            }

            if (bReset)
            {
                bKnockback = false;
                beforeKnockbackPosition = Vector2.zero;
                KnockbackIndex = 1;
                totalKnockbackMoved = 0f;

                initialPosition = transform.position;
                journeyLength = Vector2.Distance(initialPosition, movePositions[moveIndex]);
                startTime = Time.time;
            }
        }
        else
        {
            Vector2 vecArrived = movePositions[moveIndex];
            if (currentMoveIndex != moveIndex || bSlow)
            {
                currentMoveIndex = moveIndex;
                initialPosition = transform.position;
                journeyLength = Vector2.Distance(initialPosition, vecArrived);
                startTime = Time.time;
                bSlow = false;
            }

            bool bUpDown = false;
            if (bStun)
            {
                initialPosition = transform.position;
                journeyLength = Vector2.Distance(initialPosition, vecArrived);
                startTime = Time.time;
            }
            else
            {
                float distanceCovered = (Time.time - startTime) * moveSpeed;
                float journeyFraction = distanceCovered / journeyLength;

                LookVector = (vecArrived - (Vector2)transform.position).normalized;
                if (LookVector.x != 0)
                {
                    Turn(LookVector.x);
                }
                if (LookVector.y != 0)
                {
                    UpAndDown(LookVector.y);
                    if(m_eClickTargetType != ClickTargetType.FINAL)
                        bUpDown = true;
                }

                transform.position = Vector2.Lerp(initialPosition, vecArrived, journeyFraction);

                if (journeyFraction >= 1.0f)
                {
                    ++moveIndex;
                    if (moveIndex >= movePositions.Length)
                    {
                        if (bOnForceRemove)
                            ForceRemove();
                        else
                            OnDie(false);
                        return;
                    }
                }
            }

            if(!bUpDown)
                SetAnimationState(LAnimationState.Running);
        }
    }

    public override void SetComponent(Character tempCharacter)
    {
        base.SetComponent(tempCharacter);
        bOnForceRemove = false;

        if (m_HitDust)
        {
            ParticleSystem.MainModule mainModule = m_HitDust.main;
            originalParticleColor_Hit = mainModule.startColor.color;
        }
    }
    public void SetInfo(int index, int hp, int defense, float movespeed, short money)
    {
        ID = index;
        Hp = hp;
        currentHP = hp;
        Defense = defense;
        moveSpeed = movespeed;
        giveMoney = money;

        m_BuffContainer = new BuffContainer(this);
        bOnBuff = true;

        SetBasicInfo();
    }

    public bool IsDie()
    {
        return bEnabled == false;
    }
    protected virtual void OnDie(bool beKilled)
    {
        bEnabled = false;

        Player MyPlayer = GameManager.Instance.GetPlayer();
        if (MyPlayer == null)
            return;

        if (beKilled)
        {
            SetAnimationState(LAnimationState.Dead);

            int giveTotalMoney = giveMoney;
            if (m_eSpeciesType == SpeciesType.GOLDGOBLIN)
            {
                if (Oracle.m_eGameType == MapType.ADVENTURE)
                {
                    if (Oracle.PercentSuccess(10f))
                    {
                        UIManager.Instance.ShowRoundPeriodic(-1);
                        GameManager.Instance.GameStop(true);
                    }

                    MyPlayer.IncreaseHP(1);
                }
                else
                {
                    giveTotalMoney *= 2;
                }

                if (Oracle.PercentSuccess(5f))
                {
                    DropItem();
                }
            }

            if (Oracle.m_eGameType != MapType.ADVENTURE)
            {
                giveTotalMoney += MyPlayer.addMoney;
                MyPlayer.AddMoney(giveTotalMoney);
            }

            if (m_eClickTargetType == ClickTargetType.MONSTER)
                StartCoroutine(CallRemoveMonsterObject());

            SoundManager.Instance.PlayCharacterSfx(SpeciesType.NONE, CharacterState.DIE);
        }
        else
        {
            if (m_eClickTargetType == ClickTargetType.MONSTER)
                MyPlayer.ReduceHP(1);
            else
                MyPlayer.GameOver();

            gameObject.SetActive(false);
        }

        m_BuffContainer.OnDie();

        if (m_eSpeciesType != SpeciesType.MAX)
            MonsterPool.Instance.RemoveMonsterByWave(ID);
    }

    private void ForceRemove()
    {
        bEnabled = false;
        gameObject.SetActive(false);
        m_BuffContainer.OnDie();
        MonsterPool.Instance.RemoveMonsterByWave(ID);

        Destroy(gameObject);
    }

    protected virtual IEnumerator CallRemoveMonsterObject()
    {
        yield return new WaitForSeconds(1f);
        if (bSpawnPointRandom)
            Destroy(gameObject);
        else
            gameObject.SetActive(false);
        yield break;
    }

    public override void ReduceHP(int Damage, HitParticleType eHitParticleType = HitParticleType.NONE)
    {
        if (Damage == 0)
            return;

        currentHP -= Damage;
        if (currentHP < 0)
            currentHP = 0;

        if (m_eClickTargetType != ClickTargetType.BOSS)
            UIManager.Instance.ShowCharacterUI(UIIndexType.MAININFO, this, true);

        if (OptionManager.Instance.bHitEffect)
        {
            if (m_HitDust)
            {
                m_HitDust.Stop(true);

                Color newColor;
                switch (eHitParticleType)
                {
                    case HitParticleType.BE_BITTEN:
                        newColor = Color.red;
                        break;
                    default:
                        newColor = originalParticleColor_Hit;
                        break;
                };

                ParticleSystem[] childParticleSystems = m_HitDust.GetComponentsInChildren<ParticleSystem>();
                foreach (ParticleSystem childParticleSystem in childParticleSystems)
                {
                    var childMain = childParticleSystem.main;
                    childMain.startColor = newColor;
                }

                m_HitDust.Play(true);
            }
        }

        if (eHitParticleType == HitParticleType.BE_BITTEN)
        {
            SoundManager.Instance.PlaySfxFast(SFXType.SFX_BITE);
        }
    }
    
    public override void AddBuffActor(BuffType eBuffType)
    {
        
    }

    public override void AddDeBuffActor(Character giveDebuffObject)
    {
        if (giveDebuffObject.eBuffType == BuffType.NONE || m_BuffContainer == null)
            return;

        Building hitBuilding = giveDebuffObject as Building;
        if (hitBuilding == null)
            return;

        int addDamage = 0;
        Player MyPlayer = GameManager.Instance.GetPlayer();
        if (MyPlayer != null)
        {
            ControlInfo controlInfoData = MyPlayer.GetCurrentUpgradeData(hitBuilding.m_eSpeciesType);
            addDamage = controlInfoData.UnitAtk[hitBuilding.m_CharacterIndex];
        }

        m_BuffContainer.AddBuff(giveDebuffObject.eBuffType, hitBuilding.Damage + addDamage);
    }

    public void BeHitCritical(float fDamage)
    {
        GameObject CriticalFontPrefeb = Instantiate(UIManager.Instance.GetUIPrefeb(UIPrefebType.CRITICALFONT));
        if (CriticalFontPrefeb)
        {
            UI_CriticalFont UICriticalFont = CriticalFontPrefeb.GetComponent<UI_CriticalFont>();
            if (UICriticalFont)
            {
                Vector3 criticalFontPosition = transform.position;
                if (m_eClickTargetType == ClickTargetType.FINAL)
                    criticalFontPosition.y += 1.5f;

                UICriticalFont.SetUp(criticalFontPosition, (int)fDamage);
            }
        }
    }

    protected void DropItem()
    {
        float angleStep = 180f / (RubyCount + 1);
        Vector3 StartPosition = transform.position;
        for (int i = 0; i < RubyCount; ++i)
        {
            float angle = (i + 1) * angleStep;
            float radians = angle * Mathf.Deg2Rad;

            Vector2 centerPosition = StartPosition;

            // 반지름 계산
            float radius = 1.5f; // 반원의 반지름 (조정 가능)

            // 원 형태를 기준으로 한 위치 계산
            float x = centerPosition.x + radius * Mathf.Cos(radians);
            float y = centerPosition.y + radius * Mathf.Sin(radians);

            Vector2 spawnPosition = new Vector2(x, y);
            ItemManager.Instance.DropItem(ItemType.RUBY, spawnPosition, StartPosition);
        }
    }

    public string GetDefenseString()
    {
        string strDefense = "";
        if (Defense > 0 && m_BuffContainer.IsBuff(BuffType.ARMORREDUCING0, true))
        {
            BuffType curBuffType = m_BuffContainer.GetBuffAndCheck(BuffType.ARMORREDUCING0);
            int iDefense_Reducing = CalculationDamageFormula.ConvertBuffPercentAndValue(curBuffType);
            if (iDefense_Reducing > Defense)
                iDefense_Reducing = Defense;
            strDefense = string.Format("{0}<color=red>(-{1})</color>%", Defense.ToString(), iDefense_Reducing.ToString());
        }
        else
            strDefense = string.Format("{0}%", Defense.ToString());

        return strDefense;
    }

    public string GetMoveSpeedString()
    {
        string strSpeed = "";
        if (m_BuffContainer.IsBuff(BuffType.SLOW0, true))
        {
            strSpeed = string.Format("{0}<color=red>(SLOW)</color>", moveSpeed.ToString());
        }
        else
            strSpeed = moveSpeed.ToString();

        return strSpeed;
    }

    public void VersatilityDie(bool beKilled)
    {
        OnDie(beKilled);
    }
}

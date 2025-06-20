using GameDefines;
using OptionDefines;
using CharacterDefines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss10 : BossBase
{
    private float Range = 10f;
    private BossEvent BossEventClass = null;
    private bool bKill = false;

    private enum Boss10State { NORMAL, ANGRY, MAD, FURY, FIRE };
    private Boss10State eBoss10State = Boss10State.NORMAL;
    private List<KeyValuePair<int, bool>> StatusAboutHP = null;
    private float[] BossPatternsHPPercent = { 0f, 0f, 0f, 0f };
    private int PatternIndex = 0;

    protected override void Awake()
    {
        base.Awake();
        m_eClickTargetType = ClickTargetType.FINAL;

        iBossSkillPercent = 20;
        RubyCount = 20;

        eBoss10State = Boss10State.NORMAL;
        m_AttackAnimState = "Attack";
        StatusAboutHP = new List<KeyValuePair<int, bool>>();

        CameraManager.Instance.OnCameraEventEnd += HandleCameraEventEnd;
    }

    protected override void OnEnable()
    {
        currentMoveIndex = -1;

        CameraManager.Instance.bProductioning = true;
        if (BossEventClass == null)
        {
            BossEventClass = gameObject.GetComponent<BossEvent>();
            BossEventClass.enabled = false;
        }

        BossEventClass.Set(this);
    }

    protected override void SetBasicInfo()
    {
        float Maxhp = (float)Hp;
        StatusAboutHP.Add(new KeyValuePair<int, bool>((int)(Maxhp * 0.7f), true));
        StatusAboutHP.Add(new KeyValuePair<int, bool>((int)(Maxhp * 0.5f), true));
        StatusAboutHP.Add(new KeyValuePair<int, bool>((int)(Maxhp * 0.1f), true));

        PatternIndex = 0;
        BossPatternsHPPercent[0] = Hp * 0.7f;
        BossPatternsHPPercent[1] = Hp * 0.5f;
        BossPatternsHPPercent[2] = Hp * 0.3f;
        BossPatternsHPPercent[3] = Hp * 0.15f;
    }

    private void HandleCameraEventEnd()
    {
        BossEventClass.enabled = true;
    }

    protected override void FixedUpdate()
    {
        if (eBoss10State == Boss10State.ANGRY || eBoss10State == Boss10State.MAD || eBoss10State == Boss10State.FIRE)
        {
            Vector2 vecArrived = movePositions[moveIndex];
            LookVector = (vecArrived - (Vector2)transform.position).normalized;
            if (LookVector.x != 0)
            {
                Turn(LookVector.x);
            }

            Vector3 newPosition = LookVector * moveSpeed * Time.deltaTime;
            transform.position += newPosition;

            float distance = Vector3.Distance(transform.position, vecArrived);
            if (distance < 1f)
            {
                eBoss10State = Boss10State.NORMAL;

                KeyValuePair<int, bool> updatePair = new KeyValuePair<int, bool>(StatusAboutHP[(int)eBoss10State].Key, false);
                StatusAboutHP[(int)eBoss10State] = updatePair;
                return;
            }
        }
        else
        {
            base.FixedUpdate();
        }
    }

    protected override void DoInterrupt()
    {
        if (PatternIndex < BossPatternsHPPercent.Length)
        {
            if (currentHP <= BossPatternsHPPercent[PatternIndex])
            {
                BossPatternsHPPercent[PatternIndex] = 0f;
                ++PatternIndex;

                switch (PatternIndex)
                {
                    case 1:
                        Interrupt0();
                        break;
                    case 3:
                        iBossSkillPercent = 10;
                        break;
                    case 4:
                        moveSpeed *= 2f;
                        iBossSkillPercent = 7;
                        break;
                }
            }
        }

        switch (PatternIndex)
        {
            case 2:
                Interrupt1();
                break;
            case 3:
                Interrupt2();
                break;
            case 4:
                Interrupt3();
                break;
        }
    }

    protected override void ClearInterrupt()
    {
        bInterrupt = false;
        eBoss10State = Boss10State.NORMAL;
    }

    protected override void RemoveBoss()
    {
        GameManager.Instance.OnGameComplete();

        Vector3 CameraPosition = transform.position;
        CameraPosition.y += 4;
        CameraManager.Instance.Move(CameraPosition);
        SoundManager.Instance.StopBGM();
        SoundManager.Instance.PlayBossSfx(BossState.BOSSDIE);

        DropItem();
        ClearInterrupt();

        StartCoroutine(CallLastBossFinal());
    }

    IEnumerator CallLastBossFinal()
    {
        yield return new WaitForSeconds(2f);

        ItemManager.Instance.AutoRubyGet(20);
        BossEventClass.SetEnd();
        yield break;
    }

    private void Interrupt0()
    {
        bStun = true;
        SoundManager.Instance.PlayBossSfx(BossState.FLY);
        StartCoroutine(CallDestroyBuildTile());
    }

    IEnumerator CallDestroyBuildTile()
    {
        float OriginY = transform.position.y;

        while (transform.position.y < 15f)
        {
            float newY = Mathf.MoveTowards(transform.position.y, 15f, Time.deltaTime * 30);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            yield return null;
        }

        while (transform.position.y > OriginY)
        {
            float newY = Mathf.MoveTowards(transform.position.y, OriginY, Time.deltaTime * 20);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            yield return null;
        }

        SoundManager.Instance.PlayBossSfx(BossState.FOOTPRINT);
        CameraManager.Instance.CameraShake(0.2f, true);
        MapManager.Instance.DestroyBuildTile();
        bStun = false;
    }

    private void Interrupt1()
    {
        List<GameObject> buildingList = BuildingPool.Instance.GetBuildingList();
        if (buildingList != null)
        {
            for (int i = 0; i < buildingList.Count; ++i)
            {
                GameObject buildingObject = buildingList[i];
                float distance = Vector3.Distance(buildingObject.transform.position, transform.position);
                if (distance <= Range)
                {
                    Building building = buildingObject.GetComponent<Building>();
                    building.AddBuffActor(BuffType.REDUCING);
                }
            }
        }

        Range += 0.5f;
    }

    private void Interrupt2() 
    {
        List<GameObject> buildingList = BuildingPool.Instance.GetBuildingList();
        if (buildingList != null)
        {
            for (int i = 0; i < buildingList.Count; ++i)
            {
                GameObject buildingObject = buildingList[i];
                float distance = Vector3.Distance(buildingObject.transform.position, transform.position);
                if (distance <= Range)
                {
                    Building building = buildingObject.GetComponent<Building>();
                    building.AddBuffActor(BuffType.INCAPACITATE);
                }
            }
        }

        Range += 0.5f;
    }

    private void Interrupt3()
    {
        if (bKill)
            return;

        List<GameObject> buildingList = BuildingPool.Instance.GetBuildingList();
        if (buildingList == null)
            return;

        List<Building> rangeBuildingList = new List<Building>();
        for (int i = 0; i < buildingList.Count; ++i)
        {
            GameObject buildingObject = buildingList[i];
            float distance = Vector3.Distance(buildingObject.transform.position, transform.position);
            if (distance <= Range)
            {
                Building building = buildingObject.GetComponent<Building>();
                rangeBuildingList.Add(building);
            }
        }

        if (rangeBuildingList.Count > 0)
        {
            moveSpeed *= 1.1f;
            bKill = true;
            int index = Oracle.RandomDice(0, rangeBuildingList.Count);
            StartCoroutine(CallTeleportAndHit(rangeBuildingList[index]));
        }
    }

    IEnumerator CallTeleportAndHit(Building pBuilding)
    {
        bStun = true;
        Vector3 offset = new Vector3(transform.localScale.x < 0f ? 1f : -1f, 0f, 0f);
        Vector3 OriginPos = transform.position;

        Vector3 TeleportPos = pBuilding.transform.position;
        TeleportPos += offset;
        transform.position = TeleportPos;
        SoundManager.Instance.PlayBossSfx(BossState.WARP);
        yield return new WaitForSeconds(0.5f);

        SetAnimationActionState(LAnimationState.Attack);
        SoundManager.Instance.PlayBossSfx(BossState.HIT);

        pBuilding.AddBuffActor(BuffType.DIE);
        yield return new WaitForSeconds(0.5f);

        transform.position = OriginPos;
        yield return new WaitForSeconds(0.5f);
        bStun = false;
        bKill = false;
    }

    private void SetMoveIndex(int iAdd)
    {
        moveIndex = moveIndex + iAdd;
        if (moveIndex >= movePositions.Length)
        {
            moveIndex = movePositions.Length - 1;
        }
    }
}

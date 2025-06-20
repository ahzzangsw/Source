using CharacterDefines;
using DG.Tweening;
using GameDefines;
using OptionDefines;
using System.Collections;
using UIDefines;
using UnityEngine;

public class BossAdventure_Last : BossAdventure
{
    private Tween m_RotationTween = null;
    private bool m_bEnterStage = false;
    public int m_iLevel { get; private set; } = 0;

    private enum BossDieStep
    {
        None,
        Explosion,
        Appear,
    }

    private BossDieStep m_BossDieStep = BossDieStep.None;

    protected override void Update()
    {
        //base.Update();    //버프로 인한 피해 무시
        if (m_bEnterStage)
        {
            m_bEnterStage = false;
            Bosstransform();
        }

        int iLevel = 0;
        if (currentHP <= Hp / 20)           // HP 5%
        {
            iLevel = 4;
        }
        else if (currentHP <= Hp / 5)       // HP 20%
        {
            iLevel = 3;
        }
        else if (currentHP <= Hp / 2)       // HP 50%
        {
            iLevel = 2;
        }
        else if (currentHP <= Hp * 0.7f)    // HP 70%
        {
            iLevel = 1;
        }

        if (m_iLevel != iLevel)
        {
            m_iLevel = iLevel;
            BossAdventure_Last_Manager.Instance.ChangeLevel(m_iLevel);

            if (m_iLevel == 1)
            {
                StartCoroutine(CallPendulum());
            }
            else if (m_iLevel == 2)
            {
                StartCoroutine(CallBackAndForth());
            }
            else if (m_iLevel == 3)
            {
                StopAllCoroutines();
                StartCoroutine(CallSpin());
            }
            else
            {
                moveSpeed *= 2f;
            }
        }
    }

    public override void SetInfo_Adv(int index, int hp, int defense, float movespeed, float range, int damage, Vector3 pos)
    {
        ID = index;
        Hp = hp;
        currentHP = hp;
        Defense = defense;
        moveSpeed = movespeed;
        Range = range;
        Damage = damage;

        transform.position = pos;

        m_BuffContainer = new BuffContainer(this);
        bOnBuff = false;

        SetBasicInfo();

        GameManager.Instance.GetPlayer().PlayerIgnoreCollision(GetComponent<Collider>());

        m_bEnterStage = true;
    }

    private void Bosstransform()
    {
        m_RotationTween = transform.DORotate(new Vector3(0f, 0f, 360f), 0.5f, RotateMode.Fast).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
        transform.DOMove(new Vector3(0f, 0f, 0f), 3f).SetEase(Ease.InOutQuad).OnComplete(ScaleBoss);
    }

    private void ScaleBoss()
    {
        m_RotationTween.Kill();
        m_RotationTween = null;

        transform.rotation = Quaternion.Euler(Vector3.zero);

        transform.DOScale(transform.localScale * 5f, 1f).OnComplete(() => BossAdventure_Last_Manager.Instance.Ready());

        UI_TargetBar BossTargetHPBar = UIManager.Instance.GetUI(UIIndexType.TARGETBAR) as UI_TargetBar;
        if (BossTargetHPBar)
        {
            BossTargetHPBar.SetUp(this);
            UIManager.Instance.ShowUI(UIIndexType.TARGETBAR);
        }
    }

    private IEnumerator CallPendulum()
    {
        bool bChange = false;
        while (true)
        {
            m_RotationTween = transform.DORotate(new Vector3(0, 0, 45), 1f).SetEase(Ease.InOutSine).SetLoops(1, LoopType.Yoyo).OnStepComplete(() => bChange = true);
            yield return new WaitUntil(() => bChange);
            m_RotationTween = transform.DORotate(new Vector3(0, 0, -45), 1f).SetEase(Ease.InOutSine).SetLoops(1, LoopType.Yoyo).OnStepComplete(() => bChange = false);
            yield return new WaitUntil(() => !bChange);
        }
    }

    private IEnumerator CallBackAndForth()
    {
        float fDirection = 1;
        while (true)
        {
            transform.position += new Vector3(fDirection, 0f, 0f) * moveSpeed * Time.deltaTime;

            if (transform.position.x > 15f || transform.position.x < -15f)
                fDirection *= -1;

            yield return null;
        }
    }

    private IEnumerator CallSpin()
    {
        m_RotationTween.Kill();
        m_RotationTween = null;

        transform.DORotate(new Vector3(0f, 0f, 360f), moveSpeed, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
        yield return null;

        float fDirection = 1;
        while (true)
        {
            transform.position += new Vector3(fDirection, 0f, 0f) * moveSpeed * 1.5f * Time.deltaTime;

            if (transform.position.x > 15f || transform.position.x < -15f)
                fDirection *= -1;

            yield return null;
        }
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

    // - 여기서부터 사망연출
    protected override void OnDie(bool beKilled)
    {
        bEnabled = false;
        GameManager.Instance.OnGameComplete();
        BossAdventure_Last_Manager.Instance.OnDie();
        SoundManager.Instance.StopBGM();
        UIManager.Instance.HideUI(UIIndexType.TARGETBAR);

        SoundManager.Instance.PlayBossSfx(BossState.DOWN);
        Vector3 newScale = transform.localScale / 5f;
        transform.DOScale(newScale, 2.2f).OnComplete(() =>
        {
            SoundManager.Instance.PlayBossSfx(BossState.UP);
            transform.DOMoveY(-15f, 1f).OnComplete(() =>
            {
                StartCoroutine(ExplosionBoss());
            });
        });
    }

    private IEnumerator ExplosionBoss()
    {
        float scaleVector = 7f;

        int SFXIndex = 0;
        SFXType[] SFXTypeList = new SFXType[3]
            {SFXType.SFX_EXPLOSION0, SFXType.SFX_EXPLOSION1, SFXType.SFX_EXPLOSION2};

        while (scaleVector > 2f)
        {
            GameObject ExplosionPrefeb = EffectPool.Instance.GetEffectPrefab(EffectType.EXPLOSION_MISSILE);
            if (ExplosionPrefeb == null)
                break;

            Vector3 newPosition = transform.position;
            newPosition.y += 2f;

            GameObject EffectClone = Instantiate(ExplosionPrefeb, transform.position, Quaternion.identity);
            if (EffectClone == null)
                break;

            EffectBase effectBase = EffectClone.GetComponent<EffectBase>();
            if (effectBase == null)
                break;

            effectBase.transform.localScale *= scaleVector;
            effectBase.SetInfo(null);
            --scaleVector;

            if (SFXIndex >= SFXTypeList.Length)
                SFXIndex = 0;

            SoundManager.Instance.PlaySfx(SFXTypeList[SFXIndex]);
            ++SFXIndex;
            yield return new WaitForSeconds(0.3f);
        }

        
        yield return new WaitForSeconds(2f);
        StartCoroutine(Appear());
    }

    private IEnumerator Appear()
    {
        yield return new WaitForSeconds(3f);

        SoundManager.Instance.PlayBGM(BGMType.ENDING);
        UIManager.Instance.ShowUI(UIIndexType.COMPLETE);
    }
}

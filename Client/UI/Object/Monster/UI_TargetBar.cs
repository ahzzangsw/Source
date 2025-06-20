using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_TargetBar : UIBase
{
    [SerializeField] private Slider m_Slider;
    [SerializeField] private float fillDuration = 1f;

    [SerializeField] private float mediateX = 1f;
    [SerializeField] private float mediateY = 1f;

    private BossBase m_pTarget = null;
    private List<BossBase> PenddingTarget = null;

    private BossAdventure_Last m_pTargetAdventure = null;

    private bool m_bRun = false;
    private bool bFillup = false;

    protected override void Awake()
    {
        PenddingTarget = new List<BossBase>();
    }

    protected override void Update()
    {
        if (m_pTarget == null && m_pTargetAdventure == null)
        {
            if (PenddingTarget.Count == 0)
                Hide();
            else
            {
                for (int i = 0; i < PenddingTarget.Count; ++i)
                {
                    if (PenddingTarget[i] == null)
                    {
                        PenddingTarget.RemoveAt(i);
                        --i;
                        continue;
                    }

                    if (PenddingTarget[i].gameObject.activeSelf == false)
                    {
                        PenddingTarget.RemoveAt(i);
                        --i;
                        continue;
                    }

                    if (PenddingTarget[i].IsDie())
                    {
                        PenddingTarget.RemoveAt(i);
                        --i;
                        continue;
                    }

                    SetUp(PenddingTarget[i], false);
                    break;
                }
            }
                
            return;
        }

        if (m_bRun == false)
            return;

        if (!bFillup)
        {
            RefreshHPBar();
        }

        if (m_pTargetAdventure)
        {
            Vector3 newPosition = m_pTargetAdventure.transform.position;
            newPosition.x += mediateX;
            newPosition.y += mediateY;
            m_Slider.transform.position = newPosition;
        }
    }

    protected override void PreShow()
    {
        m_bRun = true;
    }

    void OnEnable()
    {
        if (bFillup == false)
            return;

        StartCoroutine(FillHPOverTime());
        SoundManager.Instance.PlayForceSound(true);
    }
    public void SetUp(BossBase pTarget, bool isFillup = true)
    {
        if (pTarget == null)
            return;

        //Pendding
        if (m_pTarget != null)
        {
            PenddingTarget.Add(pTarget);
            return;
        }

        m_pTarget = pTarget;
        m_Slider.maxValue = m_pTarget.Hp;
        bFillup = isFillup;
        if (bFillup)
        {
            m_Slider.value = 0f;
        }
        else
        {
            m_Slider.value = m_pTarget.currentHP;
            m_bRun = true;
        }

        Image fillImage = m_Slider.fillRect.GetComponent<Image>();
        if (fillImage.enabled == false)
            fillImage.enabled = true;
    }

    public void SetUp(BossAdventure_Last pTarget, bool isFillup = true)
    {
        if (pTarget == null)
            return;

        m_pTargetAdventure = pTarget;
        m_Slider.maxValue = m_pTargetAdventure.Hp;
        bFillup = isFillup;
        if (bFillup)
        {
            m_Slider.value = 0f;
        }
        else
        {
            m_Slider.value = m_pTargetAdventure.currentHP;
            m_bRun = true;
        }

        Image fillImage = m_Slider.fillRect.GetComponent<Image>();
        if (fillImage.enabled == false)
            fillImage.enabled = true;
    }

    public void RefreshHPBar()
    {
        if (m_pTarget != null)
            m_Slider.value = (float)m_pTarget.currentHP;
        else if (m_pTargetAdventure != null)
            m_Slider.value = (float)m_pTargetAdventure.currentHP;

        if (m_Slider.value <= 0)
        {
            Image fillImage = m_Slider.fillRect.GetComponent<Image>();
            if (fillImage.enabled)
                fillImage.enabled = false;

            m_bRun = false;
        }
    }

    private IEnumerator FillHPOverTime()
    {
        float elapsedTime = 0f;
        float startValue = m_Slider.value;
        float endValue = m_Slider.maxValue;

        while (elapsedTime < fillDuration)
        {
            elapsedTime += Time.deltaTime;
            float currentValue = Mathf.Lerp(startValue, endValue, elapsedTime / fillDuration);
            m_Slider.value = currentValue;
            yield return null;
        }

        m_Slider.value = endValue;
        bFillup = false;
        SoundManager.Instance.PlayForceSound(false);
    }
}

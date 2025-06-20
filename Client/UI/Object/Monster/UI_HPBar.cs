using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HPBar : UIBase
{
    [SerializeField] private Vector3 distance = Vector3.down * 20f;

    private Camera mainCamera;
    private MonsterBase m_pTarget;
    private RectTransform rectTransform;
    private Slider m_Slider;

    protected override void Update()
    {
        if (m_pTarget == null)
        {
            return;
        }

        RefreshHPBar();

        Vector3 targetPosition = m_pTarget.transform.position + distance;
        rectTransform.position = new Vector3(targetPosition.x, targetPosition.y, -2); 
    }

    public void SetUp(MonsterBase target)
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Destroy(gameObject);
        }

        m_pTarget = target;
        rectTransform = GetComponent<RectTransform>();
        m_Slider = GetComponent<Slider>();
    }

    public void RefreshHPBar()
    {
        m_Slider.value = (float)m_pTarget.currentHP / (float)m_pTarget.Hp;
    }
}

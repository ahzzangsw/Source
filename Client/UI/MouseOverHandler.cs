using System.Collections;
using UIDefines;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class MouseOverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private UITooltipType eUITooltipType = UITooltipType.STRING;
    [SerializeField] private string m_Text = "";//String Tooltip
    [SerializeField] private Vector2 m_Pivot = new Vector2(999f, 999f);

    public event EventHandler<GameObject> ButtonMouseOver;
    public event EventHandler<GameObject> ButtonMouseOut;

    public void OnPointerEnter(PointerEventData eventData)
    {
        switch (eUITooltipType)
        {
            case UITooltipType.STRING:
            case UITooltipType.BUILDINGINFO:
            case UITooltipType.SPAWNBOSSINFO:
                UIManager.Instance.ShowTooltip(eUITooltipType, m_Text, m_Pivot);
                break;
            default:
                UIManager.Instance.HideUI(UIIndexType.TOOLTIP);
                break;
        };

        if (ButtonMouseOver == null || eventData == null)
            return;

        GameObject overedObject = eventData.pointerEnter;
        if (overedObject == null)
            return;

        ButtonMouseOver(this, overedObject);

        Player player = GameManager.Instance.GetPlayer();
        if (player)
            player.isUIMouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.Instance.HideUI(UIIndexType.TOOLTIP);

        if (ButtonMouseOut == null || eventData == null)
            return;

        GameObject overedObject = eventData.pointerEnter;
        ButtonMouseOut(this, overedObject);

        Player player = GameManager.Instance.GetPlayer();
        if (player)
            player.isUIMouseOver = false;
    }

    public void SetStringTooltip(string text)
    {
        m_Text = text;
    }
}

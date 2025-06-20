using GameDefines;
using OptionDefines;
using UnityEngine;
using UnityEngine.UI;

public class UI_MapChoice : UIBase
{
    //[SerializeField] private Button[] m_MapBtnList;
    //[SerializeField] private Button m_StartBtn;
    //[SerializeField] private Button m_BackBtn;

    //private MapType m_eSelectedMapType = MapType.MAX;

    //public override void SetControlInfo()
    //{
    //    if (m_MapBtnList != null)
    //    {
    //        for (int i = 0; i < m_MapBtnList.Length; ++i)
    //        {
    //            int index = i;
    //            m_MapBtnList[i].onClick.AddListener(() => OnClickMap(index));
    //        }
    //    }

    //    if (m_StartBtn)
    //    {
    //        m_StartBtn.onClick.AddListener(OnClickGame);
    //    }

    //    if (m_BackBtn)
    //        m_BackBtn.onClick.AddListener(OnClickBack); 
    //}

    //private void OnEnable()
    //{
    //    m_StartBtn.enabled = false;
    //    m_eSelectedMapType = MapType.MAX;
    //}

    //private void OnClickGame()
    //{
    //    if (m_eSelectedMapType == MapType.MAX)
    //        return;

    //    LobbyManager.Instance.StartGame(m_eSelectedMapType);
    //}
    //private void OnClickBack()
    //{
    //    SoundManager.Instance.PlayUISound(UISoundType.BUTTONCLICK);
    //    UIManager.Instance.HideUI(eUIIndexType);
    //}

    //private void OnClickMap(int index)
    //{
    //    if (index < 0 || index >= (int)MapType.MAX)
    //        return;

    //    MapType eSelectedMapType = (MapType)index;
    //    if (m_eSelectedMapType == eSelectedMapType)
    //        return;

    //    m_eSelectedMapType = eSelectedMapType;
    //    SoundManager.Instance.PlayUISound(UISoundType.BUTTONCLICK);
    //    m_StartBtn.enabled = true;
    //}
}

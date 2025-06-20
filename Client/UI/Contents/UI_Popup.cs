using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_Popup : UIBase 
{
    [SerializeField] public Text m_Text;
    [SerializeField] public Button m_YesBtn;
    [SerializeField] public Button m_NoBtn;

    public UIEventArgs args;
    public event Action<UIEventArgs> OnPopUpOK;

    protected override void Awake()
    {
        args = new UIEventArgs();
        m_YesBtn.onClick.AddListener(OnClickOK);
        m_NoBtn.onClick.AddListener(DestroyData);
    }

    protected override void Update()
    {
        if (UIManager.Instance.bShowDialog == false)
            return;

        if(Input.GetKeyDown(KeyCode.Escape))
            DestroyData();
    }

    public void SetText(string text)
    {
        m_Text.text = text;
    }

    private void OnClickOK()
    {
        OnPopUpOK?.Invoke(args);
    }

    private void DestroyData()
    {
        UIManager.Instance.bShowDialog = false;

        OnPopUpOK -= OnPopUpOK;
        Destroy(gameObject);
    }
}

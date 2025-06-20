using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Intro : UIBase
{
    [SerializeField] private Image m_Image;
    [SerializeField] private Text m_Title;
    [SerializeField] private float changeDuration;

    private float currentTime = 0.0f; // 현재 시간

    protected override void Awake()
    {
        StartCoroutine(ChangeAlphaOverTime());
    }

    IEnumerator ChangeAlphaOverTime()
    {
        while (currentTime < changeDuration)
        {
            currentTime += Time.deltaTime;

            float normalizedTime = currentTime / changeDuration;
            float alphaValue = Mathf.Lerp(0f, 1f, normalizedTime);

            Color newImageColor = m_Image.color;
            newImageColor.a = alphaValue;
            m_Image.color = newImageColor;

            Color newTextColor = m_Title.color;
            newTextColor.a = alphaValue;
            m_Title.color = newTextColor;

            yield return null;
        }

        NextScean();
    }

    void NextScean()
    {
        Oracle.m_bIntro = true;
        SceneManager.LoadScene("LobbyScene");
    }
}

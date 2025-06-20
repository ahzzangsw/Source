using OptionDefines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using UIDefines;

public class OptionManager : Singleton<OptionManager>
{
    public int iCursorIndex = 0;

    public bool bFullScreen = true; // Full Screen Mode
    public int iResolutionIndex = 0;
    public List<Vector2> vResolutionList = new List<Vector2>();

    public bool bFlashEffect = true;
    public bool bHitEffect = true;

    public float fCameraSpeed = 0f;

    public int iTooltipSortType = 0;

    private UI_Cursor CursorUI = null;

    protected override void Awake()
    {
        base.Awake();

        CursorUI = GetComponent<UI_Cursor>();

        vResolutionList.Clear();
        vResolutionList.Add(new Vector2(1280, 720));
        vResolutionList.Add(new Vector2(1920, 1080));

        LoadOptionData();
    }

    public void SetFullScreen(bool newFullScreen)
    {
        if (bFullScreen == newFullScreen)
            return;

        bFullScreen = newFullScreen;
        Screen.fullScreen = bFullScreen;
    }

    public void SetResolution(int index)
    {
        if (index < 0 || index >= vResolutionList.Count)
            return;

        if (iResolutionIndex == index)
            return;

        iResolutionIndex = index;
        int iResolutionX = (int)(vResolutionList[index].x);
        int iResolutionY = (int)(vResolutionList[index].y);
        Screen.SetResolution(iResolutionX, iResolutionY, bFullScreen);
    }

    private void LoadOptionData()
    {
        // Cursor
        iCursorIndex = PlayerPrefs.GetInt("cursor", 0);

        // Effect
        bHitEffect = PlayerPrefs.GetInt("hiteffect", 1) > 0 ? true : false;
        bFlashEffect = PlayerPrefs.GetInt("flasheffect", 1) > 0 ? true : false;

        // Screen
        bFullScreen = PlayerPrefs.GetInt("fullscreen", 0) > 0 ? true : false;
        iResolutionIndex = PlayerPrefs.GetInt("resolutionindex", 1);

        if (iResolutionIndex < 0 || iResolutionIndex >= vResolutionList.Count)
        {
            Debug.Log("OptionManager::LoadOptionData iResolutionIndex error");
            return;   
        }

        Screen.SetResolution((int)(vResolutionList[iResolutionIndex].x), (int)(vResolutionList[iResolutionIndex].y), bFullScreen);

        fCameraSpeed = PlayerPrefs.GetFloat("gamecameraspeed", 15f);

        iTooltipSortType = PlayerPrefs.GetInt("tooltipsorttype", 0);
    }

    public void SaveOptionData()
    {
        PlayerPrefs.SetInt("cursor", iCursorIndex);
        PlayerPrefs.SetInt("hiteffect", bHitEffect ? 1 : 0);
        PlayerPrefs.SetInt("flasheffect", bFlashEffect ? 1 : 0);
        PlayerPrefs.SetInt("fullscreen", bFullScreen ? 1 : 0);
        PlayerPrefs.SetInt("resolutionindex", iResolutionIndex);
        PlayerPrefs.SetFloat("gamecameraspeed", fCameraSpeed);
        PlayerPrefs.SetInt("tooltipsorttype", iTooltipSortType);
        SoundManager.Instance.SaveOptionData();
    }

    public bool ChangeEffect(bool bHit)
    {
        if (bHit)
        {
            bHitEffect = !bHitEffect;
            return bHitEffect;
        }
        else
        {
            bFlashEffect = !bFlashEffect;
            return bFlashEffect;
        }
    }
        

    public void ChangeCursor(int index)
    {
        if (iCursorIndex == index)
            return;

        iCursorIndex = index;
        PlayerPrefs.SetInt("cursor", iCursorIndex);

        CursorUI.SetCursor();
        SoundManager.Instance.PlayUISound(UISoundType.CLICK);
    }

    public void RefreshCameraSpeed(float fSpeed)
    {
        fCameraSpeed = fSpeed;
        if (fCameraSpeed < 5f)
            fCameraSpeed = 5f;
        else if (fCameraSpeed > 25f)
            fCameraSpeed = 25f;

        CameraManager.Instance.moveSpeed = fCameraSpeed;
    }

    public void ChangeTooltipSortType(UITooltipSortType eUITooltipSortType)
    {
        iTooltipSortType = (int)eUITooltipSortType;
        UIManager.Instance.ChangeTooltipSortType();
    }

    public void FirstSetCursor()
    {
        CursorUI.SetCursor();
    }
}

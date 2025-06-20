using GameDefines;
using UnityEngine;
using OptionDefines;
using UnityEngine.Audio;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] private AudioMixer mainAudioController;
    [Header("#BGM")]
    [SerializeField] private AudioClip[] bgmClip = null;
    [SerializeField] public float bgmVolume;
    public bool bBGMMute = false;
    private AudioSource bgmPlayer = null;
    private BGMType m_eBGMType = BGMType.MAX;
    private int m_iBGMIndex = -1;

    [Header("#SFX")]
    [SerializeField] public float sfxVolume;
    public bool bSFXMute = false;

    [SerializeField] private int channelMaxCount;
    [SerializeField] private AudioClip[] sfxClip = null;
    private AudioSource[] sfxPlayers = null;
    private AudioSource[] sfxPlayers_Fast = null;
    private int sfxFastMaxCount = 5;

    [Header("#SFX_Character")]
    [SerializeField] int channelCharMaxCount;
    [SerializeField] private AudioClip[] sfxClip_Character = null;
    private AudioSource[] sfxCharacterPlayers = null;
    private int channelCharIndex;

    [Header("#SFX_Boss")]
    [SerializeField] private AudioClip[] sfxClip_Boss = null;
    private AudioSource sfxBossPlayer;

    [Header("#UI")]
    [SerializeField] private AudioClip[] uiClip = null;
    [SerializeField] int channelUIMaxCount;
    private AudioSource[] sfxUIPlayers = null;
    private int channelUIIndex;

    [Header("#Force")]
    private AudioSource sfxForcePlayer;
    private int SfxSaveIndex = -1;

    protected override void Awake()
    {
        base.Awake();

        LoadOptionData();
        InitSound();
    }

    private void LoadOptionData()
    {
        bBGMMute = PlayerPrefs.GetInt("bgmmute", 0) > 0 ? true : false;
        bSFXMute = PlayerPrefs.GetInt("sfxmute", 0) > 0 ? true : false;
        bgmVolume = PlayerPrefs.GetFloat("bgmvolume", 0.5f);
        sfxVolume = PlayerPrefs.GetFloat("sfxvolume", 0.5f);
    }

    public void SaveOptionData()
    {
        PlayerPrefs.SetInt("bgmmute", bBGMMute ? 1 : 0);
        PlayerPrefs.SetInt("sfxmute", bSFXMute ? 1 : 0);
        PlayerPrefs.SetFloat("bgmvolume", bgmVolume);
        PlayerPrefs.SetFloat("sfxvolume", sfxVolume);
    }

    private void InitSound()
    {
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.outputAudioMixerGroup = mainAudioController.FindMatchingGroups("BGM")[0];
        bgmPlayer.priority = 0;
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume;
        bgmPlayer.mute = bBGMMute;

        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[channelMaxCount];
        for (int i = 0; i < channelMaxCount; ++i)
        {
            sfxPlayers[i] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[i].outputAudioMixerGroup = mainAudioController.FindMatchingGroups("SFX")[0];
            sfxPlayers[i].playOnAwake = false;
            sfxPlayers[i].loop = false;
            sfxPlayers[i].volume = sfxVolume;
            sfxPlayers[i].mute = bSFXMute;
        }

        sfxPlayers_Fast = new AudioSource[sfxFastMaxCount];
        for (int i = 0; i < sfxFastMaxCount; ++i)
        {
            sfxPlayers_Fast[i] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers_Fast[i].outputAudioMixerGroup = mainAudioController.FindMatchingGroups("SFX")[0];
            sfxPlayers_Fast[i].playOnAwake = false;
            sfxPlayers_Fast[i].loop = false;
            sfxPlayers_Fast[i].volume = sfxVolume;
            sfxPlayers_Fast[i].mute = bSFXMute;
        }

        GameObject sfxCharObject = new GameObject("SfxCharPlayer");
        sfxCharObject.transform.parent = transform;
        sfxCharacterPlayers = new AudioSource[channelCharMaxCount];
        for (int i = 0; i < channelCharMaxCount; ++i)
        {
            sfxCharacterPlayers[i] = sfxCharObject.AddComponent<AudioSource>();
            sfxCharacterPlayers[i].outputAudioMixerGroup = mainAudioController.FindMatchingGroups("SFX")[0];
            sfxCharacterPlayers[i].playOnAwake = false;
            sfxCharacterPlayers[i].loop = false;
            sfxCharacterPlayers[i].volume = sfxVolume;
            sfxCharacterPlayers[i].mute = bSFXMute;
        }

        GameObject sfxBossObject = new GameObject("SfxBossPlayer");
        sfxBossObject.transform.parent = transform;
        sfxBossPlayer = sfxBossObject.AddComponent<AudioSource>();
        sfxBossPlayer.outputAudioMixerGroup = mainAudioController.FindMatchingGroups("SFX")[0];
        sfxBossPlayer.playOnAwake = false;
        sfxBossPlayer.loop = false;
        sfxBossPlayer.volume = sfxVolume;
        sfxBossPlayer.mute = bSFXMute;

        GameObject sfxForceObject = new GameObject("SfxForcePlayer");
        sfxForceObject.transform.parent = transform;
        sfxForcePlayer = sfxForceObject.AddComponent<AudioSource>();
        sfxForcePlayer.outputAudioMixerGroup = mainAudioController.FindMatchingGroups("SFX")[0];
        sfxForcePlayer.playOnAwake = false;
        sfxForcePlayer.loop = true;
        sfxForcePlayer.volume = sfxVolume;
        sfxForcePlayer.mute = bSFXMute;

        GameObject uiObject = new GameObject("UIPlayer");
        uiObject.transform.parent = transform;
        sfxUIPlayers = new AudioSource[channelUIMaxCount];
        for (int i = 0; i < channelUIMaxCount; ++i)
        {
            sfxUIPlayers[i] = uiObject.AddComponent<AudioSource>();
            sfxUIPlayers[i].playOnAwake = false;
            sfxUIPlayers[i].loop = false;
            sfxUIPlayers[i].volume = sfxVolume;
            sfxUIPlayers[i].mute = bSFXMute;
        }
    }

    public void RefreshVolume(SoundType eSoundType, float fVolume)
    {
        if (eSoundType == SoundType.BGM)
        {
            if (bgmVolume == fVolume)
                return;

            bgmVolume = fVolume;
            if (bgmVolume < 0f)
                bgmVolume = 0f;

            bgmPlayer.volume = bgmVolume;
        }
        else
        {
            if (sfxVolume == fVolume)
                return;

            sfxVolume = fVolume;
            if (sfxVolume < 0f)
                sfxVolume = 0f;
            else if (sfxVolume > 1f)
                sfxVolume = 1f;

            for (int i = 0; i < channelMaxCount; ++i)
            {
                if(sfxPlayers[i] != null)
                    sfxPlayers[i].volume = sfxVolume;
            }
            for (int i = 0; i < sfxFastMaxCount; ++i)
            {
                if (sfxPlayers_Fast[i] != null)
                    sfxPlayers_Fast[i].volume = sfxVolume;
            }

            for (int i = 0; i < channelCharMaxCount; ++i)
            {
                if (sfxCharacterPlayers[i] != null)
                    sfxCharacterPlayers[i].volume = sfxVolume;
            }

            for (int i = 0; i < channelUIMaxCount; ++i)
            {
                if (sfxUIPlayers[i] != null)
                    sfxUIPlayers[i].volume = sfxVolume;
            }

            sfxBossPlayer.volume = sfxVolume;
        }
    }

    public bool MuteVolume(SoundType eSoundType)
    {
        if (eSoundType == SoundType.BGM)
        {
            bBGMMute = !bBGMMute;
            bgmPlayer.mute = bBGMMute;
            return bBGMMute;
        }
        else
        {
            bSFXMute = !bSFXMute;
            for (int i = 0; i < channelMaxCount; ++i)
            {
                if (sfxPlayers[i] != null)
                {
                    sfxPlayers[i].mute = bSFXMute;
                }
            }
            for (int i = 0; i < sfxFastMaxCount; ++i)
            {
                if (sfxPlayers_Fast[i] != null)
                {
                    sfxPlayers_Fast[i].mute = bSFXMute;
                }
            }

            for (int i = 0; i < channelCharMaxCount; ++i)
            {
                if (sfxCharacterPlayers[i] != null)
                {
                    sfxCharacterPlayers[i].mute = bSFXMute;
                }
            }

            for (int i = 0; i < channelUIMaxCount; ++i)
            {
                if (sfxUIPlayers[i] != null)
                    sfxUIPlayers[i].mute = bSFXMute;
            }

            sfxBossPlayer.mute = bSFXMute;
            return bSFXMute;
        }
    }

    public void PlayBGM(BGMType eBGMType, int stageindex = 0)
    {
        int index = (int)eBGMType;
        if (Oracle.m_eGameType == MapType.SPAWN)
        {
            index = stageindex / 10 + 1;
            if (stageindex % 10 == 0)
                --index;
            if (m_iBGMIndex == index)
                return;
        }
        else
        {
            if (m_eBGMType == eBGMType)
                return;

            if (eBGMType == BGMType.STAGE)
            {
                index = stageindex / 10 + 1;
            }
        }

        m_eBGMType = eBGMType;
        m_iBGMIndex = index;

        if (Oracle.m_eGameType == MapType.SPAWN)
        {
            if (index > 17)
            {
                index = Oracle.RandomDice(1, (int)(BGMType.BOSS));
            }
        }

        bgmPlayer.Stop();
        bgmPlayer.clip = bgmClip[index];
        bgmPlayer.Play();
    }

    public void StopBGM()
    {
        bgmPlayer.Stop();
    }

    public void PlaySfx(SFXType eSFXType)
    {
        int sfxIndex = (int)eSFXType;
        if (sfxIndex < 0 || sfxIndex >= sfxClip.Length)
        {
            Debug.Log("PlaySfx Index error SFXType=" + eSFXType);
            return;
        }

        int tempChannelIndex = -1;
        for (int i = 0; i < sfxPlayers.Length; ++i)
        {
            int loopIndex = (i + channelCharIndex) % sfxPlayers.Length;
            if (sfxPlayers[loopIndex].isPlaying)
                continue;

            tempChannelIndex = loopIndex;
            break;
        }

        if (tempChannelIndex < 0)
            return;

        sfxPlayers[tempChannelIndex].Stop();
        sfxPlayers[tempChannelIndex].clip = sfxClip[sfxIndex];
        sfxPlayers[tempChannelIndex].Play();
    }

    public int PlaySfxLoop(SFXType eSFXType)
    {
        int sfxIndex = (int)eSFXType;
        if (sfxIndex < 0 || sfxIndex >= sfxClip.Length)
        {
            Debug.Log("PlaySfx Index error SFXType=" + eSFXType);
            return -1;
        }

        int tempChannelIndex = -1;
        for (int i = 0; i < sfxPlayers.Length; ++i)
        {
            int loopIndex = (i + channelCharIndex) % sfxPlayers.Length;
            if (sfxPlayers[loopIndex].isPlaying)
                continue;

            tempChannelIndex = loopIndex;
            break;
        }

        if (tempChannelIndex < 0)
            return -1;

        sfxPlayers[tempChannelIndex].Stop();
        sfxPlayers[tempChannelIndex].clip = sfxClip[sfxIndex];
        sfxPlayers[tempChannelIndex].loop = true;
        sfxPlayers[tempChannelIndex].Play();
        return tempChannelIndex;
    }

    public void StopSfxLoop(int index)
    {
        if (index < 0 || index >= sfxPlayers.Length)
            return;

        sfxPlayers[index].loop = false;
        sfxPlayers[index].Stop();
    }

    public void PlaySfxFast(SFXType eSFXType)
    {
        int sfxIndex = (int)eSFXType;
        if (sfxIndex < 0 || sfxIndex >= sfxClip.Length)
        {
            Debug.Log("PlaySfx Index error SFXType=" + eSFXType);
            return;
        }

        int tempChannelIndex = -1;
        for (int i = 0; i < sfxFastMaxCount; ++i)
        {
            int loopIndex = (i + channelCharIndex) % sfxPlayers_Fast.Length;
            if (sfxPlayers_Fast[loopIndex].isPlaying)
                continue;

            tempChannelIndex = loopIndex;
            break;
        }

        if (tempChannelIndex < 0)
            return;

        sfxPlayers_Fast[tempChannelIndex].Stop();
        sfxPlayers_Fast[tempChannelIndex].clip = sfxClip[sfxIndex];
        sfxPlayers_Fast[tempChannelIndex].Play();
    }

    public void PlayCharacterSfx(SpeciesType eSpeciesType, CharacterState eCharacterState)
    {
        int sfxIndex = (int)eSpeciesType * (int)eCharacterState;
        if(eCharacterState == CharacterState.OINK)
            sfxIndex = sfxClip_Character.Length - 1;
        else if (eCharacterState == CharacterState.SPAWN_BASE)
            sfxIndex = sfxClip_Character.Length - 2;
        else if (eCharacterState == CharacterState.DIE)
            sfxIndex = sfxClip_Character.Length - 3;
            
        if (sfxIndex < 0 || sfxIndex >= sfxClip_Character.Length)
        {
            Debug.Log("PlayCharacterSfx Index error Species=" + eSpeciesType + ", State=" + eCharacterState);
            return;
        }

        for (int i = 0; i < sfxCharacterPlayers.Length; ++i)
        {
            int loopIndex = (i + channelCharIndex) % sfxCharacterPlayers.Length;
            if (sfxCharacterPlayers[loopIndex].isPlaying)
                continue;

            channelCharIndex = loopIndex;
            break;
        }

        if (channelCharIndex == sfxCharacterPlayers.Length - 1)
        {
            channelCharIndex = 0;
        }

        sfxCharacterPlayers[channelCharIndex].Stop();
        sfxCharacterPlayers[channelCharIndex].clip = sfxClip_Character[sfxIndex];
        sfxCharacterPlayers[channelCharIndex].Play();
    }

    public void PlayCharacterSfx_SaveIndex(CharacterState eCharacterState) // 하나만 써야함
    {
        int sfxIndex = (int)eCharacterState;
        if (eCharacterState == CharacterState.OINK)
            sfxIndex = sfxClip_Character.Length - 1;

        if (sfxIndex < 0 || sfxIndex >= sfxClip_Character.Length)
        {
            Debug.Log("PlayCharacterSfx Index error = " + eCharacterState);
            return;
        }

        if (SfxSaveIndex >= 0)
        {
            if (sfxCharacterPlayers[SfxSaveIndex].isPlaying)
                return;

            SfxSaveIndex = -1;
        }

        int tempChannelIndex = -1;
        for (int i = 0; i < sfxCharacterPlayers.Length; ++i)
        {
            int loopIndex = (i + channelCharIndex) % sfxCharacterPlayers.Length;
            if (sfxCharacterPlayers[loopIndex].isPlaying)
                continue;

            tempChannelIndex = loopIndex;
            break;
        }

        if (tempChannelIndex < 0)
            return;

        sfxCharacterPlayers[tempChannelIndex].Stop();
        sfxCharacterPlayers[tempChannelIndex].clip = sfxClip_Character[sfxIndex];
        sfxCharacterPlayers[tempChannelIndex].Play();

        SfxSaveIndex = tempChannelIndex;
    }

    public void PlayBossSfx(BossState eBossState)
    {
        int sfxIndex = (int)eBossState;
        if (sfxIndex < 0 || sfxIndex >= sfxClip_Boss.Length)
        {
            Debug.Log("PlayBossSfx Index error BossState=" + eBossState);
            return;
        }

        sfxBossPlayer.Stop();
        sfxBossPlayer.clip = sfxClip_Boss[sfxIndex];
        sfxBossPlayer.Play();
    }

    public void PlayForceSound(bool bPlay)
    {
        sfxForcePlayer.Stop();
        if (bPlay)
        {
            if (Oracle.RandomDice(0, 2) > 0)
                sfxForcePlayer.clip = sfxClip_Boss[(int)(BossState.RISEHP1)];
            else
                sfxForcePlayer.clip = sfxClip_Boss[(int)(BossState.RISEHP2)];
            sfxForcePlayer.Play();
        }
    }

    public void PlayUISound(UISoundType eUISoundType)
    {
        int uiIndex = (int)eUISoundType;
        if (uiIndex < 0 || uiIndex >= uiClip.Length)
        {
            Debug.Log("PlayUISound Index error UISoundType=" + eUISoundType);
            return;
        }

        int tempChannelIndex = -1;
        for (int i = 0; i < sfxUIPlayers.Length; ++i)
        {
            int loopIndex = (i + channelCharIndex) % sfxUIPlayers.Length;
            if (sfxUIPlayers[loopIndex].isPlaying)
                continue;

            tempChannelIndex = loopIndex;
            break;
        }

        if (tempChannelIndex < 0)
            return;

        channelUIIndex = tempChannelIndex;
        sfxUIPlayers[channelUIIndex].Stop();
        sfxUIPlayers[channelUIIndex].clip = uiClip[uiIndex];
        sfxUIPlayers[channelUIIndex].Play();
    }

    public void PlayUISoundLoop(UISoundType eUISoundType)
    {
        int uiIndex = (int)eUISoundType;
        if (uiIndex < 0 || uiIndex >= uiClip.Length)
        {
            Debug.Log("PlayUISound Index error UISoundType=" + eUISoundType);
            return;
        }

        int tempChannelIndex = -1;
        for (int i = 0; i < sfxUIPlayers.Length; ++i)
        {
            int loopIndex = (i + channelCharIndex) % sfxUIPlayers.Length;
            if (sfxUIPlayers[loopIndex].isPlaying)
                continue;

            tempChannelIndex = loopIndex;
            break;
        }

        if (tempChannelIndex < 0)
            return;

        channelUIIndex = tempChannelIndex;
        sfxUIPlayers[channelUIIndex].Stop();
        sfxUIPlayers[channelUIIndex].clip = uiClip[uiIndex];
        sfxUIPlayers[channelUIIndex].Play();
    }

    public void StopUISound(UISoundType eUISoundType)
    {
        int uiIndex = (int)eUISoundType;
        if (uiIndex < 0 || uiIndex >= uiClip.Length)
        {
            Debug.Log("StopUISound Index error UISoundType=" + eUISoundType);
            return;
        }

        for (int i = 0; i < sfxUIPlayers.Length; ++i)
        {
            int loopIndex = (i + channelCharIndex) % sfxUIPlayers.Length;
            if (sfxUIPlayers[loopIndex].isPlaying == false)
                continue;

            if (sfxUIPlayers[channelUIIndex].clip == uiClip[uiIndex])
            {
                sfxUIPlayers[channelUIIndex].Stop();
                break;
            }
        }
        
    }

    public void ReStart()
    {
        m_eBGMType = BGMType.MAX;
        AllClear();
    }

    public void AllClear()
    {
        Action<AudioSource> forceStopSound = (player) =>
        {
            player.Stop();
            player.clip = null;
        };
        forceStopSound(bgmPlayer);
        forceStopSound(sfxBossPlayer);
        forceStopSound(sfxForcePlayer);

        Action<AudioSource[]> forceStopSounds = (players) =>
        {
            for (int i = 0; i < players.Length; ++i)
            {
                players[i].Stop();
                players[i].clip = null;
            }
        };
        forceStopSounds(sfxPlayers);
        forceStopSounds(sfxPlayers_Fast);
        forceStopSounds(sfxCharacterPlayers);
        forceStopSounds(sfxUIPlayers);
    }
}

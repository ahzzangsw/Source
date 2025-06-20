using GameDefines;
using OptionDefines;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : Singleton<LobbyManager>
{
    [SerializeField] private float MonsterDelay = 1f;

    [SerializeField] private UISoundType eUISoundType;
    protected override void Awake()
    {
        MapManager.Instance.mapIndex = 0;
        StartCoroutine(SpawnMonsterCoroutine());
    }

    protected override void OnDestroy()
    {
        StopCoroutine(SpawnMonsterCoroutine());
        //base.OnDestroy();
    }

    public void Exit()
    {
        SoundManager.Instance.PlayUISound(UISoundType.BACK);
        OptionDefines.CSetOption.GameQuit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private IEnumerator SpawnMonsterCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(MonsterDelay);
            SpawnMonster();
        }
    }

    private void SpawnMonster()
    {
        SpeciesType eSpeciesType = (SpeciesType)Oracle.RandomDice(1, (int)SpeciesType.MAX);
        List<GameObject> refMonsterPrefabs = ResourceAgent.Instance.GetPrefab(eSpeciesType);
        if (refMonsterPrefabs == null)
        {
            Debug.Log("SpawnMonster is null = " + eSpeciesType);
        }

        int prefabsIndex = Oracle.RandomDice(0, 5);
        GameObject monster = Instantiate(refMonsterPrefabs[prefabsIndex]);
        Character tempCharacterInfo = monster.GetComponent<Character>();
        if (tempCharacterInfo != null)
        {
            MonsterBase tempMonsterInfo = monster.AddComponent<MonsterBase>();
            tempMonsterInfo.SetComponent(tempCharacterInfo);
            tempMonsterInfo.m_eSpeciesType = eSpeciesType;
            tempMonsterInfo.bOnForceRemove = true;
            tempMonsterInfo.SetInfo(0, 1, 0, 1, 0);
            Destroy(tempCharacterInfo);
        }
    }

    public void StartGame(MapType eMapType)
    {
        Oracle.m_eGameType = eMapType;
        if (eMapType == MapType.BUILD)
            SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Single).completed += GameManager.Instance.OnLoadComplete;
        else if (eMapType == MapType.SPAWN)
            SceneManager.LoadSceneAsync("GameScene2", LoadSceneMode.Single).completed += GameManager.Instance.OnLoadComplete;
        else if (eMapType == MapType.ADVENTURE)
            SceneManager.LoadSceneAsync("GameScene3", LoadSceneMode.Single).completed += GameManager.Instance.OnLoadComplete;
    }
}

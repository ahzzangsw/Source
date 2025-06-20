using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using CharacterDefines;
using GameDefines;
using System.Threading;

public class Put : Projectile
{
    private Tilemap MonsterWayTileInfo;
    private Vector3[] Directions8Position;
    private List<Vector3> PutPositionList;

    private List<int> AlreadyIndexList;
    private List<int> AlreadyPutIndexList;

    void Awake()
    {
        if (MonsterWayTileInfo == null)
        {
            GameObject pTileObject = GameObject.FindWithTag("MonsterWayTile");
            if (pTileObject)
            {
                MonsterWayTileInfo = pTileObject.GetComponent<Tilemap>();
            }
        }

        PutPositionList = new List<Vector3>();
        AlreadyIndexList = new List<int>();
        AlreadyPutIndexList = new List<int>();
    }

    protected override IEnumerator Search()
    {
        while (true)
        {
            if (MonsterWayTileInfo)
            {
                Vector3 MasterPosition = m_Master.transform.position;
                if (Oracle.m_eGameType == MapType.BUILD)
                {
                    Directions8Position = new Vector3[8];
                    Directions8Position[0] = new Vector3(MasterPosition.x, MasterPosition.y + 1f, MasterPosition.z);// »ó
                    Directions8Position[1] = new Vector3(MasterPosition.x, MasterPosition.y - 1f, MasterPosition.z);// ÇÏ
                    Directions8Position[2] = new Vector3(MasterPosition.x - 1f, MasterPosition.y, MasterPosition.z);// ÁÂ
                    Directions8Position[3] = new Vector3(MasterPosition.x + 1f, MasterPosition.y, MasterPosition.z);// ¿ì
                    Directions8Position[4] = new Vector3(MasterPosition.x - 1f, MasterPosition.y + 1f, MasterPosition.z);// ÁÂ»ó
                    Directions8Position[5] = new Vector3(MasterPosition.x - 1f, MasterPosition.y - 1f, MasterPosition.z);// ÁÂÇÏ
                    Directions8Position[6] = new Vector3(MasterPosition.x + 1f, MasterPosition.y + 1f, MasterPosition.z);// ¿ì»ó
                    Directions8Position[7] = new Vector3(MasterPosition.x + 1f, MasterPosition.y - 1f, MasterPosition.z);// ¿ìÇÏ
                }
                else
                {
                    Directions8Position = new Vector3[23];
                    Directions8Position[0] = new Vector3(MasterPosition.x + 2f, MasterPosition.y, MasterPosition.z);// ¿ì
                    Directions8Position[1] = new Vector3(MasterPosition.x + 2f, MasterPosition.y - 1f, MasterPosition.z);// ¿ìÇÏ
                    Directions8Position[2] = new Vector3(MasterPosition.x + 2f, MasterPosition.y + 1f, MasterPosition.z);// ¿ì»ó
                    Directions8Position[3] = new Vector3(MasterPosition.x - 2.5f, MasterPosition.y, MasterPosition.z);// ÁÂ
                    Directions8Position[4] = new Vector3(MasterPosition.x - 2.5f, MasterPosition.y - 1f, MasterPosition.z);// ÁÂÇÏ
                    Directions8Position[5] = new Vector3(MasterPosition.x - 2.5f, MasterPosition.y + 1f, MasterPosition.z);// ÁÂ»ó
                    Directions8Position[6] = new Vector3(MasterPosition.x, MasterPosition.y + 2f, MasterPosition.z);// »ó
                    Directions8Position[7] = new Vector3(MasterPosition.x, MasterPosition.y - 2.5f, MasterPosition.z);// ÇÏ
                    Directions8Position[8] = new Vector3(MasterPosition.x + 1f, MasterPosition.y, MasterPosition.z);// ¿ì
                    Directions8Position[9] = new Vector3(MasterPosition.x - 1f, MasterPosition.y, MasterPosition.z);// ÁÂ
                    Directions8Position[10] = new Vector3(MasterPosition.x, MasterPosition.y - 1f, MasterPosition.z);// ÇÏ
                    Directions8Position[11] = new Vector3(MasterPosition.x, MasterPosition.y - 1.5f, MasterPosition.z);// ÇÏ
                    Directions8Position[12] = new Vector3(MasterPosition.x, MasterPosition.y + 2f, MasterPosition.z);// »ó
                    Directions8Position[13] = new Vector3(MasterPosition.x, MasterPosition.y + 2.5f, MasterPosition.z);// »ó
                    Directions8Position[14] = new Vector3(MasterPosition.x - 1f, MasterPosition.y + 2.5f, MasterPosition.z);// ÁÂ»ó
                    Directions8Position[15] = new Vector3(MasterPosition.x + 1f, MasterPosition.y + 2.5f, MasterPosition.z);// ¿ì»ó
                    Directions8Position[17] = new Vector3(MasterPosition.x + 1f, MasterPosition.y - 1.5f, MasterPosition.z);// ÁÂÇÏ
                    Directions8Position[18] = new Vector3(MasterPosition.x - 1f, MasterPosition.y - 1.5f, MasterPosition.z);// ¿ìÇÏ
                    Directions8Position[19] = new Vector3(MasterPosition.x - 2.5f, MasterPosition.y - 2.5f, MasterPosition.z);// ÁÂÇÏ
                    Directions8Position[20] = new Vector3(MasterPosition.x + 2.5f, MasterPosition.y - 2.5f, MasterPosition.z);// ¿ìÇÏ
                    Directions8Position[21] = new Vector3(MasterPosition.x - 2.5f, MasterPosition.y + 2.5f, MasterPosition.z);// ¿ì»ó
                    Directions8Position[22] = new Vector3(MasterPosition.x + 2.5f, MasterPosition.y + 2.5f, MasterPosition.z);// ¿ì»ó
                }

                PutPositionList.Clear();
                for (int i = 0; i < Directions8Position.Length; ++i)
                {
                    Vector3Int cellPosition = MonsterWayTileInfo.WorldToCell(Directions8Position[i]);
                    TileBase clickedTile = MonsterWayTileInfo.GetTile(cellPosition);
                    if (clickedTile == null)
                        continue;

                    PutPositionList.Add(cellPosition);
                }

                if (PutPositionList.Count > 0)
                {
                    ClearAlreadyIndexList();
                    ChangeState(BuildingActionState.Attack);
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }

            yield return null;
        }
    }

    protected override IEnumerator Attack()
    {
        while (true)
        {
            float fAttackCountPerSecond = 1f / m_Master.AttackSpeed;
            if (fAttackCountPerSecond == 0)
            {
                ChangeState(BuildingActionState.Ready);
                yield return null;
            }

            int index = AlreadyIndexList[Oracle.RandomDice(0, AlreadyIndexList.Count)];
            Vector3 FirePosition = PutPositionList[index] + new Vector3(0.5f, 0.5f, 0f);
            Fire(FirePosition, true, MagicType.NONE);

            AlreadyPutIndexList.Add(index);
            AlreadyIndexList.Remove(index);
            if (AlreadyPutIndexList.Count == PutPositionList.Count)
            {
                AlreadyPutIndexList.Clear();
                ClearAlreadyIndexList();
            }

            yield return new WaitForSeconds(fAttackCountPerSecond);
        }
    }

    private void ClearAlreadyIndexList()
    {
        AlreadyIndexList.Clear();
        for (int i = 0; i < PutPositionList.Count; ++i)
        {
            AlreadyIndexList.Add(i);
        }
    }

    protected override IEnumerator SearchAndAttack()
    {
        MapBase pMapBase = MapManager.Instance.GetCurrentMapInfo();
        if (pMapBase == null)
            yield break;

        if (pMapBase is Map_Adventure == false)
            yield break;

        if (m_Master is Player_Adventure == false)
            yield break;

        Map_Adventure pMapAdventure = pMapBase as Map_Adventure;
        Player_Adventure Master_Adv = m_Master as Player_Adventure;
        float CurrentPos = pMapAdventure.GetFloorBothEnds(Master_Adv.GetCurrentLayerFloor(), true).y + 0.5f;
        Vector3 FirePosition = new Vector3(Master_Adv.transform.position.x, CurrentPos, Master_Adv.transform.position.z);

        isCoroutineRunning = true;
        Fire(FirePosition, true, MagicType.NONE);

        float fAttackCountPerSecond = 1f / m_Master.AttackSpeed;
        yield return new WaitForSeconds(fAttackCountPerSecond);
        isCoroutineRunning = false;
    }
}

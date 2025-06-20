using GameDefines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Bomb : WeaponBase
{
    private enum StraightExplosionType
    { 
        Center,
        Middle,
        Corner
    };

    private struct StraightExplosionInfo
    {
        public StraightExplosionType Type;
        public Vector3 Pos;
        public Vector2 Rotate;

        public StraightExplosionInfo(StraightExplosionType eType, Vector3 vPos, Vector2 vRotate)
        {
            Type = eType;
            Pos = vPos;
            Rotate = vRotate;
        }
    }

    [SerializeField] private float fMaxtimer = 5f;
    [SerializeField] private GameObject[] ExplosionPrefeb;
    [SerializeField] private int iDistance = 5;

    private Tilemap MonsterWayTileInfo;
    private float fTime = 0f;
    private Animator animator = null;

    protected override void Awake()
    {
        m_eWeaponType = WeaponType.BOMB;
        animator = GetComponent<Animator>();

        //if (Oracle.m_eGameType != MapType.ADVENTURE)
        {
            if (MonsterWayTileInfo == null)
            {
                GameObject pTileObject = GameObject.FindWithTag("MonsterWayTile");
                MonsterWayTileInfo = pTileObject.GetComponent<Tilemap>();
            }
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        fTime = Time.time;
        animator.Play("Bomb");
    }

    protected override void FixedUpdate()
    {
        if (m_MasterObject == null)
        {
            DestroyPool();
            return;
        }

        if (Time.time - fTime < fMaxtimer)
            return;

        Explode();
        DestroyPool();
    }

    private void Explode()
    {
        if (ExplosionPrefeb == null)
            return;

        if (ExplosionPrefeb.Length < 3)
            return;

        System.Func<Vector2, List<StraightExplosionInfo>, bool> CalculateRadius = (direction, ExplodeList) =>
        {
            List<Vector3> infoList = new List<Vector3>();
            Vector2 currentPosition = transform.position;
            for (int i = 1; i < iDistance; ++i)
            {
                Vector3 TilePosition = currentPosition + direction * i;
                Vector3Int cellPosition = MonsterWayTileInfo.WorldToCell(TilePosition);
                TileBase clickedTile = MonsterWayTileInfo.GetTile(cellPosition);
                if (clickedTile == null)
                    break;

                infoList.Add(TilePosition);
            }

            for (int i = 0; i < infoList.Count; ++i)
            {
                StraightExplosionType eType = i == infoList.Count - 1 ? StraightExplosionType.Corner : StraightExplosionType.Middle;
                ExplodeList.Add(new StraightExplosionInfo(eType, infoList[i], direction));
            }

            return true;
        };

        List<StraightExplosionInfo> ExplodeList = new List<StraightExplosionInfo>();
        ExplodeList.Add(new StraightExplosionInfo(StraightExplosionType.Center, transform.position, Vector2.right));
        CalculateRadius(Vector2.up, ExplodeList);
        CalculateRadius(Vector2.down, ExplodeList);
        CalculateRadius(Vector2.left, ExplodeList);
        CalculateRadius(Vector2.right, ExplodeList);

        for (int i = 0; i < ExplodeList.Count; ++i)
        {
            EffectBase pEffectBase = EffectPool.Instance.GetEffect(ExplodeList[i].Pos, ExplosionPrefeb[(int)ExplodeList[i].Type]);
            if (pEffectBase != null)
            {
                pEffectBase.SetRotation(ExplodeList[i].Rotate);
                pEffectBase.SetInfo(m_MasterObject);
            }
        }

        if (Oracle.m_eGameType == MapType.ADVENTURE)
        {
            // Bomb는 무조건 스킬이니까 그냥 강제로 박아주자
            m_MasterObject.WeaponFlashSound(WeaponType.BOMB, true);
        }
        else
            m_MasterObject.WeaponFlashSound(true);
    }
}
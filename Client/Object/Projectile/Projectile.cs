using CharacterDefines;
using GameDefines;
using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] protected bool skipCollision = false;
    public ProjectileType eProjectileType { get; protected set; } = ProjectileType.NONE;

    protected BuildingActionState eActionState = BuildingActionState.Search;

    protected int Damage = 0;
    protected int moveSpeed = 0;

    protected Building m_Master = null;
    protected Vector3 m_MuzzlePosition = Vector3.zero;

    protected Vector3 vLookVector = Vector3.zero;
    protected bool isCoroutineRunning = false;

    protected virtual void Update()
    {
        if (m_Master == null)
            return;
    }

    public void SetMaster(Building master)
    {
        m_Master = master;
        m_MuzzlePosition = transform.position + new Vector3(0f, 0.5f, 0f);

        if (Oracle.m_eGameType != MapType.ADVENTURE)
            ChangeState(BuildingActionState.Search);
    }

    protected virtual void ChangeState(BuildingActionState newActionState)
    {
        StopCoroutine(eActionState.ToString());
        eActionState = newActionState;
        StartCoroutine(eActionState.ToString());
    }

    protected virtual void StopState()
    {
        StopCoroutine(eActionState.ToString());
        eActionState = BuildingActionState.Search;
    }

    protected bool CheckTarget(GameObject targetObject)
    {
        if (targetObject == null)
            return false;

        if (targetObject.gameObject.activeSelf == false)
            return false;

        MonsterBase findMonster = targetObject.GetComponent<MonsterBase>();
        if (findMonster.IsDie())
            return false;

        if (Oracle.m_eGameType != MapType.ADVENTURE)
        {
            if (m_Master.m_eSpeciesType == findMonster.m_eSpeciesType)
                return false;
        }

        return true;
    }

    protected virtual void Fire(Transform target, bool bChange, MagicType eMagicType = MagicType.NONE)
    {
        if (bChange)
        {
            vLookVector = (target.position - m_MuzzlePosition).normalized;
            if (Oracle.m_eGameType != MapType.ADVENTURE)
                m_Master.Turn(vLookVector.x);
            m_Master.SetAnimationActionState(LAnimationState.Attack);
            m_Master.WeaponFlashSound(false);
        }

        WeaponBase weapon = WeaponPool.Instance.GetWeapon(m_Master.eWeaponType, m_MuzzlePosition);
        if (weapon)
        {
            weapon.SetInfo(m_Master, target, skipCollision, m_Master.TargetCount);
            weapon.SetMagicType(eMagicType);
            weapon.bEnableUpdate = true;
        }
    }
    protected virtual void Fire(Vector3 position, bool bChange, MagicType eMagicType)
    {
        if (bChange)
        {
            vLookVector = (position - m_MuzzlePosition).normalized;
            if (Oracle.m_eGameType != MapType.ADVENTURE)
                m_Master.Turn(vLookVector.x);
            m_Master.SetAnimationActionState(LAnimationState.Attack);
            m_Master.WeaponFlashSound(false);
        }

        WeaponBase weapon = WeaponPool.Instance.GetWeapon(m_Master.eWeaponType, position);
        if (weapon)
        {
            weapon.SetInfo(m_Master, Vector3.zero);
            weapon.SetMagicType(eMagicType);
            weapon.bEnableUpdate = true;
        }
    }

    protected virtual void Fire(Vector3 position, bool bChange, MagicType eMagicType, Vector3 endPosition)
    {
        if (bChange)
        {
            vLookVector = (position - m_MuzzlePosition).normalized;
            if (Oracle.m_eGameType != MapType.ADVENTURE)
                m_Master.Turn(vLookVector.x);
            m_Master.SetAnimationActionState(LAnimationState.Attack);
            m_Master.WeaponFlashSound(false);
        }

        WeaponBase weapon = WeaponPool.Instance.GetWeapon(m_Master.eWeaponType, position);
        if (weapon)
        {
            weapon.SetInfo(m_Master, endPosition);
            weapon.SetMagicType(eMagicType);
            weapon.bEnableUpdate = true;
        }
    }

    public void SetIncapacitate(bool bIncapacitate)
    {
        if (Oracle.m_eGameType != MapType.ADVENTURE)
            ChangeState(bIncapacitate ? BuildingActionState.Ready : BuildingActionState.Search);
    }

    protected virtual IEnumerator Search()
    {
        yield return null;
    }

    protected virtual IEnumerator Attack()
    {
        yield return null;
    }

    protected virtual IEnumerator Ready()
    {
        yield return null;
    }

    protected virtual IEnumerator SearchAndAttack()
    {
        yield return null;
    }

    protected virtual IEnumerator SearchAndSkill()
    {
        yield return null;
    }

    public void DieProjectile()
    {
        StopCoroutine(eActionState.ToString());
        enabled = false;
    }

    public void AdventureAttack()
    {
        if (isCoroutineRunning)
            return;

        m_MuzzlePosition = transform.position + new Vector3(0f, 0.5f, 0f);
        StartCoroutine(SearchAndAttack());
    }
}

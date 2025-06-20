using CharacterDefines;
using OptionDefines;
using System.Collections;
using UnityEngine;

public class ImpedimentLaser : ImpedimentsBase
{
    [SerializeField] private GameObject LaserPrefab = null;
    [SerializeField] private ParticleSystem LaserParticle = null;
    private GameObject CreateLaserPrefabClone = null;

    private float m_LifeTime = 0f;
    private float m_FireTime = 0f;

    private bool bCrash = false;
    private bool bFire = false;
    private bool bDestory = false;
    private bool bParticle = true;
    void Update()
    {
        if (bDestory)
        {
            bDestory = false;
            DestroyPool();
            return;
        }

        if (bFire && bCrash)
        {
            SetCrashCollision();
        }

        //if (bParticle)
        //{
        //    LaserParticle.transform.position = new Vector3(m_Target.position.x, m_Target.position.y + 0.5f, m_Target.position.z);
        //}
    }

    protected override void OnTriggerEnter(Collider collision)
    {
        if (collision.transform == m_Target)
        {
            bCrash = true;
        }
    }

    void OnTriggerExit(Collider collision)
    {
        if (collision.transform == m_Target)
        {
            bCrash = false;
        }
    }

    public override void SetInfo(Transform player, ADVLayerType eADVLayerType, int ilevel)
    {
        if (player == null || LaserPrefab == null)
        {
            Debug.Log("ImpedimentsBase set error =" + m_eAdventurePrefabsType);
        }

        m_Target = player;
        ImpedimentLevel = (float)ilevel;

        bCrash = false;
        bFire = false;
        bDestory = false;

        m_FireTime = 3f;
        m_LifeTime = 0.1f;

        LaserParticle.Stop();

        // 2°³
        if (ImpedimentLevel == 2)
        {
        }
        // 1°³
        else
        {
        }

        Damage = 2;

        StartCoroutine(FireDelay());
    }
    
    private IEnumerator FireDelay()
    {
        SoundManager.Instance.PlayUISound(UISoundType.LASERALARM);
        yield return new WaitForSeconds(m_FireTime);

        CreateLaserPrefabClone = Instantiate(LaserPrefab, transform);
        if (CreateLaserPrefabClone == null)
            yield break;

        bFire = true;

        SoundManager.Instance.PlayUISound(UISoundType.LASER);
        yield return new WaitForSeconds(m_LifeTime);
        bDestory = true;
    }

    private void SetCrashCollision()
    {
        bFire = false;
        bCrash = false;
        if (gameObject == null)
            return;

        Player MyPlayer = GameManager.Instance.GetPlayer();
        if (MyPlayer == null)
            return;

        bParticle = true;
        //LaserParticle.Play();
        //MyPlayer.DivideHP(Damage);
        MyPlayer.ReduceHP(5);
    }

    public override void DestroyPool()
    {
        //SoundManager.Instance.StopUISound(UISoundType.LASER);

        Destroy(CreateLaserPrefabClone);
        CreateLaserPrefabClone = null;

        bCrash = false;
        bFire = false;
        bParticle = false;

        base.DestroyPool();
    }
}

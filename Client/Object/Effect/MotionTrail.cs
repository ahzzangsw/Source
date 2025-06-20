using GameDefines;
using UnityEngine;
using UnityEngine.Pool;

public class MotionTrail : MonoBehaviour
{
    [SerializeField] private float m_fOffSet = 0f;
    [SerializeField] private float m_fSpeed = 20f;

    private IObjectPool<MotionTrail> ManagedPool;

    private TrailRenderer m_TrailRenderer = null;
    private MeleeType m_eMeleeType = MeleeType.NONE;
    private Vector3 m_vLookVector = Vector3.zero;

    private Vector3 m_vStartPoint;
    private Vector3 m_vEndPoint;
    private float m_fJourneyLength;
    private float m_fStartTime;

    private void Awake()
    {
        m_TrailRenderer = GetComponent<TrailRenderer>();
    }

    void Update()
    {
        if (m_eMeleeType == MeleeType.NONE)
            return;

        switch (m_eMeleeType)
        {
            case MeleeType.HAND:
                UpdateMotionTrail_Hand();
                break;
            case MeleeType.STICK:
                UpdateMotionTrail_Stick();
                break;
            case MeleeType.SWORD:
                UpdateMotionTrail_Sword();
                break;
            case MeleeType.SPEAR:
                UpdateMotionTrail_Spear();
                break;
        };
    }

    public void SetMotionTrail(MeleeType eMeleeType, Vector3 vLookVector, Vector3 MotionTrailPos)
    {
        m_eMeleeType = eMeleeType;
        m_vLookVector = vLookVector;

        // clear
        m_fJourneyLength = 0f;
        switch (m_eMeleeType)
        {
            case MeleeType.HAND:
                break;
            case MeleeType.STICK:
            {
                float offset = vLookVector.x < 0 ? -m_fOffSet : m_fOffSet;
                m_vStartPoint = new Vector3(MotionTrailPos.x, MotionTrailPos.y + m_fOffSet, 0);
                m_vEndPoint = new Vector3(MotionTrailPos.x + offset, MotionTrailPos.y, 0);
                m_fJourneyLength = Vector3.Distance(m_vStartPoint, m_vEndPoint);
            }
            break;
            case MeleeType.SWORD:
            {
                float offset = vLookVector.x < 0 ? -m_fOffSet : m_fOffSet;
                m_vStartPoint = new Vector3(MotionTrailPos.x, MotionTrailPos.y + m_fOffSet, 0);
                m_vEndPoint = new Vector3(MotionTrailPos.x + offset, MotionTrailPos.y, 0);
            }
            break;
            case MeleeType.SPEAR:
                break;
        };

        transform.position = m_vStartPoint;
        m_fStartTime = Time.time;

        m_TrailRenderer.Clear();
        m_TrailRenderer.transform.position = m_vStartPoint;
        m_TrailRenderer.enabled = true;
    }

    private void UpdateMotionTrail_Hand()
    {

    }
    private void UpdateMotionTrail_Stick()
    {
        float distanceCovered = (Time.time - m_fStartTime) * m_fSpeed;
        float fractionOfJourney = distanceCovered / m_fJourneyLength;
        Vector3 newPosition = Vector3.Lerp(m_vStartPoint, m_vEndPoint, fractionOfJourney);

        float curve = Mathf.Sin(fractionOfJourney * Mathf.PI) * 1f;
        newPosition.y += curve;

        transform.position = newPosition;
        if (m_vEndPoint.y >= newPosition.y)
        {
            DestroyPool();
        }
    }
    private void UpdateMotionTrail_Sword()
    {
        Vector3 newPosition = (m_vEndPoint - transform.position).normalized;
        transform.position += newPosition * m_fSpeed * Time.deltaTime;
        
        if (m_vEndPoint.y >= transform.position.y)
        {
            DestroyPool();
        }
    }
    private void UpdateMotionTrail_Spear()
    {
    }
    public void SetManagedPool(IObjectPool<MotionTrail> pool)
    {
        ManagedPool = pool;
    }
    public virtual void DestroyPool()
    {
        m_TrailRenderer.enabled = false;
        ManagedPool.Release(this);
    }
}

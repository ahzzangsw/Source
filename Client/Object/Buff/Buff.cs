using GameDefines;
using UnityEngine;

public class Buff : MonoBehaviour
{
    [SerializeField] public BuffType m_eBuffType = BuffType.NONE;
    public int m_Duration { get; set; }

    public void Set(BuffType eBuffType, int durationInSeconds)
    {
        m_eBuffType = eBuffType;
        m_Duration = durationInSeconds;
    }
}

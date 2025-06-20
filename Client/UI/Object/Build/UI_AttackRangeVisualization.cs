using UnityEngine;

public class UI_AttackRangeVisualization : UIBase
{
    private Character m_pOwner = null;
    protected override void Update()
    {
        if (isShow == false)
            return;

        if (m_pOwner == null)
            return;

        transform.position = m_pOwner.transform.position + new Vector3(0f, 0.5f, 0);
    }

    public void OnVisualization(Vector3 newPosition, float range)
    {
        float diameter = range * 2f;
        transform.localScale = Vector3.one * diameter;
        transform.position = newPosition;

        m_pOwner = null;
    }

    public void OnVisualization(Character target, float range)
    {
        if (target == null)
        {
            m_pOwner = null;
            Hide();
            return;
        }

        m_pOwner = target;

        float diameter = range * 2f;
        transform.localScale = Vector3.one * diameter;
        transform.position = m_pOwner.transform.position + new Vector3(0f, 0.5f, 0);
    }
}

using UIDefines;

public class Boss2 : BossBase
{
    UI_Interrupt UIInterrupt = null;
    protected override void Awake()
    {
        base.Awake();
        iBossSkillPercent = 20;
        UIInterrupt = UIManager.Instance.GetUI(UIIndexType.INTERRUPT) as UI_Interrupt;
        RubyCount = 2;
    }

    protected override void DoInterrupt()
    {
        if (UIInterrupt == null)
            return;

        if (UIInterrupt.IsShow() == false)
        {
            UIManager.Instance.ShowUI(UIIndexType.INTERRUPT);
        }

        UIInterrupt.Process();
    }

    protected override void ClearInterrupt()
    {
        bInterrupt = false;
        if (UIInterrupt == null)
            return;

        UIInterrupt.Clear();
        UIInterrupt.Hide();
    }

    protected override void HandleUpdateBossPatternEvent()
    {
        moveSpeed *= 5f;
        m_BuffContainer.UpdateMonsterInfo(moveSpeed);

        base.HandleUpdateBossPatternEvent();
    }
}


public class Item_Money : ItemBase
{
    private int addmoney = 0;

    protected override void Awake()
    {
        m_fDisappearTime = 0.1f;
    }

    private void OnEnable()
    {
    }

    protected override void Update()
    {

    }

    public override void SetInt(int i)
    {
        addmoney = i;
    }

    public override void PickUp()
    {
        Player player = GameManager.Instance.GetPlayer();
        if (player != null)
        {
            player.AddMoney(addmoney);
        }

        base.PickUp();
    }
}

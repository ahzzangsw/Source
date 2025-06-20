namespace UIDefines
{
    public enum UIIndexType
    {
        NONE = 0,
        LOBBY,          ///< �κ�
        MAPCHOICE,      ///< �ʼ���
        ACHIEVEMENTS,   ///< ����
        MAININFO,       ///< ���� ����â
        GAMEINFO,       ///< ���� ����â
        ANNOUNCE,       ///< �˸�
        BUILDINGLIST,   ///< ���� ����Ʈ
        HUD,            ///< �÷��̾� HUD(�÷��� �ð�, HP, ����, Ư��)
        TOOLTIP,        ///< ����
        HPBAR,          ///< ���� HP��
        EXITMENU,       ///< ����޴�
        CRITICALFONT,   ///< ũ��Ƽ�� ��Ʈ
        INTERRUPT,      ///< ���� ���
        TARGETBAR,      ///< ���� HP��
        OPTION,         ///< �ɼ�(�κ�)
        COMPLETE,       ///< Ŭ����
        CONTROL,        ///< ��Ʈ��(���׷��̵�, �ϲۻ̱�, ������ȯ ��)
        CHARACTERDECK,  ///< ĳ���� ��
        PICKER,         ///< ����â(��庥�ĸ��)
        MAX,
    };

    public enum UITooltipType
    {
        STRING,
        BUILDINGINFO,
        SPAWNBOSSINFO,
        MAX,
    };

    public enum UITooltipSortType
    {
        HORIZONTAL,
        VERTICAL,
    }

    public enum UIPrefebType
    {
        HPBAR,
        CRITICALFONT,
        MAX,
    }
}
namespace UIDefines
{
    public enum UIIndexType
    {
        NONE = 0,
        LOBBY,          ///< 로비
        MAPCHOICE,      ///< 맵선택
        ACHIEVEMENTS,   ///< 업적
        MAININFO,       ///< 메인 정보창
        GAMEINFO,       ///< 게임 정보창
        ANNOUNCE,       ///< 알림
        BUILDINGLIST,   ///< 생성 리스트
        HUD,            ///< 플레이어 HUD(플레이 시간, HP, 종족, 특성)
        TOOLTIP,        ///< 툴팁
        HPBAR,          ///< 몬스터 HP바
        EXITMENU,       ///< 종료메뉴
        CRITICALFONT,   ///< 크리티컬 폰트
        INTERRUPT,      ///< 방해 요소
        TARGETBAR,      ///< 보스 HP바
        OPTION,         ///< 옵션(로비)
        COMPLETE,       ///< 클리어
        CONTROL,        ///< 컨트롤(업그레이드, 일꾼뽑기, 보스소환 등)
        CHARACTERDECK,  ///< 캐릭터 덱
        PICKER,         ///< 선택창(어드벤쳐모드)
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
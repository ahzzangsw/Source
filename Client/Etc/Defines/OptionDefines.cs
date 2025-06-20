using UnityEngine;

namespace OptionDefines
{
    public static class CSetOption
    {
        public static void GameQuit()
        {
            OptionManager.Instance.SaveOptionData();
            SoundManager.Instance.SaveOptionData();
        }
    }

    public enum SoundType
    {
        NONE = -1,
        BGM,
        SFX,
    }

    public enum BGMType
    {
        LOBBY,
        STAGE,
        BOSS = 18,
        LASTBOSS,
        ENDING,
        SHOP,
        MAX,
    };

    public enum SFXType
    {
        SFX_NONE = -1,
        // Flash
        SFX_ARROW = 0,
        SFX_ARROW_HIT,
        SFX_ASTRAPHE_HIT,
        SFX_BEE,
        SFX_BIGAXE_HIT,
        SFX_BLOOD_HIT,
        SFX_BOMB,
        SFX_BONE,
        SFX_BUBBLE_HIT,
        SFX_BULLET,
        SFX_CHERRY_HIT,
        SFX_CONSTELLATION_HIT,
        SFX_CROSSBOW,
        SFX_CUBE_HIT,
        SFX_CUTTER_HIT,
        SFX_DANGGER,
        SFX_DEBRIS_HIT,
        SFX_FIREBALL,
        SFX_FIREWORK_HIT,
        SFX_FORESHARROW,
        SFX_GRAVITY_HIT,
        SFX_HEART_HIT,
        SFX_ICEBALL,
        SFX_ICEBALL_HIT,
        SFX_JAB,
        SFX_LASER_HIT,
        SFX_LEAF_HIT,
        SFX_LIGHT_HIT,
        SFX_LIGHTNING_HIT,
        SFX_MAGNETIC_HIT,
        SFX_MISSILE,
        SFX_POISON,
        SFX_PULSE,
        SFX_PURPLESPARKS,
        SFX_REDSPARKS,
        SFX_RIFLE,
        SFX_SHOCKWAVE,
        SFX_SHURIKEN,
        SFX_SHURIKEN_HIT,
        SFX_THROW,
        SFX_SMALLMETEOR_HIT,
        SFX_SNOW,
        SFX_SNOW_HIT,
        SFX_STONE_HIT,
        SFX_SULFURICACID,
        SFX_SULFURICACID_HIT,
        SFX_SWORD,
        SFX_SLASH,
        SFX_WATER_HIT,
        SFX_SPEAR_HIT,
        SFX_METEOR,
        SFX_EXPLOSION0,
        SFX_EXPLOSION1,
        SFX_EXPLOSION2,
        SFX_BITE,
        SFX_BARREL,
        SFX_POOP0,
        SFX_POOP1,
        SFX_BEESTING,

        // Buff
        SFX_BUFF_ATKUP = 1000,
        SFX_BUFF_ATKSPEEDUP,
        SFX_BUFF_RANGEUP,
    }
    public enum UISoundType
    {
        BACK,
        BUTTONCLICK,
        CLICK,
        CONFIRM,
        DECLINE,
        EQUIP,
        FAIL,
        GAMEOVER,
        GAMEOVER1,
        GAMESTART,
        OVER,
        PAUSE,
        SLOTCLICK,
        SUCCESS,
        UNPAUSE,
        USEITEM,
        INGAMESTART,
        GAMECOMPLETE,
        DROPITEM,
        PICKUPITEM,
        UPGRADE,
        UPGRADEFAIL,
        SPAWN,
        SHUFFLE,
        SHUFFLE1,
        PICK,
        ACHIEVEMENT,
        FANFARE,
        LASERALARM,
        LASER,
        ENEMYSHOCKWAVE,
        ENEMYSTONE,
        ENEMYREX,
        ENEMYBALL,
        ENEMYMELEE,
        ENEMYBULLET,
    }

    public enum CharacterState 
    {
        SELL = 0,
        SPAWN = 1,
        UNIQUE,
        DIE,
        SPAWN_BASE,
        OINK,
    }

    public enum BossState
    { 
        DOWN,
        PONG,
        UP,
        DIE0,
        DIE1,
        DIE2,
        RISEHP1,
        RISEHP2,
        SIGN,
        BOSSDIE,
        FOOTPRINT,
        HEADSHOT,
        FLY,
        TILEBROKEN,
        WARP,
        HIT,
        WING,
    }

    public enum OtherShopProductItemType
    { 
        GAMESPEED2X = 0,
        GAMESPEED3X = 1,
        MAX,
    }
}
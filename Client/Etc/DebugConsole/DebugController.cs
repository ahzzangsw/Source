using GameDefines;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugController : MonoBehaviour
{
    [SerializeField] private bool bUseDebugController = false;
    private bool showConsole = false;

    private string cmd = "";
    private bool textFieldFocused = false;

    private Queue<string> saveCmd;
    private List<string> saveCmdList;
    private int saveCmdIndex = 0;

    private string addmoney = "money";
    private string gamespeed = "speed";
    private string stage = "stage";
    private string go = "go";
    private string character = "character";
    private string AddCharacter = "AddCharacter";
    private string AddShopItem = "AddShopItem"; 
    private string AddCursor = "AddCursor";
    private string AddRuby = "AddRuby";
    private string AddAll = "All";
    private string SetHp = "sethp";

    void Awake()
    {
        if (!bUseDebugController)
            enabled = false;

        saveCmd = new Queue<string>();
        saveCmdList = new List<string>();

        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (!bUseDebugController)
            return;

        if (Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.KeypadEnter))
        {
            showConsole = true;
        }
    }

    private void OnGUI()
    {
        if (!showConsole)
            return;

        float y = Screen.height / 2f;
        GUI.Box(new Rect(0, y, Screen.width, 30f), "");
        GUI.backgroundColor = Color.black;

        textFieldFocused = GUI.GetNameOfFocusedControl() == "CmdField";
        GUI.SetNextControlName("CmdField");
        cmd = GUI.TextField(new Rect(10f, y + 5f, Screen.width-20f, 20f), cmd);
        if (!textFieldFocused)
            GUI.FocusControl("CmdField");
        else
        {
            Event e = Event.current;
            if (e.type == EventType.KeyDown)
            {
                if (e.keyCode == KeyCode.None)
                {
                    GUIUtility.keyboardControl = 0;
                    Run();
                    cmd = "";
                    showConsole = false;
                }
            }
            else if(e.type == EventType.Used)
            {
                if (saveCmdList.Count == 0)
                    return;

                if (e.keyCode == KeyCode.UpArrow)
                {
                    --saveCmdIndex;
                }
                else if (e.keyCode == KeyCode.DownArrow)
                {
                    ++saveCmdIndex;
                }
                else
                    return;

                if (saveCmdIndex < 0)
                    saveCmdIndex = saveCmdList.Count - 1;
                else if(saveCmdIndex >= saveCmdList.Count)
                    saveCmdIndex = 0;

                cmd = GUI.TextField(new Rect(10f, y + 5f, Screen.width - 20f, 20f), saveCmdList[saveCmdIndex]);
            }
        }
    }

    private void Run()
    {
        if (cmd == "")
            return;

        string[] cmdArray = cmd.Split(' ');
        if (cmdArray.Length < 1)
            return;
        
        Player player = GameManager.Instance.GetPlayer();
        if (string.Equals(cmdArray[0], gamespeed, StringComparison.OrdinalIgnoreCase))
        {
            int gameSpeed = int.Parse(cmdArray[1]);
            if (gameSpeed > 9)
                return;

            GameManager.Instance.gameSpeed = gameSpeed;
        }
        else if (string.Equals(cmdArray[0], go, StringComparison.OrdinalIgnoreCase))
        {
            if (Oracle.m_eGameType != MapType.ADVENTURE)
                return;

            byte stageIndex = byte.Parse(cmdArray[1]);
            if (stageIndex > 100)
            {
                stageIndex = 100;
            }

            --stageIndex;
            if (stageIndex < 0)
                stageIndex = 0;

            GameManager.Instance.stageIndex = stageIndex;
            MonsterPool.Instance.ForceDeSpawnMonster();
        }
        else if(string.Equals(cmdArray[0], SetHp, StringComparison.OrdinalIgnoreCase))
        {
            if (Oracle.m_eGameType != MapType.ADVENTURE)
                return;

            int Hp = int.Parse(cmdArray[1]);
            int HpPercent = 0;
            if (cmdArray.Length > 2)
            {
                HpPercent = int.Parse(cmdArray[2]);
            }

            MonsterPool.Instance.ChangeBossHP(Hp, HpPercent);
        }
        else if (string.Equals(cmdArray[0], addmoney, StringComparison.OrdinalIgnoreCase))
        {
            int money = int.Parse(cmdArray[1]);
            player.AddMoney(money);
        }
        else if (string.Equals(cmdArray[0], stage, StringComparison.OrdinalIgnoreCase))
        {
            byte stageIndex = byte.Parse(cmdArray[1]);  
            if (stageIndex > 100)
            {
                if (Oracle.m_eGameType == MapType.BUILD || Oracle.m_eGameType == MapType.ADVENTURE)
                    stageIndex = 100;
            }

            --stageIndex;
            if (stageIndex < 0)
                stageIndex = 0;

            GameManager.Instance.stageIndex = stageIndex;
        }
        else if (string.Equals(cmdArray[0], character, StringComparison.OrdinalIgnoreCase))
        {
            int iSpeciesType = int.Parse(cmdArray[1]);
            if (iSpeciesType >= (int)SpeciesType.MAX)
                return;

            GameManager.Instance.m_TestSpeciesType = (SpeciesType)iSpeciesType;
            GameManager.Instance.bForceTest = true;
        }
        else if (string.Equals(cmdArray[0], AddCharacter, StringComparison.OrdinalIgnoreCase))
        {
            UnlockManager.Instance.AddAllCharacter();
        }
        else if (string.Equals(cmdArray[0], AddShopItem, StringComparison.OrdinalIgnoreCase))
        {
            UnlockManager.Instance.AddAllShopItem();
        }
        else if (string.Equals(cmdArray[0], AddCursor, StringComparison.OrdinalIgnoreCase))
        {
            UnlockManager.Instance.AddAllCursor();
        }
        else if (string.Equals(cmdArray[0], AddRuby, StringComparison.OrdinalIgnoreCase))
        {
            int RubyCount = int.Parse(cmdArray[1]);
            GameManager.Instance.AddGameMoney(RubyCount);
        }
        else if (string.Equals(cmdArray[0], AddAll, StringComparison.OrdinalIgnoreCase))
        {
            UnlockManager.Instance.AddAllCursor();
            UnlockManager.Instance.AddAllShopItem();
        }
        
        else
            return;

        saveCmd.Enqueue(cmd);
        if (saveCmd.Count > 5)
        {
            saveCmd.Dequeue();
        }

        saveCmdList.Clear();
        foreach (string item in saveCmd)
        {
            saveCmdList.Add(item);
        }
    }
}

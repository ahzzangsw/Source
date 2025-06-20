#if UNITY_EDITOR
using GameDefines;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using NUnit.Framework;
using System.Collections.Generic;

public class DataEditor : EditorWindow
{
    private bool isRunning = false;
    private string startCode = "";
    private string lastCode = "";

    private enum DATATYPE
    {
        BUILDING,
        BUILDING2,
        BUILDING3,
        MONSTER,
        MONSTER2,
        MONSTER3,
        CURSOR,
        SHOP,
        UNITACHIEVEMENT,
        LEVELUP,

        CONTROL = 99,
    };

    [MenuItem("Extract Tools/DataEditor")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(DataEditor), false, "DataEditor Extract");
    }
    void OnGUI()
    {
        GUILayout.Label("Custom Editor", EditorStyles.boldLabel);

        GUILayout.Space(20);

        if (!isRunning && GUILayout.Button("Run Process"))
        {
            isRunning = true;

            startCode = "using GameDefines;\nusing System;\nusing System.IO;\nusing System.Collections.Generic;\nusing UnityEngine;\n\n";
            lastCode = "\t}\n}\n";

            int index = 0;
            bool bComplete = true;
            string errorPath = "";

            string filePath = Path.Combine(Application.streamingAssetsPath, "BuildingData.csv");
            if (ConvertExcel(filePath, index++) == false)
            {
                bComplete = false;
                errorPath += (filePath + "\n");
            }

            filePath = Path.Combine(Application.streamingAssetsPath, "BuildingData2.csv");
            if (ConvertExcel(filePath, index++) == false)
            {
                bComplete = false;
                errorPath += (filePath + "\n");
            }

            filePath = Path.Combine(Application.streamingAssetsPath, "BuildingData3.csv");
            if (ConvertExcel(filePath, index++) == false)
            {
                bComplete = false;
                errorPath += (filePath + "\n");
            }

            filePath = Path.Combine(Application.streamingAssetsPath, "MonsterData.csv");
            if (ConvertExcel(filePath, index++) == false)
            {
                bComplete = false;
                errorPath += (filePath + "\n");
            }

            filePath = Path.Combine(Application.streamingAssetsPath, "MonsterData2.csv");
            if (ConvertExcel(filePath, index++) == false)
            {
                bComplete = false;
                errorPath += (filePath + "\n");
            }

            filePath = Path.Combine(Application.streamingAssetsPath, "MonsterData3.csv");
            if (ConvertExcel(filePath, index++) == false)
            {
                bComplete = false;
                errorPath += (filePath + "\n");
            }

            filePath = Path.Combine(Application.streamingAssetsPath, "CursorData.csv");
            if (ConvertExcel(filePath, index++) == false)
            {
                bComplete = false;
                errorPath += (filePath + "\n");
            }

            filePath = Path.Combine(Application.streamingAssetsPath, "ShopData.csv");
            if (ConvertExcel(filePath, index++) == false)
            {
                bComplete = false;
                errorPath += (filePath + "\n");
            }

            filePath = Path.Combine(Application.streamingAssetsPath, "UnitAchievement.csv");
            if (ConvertExcel(filePath, index++) == false)
            {
                bComplete = false;
                errorPath += (filePath + "\n");
            }

            filePath = Path.Combine(Application.streamingAssetsPath, "LevelUP.csv");
            if (ConvertExcel(filePath, index++) == false)
            {
                bComplete = false;
                errorPath += (filePath + "\n");
            }

            if (EtcInfoFileRead(ref errorPath) == false) 
            {
                bComplete = false;
            }

            if (bComplete)
            {
                EditorUtility.DisplayDialog("Complete", "", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Error", errorPath, "OK");
            }

            isRunning = false;
        }

        GUILayout.Space(20);

        if (GUILayout.Button("Close Window"))
        {
            // Á¾·á ¹öÆ°À» ´­·¶À» ¶§ Ã¢À» ´Ý½À´Ï´Ù.
            this.Close();
        }
    }

    bool ConvertExcel(string filePath, int index)
    {
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);

        FileInfo fileInfo = new FileInfo(filePath);
        string Fulltext = "";

        if (fileInfo.Exists)
        {
            StreamReader reader = new StreamReader(filePath);
            Fulltext = reader.ReadToEnd();
            reader.Close();
        }
        else
        {
            return false;
        }

        DATATYPE eDatatype = (DATATYPE)index;
        switch (eDatatype)
        {
            case DATATYPE.BUILDING:
                BuildingInfoFileRead(fileNameWithoutExtension, Fulltext, MapType.BUILD);
                break;
            case DATATYPE.BUILDING2:
                BuildingInfoFileRead(fileNameWithoutExtension, Fulltext, MapType.SPAWN);
                break;
            case DATATYPE.BUILDING3:
                BuildingInfoFileRead(fileNameWithoutExtension, Fulltext, MapType.ADVENTURE);
                break;
            case DATATYPE.MONSTER:
                MonsterInfoFileRead(fileNameWithoutExtension, Fulltext, MapType.BUILD);
                break;
            case DATATYPE.MONSTER2:
                MonsterInfoFileRead(fileNameWithoutExtension, Fulltext, MapType.SPAWN);
                break;
            case DATATYPE.MONSTER3:
                MonsterInfoFileRead(fileNameWithoutExtension, Fulltext, MapType.ADVENTURE);
                break;
            case DATATYPE.CURSOR:
                CursorInfoFileRead(fileNameWithoutExtension, Fulltext);
                break;
            case DATATYPE.SHOP:
                ShopInfoFileRead(fileNameWithoutExtension, Fulltext);
                break;
            case DATATYPE.UNITACHIEVEMENT:
                UnitAchievementInfoFileRead(fileNameWithoutExtension, Fulltext);
                break;
            case DATATYPE.LEVELUP:
                LevelUpInfoFileRead(fileNameWithoutExtension, Fulltext);
                break;
        };

        return true;
    }

    private void BuildingInfoFileRead(string fileName, string Fulltext, MapType eMapType) 
    {
        string csCode = startCode;
        if (eMapType == MapType.BUILD)
            csCode += "public class BuildingDatabase\n{\n\tpublic Dictionary<SpeciesType, List<BuildingInfo>> BuildingInfoList;";
        else if (eMapType == MapType.SPAWN)
            csCode += "public class BuildingDatabase2\n{\n\tpublic Dictionary<SpeciesType, List<BuildingInfo>> BuildingInfoList;";
        else
            csCode += "public class BuildingDatabase3\n{\n\tpublic Dictionary<SpeciesType, List<BuildingInfo>> BuildingInfoList;";
        csCode += "\n\n\tpublic void SetData()\n\t{\n";

        int MaxInfoLength = 6; //(Damage/Range/AttackSpeed/Cost/Count/Buff)
        int lineIndex = -1;

        int index = 0;
        string key = "";
        string value = "";

        csCode += "\t\tBuildingInfoList = new Dictionary<SpeciesType, List<BuildingInfo>>();\n\n";

        string[] lines = Fulltext.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
        foreach (string line in lines)
        {
            ++lineIndex;
            if (!line.Trim().StartsWith("//"))
            {
                if (line.StartsWith("["))
                {
                    if (key.Length > 0)
                    {
                        csCode += $"\t\tList<BuildingInfo> items{index} = new List<BuildingInfo>();\n";
                        csCode += value;
                        csCode += $"\t\tBuildingInfoList.Add(SpeciesType.{key}, items{index});\n\n";
                        ++index;
                    }

                    int indexOfEnd = line.IndexOf("]");
                    key = line.Substring(1, indexOfEnd - 1);
                    value = "";
                    continue;
                }

                string[] valueArray = line.Split(',');
                if (valueArray[0] == "end;")
                {
                    csCode += $"\t\tList<BuildingInfo> items{index} = new List<BuildingInfo>();\n";
                    csCode += value;
                    csCode += $"\t\tBuildingInfoList.Add(SpeciesType.{key}, items{index});\n";
                    break;
                }

                if (valueArray.Length != MaxInfoLength)
                {
                    Debug.LogError("MapType = " + eMapType + " ,BuildingData Error LineIndex = " + lineIndex);
                    break;
                }

                string buff = valueArray[5].Length > 0 ? $"BuffType.{valueArray[5]}" : "BuffType.NONE";
                value += $"\t\titems{index}.Add(new BuildingInfo({valueArray[0]}, {valueArray[1]}f, {valueArray[2]}f, {valueArray[3]}, {valueArray[4]}, {buff}));\n";
            }
        }

        csCode += lastCode;
        ExtractCSFile(fileName, csCode);
    }
    private void MonsterInfoFileRead(string fileName, string Fulltext, MapType eMapType)
    {
        string csCode = startCode;
        if (eMapType == MapType.BUILD)
            csCode += "public class MonsterDatabase\n{\n\tpublic List<StageInfo> StageInfoList = new List<StageInfo>();";
        else if (eMapType == MapType.SPAWN)
            csCode += "public class MonsterDatabase2\n{\n\tpublic List<StageInfo> StageInfoList = new List<StageInfo>();";
        else
            csCode += "public class MonsterDatabase3\n{\n\tpublic List<StageInfo_Adv> StageInfoList = new List<StageInfo_Adv>();";
        csCode += "\n\n\tpublic void SetData()\n\t{\n";

        int MaxInfoLength = 6; //(HP/Def/MoveSpeed/Money/StageMoney/NextTime)
        int lineIndex = 0;
        string[] lines = Fulltext.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
        foreach (string line in lines)
        {
            if (!line.Trim().StartsWith("//"))
            {
                string[] valueArray = line.Split(',');
                if (valueArray[0] == "end;")
                    break;

                if (valueArray.Length != MaxInfoLength)
                {
                    Debug.LogError("eMapType = " + eMapType + " ,MonsterData Error LineIndex = " + lineIndex);
                    break;
                }

                if (eMapType == MapType.ADVENTURE)
                    csCode += $"\t\tStageInfoList.Add(new StageInfo_Adv({valueArray[0]}, {valueArray[1]}, {valueArray[2]}f, {valueArray[3]}f, {valueArray[4]}, {valueArray[5]}));\n";
                else
                    csCode += $"\t\tStageInfoList.Add(new StageInfo({valueArray[0]}, {valueArray[1]}, {valueArray[2]}f, {valueArray[3]}, {valueArray[4]}, {valueArray[5]}));\n";
            }

            ++lineIndex;
        }

        csCode += lastCode;
        ExtractCSFile(fileName, csCode);
    }
    private void CursorInfoFileRead(string fileName, string Fulltext)
    {
        string csCode = startCode;
        csCode += "public class CursorDatabase\n{\n\tpublic List<CursorInfo> CursorInfoList = new List<CursorInfo>();";
        csCode += "\n\n\tpublic void SetData()\n\t{\n";

        int MaxInfoLength = 5; //(Money/Damage/Range/AtkSpeed/tooltip)
        int lineIndex = 0;
        string[] lines = Fulltext.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
        foreach (string line in lines)
        {
            if (!line.Trim().StartsWith("//"))
            {
                string[] valueArray = line.Split(',');
                if (valueArray[0] == "end;")
                    break;

                if (valueArray.Length != MaxInfoLength)
                {
                    Debug.LogError("CursorData Error LineIndex = " + lineIndex);
                    break;
                }

                csCode += $"\t\tCursorInfoList.Add(new CursorInfo({valueArray[0]}, {valueArray[1]}f, {valueArray[2]}f, {valueArray[3]}f, \"{valueArray[4]}\"));\n";
            }

            ++lineIndex;
        }

        csCode += lastCode;
        ExtractCSFile(fileName, csCode);
    }
    private void ShopInfoFileRead(string fileName, string Fulltext)
    {
        string csCode = startCode;
        csCode += "public class ShopDatabase\n{\n\tpublic List<ShopInfo> ShopInfoList = new List<ShopInfo>();";
        csCode += "\n\n\tpublic void SetData()\n\t{\n";

        int MaxInfoLength = 5; //(Name/Desc/Cost/Species/SlotID)
        int lineIndex = 0;
        string[] lines = Fulltext.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
        foreach (string line in lines)
        {
            if (!line.Trim().StartsWith("//"))
            {
                string[] valueArray = line.Split(',');
                if (valueArray[0] == "end;")
                    break;

                if (valueArray.Length != MaxInfoLength)
                {
                    Debug.LogError("ShopData Error LineIndex = " + lineIndex);
                    break;
                }

                csCode += $"\t\tShopInfoList.Add(new ShopInfo(\"{valueArray[0]}\", \"{valueArray[1]}\", {valueArray[2]}, SpeciesType.{valueArray[3]}, {valueArray[4]}));\n";
            }

            ++lineIndex;
        }

        csCode += lastCode;
        ExtractCSFile(fileName, csCode);
    }
    private void UnitAchievementInfoFileRead(string fileName, string Fulltext)
    {
        string csCode = startCode;
        csCode += "public class UnitAchievementDatabase\n{\n\tpublic List<KeyAchievementInfo> KeyAchievementInfoList = new List<KeyAchievementInfo>();";
        csCode += "\n\tpublic List<UnitAchievementInfo> UnitAchievementInfoList = new List<UnitAchievementInfo>();";
        csCode += "\n\n\tpublic void SetData()\n\t{\n";

        int MaxInfoLength = 4; //(Name/Type/Money/SteamName.............)
        int lineIndex = 0;
        string[] lines = Fulltext.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
        foreach (string line in lines)
        {
            if (!line.Trim().StartsWith("//"))
            {
                string[] valueArray = line.Split(',');
                if (valueArray[0] == "end;")
                    break;

                if (valueArray.Length <= MaxInfoLength)
                {
                    Debug.LogError("UnitAchievementInfo Error LineIndex = " + lineIndex);
                    break;
                }

                string name = valueArray[0];
                string type = valueArray[1];
                int money = int.Parse(valueArray[2]);
                string steamname = valueArray[3];

                int enumIndex = 0;  // 0: KeyCode, 1:SpeciesType
                if (type.Equals("KeyCode", StringComparison.OrdinalIgnoreCase))
                {
                    enumIndex = 0;
                    csCode += $"\t\tKeyAchievementInfoList.Add(new KeyAchievementInfo(\"{name}\", \"{steamname}\", {money}, ";
                }
                else if (type.Equals("SpeciesType", StringComparison.OrdinalIgnoreCase))
                {
                    enumIndex = 1;
                    csCode += $"\t\tUnitAchievementInfoList.Add(new UnitAchievementInfo(\"{name}\", \"{steamname}\", {money}, ";
                }
                else
                {
                    Debug.LogError("UnitAchievementInfo Type Error LineIndex = " + lineIndex);
                    break;
                }

                for (int i = MaxInfoLength; i < valueArray.Length; ++i)
                {
                    string value = valueArray[i];
                    value = value.Trim();

                    if (value.Length == 0)
                        break;

                    if (i != MaxInfoLength)
                        csCode += ", ";

                    if (enumIndex == 0)
                    {
                        csCode += $"KeyCode.{valueArray[i]}";
                    }
                    else if (enumIndex == 1)
                    {
                        csCode += $"SpeciesType.{valueArray[i]}";
                    }
                }

                csCode += "));\n";
            }

            ++lineIndex;
        }

        csCode += lastCode;
        ExtractCSFile(fileName, csCode);
    }
    private void LevelUpInfoFileRead(string fileName, string Fulltext)
    {
        string csCode = startCode;
        csCode += "public class LevelUpSystemDatabase\n{\n\tpublic Dictionary<AdventureLevelUpItemType, List<LevelUpSystemInfo>> LevelUpSystemInfoList;";
        csCode += "\n\tpublic void SetData()\n\t{\n";
        csCode += "\t\tLevelUpSystemInfoList = new Dictionary<AdventureLevelUpItemType, List<LevelUpSystemInfo>>();\n\n";

        int MaxInfoLength = 8; //(KeyType/ValueType/Desc/1/2/3/4/5)
        int lineIndex = 0;
        string[] lines = Fulltext.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

        Dictionary<AdventureLevelUpItemType, List<string>> tempSaveList = new Dictionary<AdventureLevelUpItemType, List<string>>();
        foreach (string line in lines)
        {
            if (!line.Trim().StartsWith("//"))
            {
                string[] valueArray = line.Split(',');
                if (valueArray[0] == "end;")
                    break;

                if (valueArray.Length < MaxInfoLength)
                {
                    Debug.LogError("LevelUpSystemInfo Error LineIndex = " + lineIndex);
                    break;
                }

                string keyType = valueArray[0];
                string valueType = valueArray[1];
                string Desc = valueArray[2];
                if (Enum.TryParse(keyType, true, out AdventureLevelUpItemType eAdventureLevelUpItemType) == false)
                    continue;

                string values = $"{valueArray[3]}f, {valueArray[4]}f, {valueArray[5]}f, {valueArray[6]}f, {valueArray[7]}f";

                string valueString = $"new LevelUpSystemInfo(AdventureLevelUpItemType.{keyType}";
                if (eAdventureLevelUpItemType == AdventureLevelUpItemType.CHARACTER)
                    valueString += $", \"{Desc}\")";
                else if (eAdventureLevelUpItemType == AdventureLevelUpItemType.STAT)
                    valueString += $", AdventureLevelUpStatType.{valueType}, \"{Desc}\", {values})";
                else if (eAdventureLevelUpItemType == AdventureLevelUpItemType.UTIL)
                    valueString += $", {valueType}, \"{Desc}\", {values})";
                else
                    continue;

                tempSaveList.TryAdd(eAdventureLevelUpItemType, new List<string>());
                tempSaveList[eAdventureLevelUpItemType].Add(valueString);
            }

            ++lineIndex;
        }

        int index = 0;
        foreach (var tempSave in tempSaveList)
        {
            csCode += $"\t\tList<LevelUpSystemInfo> items{index} = new List<LevelUpSystemInfo>();\n";
            for (int i = 0; i < tempSave.Value.Count; ++i)
            {
                csCode += $"\t\titems{index}.Add({tempSave.Value[i]});\n";
            }
            csCode += $"\t\tLevelUpSystemInfoList.Add(AdventureLevelUpItemType.{tempSave.Key}, items{index});\n\n";

            ++index;
        }

        csCode += lastCode;
        ExtractCSFile(fileName, csCode);
    }
    private bool EtcInfoFileRead(ref string errorPath)
    {
        //Control
        string filePath = Path.Combine(Application.streamingAssetsPath, "ControlData.csv");
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);

        FileInfo fileInfo = new FileInfo(filePath);
        string Fulltext = "";

        if (fileInfo.Exists)
        {
            StreamReader reader = new StreamReader(filePath);
            Fulltext = reader.ReadToEnd();
            reader.Close();
        }
        else
        {
            errorPath += (fileNameWithoutExtension + "\n");
            return false;
        }

        string csCode = startCode;
        csCode += "public class ControlDatabase\n{\n\tpublic ControlInfo ControlInfoData;";
        csCode += "\n\n\tpublic void SetData()\n\t{\n";

        int MaxInfoLength = 25; //(Cost/1Atk/2Atk/3Atk/4Atk/5Atk/WorkmanCost/BossMoneyEarned0~8/BossHP0~8)
        string[] lines = Fulltext.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
        foreach (string line in lines)
        {
            if (!line.Trim().StartsWith("//"))
            {
                string[] valueArray = line.Split(',');
                if (valueArray[0] == "end;")
                    break;

                if (valueArray.Length != MaxInfoLength)
                {
                    Debug.LogError("ControlData Error");
                    break;
                }

                int[] BossMoneyEarnedList = new int[9];
                for (int i = 0; i < 9; ++i)
                {
                    int BossMoneyEarned = int.Parse(valueArray[i + 7]);
                    BossMoneyEarnedList[i] = BossMoneyEarned;
                }

                int[] BossHPList = new int[9];
                for (int i = 0; i < 9; ++i)
                {
                    int BossHP = int.Parse(valueArray[i + 16]);
                    BossHPList[i] = BossHP;
                }

                csCode += $"\t\tControlInfoData = new ControlInfo({valueArray[0]}, {valueArray[1]}, {valueArray[2]}, {valueArray[3]}, {valueArray[4]}, {valueArray[5]}, {valueArray[6]}, new int[] {{ {string.Join(", ", BossMoneyEarnedList)} }}, new int[] {{ {string.Join(", ", BossHPList)} }});\n";
            }
        }

        csCode += lastCode;
        ExtractCSFile(fileNameWithoutExtension, csCode);
        //Control

        return true;
    }

    private void ExtractCSFile(string filename, string csCode)
    {
        string outputFile = Application.dataPath + "\\Source\\Client\\Data\\" + filename;
        outputFile += ".cs" ;

        try
        {
            File.WriteAllText(outputFile, csCode);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"¾¾¹ß ¿Ö¾ÈµÅ? : {ex.Message}");
        }
    }
}
#endif
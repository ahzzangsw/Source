using GameDefines;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class UnitAchievementDatabase
{
	public List<KeyAchievementInfo> KeyAchievementInfoList = new List<KeyAchievementInfo>();
	public List<UnitAchievementInfo> UnitAchievementInfoList = new List<UnitAchievementInfo>();

	public void SetData()
	{
		KeyAchievementInfoList.Add(new KeyAchievementInfo("god of choice", "KEYCHOICE", 10, KeyCode.T, KeyCode.R, KeyCode.Y, KeyCode.C, KeyCode.A, KeyCode.N));
		UnitAchievementInfoList.Add(new UnitAchievementInfo("rival1", "RIVAL", 5, SpeciesType.HUMAN, SpeciesType.ORC));
		UnitAchievementInfoList.Add(new UnitAchievementInfo("rival2", "RIVAL", 5, SpeciesType.ELF, SpeciesType.DARKELF));
		UnitAchievementInfoList.Add(new UnitAchievementInfo("villain", "VILLAIN", 7, SpeciesType.ORC, SpeciesType.GOBLIN, SpeciesType.DEMON));
		UnitAchievementInfoList.Add(new UnitAchievementInfo("protagonist", "", 10, SpeciesType.HUMAN, SpeciesType.ORC, SpeciesType.ELF));
		UnitAchievementInfoList.Add(new UnitAchievementInfo("religion", "RELIGION", 10, SpeciesType.FANATIC, SpeciesType.MONK, SpeciesType.NECROMANCER));
		UnitAchievementInfoList.Add(new UnitAchievementInfo("family", "", 10, SpeciesType.HUMAN, SpeciesType.HUMAN, SpeciesType.HUMAN));
		UnitAchievementInfoList.Add(new UnitAchievementInfo("not alive", "", 10, SpeciesType.ANDROID, SpeciesType.UNDEAD, SpeciesType.ZOMBIE));
		UnitAchievementInfoList.Add(new UnitAchievementInfo("technician", "", 15, SpeciesType.MONK, SpeciesType.DWARF, SpeciesType.ANDROID));
		UnitAchievementInfoList.Add(new UnitAchievementInfo("japan", "", 15, SpeciesType.SAMURAI, SpeciesType.SAMURAI, SpeciesType.SAMURAI, SpeciesType.SAMURAI));
		UnitAchievementInfoList.Add(new UnitAchievementInfo("monster", "MONSTER", 15, SpeciesType.FURRY, SpeciesType.FISHMAN, SpeciesType.DRAKE));
		UnitAchievementInfoList.Add(new UnitAchievementInfo("death", "DEATH", 15, SpeciesType.ZOMBIE, SpeciesType.UNDEAD, SpeciesType.NECROMANCER));
		UnitAchievementInfoList.Add(new UnitAchievementInfo("Hotter", "HOTTER", 50, SpeciesType.HUMAN, SpeciesType.GOBLIN, SpeciesType.DWARF, SpeciesType.WIZARD, SpeciesType.NECROMANCER));
		UnitAchievementInfoList.Add(new UnitAchievementInfo("R Evil", "REVIL", 50, SpeciesType.ZOMBIE, SpeciesType.ZOMBIE, SpeciesType.ZOMBIE, SpeciesType.ZOMBIE, SpeciesType.ZOMBIE));
		UnitAchievementInfoList.Add(new UnitAchievementInfo("D&D", "DND", 50, SpeciesType.HUMAN, SpeciesType.DWARF, SpeciesType.ELF, SpeciesType.GOBLIN, SpeciesType.WIZARD));
		UnitAchievementInfoList.Add(new UnitAchievementInfo("untouchable", "UNTOUCHABLE", 50, SpeciesType.UNKNOWN, SpeciesType.UNKNOWN, SpeciesType.UNKNOWN, SpeciesType.WIZARD, SpeciesType.WIZARD));
	}
}

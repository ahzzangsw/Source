using GameDefines;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpSystemDatabase
{
	public Dictionary<AdventureLevelUpItemType, List<LevelUpSystemInfo>> LevelUpSystemInfoList;
	public void SetData()
	{
		LevelUpSystemInfoList = new Dictionary<AdventureLevelUpItemType, List<LevelUpSystemInfo>>();

		List<LevelUpSystemInfo> items0 = new List<LevelUpSystemInfo>();
		items0.Add(new LevelUpSystemInfo(AdventureLevelUpItemType.CHARACTER, "Pull some strings"));
		LevelUpSystemInfoList.Add(AdventureLevelUpItemType.CHARACTER, items0);

		List<LevelUpSystemInfo> items1 = new List<LevelUpSystemInfo>();
		items1.Add(new LevelUpSystemInfo(AdventureLevelUpItemType.STAT, AdventureLevelUpStatType.DAMAGE, "Increased damage +{0}%", 10f, 20f, 30f, 40f, 50f));
		items1.Add(new LevelUpSystemInfo(AdventureLevelUpItemType.STAT, AdventureLevelUpStatType.ATTACKSPEED, "Increased Attack Speed +{0}%", 10f, 20f, 30f, 40f, 50f));
		items1.Add(new LevelUpSystemInfo(AdventureLevelUpItemType.STAT, AdventureLevelUpStatType.RANGE, "Increased range +{0}%", 10f, 20f, 30f, 40f, 50f));
		items1.Add(new LevelUpSystemInfo(AdventureLevelUpItemType.STAT, AdventureLevelUpStatType.MOVE, "Increased movement speed +{0}", 3.5f, 4f, 5f, 6f, 7f));
		items1.Add(new LevelUpSystemInfo(AdventureLevelUpItemType.STAT, AdventureLevelUpStatType.JUMP, "Increased jumping power +{0}", 7.5f, 8f, 8.5f, 9f, 10f));
		LevelUpSystemInfoList.Add(AdventureLevelUpItemType.STAT, items1);

		List<LevelUpSystemInfo> items2 = new List<LevelUpSystemInfo>();
		items2.Add(new LevelUpSystemInfo(AdventureLevelUpItemType.UTIL, 1, "Heal 50 HP", 50f, -1f, -1f, -1f, -1f));
		items2.Add(new LevelUpSystemInfo(AdventureLevelUpItemType.UTIL, 2, "Character levelUP {0}", 1f, 2f, 3f, 4f, -1f));
		LevelUpSystemInfoList.Add(AdventureLevelUpItemType.UTIL, items2);

	}
}

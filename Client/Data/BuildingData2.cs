using GameDefines;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class BuildingDatabase2
{
	public Dictionary<SpeciesType, List<BuildingInfo>> BuildingInfoList;

	public void SetData()
	{
		BuildingInfoList = new Dictionary<SpeciesType, List<BuildingInfo>>();

		List<BuildingInfo> items0 = new List<BuildingInfo>();
		items0.Add(new BuildingInfo(18, 6.5f, 1.5f, 15, 1, BuffType.NONE));
		items0.Add(new BuildingInfo(30, 6.5f, 1.5f, 15, 2, BuffType.NONE));
		items0.Add(new BuildingInfo(160, 6.5f, 1.5f, 15, 3, BuffType.NONE));
		items0.Add(new BuildingInfo(1550, 7.5f, 2f, 15, 4, BuffType.SLOW1));
		items0.Add(new BuildingInfo(3250, 8.5f, 3f, 15, 5, BuffType.CRITICAL1));
		BuildingInfoList.Add(SpeciesType.ELF, items0);

		List<BuildingInfo> items1 = new List<BuildingInfo>();
		items1.Add(new BuildingInfo(20, 4f, 2f, 15, 1, BuffType.ARMORREDUCING0));
		items1.Add(new BuildingInfo(35, 4f, 2f, 15, 2, BuffType.ARMORREDUCING0));
		items1.Add(new BuildingInfo(260, 4f, 2f, 15, 1, BuffType.ARMORREDUCING1));
		items1.Add(new BuildingInfo(1600, 4f, 2f, 15, 3, BuffType.ARMORREDUCING1));
		items1.Add(new BuildingInfo(3000, 4f, 2f, 15, 1, BuffType.ARMORREDUCING2));
		BuildingInfoList.Add(SpeciesType.HUMAN, items1);

		List<BuildingInfo> items2 = new List<BuildingInfo>();
		items2.Add(new BuildingInfo(22, 6.5f, 1.5f, 15, 1, BuffType.CRITICAL0));
		items2.Add(new BuildingInfo(40, 6.5f, 1.5f, 15, 1, BuffType.CRITICAL0));
		items2.Add(new BuildingInfo(220, 6.5f, 1.5f, 15, 2, BuffType.CRITICAL1));
		items2.Add(new BuildingInfo(1650, 6.5f, 2f, 15, 2, BuffType.CRITICAL1));
		items2.Add(new BuildingInfo(4000, 6.5f, 3f, 15, 3, BuffType.CRITICAL2));
		BuildingInfoList.Add(SpeciesType.DRAKE, items2);

		List<BuildingInfo> items3 = new List<BuildingInfo>();
		items3.Add(new BuildingInfo(24, 4f, 1f, 15, 1, BuffType.CRITICAL0));
		items3.Add(new BuildingInfo(50, 4f, 1f, 15, 1, BuffType.CRITICAL0));
		items3.Add(new BuildingInfo(480, 4f, 1f, 15, 1, BuffType.CRITICAL1));
		items3.Add(new BuildingInfo(2200, 4f, 1f, 15, 2, BuffType.CRITICAL1));
		items3.Add(new BuildingInfo(6000, 4f, 0.25f, 15, 3, BuffType.CRITICAL2));
		BuildingInfoList.Add(SpeciesType.ORC, items3);

		List<BuildingInfo> items4 = new List<BuildingInfo>();
		items4.Add(new BuildingInfo(18, 4.5f, 3f, 15, 1, BuffType.NONE));
		items4.Add(new BuildingInfo(40, 4.5f, 3f, 15, 1, BuffType.NONE));
		items4.Add(new BuildingInfo(350, 4.5f, 3f, 15, 2, BuffType.NONE));
		items4.Add(new BuildingInfo(2000, 4.5f, 3f, 15, 2, BuffType.NONE));
		items4.Add(new BuildingInfo(3700, 4.5f, 3f, 15, 4, BuffType.NONE));
		BuildingInfoList.Add(SpeciesType.SAMURAI, items4);

		List<BuildingInfo> items5 = new List<BuildingInfo>();
		items5.Add(new BuildingInfo(10, 5f, 4f, 15, 1, BuffType.POISON0));
		items5.Add(new BuildingInfo(30, 5f, 5f, 15, 1, BuffType.POISON0));
		items5.Add(new BuildingInfo(100, 5f, 6f, 15, 3, BuffType.POISON1));
		items5.Add(new BuildingInfo(1200, 5f, 7f, 15, 2, BuffType.POISON1));
		items5.Add(new BuildingInfo(600, 5f, 8f, 15, 1, BuffType.SLOW2));
		BuildingInfoList.Add(SpeciesType.UNDEAD, items5);

		List<BuildingInfo> items6 = new List<BuildingInfo>();
		items6.Add(new BuildingInfo(5, 5f, 6f, 15, 1, BuffType.NONE));
		items6.Add(new BuildingInfo(15, 5f, 7f, 15, 2, BuffType.NONE));
		items6.Add(new BuildingInfo(65, 5f, 8f, 15, 2, BuffType.POISON1));
		items6.Add(new BuildingInfo(1120, 5f, 9f, 15, 3, BuffType.POISON1));
		items6.Add(new BuildingInfo(500, 5f, 10f, 15, 5, BuffType.POISON2));
		BuildingInfoList.Add(SpeciesType.ZOMBIE, items6);

		List<BuildingInfo> items7 = new List<BuildingInfo>();
		items7.Add(new BuildingInfo(21, 5f, 4f, 15, 2, BuffType.KNOCKBACK0));
		items7.Add(new BuildingInfo(45, 5f, 4f, 15, 2, BuffType.KNOCKBACK0));
		items7.Add(new BuildingInfo(199, 5f, 4f, 15, 2, BuffType.KNOCKBACK1));
		items7.Add(new BuildingInfo(1600, 5f, 4f, 15, 2, BuffType.KNOCKBACK1));
		items7.Add(new BuildingInfo(2400, 5f, 4f, 15, 2, BuffType.KNOCKBACK2));
		BuildingInfoList.Add(SpeciesType.FURRY, items7);

		List<BuildingInfo> items8 = new List<BuildingInfo>();
		items8.Add(new BuildingInfo(13, 5f, 1f, 15, 1, BuffType.ATTACK_UP0));
		items8.Add(new BuildingInfo(25, 5.5f, 1f, 15, 1, BuffType.ATTACK_UP0));
		items8.Add(new BuildingInfo(100, 6.5f, 1f, 15, 2, BuffType.ATTACK_UP1));
		items8.Add(new BuildingInfo(1500, 7.5f, 1f, 15, 1, BuffType.ATTACK_UP1));
		items8.Add(new BuildingInfo(2000, 10.5f, 1f, 15, 4, BuffType.ATTACK_UP2));
		BuildingInfoList.Add(SpeciesType.DWARF, items8);

		List<BuildingInfo> items9 = new List<BuildingInfo>();
		items9.Add(new BuildingInfo(20, 4.5f, 2f, 15, 1, BuffType.BURN0));
		items9.Add(new BuildingInfo(45, 4.5f, 2f, 15, 1, BuffType.BURN0));
		items9.Add(new BuildingInfo(222, 4.5f, 2f, 15, 2, BuffType.BURN1));
		items9.Add(new BuildingInfo(1500, 4.5f, 2f, 15, 2, BuffType.BURN1));
		items9.Add(new BuildingInfo(4000, 4.5f, 2f, 15, 3, BuffType.BURN2));
		BuildingInfoList.Add(SpeciesType.DEMON, items9);

		List<BuildingInfo> items10 = new List<BuildingInfo>();
		items10.Add(new BuildingInfo(18, 5f, 1f, 15, 2, BuffType.SLOW0));
		items10.Add(new BuildingInfo(33, 5f, 1f, 15, 3, BuffType.KNOCKBACK0));
		items10.Add(new BuildingInfo(212, 5f, 1f, 15, 4, BuffType.STUN0));
		items10.Add(new BuildingInfo(1500, 5f, 1f, 15, 3, BuffType.ARMORREDUCING0));
		items10.Add(new BuildingInfo(10000, 5.5f, 1f, 15, 1, BuffType.CRITICAL0));
		BuildingInfoList.Add(SpeciesType.UNKNOWN, items10);

		List<BuildingInfo> items11 = new List<BuildingInfo>();
		items11.Add(new BuildingInfo(16, 5f, 0.75f, 15, 1, BuffType.STEAL0));
		items11.Add(new BuildingInfo(40, 5f, 0.75f, 15, 2, BuffType.STEAL1));
		items11.Add(new BuildingInfo(200, 5f, 0.75f, 15, 1, BuffType.STEAL1));
		items11.Add(new BuildingInfo(1650, 5f, 0.75f, 15, 2, BuffType.STEAL2));
		items11.Add(new BuildingInfo(4000, 5f, 0.1f, 15, 1, BuffType.STEAL2));
		BuildingInfoList.Add(SpeciesType.GOBLIN, items11);

		List<BuildingInfo> items12 = new List<BuildingInfo>();
		items12.Add(new BuildingInfo(15, 5f, 1f, 15, 1, BuffType.STUN0));
		items12.Add(new BuildingInfo(30, 5f, 1f, 15, 1, BuffType.STUN0));
		items12.Add(new BuildingInfo(150, 5f, 1f, 15, 1, BuffType.STUN0));
		items12.Add(new BuildingInfo(1500, 5f, 1f, 15, 1, BuffType.STUN1));
		items12.Add(new BuildingInfo(3000, 5f, 1f, 15, 1, BuffType.STUN1));
		BuildingInfoList.Add(SpeciesType.FANATIC, items12);

		List<BuildingInfo> items13 = new List<BuildingInfo>();
		items13.Add(new BuildingInfo(12, 5f, 2f, 15, 1, BuffType.NONE));
		items13.Add(new BuildingInfo(22, 5f, 2f, 15, 2, BuffType.NONE));
		items13.Add(new BuildingInfo(160, 5f, 2f, 15, 3, BuffType.NONE));
		items13.Add(new BuildingInfo(1650, 5f, 2f, 15, 4, BuffType.NONE));
		items13.Add(new BuildingInfo(4000, 5f, 2f, 15, 5, BuffType.NONE));
		BuildingInfoList.Add(SpeciesType.DARKELF, items13);

		List<BuildingInfo> items14 = new List<BuildingInfo>();
		items14.Add(new BuildingInfo(10, 5f, 1f, 15, 1, BuffType.SLOW0));
		items14.Add(new BuildingInfo(35, 5f, 0.5f, 15, 1, BuffType.SLOW0));
		items14.Add(new BuildingInfo(150, 5f, 1.5f, 15, 1, BuffType.SLOW1));
		items14.Add(new BuildingInfo(1450, 5f, 1.5f, 15, 1, BuffType.SLOW1));
		items14.Add(new BuildingInfo(2000, 5f, 1.5f, 15, 1, BuffType.SLOW2));
		BuildingInfoList.Add(SpeciesType.ANDROID, items14);

		List<BuildingInfo> items15 = new List<BuildingInfo>();
		items15.Add(new BuildingInfo(25, 5f, 1f, 15, 1, BuffType.NONE));
		items15.Add(new BuildingInfo(55, 5f, 1f, 15, 1, BuffType.NONE));
		items15.Add(new BuildingInfo(350, 5f, 1f, 15, 2, BuffType.NONE));
		items15.Add(new BuildingInfo(1850, 5f, 1f, 15, 3, BuffType.NONE));
		items15.Add(new BuildingInfo(9999, 5f, 1f, 15, 1, BuffType.NONE));
		BuildingInfoList.Add(SpeciesType.WIZARD, items15);

		List<BuildingInfo> items16 = new List<BuildingInfo>();
		items16.Add(new BuildingInfo(15, 5f, 0.75f, 15, 2, BuffType.STUN0));
		items16.Add(new BuildingInfo(30, 5f, 0.75f, 15, 2, BuffType.STUN0));
		items16.Add(new BuildingInfo(180, 5f, 0.75f, 15, 1, BuffType.STUN0));
		items16.Add(new BuildingInfo(1350, 5f, 0.75f, 15, 5, BuffType.STUN1));
		items16.Add(new BuildingInfo(3000, 5f, 0.75f, 15, 1, BuffType.STUN1));
		BuildingInfoList.Add(SpeciesType.NECROMANCER, items16);

		List<BuildingInfo> items17 = new List<BuildingInfo>();
		items17.Add(new BuildingInfo(18, 5f, 1f, 15, 2, BuffType.NONE));
		items17.Add(new BuildingInfo(40, 5f, 1f, 15, 2, BuffType.NONE));
		items17.Add(new BuildingInfo(235, 5f, 1f, 15, 3, BuffType.NONE));
		items17.Add(new BuildingInfo(2200, 5f, 2f, 15, 1, BuffType.NONE));
		items17.Add(new BuildingInfo(6000, 5f, 1f, 15, 1, BuffType.NONE));
		BuildingInfoList.Add(SpeciesType.FISHMAN, items17);

		List<BuildingInfo> items18 = new List<BuildingInfo>();
		items18.Add(new BuildingInfo(15, 5f, 1f, 15, 1, BuffType.ATTACKSPEED_UP0));
		items18.Add(new BuildingInfo(25, 5.5f, 1f, 15, 1, BuffType.ATTACKSPEED_UP0));
		items18.Add(new BuildingInfo(150, 6.5f, 1f, 15, 1, BuffType.ATTACKSPEED_UP1));
		items18.Add(new BuildingInfo(1500, 7.5f, 1f, 15, 1, BuffType.ATTACKSPEED_UP1));
		items18.Add(new BuildingInfo(2000, 10.5f, 1f, 15, 1, BuffType.ATTACKSPEED_UP2));
		BuildingInfoList.Add(SpeciesType.MONK, items18);
	}
}

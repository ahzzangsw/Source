using GameDefines;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class BuildingDatabase3
{
	public Dictionary<SpeciesType, List<BuildingInfo>> BuildingInfoList;

	public void SetData()
	{
		BuildingInfoList = new Dictionary<SpeciesType, List<BuildingInfo>>();

		List<BuildingInfo> items0 = new List<BuildingInfo>();
		items0.Add(new BuildingInfo(15, 4f, 1.5f, 2, 2, BuffType.NONE));
		items0.Add(new BuildingInfo(250, 5f, 2f, 2, 2, BuffType.NONE));
		items0.Add(new BuildingInfo(800, 5f, 2f, 2, 3, BuffType.NONE));
		items0.Add(new BuildingInfo(2200, 6f, 2f, 2, 4, BuffType.SLOW1));
		items0.Add(new BuildingInfo(7500, 6f, 3.5f, 2, 5, BuffType.CRITICAL1));
		BuildingInfoList.Add(SpeciesType.ELF, items0);

		List<BuildingInfo> items1 = new List<BuildingInfo>();
		items1.Add(new BuildingInfo(17, 2.5f, 3f, 1, 1, BuffType.ARMORREDUCING0));
		items1.Add(new BuildingInfo(250, 4f, 3f, 1, 2, BuffType.ARMORREDUCING0));
		items1.Add(new BuildingInfo(750, 3.5f, 3f, 1, 2, BuffType.ARMORREDUCING1));
		items1.Add(new BuildingInfo(2200, 5f, 4f, 1, 3, BuffType.ARMORREDUCING1));
		items1.Add(new BuildingInfo(7500, 5f, 4f, 1, 3, BuffType.ARMORREDUCING2));
		BuildingInfoList.Add(SpeciesType.HUMAN, items1);

		List<BuildingInfo> items2 = new List<BuildingInfo>();
		items2.Add(new BuildingInfo(16, 5f, 1.5f, 2, 1, BuffType.CRITICAL0));
		items2.Add(new BuildingInfo(300, 6f, 1.5f, 2, 2, BuffType.CRITICAL0));
		items2.Add(new BuildingInfo(700, 6f, 2f, 2, 3, BuffType.CRITICAL1));
		items2.Add(new BuildingInfo(2200, 7f, 2f, 2, 3, BuffType.CRITICAL1));
		items2.Add(new BuildingInfo(6000, 7f, 3f, 2, 3, BuffType.CRITICAL2));
		BuildingInfoList.Add(SpeciesType.DRAKE, items2);

		List<BuildingInfo> items3 = new List<BuildingInfo>();
		items3.Add(new BuildingInfo(20, 3.5f, 2.5f, 1, 1, BuffType.CRITICAL0));
		items3.Add(new BuildingInfo(350, 3.5f, 2.5f, 1, 1, BuffType.CRITICAL0));
		items3.Add(new BuildingInfo(800, 4f, 2.5f, 1, 2, BuffType.CRITICAL1));
		items3.Add(new BuildingInfo(2750, 4f, 2.5f, 1, 2, BuffType.CRITICAL1));
		items3.Add(new BuildingInfo(3000, 4f, 0.5f, 1, 3, BuffType.CRITICAL2));
		BuildingInfoList.Add(SpeciesType.ORC, items3);

		List<BuildingInfo> items4 = new List<BuildingInfo>();
		items4.Add(new BuildingInfo(15, 3f, 5f, 1, 1, BuffType.NONE));
		items4.Add(new BuildingInfo(280, 3f, 5f, 1, 1, BuffType.NONE));
		items4.Add(new BuildingInfo(550, 3.5f, 5f, 1, 2, BuffType.NONE));
		items4.Add(new BuildingInfo(1500, 3.5f, 5f, 1, 2, BuffType.NONE));
		items4.Add(new BuildingInfo(3000, 3.5f, 6f, 1, 3, BuffType.NONE));
		BuildingInfoList.Add(SpeciesType.SAMURAI, items4);

		List<BuildingInfo> items5 = new List<BuildingInfo>();
		items5.Add(new BuildingInfo(14, 3f, 4f, 4, 2, BuffType.POISON0));
		items5.Add(new BuildingInfo(250, 3f, 5f, 4, 2, BuffType.POISON0));
		items5.Add(new BuildingInfo(450, 3.5f, 6f, 4, 2, BuffType.POISON1));
		items5.Add(new BuildingInfo(1500, 4f, 7f, 4, 2, BuffType.POISON1));
		items5.Add(new BuildingInfo(4500, 4f, 7f, 4, 3, BuffType.SLOW2));
		BuildingInfoList.Add(SpeciesType.UNDEAD, items5);

		List<BuildingInfo> items6 = new List<BuildingInfo>();
		items6.Add(new BuildingInfo(11, 3f, 3f, 2, 2, BuffType.NONE));
		items6.Add(new BuildingInfo(255, 3f, 4f, 2, 2, BuffType.NONE));
		items6.Add(new BuildingInfo(480, 3.5f, 5f, 2, 2, BuffType.POISON1));
		items6.Add(new BuildingInfo(1500, 4f, 6f, 2, 2, BuffType.POISON1));
		items6.Add(new BuildingInfo(4500, 4f, 6f, 2, 3, BuffType.POISON2));
		BuildingInfoList.Add(SpeciesType.ZOMBIE, items6);

		List<BuildingInfo> items7 = new List<BuildingInfo>();
		items7.Add(new BuildingInfo(15, 3f, 4f, 4, 2, BuffType.KNOCKBACK0));
		items7.Add(new BuildingInfo(170, 3f, 4f, 4, 2, BuffType.KNOCKBACK0));
		items7.Add(new BuildingInfo(550, 3.5f, 4f, 4, 2, BuffType.KNOCKBACK1));
		items7.Add(new BuildingInfo(1600, 3.5f, 4f, 4, 2, BuffType.KNOCKBACK1));
		items7.Add(new BuildingInfo(5000, 3.5f, 4f, 4, 2, BuffType.KNOCKBACK2));
		BuildingInfoList.Add(SpeciesType.FURRY, items7);

		List<BuildingInfo> items8 = new List<BuildingInfo>();
		items8.Add(new BuildingInfo(17, 10f, 1.5f, 5, 1, BuffType.ATTACK_UP0));
		items8.Add(new BuildingInfo(250, 10f, 1.5f, 5, 1, BuffType.ATTACK_UP0));
		items8.Add(new BuildingInfo(600, 10f, 1.5f, 5, 2, BuffType.ATTACK_UP1));
		items8.Add(new BuildingInfo(2250, 10f, 1.5f, 5, 2, BuffType.ATTACK_UP1));
		items8.Add(new BuildingInfo(6000, 10f, 2f, 5, 4, BuffType.ATTACK_UP2));
		BuildingInfoList.Add(SpeciesType.DWARF, items8);

		List<BuildingInfo> items9 = new List<BuildingInfo>();
		items9.Add(new BuildingInfo(20, 4.5f, 2f, 2, 1, BuffType.BURN0));
		items9.Add(new BuildingInfo(300, 4.5f, 2f, 2, 2, BuffType.BURN0));
		items9.Add(new BuildingInfo(800, 5f, 2f, 2, 2, BuffType.BURN1));
		items9.Add(new BuildingInfo(2700, 5f, 2f, 2, 2, BuffType.BURN1));
		items9.Add(new BuildingInfo(8000, 6f, 3f, 2, 3, BuffType.BURN2));
		BuildingInfoList.Add(SpeciesType.DEMON, items9);

		List<BuildingInfo> items10 = new List<BuildingInfo>();
		items10.Add(new BuildingInfo(25, 4f, 2f, 6, 1, BuffType.SLOW0));
		items10.Add(new BuildingInfo(250, 6f, 2f, 6, 2, BuffType.KNOCKBACK0));
		items10.Add(new BuildingInfo(650, 6f, 1.5f, 6, 2, BuffType.STUN0));
		items10.Add(new BuildingInfo(2500, 7f, 2f, 6, 2, BuffType.ARMORREDUCING0));
		items10.Add(new BuildingInfo(10000, 5f, 2f, 6, 1, BuffType.CRITICAL0));
		BuildingInfoList.Add(SpeciesType.UNKNOWN, items10);

		List<BuildingInfo> items11 = new List<BuildingInfo>();
		items11.Add(new BuildingInfo(18, 5f, 2f, 1, 1, BuffType.NONE));
		items11.Add(new BuildingInfo(270, 5f, 2f, 1, 2, BuffType.NONE));
		items11.Add(new BuildingInfo(700, 5f, 2f, 1, 3, BuffType.NONE));
		items11.Add(new BuildingInfo(2400, 6f, 2f, 1, 3, BuffType.NONE));
		items11.Add(new BuildingInfo(20000, 15f, 0.4f, 1, 1, BuffType.NONE));
		BuildingInfoList.Add(SpeciesType.GOBLIN, items11);

		List<BuildingInfo> items12 = new List<BuildingInfo>();
		items12.Add(new BuildingInfo(17, 5f, 2f, 4, 1, BuffType.STUN0));
		items12.Add(new BuildingInfo(230, 5f, 1f, 4, 2, BuffType.STUN0));
		items12.Add(new BuildingInfo(650, 5f, 1f, 4, 3, BuffType.STUN0));
		items12.Add(new BuildingInfo(2400, 5f, 1f, 4, 3, BuffType.STUN1));
		items12.Add(new BuildingInfo(6500, 5f, 2f, 4, 3, BuffType.STUN1));
		BuildingInfoList.Add(SpeciesType.FANATIC, items12);

		List<BuildingInfo> items13 = new List<BuildingInfo>();
		items13.Add(new BuildingInfo(15, 4f, 2f, 3, 2, BuffType.NONE));
		items13.Add(new BuildingInfo(185, 4f, 2f, 3, 3, BuffType.NONE));
		items13.Add(new BuildingInfo(550, 4f, 2f, 3, 4, BuffType.NONE));
		items13.Add(new BuildingInfo(1550, 4f, 2f, 3, 5, BuffType.NONE));
		items13.Add(new BuildingInfo(4500, 4f, 3f, 3, 6, BuffType.NONE));
		BuildingInfoList.Add(SpeciesType.DARKELF, items13);

		List<BuildingInfo> items14 = new List<BuildingInfo>();
		items14.Add(new BuildingInfo(17, 6f, 1.5f, 1, 2, BuffType.SLOW0));
		items14.Add(new BuildingInfo(200, 9f, 1.5f, 1, 2, BuffType.SLOW0));
		items14.Add(new BuildingInfo(800, 6f, 1.5f, 1, 2, BuffType.SLOW1));
		items14.Add(new BuildingInfo(2200, 6f, 1.5f, 1, 3, BuffType.SLOW1));
		items14.Add(new BuildingInfo(5500, 6f, 3f, 1, 3, BuffType.SLOW2));
		BuildingInfoList.Add(SpeciesType.ANDROID, items14);

		List<BuildingInfo> items15 = new List<BuildingInfo>();
		items15.Add(new BuildingInfo(25, 3.5f, 1.5f, 5, 1, BuffType.NONE));
		items15.Add(new BuildingInfo(250, 3.5f, 1.5f, 5, 1, BuffType.NONE));
		items15.Add(new BuildingInfo(700, 3.5f, 1.5f, 5, 2, BuffType.NONE));
		items15.Add(new BuildingInfo(2500, 3.5f, 1.5f, 5, 2, BuffType.NONE));
		items15.Add(new BuildingInfo(12000, 3.5f, 2f, 5, 1, BuffType.NONE));
		BuildingInfoList.Add(SpeciesType.WIZARD, items15);

		List<BuildingInfo> items16 = new List<BuildingInfo>();
		items16.Add(new BuildingInfo(17, 3.5f, 1.5f, 3, 1, BuffType.STUN0));
		items16.Add(new BuildingInfo(210, 3.5f, 1.5f, 3, 1, BuffType.STUN0));
		items16.Add(new BuildingInfo(600, 4f, 1.5f, 3, 3, BuffType.STUN0));
		items16.Add(new BuildingInfo(2400, 4f, 1.5f, 3, 3, BuffType.STUN1));
		items16.Add(new BuildingInfo(6000, 4f, 1f, 3, 3, BuffType.STUN1));
		BuildingInfoList.Add(SpeciesType.NECROMANCER, items16);

		List<BuildingInfo> items17 = new List<BuildingInfo>();
		items17.Add(new BuildingInfo(18, 3.5f, 1.5f, 4, 2, BuffType.NONE));
		items17.Add(new BuildingInfo(220, 3.5f, 1.5f, 4, 2, BuffType.NONE));
		items17.Add(new BuildingInfo(600, 3.5f, 1.5f, 4, 3, BuffType.NONE));
		items17.Add(new BuildingInfo(2700, 3.5f, 2f, 4, 3, BuffType.NONE));
		items17.Add(new BuildingInfo(4500, 3.5f, 0.3f, 4, 1, BuffType.NONE));
		BuildingInfoList.Add(SpeciesType.FISHMAN, items17);

		List<BuildingInfo> items18 = new List<BuildingInfo>();
		items18.Add(new BuildingInfo(17, 7f, 1.5f, 4, 1, BuffType.ATTACKSPEED_UP0));
		items18.Add(new BuildingInfo(200, 7f, 1.5f, 4, 1, BuffType.ATTACKSPEED_UP0));
		items18.Add(new BuildingInfo(650, 7f, 1.5f, 4, 1, BuffType.ATTACKSPEED_UP1));
		items18.Add(new BuildingInfo(2400, 7f, 1.5f, 4, 1, BuffType.ATTACKSPEED_UP1));
		items18.Add(new BuildingInfo(6000, 7f, 2f, 4, 1, BuffType.ATTACKSPEED_UP2));
		BuildingInfoList.Add(SpeciesType.MONK, items18);
	}
}

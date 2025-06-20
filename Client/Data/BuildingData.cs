using GameDefines;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class BuildingDatabase
{
	public Dictionary<SpeciesType, List<BuildingInfo>> BuildingInfoList;

	public void SetData()
	{
		BuildingInfoList = new Dictionary<SpeciesType, List<BuildingInfo>>();

		List<BuildingInfo> items0 = new List<BuildingInfo>();
		items0.Add(new BuildingInfo(1, 5f, 1f, 1, 3, BuffType.NONE));
		items0.Add(new BuildingInfo(1, 3f, 1f, 1, 1, BuffType.NONE));
		items0.Add(new BuildingInfo(1, 3f, 1f, 1, 1, BuffType.NONE));
		items0.Add(new BuildingInfo(1, 5f, 1f, 1, 4, BuffType.NONE));
		items0.Add(new BuildingInfo(1, 3f, 3f, 1, 5, BuffType.NONE));
		BuildingInfoList.Add(SpeciesType.NONE, items0);

		List<BuildingInfo> items1 = new List<BuildingInfo>();
		items1.Add(new BuildingInfo(18, 3.5f, 1f, 10, 1, BuffType.NONE));
		items1.Add(new BuildingInfo(30, 4f, 1f, 50, 2, BuffType.NONE));
		items1.Add(new BuildingInfo(160, 5f, 1f, 100, 3, BuffType.NONE));
		items1.Add(new BuildingInfo(550, 6.5f, 1f, 300, 4, BuffType.SLOW1));
		items1.Add(new BuildingInfo(1500, 8.5f, 1f, 800, 5, BuffType.CRITICAL1));
		BuildingInfoList.Add(SpeciesType.ELF, items1);

		List<BuildingInfo> items2 = new List<BuildingInfo>();
		items2.Add(new BuildingInfo(20, 3f, 1f, 10, 1, BuffType.ARMORREDUCING0));
		items2.Add(new BuildingInfo(35, 3.5f, 1f, 50, 2, BuffType.ARMORREDUCING0));
		items2.Add(new BuildingInfo(260, 3f, 1f, 100, 1, BuffType.ARMORREDUCING1));
		items2.Add(new BuildingInfo(600, 5f, 1.5f, 300, 3, BuffType.ARMORREDUCING1));
		items2.Add(new BuildingInfo(3000, 3f, 2f, 800, 1, BuffType.ARMORREDUCING2));
		BuildingInfoList.Add(SpeciesType.HUMAN, items2);

		List<BuildingInfo> items3 = new List<BuildingInfo>();
		items3.Add(new BuildingInfo(22, 3f, 1f, 10, 1, BuffType.CRITICAL0));
		items3.Add(new BuildingInfo(40, 3f, 1f, 50, 1, BuffType.CRITICAL0));
		items3.Add(new BuildingInfo(220, 6.5f, 1f, 100, 2, BuffType.CRITICAL1));
		items3.Add(new BuildingInfo(650, 6.5f, 1f, 300, 2, BuffType.CRITICAL1));
		items3.Add(new BuildingInfo(2000, 6.5f, 2f, 800, 3, BuffType.CRITICAL2));
		BuildingInfoList.Add(SpeciesType.DRAKE, items3);

		List<BuildingInfo> items4 = new List<BuildingInfo>();
		items4.Add(new BuildingInfo(24, 3f, 1f, 10, 1, BuffType.CRITICAL0));
		items4.Add(new BuildingInfo(50, 3.5f, 1f, 50, 1, BuffType.CRITICAL0));
		items4.Add(new BuildingInfo(320, 4f, 1f, 100, 1, BuffType.CRITICAL1));
		items4.Add(new BuildingInfo(800, 3f, 1f, 300, 2, BuffType.CRITICAL1));
		items4.Add(new BuildingInfo(2000, 3f, 0.25f, 800, 3, BuffType.CRITICAL2));
		BuildingInfoList.Add(SpeciesType.ORC, items4);

		List<BuildingInfo> items5 = new List<BuildingInfo>();
		items5.Add(new BuildingInfo(18, 3f, 1.5f, 10, 1, BuffType.NONE));
		items5.Add(new BuildingInfo(40, 3f, 1.5f, 50, 1, BuffType.NONE));
		items5.Add(new BuildingInfo(165, 3.5f, 1.75f, 100, 2, BuffType.NONE));
		items5.Add(new BuildingInfo(450, 3.5f, 2f, 300, 2, BuffType.NONE));
		items5.Add(new BuildingInfo(1250, 3f, 2f, 800, 4, BuffType.NONE));
		BuildingInfoList.Add(SpeciesType.SAMURAI, items5);

		List<BuildingInfo> items6 = new List<BuildingInfo>();
		items6.Add(new BuildingInfo(19, 3f, 1.25f, 10, 1, BuffType.POISON0));
		items6.Add(new BuildingInfo(45, 3f, 1.25f, 50, 1, BuffType.POISON0));
		items6.Add(new BuildingInfo(155, 3f, 1.5f, 100, 3, BuffType.POISON1));
		items6.Add(new BuildingInfo(444, 3f, 1.5f, 300, 2, BuffType.POISON1));
		items6.Add(new BuildingInfo(3000, 3f, 0.75f, 800, 1, BuffType.SLOW2));
		BuildingInfoList.Add(SpeciesType.UNDEAD, items6);

		List<BuildingInfo> items7 = new List<BuildingInfo>();
		items7.Add(new BuildingInfo(5, 3f, 8f, 10, 1, BuffType.NONE));
		items7.Add(new BuildingInfo(15, 3f, 9f, 50, 2, BuffType.NONE));
		items7.Add(new BuildingInfo(65, 3f, 10f, 100, 2, BuffType.POISON1));
		items7.Add(new BuildingInfo(120, 3f, 11f, 300, 3, BuffType.POISON1));
		items7.Add(new BuildingInfo(350, 3.5f, 12f, 800, 5, BuffType.POISON2));
		BuildingInfoList.Add(SpeciesType.ZOMBIE, items7);

		List<BuildingInfo> items8 = new List<BuildingInfo>();
		items8.Add(new BuildingInfo(21, 3f, 1f, 10, 1, BuffType.KNOCKBACK0));
		items8.Add(new BuildingInfo(45, 3f, 1f, 50, 2, BuffType.KNOCKBACK0));
		items8.Add(new BuildingInfo(199, 3f, 1.5f, 100, 2, BuffType.KNOCKBACK1));
		items8.Add(new BuildingInfo(600, 3f, 2f, 300, 2, BuffType.KNOCKBACK1));
		items8.Add(new BuildingInfo(2222, 3f, 2f, 800, 2, BuffType.KNOCKBACK2));
		BuildingInfoList.Add(SpeciesType.FURRY, items8);

		List<BuildingInfo> items9 = new List<BuildingInfo>();
		items9.Add(new BuildingInfo(13, 4.5f, 0.75f, 10, 1, BuffType.ATTACK_UP0));
		items9.Add(new BuildingInfo(25, 5.5f, 0.75f, 50, 1, BuffType.ATTACK_UP0));
		items9.Add(new BuildingInfo(100, 6.5f, 0.75f, 100, 2, BuffType.ATTACK_UP1));
		items9.Add(new BuildingInfo(500, 7.5f, 0.75f, 300, 1, BuffType.ATTACK_UP1));
		items9.Add(new BuildingInfo(1000, 10.5f, 0.75f, 800, 4, BuffType.ATTACK_UP2));
		BuildingInfoList.Add(SpeciesType.DWARF, items9);

		List<BuildingInfo> items10 = new List<BuildingInfo>();
		items10.Add(new BuildingInfo(20, 3f, 1f, 10, 1, BuffType.BURN0));
		items10.Add(new BuildingInfo(45, 3f, 1f, 50, 1, BuffType.BURN0));
		items10.Add(new BuildingInfo(222, 3f, 1.5f, 100, 2, BuffType.BURN1));
		items10.Add(new BuildingInfo(400, 4f, 1.75f, 300, 2, BuffType.BURN1));
		items10.Add(new BuildingInfo(1500, 3f, 2f, 800, 3, BuffType.BURN2));
		BuildingInfoList.Add(SpeciesType.DEMON, items10);

		List<BuildingInfo> items11 = new List<BuildingInfo>();
		items11.Add(new BuildingInfo(18, 3.5f, 1f, 10, 2, BuffType.SLOW0));
		items11.Add(new BuildingInfo(33, 3.5f, 1f, 50, 3, BuffType.KNOCKBACK0));
		items11.Add(new BuildingInfo(212, 3.5f, 1f, 100, 4, BuffType.STUN0));
		items11.Add(new BuildingInfo(500, 3.5f, 1f, 300, 3, BuffType.ARMORREDUCING0));
		items11.Add(new BuildingInfo(5000, 5.5f, 1f, 800, 1, BuffType.CRITICAL0));
		BuildingInfoList.Add(SpeciesType.UNKNOWN, items11);

		List<BuildingInfo> items12 = new List<BuildingInfo>();
		items12.Add(new BuildingInfo(16, 3f, 0.75f, 10, 1, BuffType.STEAL0));
		items12.Add(new BuildingInfo(40, 3.5f, 0.75f, 50, 2, BuffType.STEAL1));
		items12.Add(new BuildingInfo(200, 3f, 0.75f, 100, 1, BuffType.STEAL1));
		items12.Add(new BuildingInfo(650, 4f, 0.75f, 300, 2, BuffType.STEAL2));
		items12.Add(new BuildingInfo(2000, 2f, 0.1f, 800, 1, BuffType.STEAL2));
		BuildingInfoList.Add(SpeciesType.GOBLIN, items12);

		List<BuildingInfo> items13 = new List<BuildingInfo>();
		items13.Add(new BuildingInfo(15, 3f, 0.75f, 10, 1, BuffType.STUN0));
		items13.Add(new BuildingInfo(30, 3f, 0.75f, 50, 1, BuffType.STUN0));
		items13.Add(new BuildingInfo(150, 3f, 0.75f, 100, 1, BuffType.STUN1));
		items13.Add(new BuildingInfo(500, 3.5f, 0.75f, 300, 1, BuffType.STUN1));
		items13.Add(new BuildingInfo(1500, 3.5f, 0.75f, 800, 1, BuffType.STUN2));
		BuildingInfoList.Add(SpeciesType.FANATIC, items13);

		List<BuildingInfo> items14 = new List<BuildingInfo>();
		items14.Add(new BuildingInfo(8, 3f, 1f, 10, 1, BuffType.NONE));
		items14.Add(new BuildingInfo(22, 3.5f, 1f, 50, 2, BuffType.NONE));
		items14.Add(new BuildingInfo(160, 3f, 1f, 100, 3, BuffType.NONE));
		items14.Add(new BuildingInfo(300, 3.5f, 1f, 300, 4, BuffType.NONE));
		items14.Add(new BuildingInfo(1000, 3f, 1f, 800, 5, BuffType.NONE));
		BuildingInfoList.Add(SpeciesType.DARKELF, items14);

		List<BuildingInfo> items15 = new List<BuildingInfo>();
		items15.Add(new BuildingInfo(10, 3f, 1f, 10, 1, BuffType.SLOW0));
		items15.Add(new BuildingInfo(35, 3.5f, 0.5f, 50, 1, BuffType.SLOW0));
		items15.Add(new BuildingInfo(150, 4.5f, 0.75f, 100, 1, BuffType.SLOW1));
		items15.Add(new BuildingInfo(450, 5.5f, 1f, 300, 1, BuffType.SLOW1));
		items15.Add(new BuildingInfo(1000, 8.5f, 1.5f, 800, 1, BuffType.SLOW2));
		BuildingInfoList.Add(SpeciesType.ANDROID, items15);

		List<BuildingInfo> items16 = new List<BuildingInfo>();
		items16.Add(new BuildingInfo(25, 3f, 1f, 10, 1, BuffType.NONE));
		items16.Add(new BuildingInfo(55, 3.5f, 1f, 50, 1, BuffType.NONE));
		items16.Add(new BuildingInfo(210, 4.5f, 0.5f, 100, 1, BuffType.NONE));
		items16.Add(new BuildingInfo(600, 5.5f, 0.5f, 300, 3, BuffType.NONE));
		items16.Add(new BuildingInfo(5000, 6.5f, 0.75f, 800, 1, BuffType.NONE));
		BuildingInfoList.Add(SpeciesType.WIZARD, items16);

		List<BuildingInfo> items17 = new List<BuildingInfo>();
		items17.Add(new BuildingInfo(15, 3f, 0.75f, 10, 2, BuffType.STUN0));
		items17.Add(new BuildingInfo(30, 3.5f, 0.75f, 50, 2, BuffType.STUN0));
		items17.Add(new BuildingInfo(180, 3f, 0.75f, 100, 1, BuffType.STUN1));
		items17.Add(new BuildingInfo(350, 4f, 0.75f, 300, 5, BuffType.STUN1));
		items17.Add(new BuildingInfo(1500, 3f, 0.75f, 800, 1, BuffType.STUN2));
		BuildingInfoList.Add(SpeciesType.NECROMANCER, items17);

		List<BuildingInfo> items18 = new List<BuildingInfo>();
		items18.Add(new BuildingInfo(18, 3f, 0.75f, 10, 2, BuffType.NONE));
		items18.Add(new BuildingInfo(40, 3f, 0.75f, 50, 2, BuffType.NONE));
		items18.Add(new BuildingInfo(235, 3f, 0.75f, 100, 3, BuffType.NONE));
		items18.Add(new BuildingInfo(800, 3f, 1.5f, 300, 1, BuffType.NONE));
		items18.Add(new BuildingInfo(2000, 3f, 0.5f, 800, 1, BuffType.NONE));
		BuildingInfoList.Add(SpeciesType.FISHMAN, items18);

		List<BuildingInfo> items19 = new List<BuildingInfo>();
		items19.Add(new BuildingInfo(15, 4.5f, 0.5f, 10, 1, BuffType.ATTACKSPEED_UP0));
		items19.Add(new BuildingInfo(25, 5.5f, 0.75f, 50, 1, BuffType.ATTACKSPEED_UP0));
		items19.Add(new BuildingInfo(150, 6.5f, 0.75f, 100, 1, BuffType.ATTACKSPEED_UP1));
		items19.Add(new BuildingInfo(500, 7.5f, 1f, 300, 1, BuffType.ATTACKSPEED_UP1));
		items19.Add(new BuildingInfo(1000, 10.5f, 1f, 800, 1, BuffType.ATTACKSPEED_UP2));
		BuildingInfoList.Add(SpeciesType.MONK, items19);
	}
}

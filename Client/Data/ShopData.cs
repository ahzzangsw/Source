using GameDefines;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class ShopDatabase
{
	public List<ShopInfo> ShopInfoList = new List<ShopInfo>();

	public void SetData()
	{
		ShopInfoList.Add(new ShopInfo("2XSPEED(ALL)", "<color=#6dff00>Press 'Tab' key to run</color><br>2x the speed of the game", 100, SpeciesType.MAX, 0));
		ShopInfoList.Add(new ShopInfo("3XSPEED(BUILD/SPAWN)", "<color=#6dff00>Press 'Tab' key to run</color><br>3x the speed of the game<br>Disable Adventure", 200, SpeciesType.MAX, 1));
		ShopInfoList.Add(new ShopInfo("DARKELF", "Burst Shot", 30, SpeciesType.DARKELF, 0));
		ShopInfoList.Add(new ShopInfo("DWARF", "Repair Weapon", 50, SpeciesType.DWARF, 0));
		ShopInfoList.Add(new ShopInfo("FANATIC", "Rumor", 50, SpeciesType.FANATIC, 0));
		ShopInfoList.Add(new ShopInfo("FISHMAN", "Have you ever been hit by water?", 50, SpeciesType.FISHMAN, 0));
		ShopInfoList.Add(new ShopInfo("FURRY", "Cuteness rules the world", 50, SpeciesType.FURRY, 0));
		ShopInfoList.Add(new ShopInfo("MONK", "Power of Believe", 50, SpeciesType.MONK, 0));
		ShopInfoList.Add(new ShopInfo("UNDEAD", "Dead Men Tell No Tales", 50, SpeciesType.UNDEAD, 2));
		ShopInfoList.Add(new ShopInfo("WIZARD", "Magic is The Best", 70, SpeciesType.WIZARD, 0));
		ShopInfoList.Add(new ShopInfo("UNKNOWN", "Less Hair = Over Power", 100, SpeciesType.UNKNOWN, 0));
	}
}

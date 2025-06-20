using GameDefines;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class CursorDatabase
{
	public List<CursorInfo> CursorInfoList = new List<CursorInfo>();

	public void SetData()
	{
		CursorInfoList.Add(new CursorInfo(0, 0f, 0f, 0f, "Default cursor"));
		CursorInfoList.Add(new CursorInfo(0, 0f, 0f, 0f, "Default cursor"));
		CursorInfoList.Add(new CursorInfo(0, 0f, 0f, 0f, "Default cursor"));
		CursorInfoList.Add(new CursorInfo(0, 0f, 0f, 0f, "Default cursor"));
		CursorInfoList.Add(new CursorInfo(0, 1f, 0f, 0f, "Increase Damage +1"));
		CursorInfoList.Add(new CursorInfo(0, 1f, 0f, 0f, "Increase Damage +1"));
		CursorInfoList.Add(new CursorInfo(0, 0.05f, 0f, 0f, "Increase Attack +5%"));
		CursorInfoList.Add(new CursorInfo(0, 0f, 0.5f, 0f, "Increase Range +0.5"));
		CursorInfoList.Add(new CursorInfo(0, 0.1f, 0f, 0f, "Increase Attack +10%"));
		CursorInfoList.Add(new CursorInfo(0, 0f, 0f, 0.2f, "Increase AttackSpeed +0.2"));
		CursorInfoList.Add(new CursorInfo(0, 0.1f, 0.2f, 0f, "Increase Attack +10%\nRange +0.2"));
		CursorInfoList.Add(new CursorInfo(0, 0.2f, 0f, 0.2f, "Increase Attack +10%\nAttackSpeed +0.2"));
		CursorInfoList.Add(new CursorInfo(0, 0f, 0.2f, 0.2f, "Increase Range +0.2\nAttackSpeed +0.2"));
		CursorInfoList.Add(new CursorInfo(0, 0f, 0f, 0.5f, "Increase AttackSpeed +0.5"));
		CursorInfoList.Add(new CursorInfo(0, 0.15f, 0f, 0f, "Increase Attack +15%"));
		CursorInfoList.Add(new CursorInfo(0, 5f, 0f, 0f, "Increase Attack +5"));
		CursorInfoList.Add(new CursorInfo(0, 10f, 0f, 0f, "Increase Attack +10"));
		CursorInfoList.Add(new CursorInfo(0, 0.1f, 0.2f, 0.2f, "Increase Attack +10%\nIncrease Range +0.2\nAttackSpeed +0.2"));
		CursorInfoList.Add(new CursorInfo(1, 0.1f, 0.5f, 0.5f, "You did it!"));
	}
}

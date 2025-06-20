using GameDefines;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class ControlDatabase
{
	public ControlInfo ControlInfoData;

	public void SetData()
	{
		ControlInfoData = new ControlInfo(10, 3, 9, 30, 90, 300, 50, new int[] { 5, 7, 15, 30, 50, 90, 150, 200, 300 }, new int[] { 1500, 5000, 28000, 76000, 175000, 304000, 1753000, 4115000, 7777777 });
	}
}

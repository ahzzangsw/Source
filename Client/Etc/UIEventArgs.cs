using GameDefines;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UIEventArgs : EventArgs
{
    public int key { get; set; } = -1;
    public string STRING { get; set; } = string.Empty;
    public int INT { get; set; } = -999;
    public Vector2 VECTOR2D { get; set; } = new Vector2();
    
    /////////////////////////////////////////////////////////
    public UIEventArgs()
    {

    }
    public UIEventArgs(string str)
    {
        STRING = str;
    }
    public UIEventArgs(int iNumber)
    {
        INT = iNumber;
    }
    public UIEventArgs(Vector2 Vector)
    {
        VECTOR2D = Vector;
    }
    
    public void SetID(int id)
    {
        key = id;
    }
    public void SetVector2D(Vector2 vector)
    {
        VECTOR2D = vector;
    }
}
using UnityEngine;
using System;
using System.Collections;

public struct GraveInfo
{
    public string userName;
    public string deathMessage;
    public string objectId;
    public enum CurseType { None, Damage, Heal }
    public CurseType curseType;
    public Vector3 position;
    public int checkCounter;
    public bool isUsed;
}

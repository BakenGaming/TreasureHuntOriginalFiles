using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerData : ScriptableObject
{
    public string playerName;
    public float maxHP;
    public int atkPower;
    public int critChance;
    public int blockChance;
}

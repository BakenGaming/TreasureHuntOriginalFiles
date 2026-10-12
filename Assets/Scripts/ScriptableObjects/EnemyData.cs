using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public enum EnemyClassification
{
    slime, bat, spider, beetle, snake
}
[CreateAssetMenu]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public float maxHP;
    public int atkPower;
    public int minCoinsDropped;
    public int maxCoinsDropped;
    public int critChance;
    public int blockChance;
    public bool isFirstStrike;
    public bool canPoison;
    public EnemyClassification classification;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatSystem
{
    private float maxHP;
    private int attackPower;

    public StatSystem (float hp, int atk)
    {
        maxHP = hp;
        attackPower = atk;
    }

    public int GetAttackPower(){ return attackPower; }
    public float GetMaxHealth(){ return maxHP; }

    public void IncreaseAtkPower(int amount) { attackPower += amount; }
    public void IncreaseMaxHP(float amount) { maxHP += amount; }
}

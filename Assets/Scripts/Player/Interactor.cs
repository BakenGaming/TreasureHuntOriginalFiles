using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D trigger) 
    {
        ICollectable collectable = trigger.GetComponent<ICollectable>();
        IEnemy enemy = trigger.GetComponent<IEnemy>();
        if (collectable != null) collectable.Collect();
        if (enemy != null) enemy.TriggerAttack();
    }
}

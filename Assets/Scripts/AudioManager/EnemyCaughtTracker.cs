
//this tracks the number of enemies that are following the player
//and plays the correct music
using System;
using UnityEngine;

public class EnemyCaughtTracker
{
    private int totalAgroedEnemies = 0;
    
    public void IncreaseAgroedCount(int amount)
    {
        totalAgroedEnemies += amount;
        totalAgroedEnemies = Math.Max(0, totalAgroedEnemies);

        bool beingAttacked = totalAgroedEnemies > 0;

        Managers.audioManager.PlayMusic(beingAttacked ? Music.Caught : Music.Sneaky, 1.0f);
    }
}
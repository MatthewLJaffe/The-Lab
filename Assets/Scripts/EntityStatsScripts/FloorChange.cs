using System;
using System.Collections;
using System.Collections.Generic;
using EntityStatsScripts;
using UnityEngine;
using UnityEngine.Events;

public class FloorChange : MonoBehaviour
{
    /// <summary>
    /// used to scale damage and health of enemies when changing to different floor
    /// </summary>
    [SerializeField] private PlayerStats playerStats;
    public UnityEvent onFloor1;
    public UnityEvent onFloor2;
    public UnityEvent onFloor3;

    private void Awake()
    {
        PlayerStats.onStatChange += FloorChangeReaction;
    }

    private void Start()
    {
        FloorChangeReaction(PlayerStats.StatType.CurrentFloor, playerStats.playerStatsDict[PlayerStats.StatType.CurrentFloor].CurrentValue);
    }

    private void FloorChangeReaction(PlayerStats.StatType statType, float newValue)
    {
        if (statType != PlayerStats.StatType.CurrentFloor) return;
        switch (Mathf.RoundToInt(newValue))
        {
            case 1:
                onFloor1.Invoke();
                break;
            case 2:
                onFloor2.Invoke();
                break;
            case 3:
                onFloor3.Invoke();
                break;
            default:
                Debug.LogError("Invalid Floor");
                break;
        }
    }
}

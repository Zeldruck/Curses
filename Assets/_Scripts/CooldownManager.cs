using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CooldownManager : MonoBehaviour
{
    public static CooldownManager instance;

    public delegate void OnCooldownEnd();

    private List<float> cooldownTimers;
    private List<OnCooldownEnd> onCooldownEnds;

    private void Awake()
    {
        instance = this;

        cooldownTimers = new List<float>();
        onCooldownEnds = new List<OnCooldownEnd>();
    }

    private void Update()
    {
        int timerCount = cooldownTimers.Count;
        
        for (int i = 0; i < timerCount; i++)
        {
            cooldownTimers[i] -= Time.deltaTime;

            if (cooldownTimers[i] <= 0f)
            {
                onCooldownEnds[i].Invoke();
                
                cooldownTimers.RemoveAt(i);
                onCooldownEnds.RemoveAt(i);
                
                timerCount--;
                i--;
            }
        }
    }

    public void AddCooldown(float _timer, OnCooldownEnd _onCooldownEnd)
    {
        if (_timer <= 0f || _onCooldownEnd == null)
            return;
        
        cooldownTimers.Add(_timer);
        onCooldownEnds.Add(_onCooldownEnd);
    }

    public void RemoveFirstCooldown(OnCooldownEnd _onCooldownEnd)
    {
        if (_onCooldownEnd == null)
            return;

        int listIndex = onCooldownEnds.IndexOf(_onCooldownEnd);
        
        if (listIndex < 0)
            return;
        
        cooldownTimers.RemoveAt(listIndex);
        onCooldownEnds.RemoveAt(listIndex);
    }
}

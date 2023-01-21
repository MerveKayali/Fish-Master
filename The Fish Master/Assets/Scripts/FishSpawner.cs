using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FishSpawner : MonoBehaviour
{
    void Awake()
    {
        for(int i=0;i<fishTypes.Length;i++)
        {
            int num = 0;
            while(num<fishTypes[i].fishCount)
            {
                Fish fish = UnityEngine.Object.Instantiate<Fish>(FishPrefab);
                fish.Type = fishTypes[i];
                fish.ResetFish();
                num++;
            }
        }
        
    }

    [SerializeField]//private objeleri insprector içinde görmemizi saðlar
    private Fish FishPrefab;

    [SerializeField]
    private Fish.FishType[] fishTypes;

  
}

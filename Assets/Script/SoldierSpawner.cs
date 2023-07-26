using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoldierSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _soldierPrefab;
    [SerializeField] private MedicalBed[] _beds;

    private void Start()
    {
        StartCoroutine(SoldierSpawn(_soldierPrefab));
    }

    IEnumerator SoldierSpawn(GameObject soldierPrefab)
    {
        List<MedicalBed> freeBeds = new List<MedicalBed>();
        foreach (var bed in _beds)
        {
            if (bed.IsBedFree())
                freeBeds.Add(bed);
        }
        
        while (freeBeds.Count > 0)
        {
            MedicalBed soldierBed = freeBeds[Random.Range(0, freeBeds.Count)];
            Transform soldierTransform = soldierBed.transform;
            Vector3 soldierPosition = new Vector3(soldierTransform.position.x, soldierTransform.position.y);
            GameObject soldier = Instantiate(soldierPrefab, soldierPosition, Quaternion.identity);
            soldier.GetComponent<Soldier>().SetBed(soldierBed);
            soldierBed.SoldierSettling(soldier.GetComponent<Soldier>());
            freeBeds.Remove(soldierBed);
            yield return new WaitForSeconds(1f);
        }
        
        while (freeBeds.Count < 1)
        {
            foreach (var bed in _beds)
            {
                if (bed.IsBedFree())
                    freeBeds.Add(bed);
            }

            yield return new WaitForSeconds(1f);
        }
        StartCoroutine(SoldierSpawn(_soldierPrefab));
    }
}
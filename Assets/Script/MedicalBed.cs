using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedicalBed : MonoBehaviour
{
    [SerializeField] private Soldier _soldier;

    public void SoldierSettling(Soldier soldier)
    {
        _soldier = soldier;
    }

    public bool IsBedFree()
    {
        if (_soldier == null)
            return true;
        else
            return false;
    }

    public void RemoveSoldier()
    {
        _soldier = null;
    }
}

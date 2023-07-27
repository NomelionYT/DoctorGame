using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class DoctorHeal : MonoBehaviour
{
    private bool _isInTrigger;

    private Soldier _soldier;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.GetComponent<Soldier>())
            _soldier = col.GetComponent<Soldier>();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<Soldier>())
        {
            if (_soldier.WasHealed)
                _soldier.FirstAid();
            _soldier = null;
        }
    }

    private void Update()
    {
        Heal();
    }

    private void Heal()
    {
        if (_soldier != null)
            if (Input.GetKey(KeyCode.F))
                _soldier.Heal();
            else if (Input.GetKeyUp(KeyCode.F))
                _soldier.FirstAid();
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DoctorHeal : MonoBehaviour
{
    public UnityAction OnSoldierHeal;
    public UnityAction OnSoldierFirstAid;

    private bool _isInTrigger;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.GetComponent<Soldier>())
            _isInTrigger = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<Soldier>())
            _isInTrigger = false;
    }

    private void Update()
    {
        if (_isInTrigger)
        {
            if (Input.GetKey(KeyCode.F))
                OnSoldierHeal?.Invoke();
            else if (Input.GetKeyUp(KeyCode.F))
                OnSoldierFirstAid?.Invoke();
        }
    }
}

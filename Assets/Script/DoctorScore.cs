using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoctorScore : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text _scoreText;

    private int _score = 0;

    private void Update()
    {
        _scoreText.text = _score.ToString();
    }

    public void AddScore()
    {
        _score++;
    }

    public void RemoveScore()
    {
        _score--;
    }
}

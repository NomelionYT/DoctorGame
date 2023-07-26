using System;
using System.Collections;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public enum Injuries
{
    Easy,
    Medium,
    Hard
}

public class Soldier : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text _hpText;
    [SerializeField] private Image _injuryImage;

    [SerializeField] private MedicalBed _soldierBed;
    private DoctorScore _doctorScore;
    private IEnumerator _dyingCoroutine;
    private bool _canHeal = true;
    private Injuries _injury;
    private int _hp;
    private float _speedOfDying;

    private void Awake()
    {
        _doctorScore = FindObjectOfType<DoctorScore>();
    }

    private void Start()
    {
        _hp = UnityEngine.Random.Range(70, 85);
        var injuries = Enum.GetValues(typeof(Injuries));
        var injury = injuries.GetValue(UnityEngine.Random.Range(0, injuries.Length));
        _injury = (Injuries)injury;
        
        if (_injury == Injuries.Easy)
            _injuryImage.color = Color.green;
        else if (_injury == Injuries.Medium)
            _injuryImage.color = Color.yellow;
        else if (_injury == Injuries.Hard)
            _injuryImage.color = Color.red;
        
        if (_injury == Injuries.Easy)
            _speedOfDying = 1f;
        else if (_injury == Injuries.Medium)
            _speedOfDying = 0.6f;
        else if (_injury == Injuries.Hard)
            _speedOfDying = 0.3f;
        
        _dyingCoroutine = DyingCoroutine(_speedOfDying);
        StartCoroutine(_dyingCoroutine);
    }

    private void Update()
    {
        _hpText.text = _hp.ToString();
    }

    public void Heal()
    {
        if (_hp < 100)
            StartCoroutine(HealCoroutine());
        else
        {
            RemoveSoldier(true);
        }
    }

    private void RemoveSoldier(bool isAddScore)
    {
        if (isAddScore)
            _doctorScore.AddScore();
        else
            _doctorScore.RemoveScore();
        _soldierBed.RemoveSoldier();
        _soldierBed = null;
        Destroy(gameObject);
    }

    public void SetBed(MedicalBed bed)
    {
        _soldierBed = bed;
    }

    public void FirstAid()
    {
        _injury = Injuries.Easy;
        _speedOfDying = 1f;
        _injuryImage.color = Color.green;
        StopCoroutine(_dyingCoroutine);
        _dyingCoroutine = DyingCoroutine(_speedOfDying);
        StartCoroutine(_dyingCoroutine);
        _canHeal = true;
    }

    IEnumerator DyingCoroutine(float speedOfDying)
    {
        while (_hp > 0)
        {
            yield return new WaitForSeconds(speedOfDying);
            _hp--;
            print(speedOfDying);
        }
        RemoveSoldier(false);
    }

    IEnumerator HealCoroutine()
    {
        if (_canHeal)
        {
            StopCoroutine(_dyingCoroutine);
            _canHeal = false;
            _hp++;
            yield return new WaitForSeconds(0.2f);
            _canHeal = true;
        }
        yield return null;
    }
}
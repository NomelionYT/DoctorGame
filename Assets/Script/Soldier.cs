using System;
using System.Collections;
using System.Security.Cryptography;
using Unity.Mathematics;
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
    [SerializeField] private GameObject _healVFXPrefab;

    private Vector3 _VFXOffset;
    private GameObject _healVFXInstantiate;
    private MedicalBed _soldierBed;
    private DoctorScore _doctorScore;
    private IEnumerator _dyingCoroutine;
    private bool _canHeal = true;
    private Injuries _injury;
    private int _hp;
    private float _speedOfDying;
    private Coroutine _currentCoroutine;

    private void Awake()
    {
        _doctorScore = FindObjectOfType<DoctorScore>();
    }

    private void Start()
    {
        _VFXOffset = new Vector3(0, 0.4f, 0);
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
        if (_healVFXInstantiate == null)
            SetHealVFX(true);

        if (_hp < 100)
        {
            if (_currentCoroutine == null) //мой самый простой, гениальный и полезный bugfix
                _currentCoroutine = StartCoroutine(HealCoroutine());
        }
        else
            RemoveSoldier(true);
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

    private void SetHealVFX(bool state)
    {
        //тут немного поясню
        if (state)
        {
            Destroy(_healVFXInstantiate); //удаляем старый объект VKX
            _healVFXInstantiate = Instantiate(_healVFXPrefab, gameObject.transform); //создаём новый объект VFX
            _healVFXInstantiate.transform.position -= _VFXOffset; //немного корректируем позицию VFX (для красоты)
        }
        else
        {
            _healVFXInstantiate.GetComponent<ParticleSystem>().Stop(); //стопаем VFX вместо того, чтобы удалять объект (что б не было "багов" и было красиво)
            _healVFXInstantiate = null; //делаем объект VFX null, чтобы можно было создать новый
        }
    }

    public void SetBed(MedicalBed bed)
    {
        _soldierBed = bed;
    }

    public void FirstAid()
    {
        if (_injury != Injuries.Easy)
        {
            _injury = Injuries.Easy;
            _speedOfDying = 1f;
            _injuryImage.color = Color.green;
            StopCoroutine(_dyingCoroutine);
            _dyingCoroutine = DyingCoroutine(_speedOfDying);
            StartCoroutine(_dyingCoroutine);
        }
        _canHeal = true;
        SetHealVFX(false);
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
            _canHeal = false;
            StopCoroutine(_dyingCoroutine);
            _hp++;
            yield return new WaitForSeconds(0.20f);
            _canHeal = true;
        }

        _currentCoroutine = null;
    }
}
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
    [SerializeField] private Canvas _canvas;

    private Animator _animator;
    private GameObject _healVFXInstantiate;
    private MedicalBed _soldierBed;
    private DoctorScore _doctorScore;
    private IEnumerator _dyingCoroutine;
    private Vector3 _VFXOffset;
    private Injuries _injury;
    private bool _isHealCoroutineActive;
    private bool _isDyingCoroutineActive;
    private bool _canHeal = true;
    private bool _wasHealed = false;
    private bool _isInHospital = true;
    private bool _isInBed = true;
    private int _hp;
    private float _speedOfDying;

    public bool WasHealed
    {
        get => _wasHealed;
        private set { }
    }

    private void Awake()
    {
        _doctorScore = FindObjectOfType<DoctorScore>();
    }

    private void Start()
    {
        _animator = GetComponent<Animator>();
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
        _isDyingCoroutineActive = true;
        _isInBed = true;
    }

    private void Update()
    {
        _hpText.text = _hp.ToString();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Exit"))
            _isInHospital = false;
    }

    public void Heal()
    {
        if (_healVFXInstantiate == null)
        {
            SetHealVFX(true);
            _wasHealed = true;
        }
        
        if (_hp < 100)
        {
            if (_isHealCoroutineActive == false)
            {
                StartCoroutine(HealCoroutine());
                _isHealCoroutineActive = true;
            }
        }
        else
        {
            if (_isInBed)
            {
                _isInBed = false;
                RemoveSoldier(true);
            }
        }
    }

    private void RemoveSoldier(bool isAddScore)
    {
        GetComponent<BoxCollider2D>().isTrigger = false;
        _canvas.enabled = false;
        StopAllCoroutines();
        if (_soldierBed != null)
            _soldierBed.RemoveSoldier();
        _soldierBed = null;
        if (isAddScore)
        {
            _doctorScore.AddScore();
            Leaving();
        }
        else
        {
            _doctorScore.RemoveScore();
            Destroy(gameObject);
        }
    }

    private void SetHealVFX(bool state)
    {
        if (state)
        {
            Destroy(_healVFXInstantiate); //удаляем старый объект VKX
            _healVFXInstantiate = Instantiate(_healVFXPrefab, gameObject.transform); //создаём новый объект VFX
            _healVFXInstantiate.transform.position -= _VFXOffset; //немного корректируем позицию VFX (для красоты)
        }
        else
        {
            if (_healVFXInstantiate != null)
            {
                _healVFXInstantiate.GetComponent<ParticleSystem>().Stop(); //стопаем VFX вместо того, чтобы удалять объект (что б не было "багов" и было красиво)
                _healVFXInstantiate = null; //делаем объект VFX null, чтобы можно было создать новый
            }
        }
    }

    private void Leaving()
    {
        _animator.SetBool("IsWalk", true);
        StartCoroutine(MoveCoroutine(new Vector3(0, 10, 0), 10f));
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
            _isDyingCoroutineActive = false;
        }

        if (!_isDyingCoroutineActive)
        {
            _dyingCoroutine = DyingCoroutine(_speedOfDying);
            StartCoroutine(_dyingCoroutine);
            _isDyingCoroutineActive = true;
        }
        _canHeal = true;
        SetHealVFX(false);
    }

    IEnumerator MoveCoroutine(Vector3 targetPos, float speed)
    {
        while (_isInHospital)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            yield return new WaitForSeconds(0.01f); //дабы не зависало
        }
        Destroy(gameObject);
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
            _isDyingCoroutineActive = false;
            _hp++;
            yield return new WaitForSeconds(0.20f);
            _canHeal = true;
        }

        _isHealCoroutineActive = false;
    }
}
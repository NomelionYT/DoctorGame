using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementMobile : MonoBehaviour
{
    [SerializeField] private float _speed = 5;

    private Coroutine _coroutine;

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Move();
        }
    }

    private void Move()
    {
        if (_coroutine != null)
            StopCoroutine(_coroutine);

        var point = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
        _coroutine = StartCoroutine(Moving(point));
    }

    private IEnumerator Moving(Vector2 point)
    {
        while (Vector2.Distance(transform.position, point) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, point, Time.deltaTime * _speed);
            yield return null;
        }

        _coroutine = null;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] float Speed = 100f;

    Rigidbody2D _Rigidbody;

    private void Start()
    {
        Reset();
    }

    public void Reset()
    {
        transform.position = new Vector2(Random.value < 0.5f ? Random.Range(-1.0f, -0.5f) : Random.Range(0.5f, 1f), Random.value < 0.5f ? -1 : 1);

        float X = Random.value < 0.5f ? -1 : 1;
        float Y = Random.value < 0.5f ? Random.Range(-1.0f, -0.5f) : Random.Range(0.5f, 1f);

        _Rigidbody = GetComponent<Rigidbody2D>();
        _Rigidbody.velocity = Vector2.zero;
        Vector2 Direction = new Vector2(X, Y);
        _Rigidbody.AddForce(Direction * Speed);
        GetComponentInChildren<TrailRenderer>().Clear();
    }

    /*private void OnCollisionEnter2D(Collision2D collision)
    {
        GetComponent<Rigidbody2D>().velocity = Vector3.zero;
    }*/
}

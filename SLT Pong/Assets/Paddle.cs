using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Paddle : MonoBehaviour
{
    [SerializeField] bool Autonomous;

    [SerializeField] float Speed = 5f;

    //Transform BallTransform;

    Vector3 Direction;

    Rigidbody2D BallRigidbody;
    Rigidbody2D _Rigidbody;

    private void Start()
    {
        /*BallTransform = FindObjectOfType<Ball>().transform;
        BallRigidbody = BallTransform.GetComponent<Rigidbody2D>();*/

        _Rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!Autonomous)
        {
            Direction.y = Input.GetAxis("Vertical");
            transform.position += Direction * Speed * Time.deltaTime;
        }
        else
        {
            BallRigidbody = Tools.ByDistance(transform.position, FindObjectsOfType<Ball>().Select(item => item.transform).ToList())[0].GetComponent<Rigidbody2D>();
            /*Direction.y = -(transform.position - BallTransform.position).normalized.y;
            transform.position += Direction * Speed * Time.deltaTime;*/

            //Get Direction between Paddle and Ball
            //Set Direction Z and X to 0
            //Position += Direction * Speed * Time.deltaTime;

            if (BallRigidbody.velocity.x > 0)
            {
                if (BallRigidbody.position.y > transform.position.y)
                    _Rigidbody.AddForce(Vector2.up * Speed);
                else if (BallRigidbody.position.y < transform.position.y)
                    _Rigidbody.AddForce(Vector2.down * Speed);
            }
            else
            {
                if (transform.position.y > 0)
                    _Rigidbody.AddForce(Vector2.down * Speed);
                else if (transform.position.y < 0)
                    _Rigidbody.AddForce(Vector2.up * Speed);
            }
        }
    }

    private void FixedUpdate()
    {
        /*if (!Autonomous)
        {
            //if (Direction.sqrMagnitude != 0)
                _Rigidbody.AddForce(Direction * (Speed * 100));
        }*/
    }
}

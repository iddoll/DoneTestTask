using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paddle : MonoBehaviour
{
    public float _leftClamp = 380;
    public float _rightCLamp = 1200;
    private float _mousePositionPixels;
    private float _mousePositionWorldX;
    private float paddleStartY;
    private float paddleStartZ;

    private void Start()
    {
        paddleStartY = transform.position.y;
        paddleStartZ = transform.position.z;
    }
    void Update()
    {
        PaddleMovement();
    }

    private void PaddleMovement()
    {
        _mousePositionPixels = Mathf.Clamp(Input.mousePosition.x, _leftClamp, _rightCLamp);
        _mousePositionWorldX = Camera.main.ScreenToWorldPoint(new Vector3(_mousePositionPixels, 0,0)).x;
        transform.position = new Vector3(_mousePositionWorldX, paddleStartY, paddleStartZ);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.TryGetComponent(out Rigidbody ball))
        {
            GameManager.instance.AddScore();

            Vector3 hitPoint = collision.contacts[0].point;
            ball.linearVelocity = Vector3.zero;

            float difference = transform.position.x - hitPoint.x;

            if(hitPoint.x < transform.position.x)
            {
                ball.AddForce(new Vector3(-(Mathf.Abs(difference * 200)), 700));
            }
            else
            {
                ball.AddForce(new Vector3(Mathf.Abs(difference * 200), 700));
            }

        }
    }
}

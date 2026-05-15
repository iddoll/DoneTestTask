using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathWall : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.TryGetComponent(out Rigidbody ball))
        {
            Destroy(ball.gameObject);
            GameManager.instance.AddDeathScore();
            GameManager.instance.IsGameStarted = false;
        }
    }
}

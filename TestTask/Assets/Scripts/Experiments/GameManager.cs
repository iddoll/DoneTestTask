using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool IsGameStarted;
    private GameObject _ballPrefab;
    private GameObject _paddlePrefab;

    private GameObject _ball;
    private GameObject _paddle;

    private Rigidbody _ballRb;

    [SerializeField] private Text _scoreText;
    [SerializeField] private Text _deathsText;

    private int _score;
    private int _deaths;

    private void Start()
    {
        instance = this;
        IsGameStarted = false;

        _ballPrefab = Resources.Load<GameObject>("Prefabs/Ball");
        _paddlePrefab = Resources.Load<GameObject>("Prefabs/Paddle");

        _paddle = Instantiate(_paddlePrefab);
        BallInit();
    }

    void Update()
    {
        if (!IsGameStarted)
        {
            _ball.transform.position = new Vector3(_paddle.transform.position.x, _paddle.transform.position.y + 2, _paddle.transform.position.z);
            if (Input.GetMouseButtonDown(0))
            {
                System.Random random = new System.Random();
                var result = random.Next(0, 2) * 2 - 1;
                _ballRb.isKinematic = false;
                _ballRb.AddForce(new Vector3(result * 500,700, 0));
                IsGameStarted = true;
            }
        }
    }

    public void AddScore()
    {
        _score += 1;
        _scoreText.text = _score.ToString();
    }

    public void AddDeathScore()
    {
        _deaths += 1;
        _deathsText.text = _deaths.ToString();
        BallInit();
    }
    private void BallInit()
    {
        _ball = Instantiate(_ballPrefab, new Vector3(_paddle.transform.position.x, _paddle.transform.position.y + 0.5f, _paddle.transform.position.z), Quaternion.identity);
        _ballRb = _ball.GetComponent<Rigidbody>();
        _ballRb.isKinematic = true;
    }
}

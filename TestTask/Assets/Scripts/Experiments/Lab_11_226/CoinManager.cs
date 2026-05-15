using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Lab_11_226
{
    [Serializable]
    public class Variant
    {
        public int down;
        public int up;
    }

    public class CoinManager : MonoBehaviour
    {
        public event Action OnFirstCoinPutToCup;
        
        public GameObject coinPrefab;
        public Animator expAnimator;
        public Animator glass;
        public Transform glassAnchor;
        public Transform expRoot;
        public Transform spawnPoint;
        public Transform targetUp;
        public Transform targetDown;
        public Transform targetTable;
        private List<Variant> variants;
        private int count = 0;

        public List<Coin> _coins = new List<Coin>();
        public List<Coin> _downCoins = new List<Coin>();
        private static readonly int Run = Animator.StringToHash("Run");
        private float scaleFactor;

        [SerializeField] private GameObject[] coinPileArray;
        [SerializeField] private GameObject[] coinPileBrray;

        private int leftCup = 0;
        private int rightCup = 0;

        public bool isPause;

        private void FixedUpdate()
        {
            scaleFactor = expRoot.lossyScale.x;
        }
        
        public void StartPause()
        {
            isPause = true;
            for (int i = 0; i < _coins.Count; i++)
            {
                var coin = _coins[i];
                coin.SetPause(isPause);
            }

            for (int i = 0; i < _downCoins.Count; i++)
            {
                var coin = _downCoins[i];
                coin.SetPause(isPause);
            }
        }

        public void EndPause()
        {
            isPause = false;
            for (int i = 0; i < _coins.Count; i++)
            {
                var coin = _coins[i];
                coin.SetPause(isPause);
            }

            for (int i = 0; i < _downCoins.Count; i++)
            {
                var coin = _downCoins[i];
                coin.SetPause(isPause);
            }
        }

        public void SetVariants(List<Variant> other)
        {
            variants = other;
        }

        public IEnumerator Spawn()
        {
            yield return null;

            for (int i = 0; i < 100; i++)
            {
                GameObject coinInit = Instantiate(coinPrefab, glassAnchor.position, Quaternion.identity,
                    glass.transform);
                var coin = coinInit.GetComponent<Coin>();
                coin.isHeads = true;
                float randomOffsetX = Random.Range(-0.1f, 0.1f) * scaleFactor;
                float randomOffsetY = Random.Range(0f, 0.33f) * scaleFactor;
                float randomOffsetZ = Random.Range(-0.1f, 0.1f) * scaleFactor;
                coin.transform.localPosition += new Vector3(randomOffsetX, randomOffsetY, randomOffsetZ);
                _coins.Add(coin);
                AddLeftCupCoin(coin);
            }

            yield return null;
            coinPrefab.SetActive(false);
        }

        public void SpawnCoins()
        {
            glass.SetTrigger(Run);
            StartCoroutine(WaitNext(0.6f, () =>
            {
                SetStartPosition();
                SetHeadCoins();
                count++;
            }));
        }

        public void CheckResetCoins()
        {
            if (count == 6)
            {
                Finish();
            }
        }

        public void HeightLightUp()
        {
            for (int i = 0; i < _coins.Count; i++)
            {
                var coin = _coins[i];
                coin.HeightLightStop();
                if (coin.isHeads)
                {
                    coin.HeightLightStart();
                }
            }
        }

        public void HeightLightDown()
        {
            for (int i = 0; i < _coins.Count; i++)
            {
                var coin = _coins[i];
                coin.HeightLightStop();
                if (!coin.isHeads)
                {
                    coin.HeightLightStart();
                }
            }
        }

        public IEnumerator MoveUpCoinsToCup()
        {
            var headCoins = _coins.Where(coin => coin.isHeads).ToList();
            for (int coinIndex = 0; coinIndex < headCoins.Count; coinIndex++)
            {
                Coin coin = headCoins[coinIndex];
                coin.name = "Coun_MoveUp";
                coin.MoveTo(targetUp, scaleFactor);
                var localCoinIndex = coinIndex;
                coin.CoinMovement.onPlaceCoinEvent.AddListener(OnLeftCupReceiveCoin(localCoinIndex, coin));
                yield return new WaitForFixedUpdate();
            }

            yield return null;
        }

        public IEnumerator MoveDownCoinsToCup()
        {
            var downCoins = _coins.Where(coin => !coin.isHeads).ToList();
            for (int coinIndex = 0; coinIndex < downCoins.Count; coinIndex++)
            {
                Coin coin = downCoins[coinIndex];
                coin.MoveTo(targetDown, scaleFactor);
                coin.name = "Coun_MoveDown";
                var localCoinIndex = coinIndex;
                coin.CoinMovement.onPlaceCoinEvent.AddListener(OnRightCupReceiveCoin(localCoinIndex, coin));
                _downCoins.Add(coin);
                yield return new WaitForFixedUpdate();
            }

            _coins.RemoveAll(coin => !coin.isHeads);
            yield return null;
        }

        private UnityAction OnLeftCupReceiveCoin(int localCoinIndex, Coin coin)
        {
            return delegate
            {
                TryInvokeFirstCoinPutToCup(localCoinIndex);
                AddLeftCupCoin(coin);
            };
        }

        private UnityAction OnRightCupReceiveCoin(int localCoinIndex, Coin coin)
        {
            return delegate
            {
                TryInvokeFirstCoinPutToCup(localCoinIndex);
                AddRightCupCoin(coin);
            };
        }

        private void SetStartPosition()
        {
            for (int i = 0; i < _coins.Count; i++)
            {
                var coin = _coins[i];

                float randomOffsetX = Random.Range(-0.1f, 0.1f) * scaleFactor;
                float randomOffsetY = Random.Range(0f, 0.15f) * scaleFactor;
                float randomOffsetZ = Random.Range(-0.1f, 0.1f) * scaleFactor;

                Vector3 newPosition = new Vector3(randomOffsetX, randomOffsetY, randomOffsetZ) + spawnPoint.position;
                coin.transform.position = newPosition;
                coin.transform.SetParent(transform);
            }
        }

        private void SetHeadCoins()
        {
            leftCup = 0;
            StartCoroutine(HeadCoinsLoop());

            for (int i = 0; i < _coins.Count; i++)
            {
                var coin = _coins[i];
                coin.isHeads = i < variants[count].up;
                coin.FlipCoin();
            }
        }

        private IEnumerator HeadCoinsLoop()
        {
            for (int i = 0; i < _coins.Count; i++)
            {
                var coin = _coins[i];
                coin.gameObject.SetActive(true);
                coin.PlaceCoin(targetTable, () =>
                {
                    foreach (var ob in coinPileArray)
                    {
                        if (ob.activeSelf)
                        {
                            ob.SetActive(false);
                            break;
                        }
                    }
                }, true);
            }

            yield return null;
        }

        private void Finish()
        {
            Clear();
            StartCoroutine(Spawn());
        }

        private void Clear()
        {
            count = 0;
            foreach (var coin in _coins)
                Destroy(coin.gameObject);

            _coins.Clear();
            foreach (var coin in _downCoins)
                Destroy(coin.gameObject);

            _downCoins.Clear();
            leftCup = 0;
            rightCup = 0;
            foreach (GameObject coin in coinPileBrray)
                coin.SetActive(false);
        }

        private void AddLeftCupCoin(Coin coin)
        {
            coinPileArray[leftCup].SetActive(true);
            coin.CoinMovement.onPlaceCoinEvent.RemoveAllListeners();
            leftCup++;
        }

        private void AddRightCupCoin(Coin coin)
        {
            coinPileBrray[rightCup].SetActive(true);
            coin.CoinMovement.onPlaceCoinEvent.RemoveAllListeners();
            rightCup++;
        }

        private void TryInvokeFirstCoinPutToCup(int localCoinIndex)
        {
            if (localCoinIndex == 0)
                OnFirstCoinPutToCup?.Invoke();
        }

        private IEnumerator WaitNext(float time, Action action)
        {
            yield return new WaitForSeconds(time);
            action?.Invoke();
        }
    }
}
using System;
using UnityEngine;

namespace Lab_11_226
{
    public class Coin : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        private int _countRotation = 5;
        [SerializeField] private CoinMovement coinMovement;
        public bool isHeads;
        private static readonly int Flash = Animator.StringToHash("Flash");

        public CoinMovement CoinMovement => coinMovement;
        private void Awake()
        {
            coinMovement = gameObject.GetComponent<CoinMovement>();
        }

        public void FlipCoin()
        {
            if (isHeads)
            {
                gameObject.name = "Coun_Up";
                transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            }
            else
            {
                gameObject.name = "Coun_Down";
                transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
            }
        }

        public void HeightLightStart()
        {
            animator.SetBool(Flash, true);
        }

        public void HeightLightStop()
        {
            animator.SetBool(Flash, false);
        }

        public void PlaceCoin(Transform target, Action action, bool onTable = false)
        {
            coinMovement.FallCoin(target, action);
        }

        public void MoveTo(Transform target, float scaleFactor)
        {
            HeightLightStop();
            coinMovement.MoveCoin(target, scaleFactor);
        }

        public void SetPause(bool isPause)
        {
            coinMovement.SetPause(isPause);
            HeightLightStop();
        }
    }
}
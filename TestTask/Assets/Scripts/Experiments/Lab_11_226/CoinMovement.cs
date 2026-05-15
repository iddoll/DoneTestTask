using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Lab_11_226
{
    public class CoinMovement : MonoBehaviour
    {
        public float height = 100f;
        public float speed = 1.5f;
        public UnityEvent onPlaceCoinEvent;
        private bool _isPaused;

        public void MoveCoin(Transform target, float scaleFactor)
        {
            StartCoroutine(MoveTo(target, scaleFactor));
        }

        public void FallCoin(Transform target, Action action)
        {
            StartCoroutine(FallSimulation(target, action));
        }

        private IEnumerator MoveTo(Transform target, float scaleFactor)
        {
            yield return null;
            
            transform.SetParent(target);
            float elapsedTime = 0f;

            Vector3 startPosition = transform.localPosition;
            Vector3 targetPosition = target.localPosition / 1.2f;

            float randomOffsetX = Random.Range(-0.1f, 0.1f) * scaleFactor;
            float randomOffsetY = Random.Range(0, 0.1f) * scaleFactor;
            float randomOffsetZ = Random.Range(-0.1f, 0.1f) * scaleFactor;
            targetPosition += new Vector3(randomOffsetX, randomOffsetY, randomOffsetZ);
 
            while (elapsedTime < 1f)
            {
                if (!_isPaused)
                {
                    elapsedTime += Time.deltaTime * speed;

                    float t = Mathf.Lerp(0f, 1f, elapsedTime);
                    float parabolicT = t * t * (3f - 2f * t);

                    transform.localPosition =
                        Vector3.Lerp(startPosition, targetPosition, t) + Vector3.up * height * parabolicT;
                }

                yield return null;
            }
              
            elapsedTime = 0f;
            while (elapsedTime < 1f)
            {
                if (!_isPaused)
                {
                    Vector3 randomOffset = new Vector3(randomOffsetX, randomOffsetY, randomOffsetZ);

                    elapsedTime += Time.deltaTime * speed;

                    float t = Mathf.Lerp(0f, 1f, elapsedTime);
                    transform.position = Vector3.Lerp(transform.position, target.position + randomOffset, t);
                }

                yield return null;
            }
            
            transform.localPosition = target.position;

            gameObject.SetActive(false);
            transform.SetParent(target);
            onPlaceCoinEvent.Invoke();

            yield return null;
        }

        private IEnumerator FallSimulation(Transform target, Action action)
        {
            transform.SetParent(target);
            yield return new WaitForSeconds(Random.Range(0, 0.5f));

            float elapsedTime = 0f;
            Vector3 startPosition = transform.localPosition;
            Vector3 targetPosition = new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f));

            action?.Invoke();

            while (elapsedTime < 1f)
            {
                if (!_isPaused)
                {
                    elapsedTime += Time.deltaTime * speed;
                    float t = Mathf.Lerp(0f, 1f, elapsedTime);
                    transform.localPosition = Vector3.Lerp(startPosition, targetPosition, t);
                }
                yield return null;
            }

            transform.localPosition = targetPosition;
        }
        
        public void SetPause(bool date)
        {
            _isPaused = date;
        }
        
    }
}
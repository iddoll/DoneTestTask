using UnityEngine;

public class Particle_trigger : MonoBehaviour
{
    public ParticleSystem Sparks;

    void Awake()
    {
        Sparks.Stop();
    }

    void Start()
    {
        Sparks.Stop();
    }


    private void OnTriggerEnter(Collider other)

    {
        Sparks.Play();
    }

}

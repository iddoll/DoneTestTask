using UnityEngine;

public class Shoot : MonoBehaviour
{
    public float shootPower30 = 1.0f;
    public float shootPower45 = 1.0f;
    public float shootPower60 = 1.0f;

    public GameObject snaradPrefab30;
    public GameObject snaradPrefab45;
    public GameObject snaradPrefab60;
    public Transform refSnarad;

    public Vector3 degree30 = new Vector3(1f, 1.15f, 0f);
    public Vector3 degree45 = new Vector3(1f, 1.414f, 0f);
    public Vector3 degree60 = new Vector3(1f, 2f, 0f);

    private GameObject s1, s2, s3;

    public void ShootSnarad1()
    {
        s1 = Instantiate(snaradPrefab30, refSnarad.position, refSnarad.rotation);
        s1.GetComponent<Rigidbody>().AddForce(degree30.normalized * shootPower30, ForceMode.Impulse);
    }
    public void ShootSnarad2()
    {
        s2 = Instantiate(snaradPrefab45, refSnarad.position, refSnarad.rotation);
        s2.GetComponent<Rigidbody>().AddForce(degree45.normalized * shootPower45, ForceMode.Impulse);
    }
    public void ShootSnarad3()
    {
        s3 = Instantiate(snaradPrefab60, refSnarad.position, refSnarad.rotation);
        s3.GetComponent<Rigidbody>().AddForce(degree60.normalized * shootPower60, ForceMode.Impulse);
    }

    public void DestroySnarady()
    {
        Destroy(s1);
        Destroy(s2);
        Destroy(s3);
    }
}

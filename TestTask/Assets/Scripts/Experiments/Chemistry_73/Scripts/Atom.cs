using UnityEngine;
using System.Collections.Generic;

public class Atom : MonoBehaviour {

    public string elementName;

    public int protonCount;
    public Color32 protonColor = new Color32(0, 55, 255, 255);

    public int neutronCount;
    public Color32 neutronColor = new Color32(255, 28, 28, 255);

    public int[] electronConfiguration;
    public float electronShellSpacing = 3f;

    private bool prevSpinElectrons = true;

    private Transform nucleus;


    public void SetModel(Datum model, float scaleFactor)
    {
        elementName = model.name;
        protonCount = model.proton;
        neutronCount = model.neutron;
        electronConfiguration = model.orbits.ToArray();

        Generate(scaleFactor);
        transform.localScale = Vector3.zero;
    }
    private void Generate(float scaleFactor)
    {
        nucleus = transform.GetChild(0);

        int pc = protonCount;
        int nc = neutronCount;

        float scaleCount = scaleFactor;

        GenerateProton(transform.position);
        pc -= 1;

        for (int i = 1; i <= nc + pc; i++)
        {
            int nucleonsToCreate = int.Parse((Mathf.Pow(2f, i - 1f) * 5f).ToString());

            if (nc + pc >= nucleonsToCreate)
            {
                string s = CreateNucleons(nucleonsToCreate, scaleCount, pc, nc);
                scaleCount += scaleFactor;
                pc -= int.Parse(s.Split(',')[0]);
                nc -= int.Parse(s.Split(',')[1]);
            }
            else
            {
                string s = CreateNucleons(pc+nc, scaleCount, pc, nc);
                scaleCount += scaleFactor;

                pc -= int.Parse(s.Split(',')[0]);
                nc -= int.Parse(s.Split(',')[1]);
            }

        }

        //Electrons
        GameObject[] shellsGO = GenerateElectronShells();

        for (int loop = 0; loop < electronConfiguration.Length; loop++)
        {
            int numPoints = electronConfiguration[loop];

            for (int pointNum = 0; pointNum < numPoints; pointNum++)
            {
                float i = (float)(pointNum * 1.0) / numPoints;
                float angle = i * Mathf.PI * 2;
                float x = transform.position.x + (Mathf.Sin(angle) * (3*(loop+1)));
                float z = transform.position.z + (Mathf.Cos(angle) * (3*(loop+1)));
                Vector3 pos = new Vector3(x, transform.position.y, z);
                
                GenerateElectron(pos, electronShellSpacing * (loop + 1), loop+1, shellsGO[loop]);
            }
        }
    }

    private string CreateNucleons(int nPoints, float scale, int pcount, int ncount)
    {
        int npc = 0;
        int nnc = 0;

        float fPoints = (float)nPoints;

        Vector3[] points = new Vector3[nPoints];

        float inc = Mathf.PI * (3 - Mathf.Sqrt(5));
        float off = 2 / fPoints;

        for (int k = 0; k < nPoints; k++)
        {
            float y = k * off - 1 + (off / 2);
            float r = Mathf.Sqrt(1 - y * y);
            float phi = k * inc;

            points[k] = new Vector3(Mathf.Cos(phi) * r, y, Mathf.Sin(phi) * r);
        }
         
        foreach (Vector3 point in points)
        {
            if (pcount > 0 && ncount > 0)
            {
                int borc = Random.Range(1, 10);
                if (borc > 5 && pcount > 0)
                {
                    GameObject outerSphere = GenerateProton(transform.position);
                    outerSphere.transform.position += point * scale;
                    npc += 1;
                    pcount -= 1;
                }
                else if (borc < 6 && ncount > 0)
                {
                    GameObject outerSphere = GenerateNeutron(transform.position);
                    outerSphere.transform.position += point * scale;
                    nnc += 1;
                    ncount -= 1;
                }
            }

            else if (pcount > 0 && ncount == 0)
            {
                GameObject outerSphere = GenerateProton(transform.position);
                outerSphere.transform.position += point * scale;
                npc += 1;
                pcount -= 1;
            }

            else if (pcount == 0 && ncount > 0)
            {
                GameObject outerSphere = GenerateNeutron(transform.position);
                outerSphere.transform.position += point * scale;
                nnc += 1;
                ncount -= 1;
            }
        }

        return npc + "," + nnc;
    }

    public void DrawCircle(float radius, RingOrbit lr)
    {
        lr.tubeRadius = 0.01f;
        lr.outerRadius = radius;
        lr.tubeSegments = 16;
        lr.radialSegments = 360;
        lr.Draw();
    }

    public GameObject GenerateProton(Vector3 pos)
    {
        GameObject proton = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        proton.name = "Proton";
        proton.transform.position = pos;
        proton.GetComponent<Renderer>().material.SetColor("_Color", new Color32(0, 55, 255, 255));

        proton.transform.SetParent(nucleus);
        proton.transform.localScale =  Vector3.one * 0.1f;

        return proton;
    }

    public GameObject GenerateNeutron(Vector3 pos)
    {
        GameObject neutron = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        neutron.name = "Neutron";
        neutron.transform.position = pos;
        neutron.GetComponent<Renderer>().material.SetColor("_Color", new Color32(255, 28, 28, 255));

        neutron.transform.SetParent(nucleus);
        neutron.transform.localScale = Vector3.one * 0.1f;

        return neutron;
    }

    public GameObject[] GenerateElectronShells()
    {
        List<GameObject> shells = new List<GameObject>();

        for (int loop = 0; loop < electronConfiguration.Length; loop++)
        {
            GameObject shellParent = new GameObject();
            shellParent.name = "Shell " + (loop+1);
            shellParent.transform.parent = transform;
            shellParent.transform.localPosition = nucleus.localPosition;
            RotateByY rotateByY = shellParent.AddComponent<RotateByY>();
            rotateByY.centre = transform;
                
            GameObject shell = new GameObject();
            shell.name = "ShellChild " + (loop+1);
            shell.transform.parent = shellParent.transform;
            shell.transform.localPosition = nucleus.localPosition;
            RingOrbit lr = shell.AddComponent<RingOrbit>();
            DrawCircle(electronShellSpacing * (loop + 1), lr);
            shells.Add(shell);
            RotatorShell rot = shell.AddComponent<RotatorShell>();
            rot.SetVector(new Vector3(Random.Range(-360,360),0, Random.Range(-360,360)));
            rot.rotationSpeed = 80;
        }

        return shells.ToArray();
    }

    public void GenerateElectron(Vector3 pos, float r, int shell, GameObject shellGO)
    {
        GameObject electron = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        electron.name = "Electron";
        electron.transform.localScale = Vector3.one * 0.07f;
        electron.transform.position = pos;
        electron.transform.parent = shellGO.transform;
        electron.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
        Destroy(electron.GetComponent<SphereCollider>());
        electron.transform.position = (electron.transform.position - transform.position).normalized * r + transform.position;
    }
}
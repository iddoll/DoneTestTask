using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class AtomDataLoader : MonoBehaviour
{
    public static AtomDataLoader instance;
    public Transform pointForAtom;
    public string needAtom;
    public Atom atomPrefab;
    public GroupDisplayControlller groupDisplayControlller;
    public DisplayController infoView;
    public DisplayController allElements;
    private DisplayController atomView;
    private AtomData dataForAtoms = new AtomData();
    public string path;
    public float scaleFactor;
    
    private Atom atom;

    private void Start()
    {
        instance = this;
        dataForAtoms = LoadResourceTextfile(path);
        infoView.transform.localScale = Vector3.zero;
    }

    public void CreateModel(string name)
    {
        if (atomView == null)
        {
            atom = Instantiate(atomPrefab);
            var data = FindModel(name);
            if (!string.IsNullOrEmpty(name) && !name.Contains("AreaForClose"))
            {
                atom.SetModel(data,scaleFactor);
            }

            atom.transform.SetParent(transform);
            atom.transform.localPosition = pointForAtom.localPosition;
            atomView = atom.GetComponent<DisplayController>();

            float inputMin = 1f;
            float inputMax = 7f;
            float outputMin = 3f;
            float outputMax = 1f;

            float output = outputMin +
                           ((data.orbits.Count - inputMin) / (inputMax - inputMin)) * (outputMax - outputMin);

            atomView.ShowObject(output);
            infoView.GetComponentInChildren<TextMeshPro>().text = data.description;
            infoView.ShowObject();
            allElements.HideObject();
        }
        else
        {
            allElements.ShowObject();
            infoView.HideObject();
            atomView.HideObject(() =>
            {
                Destroy(atom.gameObject);
                atomView = null;
            });
        }
    }

    public void ShowGroup(string name)
    {
        groupDisplayControlller.ShowGroups(name);
    }

    private Datum FindModel(string atomName)
    {
       return dataForAtoms.data.FirstOrDefault(x => x.shortName == atomName);
    }

    public AtomData LoadResourceTextfile(string path)
    {
        string text = Resources.Load(path).ToString();

        var data = JsonUtility.FromJson<AtomData>(text);
        List<int> selectedOrbit = new();
        foreach (var item in data.data)
        {
            selectedOrbit.AddRange(item.orbits.Where(orbit => orbit == 0).ToList());
            item.orbits.RemoveAll(orbit => orbit == 0);
        }

        return data;
    }
}

[Serializable]
public class Datum
{
    public string name;
    public string shortName;
    public int proton;
    public int neutron;
    public int countOrbit;
    public List<int> orbits;
    public string description;
}

[Serializable]
public class AtomData
{
    public List<Datum> data;
}
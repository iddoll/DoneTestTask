using UnityEngine;

public class TouchOnElements : MonoBehaviour
{
    public bool isGroup = false;
    private string name;

    private void Start()
    {
        name = gameObject.name;
    }

    void OnMouseDown()
    {
        if(isGroup)
            AtomDataLoader.instance.ShowGroup(name);
        else
            AtomDataLoader.instance.CreateModel(name);
    }
}

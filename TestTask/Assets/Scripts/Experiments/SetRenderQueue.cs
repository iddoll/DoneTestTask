
using UnityEngine;

[AddComponentMenu("Rendering/SetRenderQueue")]

public class SetRenderQueue : MonoBehaviour
{

    [SerializeField]
    protected int[] m_queues = new int[] { 3000 };

    protected void Awake()
    {
        if (GetComponent<MeshRenderer>() != null)
        {
            Material[] materials = GetComponent<MeshRenderer>().materials;
            for (int i = 0; i < materials.Length && i < m_queues.Length; ++i)
            {
                materials[i].renderQueue = m_queues[i];
            }
        }
        else
        {
            SkinnedMeshRenderer[] meshRenderer = GetComponentsInChildren<SkinnedMeshRenderer>();
            int j = 0;
            foreach (var item in meshRenderer)
            {

                if (item != null)
                {
                    print(item.name);
                 
                   for (int i = 0; i < meshRenderer.Length && i < m_queues.Length; ++i)
                        {
                        item.material.renderQueue = m_queues[i];
                    }
                }
            }
        }
       
    }
}
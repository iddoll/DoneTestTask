using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class RingOrbit : MonoBehaviour
{
    public float outerRadius = 1f; 
    public float tubeRadius = 0.1f;
    public int radialSegments = 24; 
    public int tubeSegments = 16;

    public void Draw()
    {
        CreateTorus();
    }

    private void CreateTorus()
    {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().material = new Material(Shader.Find("Diffuse"));

        int verticesCount = (radialSegments + 1) * (tubeSegments + 1);
        int trianglesCount = radialSegments * tubeSegments * 6;

        Vector3[] vertices = new Vector3[verticesCount];
        int[] triangles = new int[trianglesCount];
        Vector2[] uv = new Vector2[verticesCount];
        Vector3[] normals = new Vector3[verticesCount];

        float _2pi = Mathf.PI * 2f;

        for (int seg = 0; seg <= radialSegments; seg++)
        {
            int currSeg = seg == radialSegments ? 0 : seg;

            float t1 = (float)currSeg / radialSegments * _2pi;
            Vector3 r1 = new Vector3(Mathf.Cos(t1) * outerRadius, 0f, Mathf.Sin(t1) * outerRadius);

            for (int side = 0; side <= tubeSegments; side++)
            {
                int currSide = side == tubeSegments ? 0 : side;

                float t2 = (float)currSide / tubeSegments * _2pi;
                Vector3 r2 = Quaternion.AngleAxis(-t1 * Mathf.Rad2Deg, Vector3.up) * new Vector3(Mathf.Sin(t2) * tubeRadius, Mathf.Cos(t2) * tubeRadius);

                vertices[side + seg * (tubeSegments + 1)] = r1 + r2;
                uv[side + seg * (tubeSegments + 1)] = new Vector2((float)currSeg / radialSegments, (float)currSide / tubeSegments);
                normals[side + seg * (tubeSegments + 1)] = (vertices[side + seg * (tubeSegments + 1)] - r1).normalized;
            }
        }

        for (int seg = 0; seg < radialSegments; seg++)
        {
            for (int side = 0; side < tubeSegments; side++)
            {
                int current = side + seg * (tubeSegments + 1);
                int next = side + (seg + 1) * (tubeSegments + 1);

                triangles[(side + seg * tubeSegments) * 6 + 0] = current;
                triangles[(side + seg * tubeSegments) * 6 + 1] = next;
                triangles[(side + seg * tubeSegments) * 6 + 2] = current + 1;
                triangles[(side + seg * tubeSegments) * 6 + 3] = current + 1;
                triangles[(side + seg * tubeSegments) * 6 + 4] = next;
                triangles[(side + seg * tubeSegments) * 6 + 5] = next + 1;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.normals = normals;
        mesh.RecalculateNormals();
    }
}

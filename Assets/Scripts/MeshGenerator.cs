using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    delegate Vector3 ComputeVector3FromKxKz(float kx, float kz);
    private MeshFilter m_mf;
    private Mesh mMesh;
    // Start is called before the first frame update
    void Start()
    {
        m_mf = GetComponent<MeshFilter>();

        //Plane
        //m_mf.sharedMesh = WrapNormalizedPlane(100, 100, (kx, ky) => new Vector3(kx, 0, ky));

        // Polygone Regulier
        mMesh = WrapNormalizePlaneQuads(1, 1, (kx, ky) => new Vector3(kx, 0, ky));
        m_mf.sharedMesh = mMesh;
        Debug.Log(MeshDisplayInfo.ExportMeshCSV(m_mf.sharedMesh));

        int[] indices = mMesh.GetIndices(0);
        Vector3[] vertices = mMesh.vertices;
        HalfEdgeMesh convert = HalfEdgeMesh.fromVertexFace(vertices, indices);
        Debug.Log("LESSGOOO");
        Debug.Log(MeshDisplayInfo.ExportMeshCSV(convert.edges));
        m_mf.sharedMesh = convert.ToMesh();

        //ça marche bieng
        // TODO: Tester de manière unitaire
        convert.Catmull_Clark();
        m_mf.sharedMesh = convert.ToMesh();
        Debug.Log(MeshDisplayInfo.ExportMeshCSV(m_mf.sharedMesh));
    }

    private Mesh CreateTriangle()
    {
        Mesh newMesh = new Mesh();
        Vector3[] vertices = new Vector3[3];
        int[] triangles = new int[1 * 3];

        vertices[0] = Vector3.right;
        vertices[1] = Vector3.up;
        vertices[2] = Vector3.forward;

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;

        newMesh.vertices = vertices;
        newMesh.triangles = triangles;

        newMesh.RecalculateBounds();

        return newMesh;
    }

    private Mesh CreateQuad(Vector3 size)
    {
        Vector3 halfSize = size * 0.5f;

        Mesh newMesh = new Mesh();
        Vector3[] vertices = new Vector3[4];
        int[] triangles = new int[2 * 3];

        vertices[0] = new Vector3(-halfSize.x, 0, -halfSize.z);
        vertices[1] = new Vector3(-halfSize.x, 0, halfSize.z);
        vertices[2] = new Vector3(halfSize.x, 0, -halfSize.z);
        vertices[3] = new Vector3(halfSize.x, 0, halfSize.z);


        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;

        triangles[3] = 1;
        triangles[4] = 3;
        triangles[5] = 2;

        newMesh.vertices = vertices;
        newMesh.triangles = triangles;

        newMesh.RecalculateBounds();

        return newMesh;
    }

    private Mesh CreateStripXZ(Vector3 size, int nSegments)
    {
        Vector3 halfSize = size * 0.5f;

        Mesh newMesh = new Mesh();
        newMesh.name = "strip";
        Vector3[] vertices = new Vector3[(nSegments + 1) * 2];
        int[] triangles = new int[nSegments * 2 * 3];


        int mid = vertices.Length / 2;
        for (int i = 0; i < nSegments + 1; i++)
        {
            float k = (float)i / nSegments;
            float y = Mathf.Sin(k * Mathf.PI * 2 * 3);
            vertices[i] = new Vector3(Mathf.Lerp(-halfSize.x, halfSize.x, k), y, -halfSize.z);
            vertices[nSegments + 1 + i] = new Vector3(Mathf.Lerp(-halfSize.x, halfSize.x, k), y, halfSize.z);
        }

        int index = 0;
        for (int i = 0; i < nSegments; i++)
        {
            triangles[index++] = i;
            triangles[index++] = i + nSegments + 1;
            triangles[index++] = i + nSegments + 2;

            triangles[index++] = i;
            triangles[index++] = i + nSegments + 2;
            triangles[index++] = i + 1;
        }

        newMesh.vertices = vertices;
        newMesh.triangles = triangles;

        newMesh.RecalculateBounds();

        return newMesh;
    }

    private Mesh CreatePlane(Vector3 size, int nSegmentsX, int nSegmentsZ)
    {
        Vector3 halfSize = size * 0.5f;

        Mesh newMesh = new Mesh();
        newMesh.name = "Plane";
        Vector3[] vertices = new Vector3[(nSegmentsX + 1) * (nSegmentsZ + 1)];
        int[] triangles = new int[nSegmentsX * nSegmentsZ * 6];

        int index = 0;
        for (int z = 0; z <= nSegmentsZ; z++)
        {
            float kz = (float)z / nSegmentsZ;
            for (int x = 0; x <= nSegmentsX; x++)
            {
                float kx = (float)x / nSegmentsX;
                vertices[index++] = new Vector3(Mathf.Lerp(-halfSize.x, halfSize.x, kx), 0, Mathf.Lerp(-halfSize.z, halfSize.z, kz));
            }
        }

        int ti = 0, vi = 0;
        for (int z = 0; z < nSegmentsZ; z++)
        {
            for (int x = 0; x < nSegmentsX; x++)
            {
                triangles[ti++] = vi;
                triangles[ti++] = vi + nSegmentsX + 1;
                triangles[ti++] = vi + 1;

                triangles[ti++] = vi + 1;
                triangles[ti++] = vi + nSegmentsX + 1;
                triangles[ti++] = vi + nSegmentsX + 2;
                vi++;
            }
            vi++;
        }

        newMesh.vertices = vertices;
        newMesh.triangles = triangles;

        newMesh.RecalculateBounds();

        return newMesh;
    }

    private Mesh WrapNormalizedPlane(int nSegmentsX, int nSegmentsZ, ComputeVector3FromKxKz ComputePosition)
    {

        Mesh newMesh = new Mesh();
        newMesh.name = "Plane";
        Vector3[] vertices = new Vector3[(nSegmentsX + 1) * (nSegmentsZ + 1)];
        int[] triangles = new int[nSegmentsX * nSegmentsZ * 6];

        int index = 0;
        for (int z = 0; z <= nSegmentsZ; z++)
        {
            float kz = (float)z / nSegmentsZ;
            for (int x = 0; x <= nSegmentsX; x++)
            {
                float kx = (float)x / nSegmentsX;
                vertices[index++] = ComputePosition(kx, kz);
            }
        }

        int ti = 0, vi = 0;
        for (int z = 0; z < nSegmentsZ; z++)
        {
            for (int x = 0; x < nSegmentsX; x++)
            {
                triangles[ti++] = vi;
                triangles[ti++] = vi + nSegmentsX + 1;
                triangles[ti++] = vi + 1;

                triangles[ti++] = vi + 1;
                triangles[ti++] = vi + nSegmentsX + 1;
                triangles[ti++] = vi + nSegmentsX + 2;
                vi++;
            }
            vi++;
        }

        newMesh.vertices = vertices;
        newMesh.triangles = triangles;

        newMesh.RecalculateBounds();

        return newMesh;
    }

    Mesh WrapNormalizePlaneQuads(int nSegmentsX, int nSegmentsZ, ComputeVector3FromKxKz computePosition)
    {
        Mesh newMesh = new Mesh();
        newMesh.name = "plane";
        Vector3[] vertices = new Vector3[(nSegmentsX + 1) * (nSegmentsZ + 1)];
        int[] quads = new int[nSegmentsX * nSegmentsZ * 4];

        //Vertices
        int index = 0;
        for (int i = 0; i < nSegmentsX + 1; i++)
        {
            float kX = (float)i / nSegmentsX;
            for (int j = 0; j < nSegmentsZ + 1; j++)
            {
                float kZ = (float)j / nSegmentsZ;
                vertices[index++] = computePosition(kX, kZ);
            }
        }

        //Triangles
        index = 0;
        //double boucle également
        for (int i = 0; i < nSegmentsX; i++)
        {
            for (int j = 0; j < nSegmentsZ; j++)
            {
                quads[index++] = i * (nSegmentsZ + 1) + j;
                quads[index++] = i * (nSegmentsZ + 1) + j + 1;
                quads[index++] = (i + 1) * (nSegmentsZ + 1) + j + 1;
                quads[index++] = (i + 1) * (nSegmentsZ + 1) + j;

            }
        }

        newMesh.vertices = vertices;
        newMesh.SetIndices(quads, MeshTopology.Quads, 0);
        newMesh.RecalculateBounds();
        newMesh.RecalculateNormals();
        return newMesh;
    }

    Mesh CreateRegularPolygonXZQuads(float radius, int nQuads)
    {
        Mesh newMesh = new Mesh();
        newMesh.name = "RegularPolygonQuads";

        Vector3[] vertices = new Vector3[nQuads * 2 + 1];
        int[] quads = new int[nQuads * 4];

        //On met en place le centre
        vertices[0] = Vector3.zero;

        //Vertices
        for (int i = 0; i < nQuads; i++)
        {
            float x = radius * Mathf.Sin((2 * Mathf.PI * i) / nQuads);
            float z = radius * Mathf.Cos((2 * Mathf.PI * i) / nQuads);
            Vector3 centerVertice = new Vector3(x, 0,z);
            x = radius * Mathf.Sin((2 * Mathf.PI * (i + 1)) / nQuads);
            z = radius * Mathf.Cos((2 * Mathf.PI * (i + 1)) / nQuads);
            Vector3 nextVertice = new Vector3(x, 0, z);

            //On calcule les points entre centerVertice et la prochaine vertice
            Vector3 midNextVertice = Vector3.Lerp(centerVertice, nextVertice, .5f);

            //On les ajoute à la liste
            vertices[i * 2 + 1] = centerVertice;
            vertices[i * 2 + 2] = midNextVertice;
        }

        //Quads
        int index = 0;
        for (int i = 0; i < nQuads; i++)
        {
            quads[index++] = 0; //On met le premier au centre
            quads[index++] = (i == 0)?nQuads*2:2*i; // ???
            quads[index++] = i * 2 + 1;
            quads[index++] = i * 2 + 2; 
            
        }

        newMesh.vertices = vertices;
        newMesh.SetIndices(quads, MeshTopology.Quads, 0);
        newMesh.RecalculateBounds();
        newMesh.RecalculateNormals();
        return newMesh;
    }

    // Update is called once per frame
    void Update()
    {

    }
}

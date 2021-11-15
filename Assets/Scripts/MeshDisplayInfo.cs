using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(MeshFilter))]
public class MeshDisplayInfo : MonoBehaviour
{
    MeshFilter m_Mf;
    [Header("Edges")]
    [SerializeField] bool m_DisplayEdges;
    [SerializeField] int m_NMaxEdges;

    [Header("Normals")]
    [SerializeField] bool m_DisplayNormals;
    [SerializeField] int m_NMaxNormals;
    [SerializeField] float m_NormalScaleFactor;

    [Header("Vertices")]
    [SerializeField] bool m_DisplayVertices;
    [SerializeField] int m_NMaxVertices;

    [Header("Faces")]
    [SerializeField] bool m_DisplayFaces;
    [SerializeField] int m_NMaxFaces;

    private void Awake()
    {
        m_Mf = GetComponent<MeshFilter>();
    }

    private void OnDrawGizmos()
    {
        if (!(m_Mf && m_Mf.sharedMesh))
            return;

        Vector3[] vertices = m_Mf.sharedMesh.vertices;

        //EDGES
        if (m_DisplayEdges || m_DisplayFaces)
        {
            int[] quads = m_Mf.sharedMesh.GetIndices(0);

            int index1, index2, index3, index4, index = 0;
            Vector3 pt1, pt2, pt3, pt4;

            for (int i = 0; i < Mathf.Min(quads.Length / 4, Mathf.Max(m_NMaxEdges, m_NMaxFaces)); i++)
            {
                index1 = quads[index++];
                index2 = quads[index++];
                index3 = quads[index++];
                index4 = quads[index++];

                //référentiel local
                pt1 = transform.TransformPoint(vertices[index1]);
                pt2 = transform.TransformPoint(vertices[index2]);
                pt3 = transform.TransformPoint(vertices[index3]);
                pt4 = transform.TransformPoint(vertices[index4]);

                if (m_DisplayEdges && i < m_NMaxEdges)
                {
                    Gizmos.color = Color.black;
                    //-> référenciel global
                    Gizmos.DrawLine(pt1, pt2);
                    Gizmos.DrawLine(pt3, pt2);
                    Gizmos.DrawLine(pt1, pt4);
                    Gizmos.DrawLine(pt4, pt3);
                }
                Vector3 faceCenter;
                if (m_DisplayFaces && i < m_NMaxFaces)
                {
                    string str = $"{i}:{index1},{index2},{index3},{index4}"; //string.Format("{0}:{1},{2},{3},{4}", i, index1, index2, index3, index4)
                    faceCenter = (pt1 + pt2 + pt3 + pt4) * 0.25f;
                    Gizmos.color = Color.blue;
                    Gizmos.DrawSphere(faceCenter, 01f);
                    Handles.Label(faceCenter, str);
                }
            }
        }

        //NORMALS
        if (m_DisplayNormals)
        {
            Vector3[] normals = m_Mf.sharedMesh.normals;

            Gizmos.color = Color.white;

            Vector3 pos, normal;

            for (int i = 0; i < Mathf.Min(normals.Length, m_NMaxNormals); i++)
            {
                pos = transform.TransformPoint(vertices[i]);
                normal = transform.TransformPoint(normals[i]);

                //-> référenciel global
                Gizmos.DrawLine(pos, pos + normal * m_NormalScaleFactor);
            }
        }

        //VERTICES
        if (m_DisplayVertices)
        {
            GUIStyle m_Style = new GUIStyle();
            m_Style.fontSize = 16;
            m_Style.normal.textColor = Color.red;

            Gizmos.color = Color.red;

            Vector3 pos;

            for (int i = 0; i < Mathf.Min(vertices.Length, m_NMaxVertices); i++)
            {
                pos = transform.TransformPoint(vertices[i]);

                //-> référenciel global
                Gizmos.DrawSphere(pos, .01f);
                Handles.Label(pos, i.ToString(), m_Style);
            }
        }

    }


    public static string ExportMeshCSV(Mesh mesh)
    {
        //VertexIndex   VertexPosX  VertexPosY  VertexPosZ  QuadIndex   QuadVertexIndex1    QuadVertexIndex2    QuadVertexIndex3    QuadVertexIndex4        List<string> strings = new List<string>();
        List<string> strings = new List<string>();
        strings.Add("VertexIndex\tVertexPosX\tVertexPosY\tVertexPosZ\tQuadIndex\tQuadVertexIndex1\tQuadVertexIndex2\tQuadVertexIndex3\tQuadVertexIndex4");
        Vector3[] vertices = mesh.vertices;
        int[] quads = mesh.GetIndices(0);

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 pos = vertices[i];
            strings.Add($"{i}\t{pos.x.ToString("N02")}\t{pos.y.ToString("N02")}\t{pos.z.ToString("N02")}\t");
        }

        int index = 0;
        for (int i = 0; i < quads.Length / 4; i++)
        {
            string tmpStr = $"{i}\t{quads[index++]}\t{quads[index++]}\t{quads[index++]}\t{quads[index++]}";
            if (i + 1 < strings.Count) strings[i + 1] += tmpStr;
            else strings.Add("\t\t\t\t"+tmpStr);
        }


        return string.Join("\n", strings);
    }

    public static string ExportMeshCSV(List<HalfEdge> mesh)
    {
        //VertexIndex   VertexPosX  VertexPosY  VertexPosZ  QuadIndex   QuadVertexIndex1    QuadVertexIndex2    QuadVertexIndex3    QuadVertexIndex4        List<string> strings = new List<string>();
        List<string> strings = new List<string>();
        strings.Add("HalfEdgeIndice\tSourcePosX\tSourcePosY\tSourcePosZ\tPrevEdgeIndice\tNextEdgeIndice\tTwinEdgeIndice\tFaceIndice");

        for(int i = 0; i < mesh.Count; i++)
        {
            HalfEdge edge = mesh[i];
            strings.Add($"{edge.index}\t{edge.sourceVertex.vertex.x}\t{edge.sourceVertex.vertex.y}\t{edge.sourceVertex.vertex.z}\t{edge.prevEdge.index}\t{edge.nextEdge.index}\t{(edge.twinEdge != null? edge.twinEdge.index:-1)}\t{edge.face.face.index}");
        }

        return string.Join("\n", strings);
    }
}
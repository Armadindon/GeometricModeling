using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HalfEdge
{

    public int index;

    public Vertex sourceVertex;
    public HalfEdge prevEdge;
    public HalfEdge nextEdge;

    public HalfEdge twinEdge;

    public Face face;

    public HalfEdge(int index, Vertex sourceVertex, HalfEdge prevEdge, HalfEdge nextEdge, HalfEdge twinEdge, Face face)
    {
        this.index = index;
        this.sourceVertex = sourceVertex;
        this.prevEdge = prevEdge;
        this.nextEdge = nextEdge;
        this.twinEdge = twinEdge;
        this.face = face;
    }
    
    public static List<HalfEdge> VertexFaceToHalfEdge(Vector3[] vertices, int[] faces)
    {
        int index = 0;
        List<HalfEdge> result = new List<HalfEdge>();
        Dictionary<KeyValuePair<int, int>, HalfEdge> twin = new Dictionary<KeyValuePair<int, int>, HalfEdge>();

        for(int i = 0; i < faces.Length;)
        {
            //Une face = un quad
            //On gènère 4 HalfEdge depuis un quad
            int v1Index = faces[i];
            Vertex v1 = new Vertex(vertices[faces[i++]]);
            HalfEdge p1 = new HalfEdge(index++, v1, null, null, null, null);
            Face face = new Face(p1);
            p1.face = face;

            int v2Index = faces[i];
            Vertex v2 = new Vertex(vertices[faces[i++]]);
            HalfEdge p2 = new HalfEdge(index++, v2, p1, null, null, face);
            p1.nextEdge = p2;

            int v3Index = faces[i];
            Vertex v3 = new Vertex(vertices[faces[i++]]);
            HalfEdge p3 = new HalfEdge(index++, v3, p2, null, null, face);
            p2.nextEdge = p3;

            int v4Index = faces[i];
            Vertex v4 = new Vertex(vertices[faces[i++]]);
            HalfEdge p4 = new HalfEdge(index++, v4, p3, p1, null, face);
            p3.nextEdge = p4;
            p1.prevEdge = p4;

            //TODO: Les TwinEdge se basent sur la face connexe
            HalfEdge twinEdge;
            if (twin.ContainsKey(new KeyValuePair<int, int>(v1Index, v2Index)))
            {
                twinEdge = twin[new KeyValuePair<int, int>(v1Index, v2Index)];
                twinEdge.twinEdge = p1;
                p1.twinEdge = twinEdge;
            }
            else twin.Add(new KeyValuePair<int, int>(v2Index, v1Index), p1);

            if (twin.ContainsKey(new KeyValuePair<int, int>(v2Index, v3Index)))
            {
                twinEdge = twin[new KeyValuePair<int, int>(v2Index, v3Index)];
                twinEdge.twinEdge = p2;
                p2.twinEdge = twinEdge;
            }
            else twin.Add(new KeyValuePair<int, int>(v3Index, v2Index), p2);

            if (twin.ContainsKey(new KeyValuePair<int, int>(v3Index, v4Index)))
            {
                twinEdge = twin[new KeyValuePair<int, int>(v3Index, v4Index)];
                twinEdge.twinEdge = p3;
                p3.twinEdge = twinEdge;
            }
            else twin.Add(new KeyValuePair<int, int>(v4Index, v3Index), p3);

            if (twin.ContainsKey(new KeyValuePair<int, int>(v4Index, v1Index)))
            {
                twinEdge = twin[new KeyValuePair<int, int>(v4Index, v1Index)];
                twinEdge.twinEdge = p4;
                p4.twinEdge = twinEdge;
            }
            else twin.Add(new KeyValuePair<int, int>(v1Index, v4Index), p4);

            //On ajoute le tout à la liste
            result.Add(p1);
            result.Add(p2);
            result.Add(p3);
            result.Add(p4);
        }

        return result;
    }

    private static List<Face> extractFaces(List<HalfEdge> edges)
    {
        return edges.Select(x => x.face).Distinct().ToList();
    }

    private static List<HalfEdge> FindAllEdgeUsingVertice(List<HalfEdge> edges, Vertex v)
    {
        return edges.Where<HalfEdge>(x => (x.sourceVertex == v) && (x.nextEdge.sourceVertex == v)).ToList();
    }

    public static Mesh HalfEdgeToMesh(List<HalfEdge> edges)
    {
        Mesh newMesh = new Mesh();
        newMesh.name = "HalfEdgeConversion";
        List<Vector3> vertices = new List<Vector3>();
        List<int> quads = new List<int>();

        List<Face> faces = extractFaces(edges);

        int index = 0;

        foreach(Face f in faces){

            HalfEdge edge1 = f.face;
            HalfEdge edge2 = edge1.nextEdge;
            HalfEdge edge3 = edge1.nextEdge.nextEdge;
            HalfEdge edge4 = edge1.prevEdge;

            Vector3 vertex1 = edge1.sourceVertex.vertex;
            Vector3 vertex2 = edge2.sourceVertex.vertex;
            Vector3 vertex3 = edge3.sourceVertex.vertex;
            Vector3 vertex4 = edge4.sourceVertex.vertex;

            quads.Add(vertices.Contains(vertex1)?vertices.IndexOf(vertex1): index++);
            if (!vertices.Contains(vertex1)) vertices.Add(vertex1);

            quads.Add(vertices.Contains(vertex2) ? vertices.IndexOf(vertex2) : index++);
            if (!vertices.Contains(vertex2)) vertices.Add(vertex2);

            quads.Add(vertices.Contains(vertex3) ? vertices.IndexOf(vertex3) : index++);
            if (!vertices.Contains(vertex3)) vertices.Add(vertex3);

            quads.Add(vertices.Contains(vertex4) ? vertices.IndexOf(vertex4) : index++);
            if (!vertices.Contains(vertex4)) vertices.Add(vertex4);
        }

        newMesh.vertices = vertices.ToArray();
        newMesh.SetIndices(quads.ToArray(), MeshTopology.Quads, 0);
        newMesh.RecalculateBounds();
        newMesh.RecalculateNormals();

        return newMesh;
    }


    public static Vector3 FaceAverage(Face f)
    {
        Vector3 avg = new Vector3();

        HalfEdge current = f.face;
        avg += current.sourceVertex.vertex;
        while (current != f.face)
        {
            avg += current.sourceVertex.vertex;
            current = current.nextEdge;
        }

        return avg / 4;
    }

    public static void Catmull_Clark(List<HalfEdge> mesh)
    {
        //On récupère toute les faces
        List<Face> faces = extractFaces(mesh);
        Dictionary<Face, Vertex> facePoints = new Dictionary<Face, Vertex>();

        //On calcule les Face points pour chaque Face
        for (int i = 0; i < faces.Count; i++) facePoints[faces[i]] = new Vertex(FaceAverage(faces[i]));

        for (int i = 0; i < faces.Count; i++)
        {

            // On parcours les Edge et on crée les edgePoints

            // On met à jour la position des verticles avec les VertexPoints

            //On split l'Edge avec l'edgePoint comme paramètre

            // On split la face, avec le facepoint comme paramètre

        }
    }

    public static void SplitFace(Face f, Vertex v)
    {
        HalfEdge current = f.face;
        //On calcule le Edge point
        
    }

    public static void SplitEdge(HalfEdge e, Vertex v)
    {
        
    }

}

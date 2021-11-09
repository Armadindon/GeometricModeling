using System.Collections;
using System.Collections.Generic;
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

        for(int i = 0; i < faces.Length;)
        {
            //Une face = un quad
            //On g�n�re 4 HalfEdge depuis un quad
            Vertex v1 = new Vertex(vertices[faces[i++]]);
            HalfEdge p1 = new HalfEdge(index++, v1, null, null, null, null);
            Face face = new Face(p1);
            p1.face = face;

            Vertex v2 = new Vertex(vertices[faces[i++]]);
            HalfEdge p2 = new HalfEdge(index++, v2, p1, null, null, face);
            p1.nextEdge = p2;

            Vertex v3 = new Vertex(vertices[faces[i++]]);
            HalfEdge p3 = new HalfEdge(index++, v3, p2, null, null, face);
            p2.nextEdge = p3;

            Vertex v4 = new Vertex(vertices[faces[i++]]);
            HalfEdge p4 = new HalfEdge(index++, v4, p3, p1, null, face);
            p3.nextEdge = p4;
            p1.prevEdge = p4;

            //On g�n�re les twin edge
            HalfEdge t1 = new HalfEdge(index++, v2, null, null, p1, face);
            HalfEdge t2 = new HalfEdge(index++, v3, null, t1, p2, face);
            HalfEdge t3 = new HalfEdge(index++, v4, null, t2, p3, face);
            HalfEdge t4 = new HalfEdge(index++, v1, t1, t3, p4, face);
            t1.prevEdge = t2;
            t1.nextEdge = t4;
            t2.prevEdge = t3;
            t3.prevEdge = t4;

            p1.twinEdge = t1;
            p2.twinEdge = t2;
            p3.twinEdge = t3;
            p4.twinEdge = t4;


            //On ajoute le tout � la liste
            result.Add(p1);
            result.Add(p2);
            result.Add(p3);
            result.Add(p4);

            result.Add(t1);
            result.Add(t2);
            result.Add(t3);
            result.Add(t4);
        }

        return result;
    }
}
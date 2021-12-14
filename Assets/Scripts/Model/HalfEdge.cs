using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Diagnostics.DebuggerDisplay("{ToString()}")]
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

    override public string ToString()
    {
        return prevEdge.index + " -> " + index + " -> " + nextEdge.index;
    }

}

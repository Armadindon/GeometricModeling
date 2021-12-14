using UnityEngine;
using System;

[System.Diagnostics.DebuggerDisplay("{ToString()}")]
public class Vertex
{
    public int index;
    public Vector3 vertex;

    public Vertex(int index, Vector3 vertex)
    {
        this.index = index;
        this.vertex = vertex;
    }

    public override bool Equals(System.Object obj)
    {
        //Check for null and compare run-time types.
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
        {
            return false;
        }
        else
        {
            Vertex v = (Vertex)obj;
            return (index == v.index);
        }
    }
    override public string ToString()
    {
        return "" + index;
    }
}
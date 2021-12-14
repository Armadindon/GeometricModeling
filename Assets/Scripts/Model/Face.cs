[System.Diagnostics.DebuggerDisplay("{ToString()}")]
public class Face
{
    public int index;
    public HalfEdge face;

    public Face(int index, HalfEdge face)
    {
        this.face = face;
    }

    override public string ToString()
    {
        return "" + index;
    }
}
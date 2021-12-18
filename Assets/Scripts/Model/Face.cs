[System.Diagnostics.DebuggerDisplay("{ToString()}")]
public class Face
{
    public int index;
    public HalfEdge face;

    public Face(int index, HalfEdge face)
    {
        this.index = index;
        this.face = face;
    }

    override public string ToString()
    {
        return "" + index;
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
            Face f = (Face)obj;
            return (index == f.index);
        }
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
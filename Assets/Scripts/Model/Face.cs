public class Face
{
    public int index;
    public HalfEdge face;

    public Face(int index, HalfEdge face)
    {
        this.face = face;
    }
}
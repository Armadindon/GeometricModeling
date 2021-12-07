using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class HalfEdgeMesh
{
    public List<HalfEdge> edges;
    public List<Vertex> vertices;
    public List<Face> faces;

    public HalfEdgeMesh()
    {
        edges = new List<HalfEdge>();
        vertices = new List<Vertex>();
        faces = new List<Face>();
    }

    public HalfEdgeMesh(List<HalfEdge> edges, List<Vertex> vertices, List<Face> faces)
    {
        this.edges = edges;
        this.vertices = vertices;
        this.faces = faces;
    }

    /**
     * FONCTION UTILITAIRES
     * */
    #region
    public List<HalfEdge> FindAllEdgeUsingVertice(Vertex v)
    {
        return edges.Where<HalfEdge>(x => (x.sourceVertex == v) && (x.nextEdge.sourceVertex == v)).ToList();
    }


    public int computeValenceOfPoint(Vertex v)
    {
        List<HalfEdge> edgesUsingVertex = FindAllEdgeUsingVertice(v);
        List<HalfEdge> edgesList = filterTwinsFromList(edgesUsingVertex);
        return edgesList.Count;
    }

    public static List<HalfEdge> filterTwinsFromList(List<HalfEdge> edges)
    {
        List<HalfEdge> copy = new List<HalfEdge>(edges);
        for (int i = 0; i < edges.Count; i++) copy.Remove(edges[i].twinEdge);
        return copy;
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

    public static int nbEdgeInFace(Face f)
    {
        int nb = 0;
        HalfEdge edge = f.face;
        do
        {
            nb++;
            edge = edge.nextEdge;
        } while(edge != f.face);

        return nb;
    }
    #endregion

     /**
     * FONCTION DE CONVERSIONS
     * */
    #region
    public static HalfEdgeMesh fromVertexFace(Vector3[] vertices, int[] faces)
    {
        int index = 0;
        int vertexIndice = 0;
        int facesIndex = 0;
        List<HalfEdge> result = new List<HalfEdge>();
        List<Face> facesList = new List<Face>();
        Dictionary<int, Vertex> verticesDict = new Dictionary<int, Vertex>();
        Dictionary<KeyValuePair<int, int>, HalfEdge> twin = new Dictionary<KeyValuePair<int, int>, HalfEdge>();

        //TODO: Faire en sorte de ne pas re-creer les vertex à chaque fois
        for (int i = 0; i < faces.Length;)
        {
            //Une face = un quad
            //On gènère 4 HalfEdge depuis un quad
            int v1Index = faces[i];
            Vertex v1;
            if (verticesDict.ContainsKey(v1Index))
            {
                v1 = verticesDict[v1Index];
                i++;
            }
            else v1 = new Vertex(vertexIndice++, vertices[faces[i++]]);
            HalfEdge p1 = new HalfEdge(index++, v1, null, null, null, null);
            Face face = new Face(facesIndex++, p1);
            facesList.Add(face);
            p1.face = face;

            int v2Index = faces[i];
            Vertex v2;
            if (verticesDict.ContainsKey(v2Index))
            {
                v2 = verticesDict[v2Index];
                i++;
            }
            else v2 = new Vertex(vertexIndice++, vertices[faces[i++]]);
            HalfEdge p2 = new HalfEdge(index++, v2, p1, null, null, face);
            p1.nextEdge = p2;

            int v3Index = faces[i];
            Vertex v3;
            if (verticesDict.ContainsKey(v3Index))
            {
                v3 = verticesDict[v3Index];
                i++;
            }
            else v3 = new Vertex(vertexIndice++, vertices[faces[i++]]);
            HalfEdge p3 = new HalfEdge(index++, v3, p2, null, null, face);
            p2.nextEdge = p3;

            int v4Index = faces[i];
            Vertex v4;
            if (verticesDict.ContainsKey(v4Index))
            {
                v4 = verticesDict[v4Index];
                i++;
            }
            else v4 = new Vertex(vertexIndice++, vertices[faces[i++]]);
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

        return new HalfEdgeMesh(result, verticesDict.Values.OrderBy(v => v.index).ToList(), facesList);
    }

    //TODO: Re-Work to work with vertex indices
    public Mesh ToMesh()
    {
        Mesh newMesh = new Mesh();
        newMesh.name = "HalfEdgeConversion";
        List<Vector3> vertices = new List<Vector3>();
        List<int> quads = new List<int>();

        int index = 0;

        //TODO: Faire la gestion des index des vertices
        foreach (Face f in faces)
        {

            HalfEdge edge1 = f.face;
            HalfEdge edge2 = edge1.nextEdge;
            HalfEdge edge3 = edge1.nextEdge.nextEdge;
            HalfEdge edge4 = edge1.prevEdge;

            Vector3 vertex1 = edge1.sourceVertex.vertex;
            Vector3 vertex2 = edge2.sourceVertex.vertex;
            Vector3 vertex3 = edge3.sourceVertex.vertex;
            Vector3 vertex4 = edge4.sourceVertex.vertex;

            quads.Add(vertices.Contains(vertex1) ? vertices.IndexOf(vertex1) : index++);
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
    #endregion

    /**
     * CATMULL CLARK
     * */
    #region
    public void Catmull_Clark()
    {
        Dictionary<Face, Vertex> facePoints = new Dictionary<Face, Vertex>();
        Dictionary<HalfEdge, Vertex> edgePoints = new Dictionary<HalfEdge, Vertex>();

        int vertexIndice = vertices.Count;

        //On calcule les Face points pour chaque Face
        for (int i = 0; i < faces.Count; i++) facePoints[faces[i]] = new Vertex(vertexIndice++, FaceAverage(faces[i]));

        for (int i = 0; i < faces.Count; i++)
        {
            HalfEdge current = faces[i].face;
            // On parcours les Edge et on crée les edgePoints
            do
            {
                if (edgePoints.ContainsKey(current.twinEdge)) edgePoints[current] = edgePoints[current.twinEdge];
                else
                {
                    Vector3 edgePoint = new Vector3();

                    // On addtionne les points des bords
                    edgePoint += current.sourceVertex.vertex;
                    edgePoint += current.nextEdge.sourceVertex.vertex;

                    //On additionne les points des faces points
                    edgePoint += facePoints[current.face].vertex;
                    if (current.twinEdge != null)
                    {
                        edgePoint += facePoints[current.twinEdge.face].vertex;
                        edgePoint /= 4;
                    }
                    else edgePoint /= 3;

                    edgePoints[current] = new Vertex(vertexIndice++, edgePoint);
                }

                // On met à jour la position des verticles avec les VertexPoints
                int valence = computeValenceOfPoint(current.sourceVertex);
                List<HalfEdge> edgesUsingVertex = FindAllEdgeUsingVertice(current.sourceVertex);
                List<Vertex> facesOfVertex = edgesUsingVertex
                    .Select<HalfEdge, Vertex>(e => facePoints[e.face])
                    .Distinct()
                    .ToList();

                Vector3 avgOfFacePoints = new Vector3();
                for (int j = 0; j < facesOfVertex.Count; j++) avgOfFacePoints += facesOfVertex[j].vertex;
                avgOfFacePoints /= facesOfVertex.Count;

                Vector3 midpoint = new Vector3();
                for (int j = 0; j < edgesUsingVertex.Count; j++)
                {
                    midpoint += (edgesUsingVertex[j].sourceVertex.vertex + edgesUsingVertex[j].nextEdge.sourceVertex.vertex) / 2;
                }
                midpoint /= edgesUsingVertex.Count;

                Vector3 newPos = (avgOfFacePoints + 2 * midpoint + (valence - 3) * current.sourceVertex.vertex) / valence;
                current.sourceVertex.vertex = newPos;

                //On Split l'Edge
                SplitEdge(current, edgePoints[current]);

                current = current.nextEdge;
            } while (current != faces[i].face);


            // On split la face, avec le facepoint comme paramètre
            
        }
        
        //Pour ne pas traiter les faces que l'on vient de créer
        int initialFaceCount = faces.Count;

        for (int i = 0; i < initialFaceCount; i++)
        {
            int nbEdge = nbEdgeInFace(faces[i]);
            if ( nbEdge > 4 && nbEdge%2 == 0)
            {
                SplitFace(faces[i], facePoints[faces[i]]);
            }
        }

    }

    //TODO : A faire
    public static void SplitFace(Face f, Vertex v)
    {
        //On split la face
        int newNbFace = nbEdgeInFace(f) / 2;

        //TODO: J'écrit l'algo car pas le temps ce soir
        // On récupère les 4 points originaux de la face (les pairs dans une liste)
        // On connecte chaque nouveau vertex au face point (les impairs dans une liste)
        // On set la face et on relie bien les points entre eux
        // On adapte pour former les faces
    }

    public void SplitEdge(HalfEdge edge, Vertex v)
    {
        //On split l'edge
        HalfEdge newTwinEdge = null, newEdge;

        //On récupère les données
        HalfEdge twin = edge.twinEdge;
        HalfEdge next = edge.nextEdge;
        HalfEdge twinNext = twin.nextEdge;

        //On crée les nouveaux edges
        newEdge = new HalfEdge(edges.Count, v, edge, next, twin, edge.face);
        if (twin != null)
        {
            newTwinEdge = new HalfEdge(edges.Count + 1, v, twin, twinNext, edge, twin.face);
        }

        //On fait les liens avec les edges existants
        edge.nextEdge = newEdge;
        next.prevEdge = newEdge;
        if(newTwinEdge != null)
        {
            twin.twinEdge = newEdge;
            edge.twinEdge = newTwinEdge;
            twin.nextEdge = newTwinEdge;
            twinNext.prevEdge = newTwinEdge;
        }

        //On ajoute les new edge à la liste
        edges.Add(newEdge);
        if (newTwinEdge != null)  edges.Add(newTwinEdge);
    }

    #endregion

}
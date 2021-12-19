using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

class HalfEdgeMesh
{
    private static float COEFF = .2f;
    private static bool KEEP_EDGES_IF_VALENCE_UNDER_3 = true;


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
        return edges.Where<HalfEdge>(x => (x.sourceVertex == v) || (x.nextEdge.sourceVertex == v)).ToList();
    }


    public static List<HalfEdge> filterTwinsFromList(List<HalfEdge> edges)
    {
        List<HalfEdge> copy = new List<HalfEdge>(edges);
        for (int i = 0; i < edges.Count; i++) if (copy.Contains(edges[i])) copy.Remove(edges[i].twinEdge);
        return copy;
    }

    //Fonction utilitaire pour régler tous les problèmes de twins
    public void FixTwins()
    {
        Dictionary<KeyValuePair<int, int>, HalfEdge> twin = new Dictionary<KeyValuePair<int, int>, HalfEdge>();

        foreach(var edge in edges)
        {
            KeyValuePair<int, int> path = new KeyValuePair<int, int>(edge.sourceVertex.index, edge.nextEdge.sourceVertex.index);
            if (twin.ContainsKey(path))
            {
                edge.twinEdge = twin[path];
                edge.twinEdge.twinEdge = edge;
            }
            else twin[new KeyValuePair<int, int>(path.Value, path.Key)] = edge; //Le twin aura le path inverse 
        }
    }


    public static Vector3 FaceAverage(Face f)
    {
        Vector3 avg = new Vector3();
        int i = 0;

        HalfEdge current = f.face;
        do
        {
            avg += current.sourceVertex.vertex;
            current = current.nextEdge;
            i++;
        } while (current != f.face);

        return avg / i;
    }

    public static int nbEdgeInFace(Face f)
    {
        int nb = 0;
        HalfEdge edge = f.face;
        do
        {
            nb++;
            edge = edge.nextEdge;
        } while (edge != f.face);

        return nb;
    }

    //Méthode boite a outil, pas utilisé dans le programme principal
    public bool isClosed()
    {
        return edges.All<HalfEdge>(e => e.twinEdge != null);
    }

    public int valence(Vertex v)
    {
        return filterTwinsFromList(FindAllEdgeUsingVertice(v)).Count;
    }

    public List<Face> ListFaceUsingVertice(Vertex v)
    {
        return FindAllEdgeUsingVertice(v).Select<HalfEdge, Face>(e => e.face).Distinct().ToList();
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
            else
            {
                v1 = new Vertex(vertexIndice++, vertices[faces[i++]]);
                verticesDict.Add(v1Index, v1);
            }
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
            else
            {
                v2 = new Vertex(vertexIndice++, vertices[faces[i++]]);
                verticesDict.Add(v2Index, v2);
            }
            HalfEdge p2 = new HalfEdge(index++, v2, p1, null, null, face);
            p1.nextEdge = p2;

            int v3Index = faces[i];
            Vertex v3;
            if (verticesDict.ContainsKey(v3Index))
            {
                v3 = verticesDict[v3Index];
                i++;
            }
            else
            {
                v3 = new Vertex(vertexIndice++, vertices[faces[i++]]);
                verticesDict.Add(v3Index, v3);
            }
            HalfEdge p3 = new HalfEdge(index++, v3, p2, null, null, face);
            p2.nextEdge = p3;

            int v4Index = faces[i];
            Vertex v4;
            if (verticesDict.ContainsKey(v4Index))
            {
                v4 = verticesDict[v4Index];
                i++;
            }
            else
            {
                v4 = new Vertex(vertexIndice++, vertices[faces[i++]]);
                verticesDict.Add(v4Index, v4);
            }
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

        List<Vertex> verticesList = verticesDict.Values.OrderBy(v => v.index).ToList();

        return new HalfEdgeMesh(result, verticesList, facesList);
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
        //Propriétés des faces
        Dictionary<Face, Vertex> facePoints = new Dictionary<Face, Vertex>();

        //Propriétés des points
        Dictionary<Vertex, int> valenceOfPoint = new Dictionary<Vertex, int>();

        //Propriétés des edges
        Dictionary<HalfEdge, Vertex> edgePoints = new Dictionary<HalfEdge, Vertex>();

        int vertexIndice = vertices.Count;

        //On calcule les Face points pour chaque Face
        for (int i = 0; i < faces.Count; i++) { 
            facePoints[faces[i]] = new Vertex(vertexIndice++, FaceAverage(faces[i]));
            vertices.Add(facePoints[faces[i]]);
        }

        foreach (HalfEdge edge in edges)
        {
            // On parcours les Edge et on crée les edgePoints
            if (edge.twinEdge != null && edgePoints.ContainsKey(edge.twinEdge))
            {
                edgePoints[edge] = edgePoints[edge.twinEdge];
            }
            else
            {
                Vector3 edgePoint = new Vector3();

                // On addtionne les points des bords
                edgePoint += edge.sourceVertex.vertex;
                edgePoint += edge.nextEdge.sourceVertex.vertex;

                //On additionne les points des faces points
                edgePoint += facePoints[edge.face].vertex;
                if (edge.twinEdge != null)
                {
                    edgePoint += facePoints[edge.twinEdge.face].vertex;
                    edgePoint /= 4;
                }
                else edgePoint /= 3;

                edgePoints[edge] = new Vertex(vertexIndice++, edgePoint);
                vertices.Add(edgePoints[edge]);
            }

        }

        foreach(Vertex v in vertices)
        {
            //On actualise la vertice
            if (!valenceOfPoint.ContainsKey(v)) //Si la vertice n'as pas déjà été traité
            {
                //Calcul de la valence
                List<HalfEdge> edgesOfVertice = FindAllEdgeUsingVertice(v);
                List<Face> facesUsingVertice = edgesOfVertice.Select<HalfEdge, Face>(x => x.face).Distinct().ToList();
                edgesOfVertice = filterTwinsFromList(edgesOfVertice);
                int valence = edgesOfVertice.Count;
                valenceOfPoint[v] = valence;

                //Calcul de la moyenne des midpoints
                Vector3 avgOfMidpoints = new Vector3();
                foreach (HalfEdge edgeOfVertice in edgesOfVertice)
                {
                    avgOfMidpoints += (edgeOfVertice.sourceVertex.vertex + edgeOfVertice.nextEdge.sourceVertex.vertex) / 2;
                }
                avgOfMidpoints /= edgesOfVertice.Count;

                //Calcul de la moyenne des facepoints
                Vector3 facePointAvg = new Vector3();
                foreach (Face face in facesUsingVertice)
                {
                    facePointAvg += facePoints[face].vertex;
                }
                facePointAvg /= facesUsingVertice.Count;


                //On change la position
                if (valence > 2)
                {
                    v.vertex = (facePointAvg + 2 * avgOfMidpoints + (valence - 3) * v.vertex) / valence;
                }
                else if (!KEEP_EDGES_IF_VALENCE_UNDER_3)
                {
                    v.vertex = (facePointAvg + 2 * avgOfMidpoints) / 3;
                }
            }
        }
        
        //On Split les edges
        List<HalfEdge> edgesToSplit = filterTwinsFromList(edges);
        
        foreach (HalfEdge edge in edgesToSplit)
        {
            SplitEdge(edge, edgePoints[edge]);
        }
        

        //Pour ne pas traiter les faces que l'on vient de créer
        int initialFaceCount = faces.Count;

        for (int i = 0; i < initialFaceCount; i++)
        {
            int nbEdge = nbEdgeInFace(faces[i]);
            if (nbEdge % 2 == 0)
            {
                SplitFace(faces[i], facePoints[faces[i]]);
            }
        }

        // Afin de corriger les bugs liés par exemple au splitFace
        FixTwins();
    }

    public void SplitFace(Face f, Vertex v)
    {

        int facesIndex = faces.Count;
        int edgesIndex = edges.Count;


        // On trie les edges en fonction de si il viennent d'être crée ou si ils sont là de manière anciennes
        List<HalfEdge> oldEdges = new List<HalfEdge>();
        List<HalfEdge> addedEdges = new List<HalfEdge>();
        List<HalfEdge> newEdges = new List<HalfEdge>();
        List<Face> newFaces = new List<Face>();

        int i = 0;
        HalfEdge current = f.face;
        do
        {
            if (i % 2 == 0) oldEdges.Add(current);
            else addedEdges.Add(current);
            i++;
            current = current.nextEdge;
        } while (current != f.face);

        // On crée de nouvelles faces à partir des ancien edges
        newFaces.Add(f);
        for (int j = 1; j < oldEdges.Count; j++)
        {
            newFaces.Add(new Face(facesIndex++, oldEdges[j]));
        }

        //On change les faces
        for (i = 0; i < oldEdges.Count; i++) oldEdges[i].face = newFaces[i];
        for (i = 0; i < addedEdges.Count; i++) addedEdges[i].face = newFaces[(i + 1) % newFaces.Count];

        // On crée des nouveaux liens en partant des oldEdges vers les facePoints
        foreach (HalfEdge edge in addedEdges)
        {
            HalfEdge edgeToFacePt = new HalfEdge(edgesIndex++, edge.sourceVertex, edge.prevEdge, null, null, edge.prevEdge.face);
            HalfEdge facePtToEdge = new HalfEdge(edgesIndex++, v, edgeToFacePt, edge.prevEdge.prevEdge, null, edge.prevEdge.face);
            edgeToFacePt.nextEdge = facePtToEdge;

            newEdges.Add(edgeToFacePt);
            newEdges.Add(facePtToEdge);
        }

        // On set les twin des nouveaux edges
        for(i = 0; i < newEdges.Count; i++)
        {
            newEdges[i].prevEdge.nextEdge = newEdges[i];
            newEdges[i].nextEdge.prevEdge = newEdges[i];
            //On traite que les edge pair par soucis de simplicité
            if (i % 2 != 0) continue;

            //Correspond au prochain de la fin du quad
            int twinEdgeIndex = (i + 3) % newEdges.Count;
            newEdges[i].twinEdge = newEdges[twinEdgeIndex];
            newEdges[i].twinEdge.twinEdge = newEdges[i]; 
        }

        //On ajoute les nouvelles face + nouvelles edges (et on prie pour que ça marche)
        faces.AddRange(newFaces);
        edges.AddRange(newEdges);
    }

    public void SplitEdge(HalfEdge e, Vertex v)
    {
        int halfEdgeIndex = edges.Count;

        //On récupère les données
        Face fe = e.face;
        HalfEdge nx = e.nextEdge;
        HalfEdge p = e.prevEdge;
        HalfEdge t = e.twinEdge;
        Face tfe = (t == null) ? null : t.face;
        HalfEdge nxt = (t==null)?null:t.nextEdge;
        HalfEdge pxt = (t==null)?null:t.prevEdge;

        //On crée les nouveaux edges
        HalfEdge ne = new HalfEdge(halfEdgeIndex++, v, e, nx, t, fe);
        HalfEdge nt = (t == null) ? null : new HalfEdge(halfEdgeIndex++, v, t, nxt, e, tfe);

        //On set les prev et les nexte
        ne.nextEdge.prevEdge = ne;
        ne.prevEdge.nextEdge = ne;
        if (t != null)
        {
            nt.nextEdge.prevEdge = nt;
            nt.prevEdge.nextEdge = nt;
        }

        edges.Add(ne);
        if (nt != null) edges.Add(nt);
    }

    #endregion

    #region
    #if UNITY_EDITOR
    public void displayGizmos()
    {
        //Propriétés des faces
        Dictionary<Face, Vertex> facePoints = new Dictionary<Face, Vertex>();

        GUIStyle faceStyle= new GUIStyle();
        faceStyle.fontSize = 12;
        faceStyle.normal.textColor = Color.cyan;

        //On affiche les Face points
        for (int i = 0; i < faces.Count; i++)
        {
            facePoints[faces[i]] = new Vertex(-1, FaceAverage(faces[i]));
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(facePoints[faces[i]].vertex, .01f);
            Handles.Label(facePoints[faces[i]].vertex + new Vector3(0,0,.1f), "" + faces[i].index, faceStyle);
        }

        GUIStyle edgeStyle = new GUIStyle();
        edgeStyle.fontSize = 12;
        edgeStyle.normal.textColor = Color.blue;

        foreach (HalfEdge edge in edges)
        {
            // On parcours les Edge et on affiche les edgePoints
            Vector3 edgePoint = new Vector3();
            Vector3 midPoint = (edge.sourceVertex.vertex + edge.nextEdge.sourceVertex.vertex) / 2;

            // On addtionne les points des bords
            edgePoint += edge.sourceVertex.vertex;
            edgePoint += edge.nextEdge.sourceVertex.vertex;

            //On additionne les points des faces points
            edgePoint += facePoints[edge.face].vertex;
            if (edge.twinEdge != null)
            {
                edgePoint += facePoints[edge.twinEdge.face].vertex;
                edgePoint /= 4;
            }
            else
            {
                edgePoint /= 3;
            }

            Handles.Label(midPoint + facePoints[edge.face].vertex * 0.03f, ""+edge.index , edgeStyle);

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(edgePoint, .01f);
            Gizmos.DrawLine(edgePoint, edge.sourceVertex.vertex);
            Gizmos.DrawLine(edgePoint, edge.nextEdge.sourceVertex.vertex);

        }

        GUIStyle m_Style = new GUIStyle();
        m_Style.fontSize = 16;
        m_Style.normal.textColor = Color.red;

        foreach (HalfEdge edge in edges)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(edge.sourceVertex.vertex, edge.nextEdge.sourceVertex.vertex); 

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(edge.sourceVertex.vertex, .01f);
            Handles.Label(edge.sourceVertex.vertex + new Vector3(0,0,.03f), ""+edge.sourceVertex.index, m_Style);
        }
    }
    #endif
    #endregion

}
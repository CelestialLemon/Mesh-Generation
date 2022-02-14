using System;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    //Serialized vars
    [SerializeField] int height, width;
    [SerializeField] float pnAmplititude;
    [SerializeField] Vector2 pnScale;

    Mesh mesh;

    List<Vector3> vertices;
    List<int> triangles;
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        vertices = new List<Vector3>();
        triangles = new List<int>();

        
        CreateMesh();
        Terrainize();
        UpdateMesh();
    }


    //creates a plane
    void CreateMesh()
    {
        // add vertices
        for(int i = 0; i < height; i++)
        {
            for(int j = 0; j < width; j++)
            {
                vertices.Add(new Vector3(j, 0, i));
            }
        }

        //add triangles
        for(int i = 0; i < height - 1; i++)
        {
            for(int j = 0; j < width - 1; j++)
            {
                int i_currentVertex = i * (width) + j;
                
                triangles.Add(i_currentVertex + 1 + width);
                triangles.Add(i_currentVertex + 1);
                triangles.Add(i_currentVertex);

                triangles.Add(i_currentVertex);
                triangles.Add(i_currentVertex + width);
                triangles.Add(i_currentVertex + 1 + width);
            }
        }
    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
    }

    int IndexOfVertex(int x, int z)
    {
        return z * width + x;
    }

    bool isInBounds(int x, int z)
    {
        //check if the index value is in bounds of the vertex array
        if (x < 0 || z < 0 || x >= width || z >= height) return false;
        else return true;
    }

    void Terrainize()
    {
        for(int z = 0; z < height; z++)
        {
            for(int x = 0; x < width; x++)
            {
                int index = IndexOfVertex(x, z);
                float y = Mathf.PerlinNoise(x * pnScale.x, z * pnScale.y) * pnAmplititude;
                vertices[index] = new Vector3(vertices[index].x, y, vertices[index].z);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    //Serialized vars
    [SerializeField] int height, width;
    [SerializeField] float altitudeIncreament;
    [SerializeField] int numOfPasses;


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
        for(int i = 0; i < numOfPasses; i++)
        Terrainize();
        UpdateMesh();
    }

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

    float findAverageAltitudeOfSurrounding(int x, int z)
    {
        float avg = 0f;
        int i_currentVertex = IndexOfVertex(x, z);

        List<float> altitudes = new List<float>();
        
        // height of current point
        altitudes.Add(vertices[i_currentVertex].y);

        // top top-right right bottom-right bottom bottom-left left top-left
        if (isInBounds(x, z + 1)) altitudes.Add(vertices[IndexOfVertex(x, z + 1)].y);
        if (isInBounds(x + 1, z + 1)) altitudes.Add(vertices[IndexOfVertex(x + 1, z + 1)].y);
        if (isInBounds(x + 1, z)) altitudes.Add(vertices[IndexOfVertex(x + 1, z)].y);
        if (isInBounds(x + 1, z - 1)) altitudes.Add(vertices[IndexOfVertex(x + 1, z - 1)].y);
        if (isInBounds(x, z - 1)) altitudes.Add(vertices[IndexOfVertex(x, z - 1)].y);
        if (isInBounds(x - 1, z - 1)) altitudes.Add(vertices[IndexOfVertex(x - 1, z - 1)].y);
        if (isInBounds(x - 1, z)) altitudes.Add(vertices[IndexOfVertex(x - 1, z)].y);
        if (isInBounds(x - 1, z + 1)) altitudes.Add(vertices[IndexOfVertex(x - 1, z + 1)].y);

        foreach (float altitude in altitudes)
            avg += altitude;

        avg /= altitudes.Count;

        return avg;
    }

    void Terrainize()
    {
        System.Random random = new System.Random();
        for(int z = 0; z < height; z++)
        {
            for(int x = 0; x < width; x++)
            {
                float avgAlti = findAverageAltitudeOfSurrounding(x, z);

                int rng = random.Next(0, 2);

                Vector3 vertex = vertices[IndexOfVertex(x, z)];

                if (rng == 0) vertex.y = avgAlti - altitudeIncreament;
                else vertex.y = avgAlti + altitudeIncreament;

                vertices[IndexOfVertex(x, z)] = vertex;
            }
        }
    }
}

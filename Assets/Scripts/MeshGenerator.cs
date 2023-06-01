using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    public int xSize = 20;
    public int zSize = 20;

    public Gradient _gradient;
    
    private Mesh mesh;
    
    private Vector3[] _vertices;
    private int[] _triangles;

    private Color[] _colors;

    private float _minTerrainHeight;
    private float _maxTerrainHeiht;

    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateShape();
        UpdateMesh();
    }

    private void CreateShape()
    {
        _vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        //Cria os vertices no plano 20 x 20
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = Mathf.PerlinNoise(x * 0.3f, z * 0.3f) * 2f;
                _vertices[i] = new Vector3(x, y, z);

                if (y > _maxTerrainHeiht)
                    _maxTerrainHeiht = y;
                if (y < _minTerrainHeight)
                    _minTerrainHeight = y;
                
                i++;
            }
        }

        //Muiltiplica o tamanho do grid pelo numero de vertices necessários pra fazer um quadrado
        _triangles = new int[xSize * zSize * 6];
        int verts = 0;  //Faz o ofset dos vertices
        int tris = 0;   //Faz isso por mais de uma linha

        for (int z = 0; z < zSize; z++)
        {
            for (int i = 0; i < xSize; i++)
            {
                _triangles[0 + tris] = verts + 0;
                _triangles[1 + tris] = verts + xSize + 1;
                _triangles[2 + tris] = verts + 1;
                _triangles[3 + tris] = verts + 1;
                _triangles[4 + tris] = verts + xSize + 1;
                _triangles[5 + tris] = verts + xSize + 2;
            
                verts++;
                tris += 6;
            }

            verts++;
        }

        //Refaz os mapas uvs do PLANO
        _colors = new Color[_vertices.Length];
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float height = Mathf.InverseLerp(_minTerrainHeight, _maxTerrainHeiht, _vertices[i].y);
                _colors[i] = _gradient.Evaluate(height);

                i++;
            }
        }
    }

    //Faz o assign dos valores do mesh
    private void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = _vertices;
        mesh.triangles = _triangles;

        mesh.colors = _colors;
        
        mesh.RecalculateNormals();
    }

    //Mostra os vertices
    private void OnDrawGizmos()
    {
        if (_vertices == null)
            return;
        for (int i = 0; i < _vertices.Length; i++)
        {
            Gizmos.DrawSphere(_vertices[i], 0.1f);
        }
    }
}

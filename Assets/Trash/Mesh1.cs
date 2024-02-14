using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Mesh1 : MonoBehaviour
{
    Vector3[] initVertices = new [] {new Vector3(-6, 3.5f, 0),
                                            new Vector3(4, 2, 0),
                                            new Vector3(-6, 4.5f, 0),
                                            new Vector3(4, 3, 0)};
    Mesh _mesh;
    MeshFilter meshFilter;
    Vector3[] _vertices = new Vector3[4];

    Rigidbody2D playerRigidbody;

    void Start()
    {
        meshFilter = gameObject.GetComponent<MeshFilter>();
        _mesh = new Mesh();
        for (int i = 0; i < _vertices.Length; i++)
            _vertices[i] = initVertices[i];
        _mesh.SetVertices(initVertices);
        int[] triangles = new [] {0, 2, 1,  2, 3, 1};
        _mesh.SetTriangles(triangles, 0);
        //MeshFilterへの割り当て
        meshFilter.mesh = _mesh;

        GameObject Player = GameObject.Find("Player");
        playerRigidbody = Player.gameObject.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Vector3 playerVelocity = playerRigidbody.velocity;
        Vector3 playerDirection = playerVelocity / playerVelocity.magnitude;
        Vector3 playerPosition = playerRigidbody.position;
        float gamma = 1 / Mathf.Sqrt(1 - playerVelocity.magnitude / Physics.c);

        for (int i = 0; i < _vertices.Length; i++)
        {
            _vertices[i] = initVertices[i] + (1/gamma - 1) * Vector3.Dot(initVertices[i] - playerPosition, playerDirection) * playerDirection;
        }

        _mesh.vertices = _vertices;
        //_mesh.RecalculateBounds();

        Debug.Log(gamma);
    }
}

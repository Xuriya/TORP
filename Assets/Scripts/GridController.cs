// https://nn-hokuson.hatenablog.com/entry/2018/02/13/200114

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;

public class GridController : MonoBehaviour 
{
    [SerializeField] Material grid;

    [SerializeField] int gridHeight = 3000;
    [SerializeField] int gridWidth = 6000;
    [SerializeField] float pixelPerUnit = 1080f/10f;

    Mesh mesh;
    Player player;
    MeshRenderer gridMeshRenderer;
    Vector3[] defaultVertices;

    void Start () 
    {
    mesh = new Mesh ();
    float halfHeight = (float) gridHeight/pixelPerUnit / 2;
    float halfWidth = (float) gridWidth/pixelPerUnit / 2;

    // Playerの位置を中心にGridの四隅を設定
    defaultVertices = new Vector3[] {
        new Vector3 (-halfWidth, -halfHeight, 0),
        new Vector3 (-halfWidth,  halfHeight, 0),       
        new Vector3 (halfWidth , -halfHeight, 0),
        new Vector3 (halfWidth ,  halfHeight, 0),
    };
    GameObject playerObject = GameObject.Find("Player");
    player = playerObject.GetComponent<Player>();
    Vector3[] vertices = new Vector3[defaultVertices.Length];
    for (int i = 0; i < defaultVertices.Length; i++) { vertices[i] = defaultVertices[i]; }
    mesh.vertices = vertices;

    mesh.uv = new Vector2[] {
        new Vector2 (0, 0),
        new Vector2 (0, 1),
        new Vector2 (1, 0),              
        new Vector2 (1, 1),
    };

    mesh.triangles = new int[] {
        0, 1, 2, 
        1, 3, 2,
    };
    GetComponent<MeshFilter> ().sharedMesh = mesh;
    GetComponent<MeshRenderer> ().material = grid;
    gridMeshRenderer = GetComponent<MeshRenderer>();
    
    // 初期化時点ではgridを出さない
    gridMeshRenderer.enabled = false;
    }

    void Update()
    {
        if (Player.isObserver)
        {
            Transform();
        }
    }

    public void FadeIn()
    {
        if (gridMeshRenderer != null){
            gridMeshRenderer.enabled = true;
        }
    }

    public void FadeOut()
    {
        if (gridMeshRenderer != null) {
            gridMeshRenderer.enabled = false;
        }
    }

    void Transform()
    {
        Vector3[] vertices = new Vector3[defaultVertices.Length];
        for (int i = 0; i < defaultVertices.Length; i++)
        {
            vertices[i] = player.LorentzTransform(defaultVertices[i]);
        }
        mesh.vertices = vertices;
    }

}
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

using Vector2 = UnityEngine.Vector2;

public class MeshGraphics : MonoBehaviour
{
    public enum Attribute {
        // Playerの操作に応じて変形される。通常の壁
        Player = 0,
        // 固定された壁
        Fixed,
        // 固定された壁の最初の位置を示す影
        Shadow,
        // 一度Observerモードを使うと固定される壁
        Solidifying
    }
    [SerializeField] Attribute attribute = Attribute.Player;
    [SerializeField] float _margin;
    Player Player;

    PolygonCollider2D polygonCollider2D;
    
    // Meshのarray 内側のMeshから格納されている
    Mesh[] _mesh = new Mesh[4];

    Vector3[][] defaultVertices = new Vector3[4][];
    Vector3[][] _meshVertices = new Vector3[4][];
    
    // Colliderの頂点位置
    Vector2[] _points = new Vector2[4];

    bool transformed;

    void Awake()
    {
        polygonCollider2D = gameObject.GetComponent<PolygonCollider2D>();
        Player = GameObject.Find("Player").GetComponent<Player>();
    }

    void Start()
    {
        // PolygonCollider2Dの頂点を取得
        // PolygonCollider2Dの頂点は相対座標なので注意(transformの座標は常に0, 0, 0)
        // indexの順番は反時計回り
        for(int i = 0; i < _points.Length; i++)
            _points[i] = polygonCollider2D.points[i];

        // polygonCollider2Dに沿ったメッシュを生成
        int[] triangles = new [] {0, 2, 1,  2, 0, 3};
        Vector3[] vertices = new Vector3[_points.Length];
        for (int i = 0; i < _points.Length; i++) { vertices[i] = _points[i]; }
        for (int i = 0; i < defaultVertices.Length; i++)
        {
            // 頂点位置を決める
            defaultVertices[i] = CurtailEdge(vertices, _margin * (defaultVertices.Length - i));
            _meshVertices[i] = CurtailEdge(vertices, _margin * (defaultVertices.Length - i));

            // 頂点とindexの設定
            _mesh[i] = new Mesh();
            _mesh[i].SetVertices(defaultVertices[i]);
            _mesh[i].SetTriangles(triangles, 0);

            // MeshFilterへMeshを割り当て
            transform.GetChild(i).gameObject.GetComponent<MeshFilter>().mesh = _mesh[i];
        }

        // AttributeがPlayerなら描画順をShadowの前に
        if (attribute == Attribute.Player) {
            for (int i = 0; i < _mesh.Length; i++)
            {
                transform.GetChild(i).gameObject.GetComponent<MeshRenderer>().material.renderQueue += 1;
            }
        }

        // AttributeがShadowならPolygonCollider2DをTriggerに
        if (attribute == Attribute.Shadow) {
            polygonCollider2D.isTrigger = true;
        }

        transformed = false;
        Initialize();
    }

    // 初期化時及びリセット時に呼ばれる
    void Initialize()
    {
        Transform();
        // AttributeがShadowならCollider2Dの当たり判定を少しだけ小さくする
        if (attribute == Attribute.Shadow) {
            polygonCollider2D.points = ToVector2Array(CurtailEdge(ToVector3Array(_points), 0.05f));
        }
        // もしSolidifyingからFixedに代わっていれば、戻す
        if (attribute == Attribute.Fixed && transformed == true)
        {
            TransitToSolidifying();
        }
        // Lorentz変換で変形したか
        transformed = false;
        SetColor();
    }

    void Update()
    {
        switch (GameManager.CurrentState)
        {
            case GameManager.State.Start:
                Initialize();
                break;
            case GameManager.State.Play:  
                switch (Player.CurrentState)
                {
                    case Player.State.Active:
                        switch (attribute)
                        {
                            case Attribute.Player:
                            case Attribute.Solidifying:
                                // Observerモード
                                if (Player.isObserver)
                                {
                                    Transform();
                                }
                                break;

                            // Attribute.Fixed, Attribute.Shadowは何もしない
                        }
                        break;
                    
                    case Player.State.Idol:
                        switch (attribute)
                        {
                            case Attribute.Player:
                                // Observerモード
                                if (Player.isObserver)
                                {
                                    Transform();
                                }
                                break;
                            case Attribute.Solidifying:
                                // Observerモードを使ったならFixedに
                                if (transformed) TransitToFixed();
                                break;
                        }
                        break;
                }
                break;
        }
    }
    
    Vector3[] CurtailEdge(Vector3[] vertices, float margin)
    {
        // 一先ずwallsが長方形の時を想定
        // 一般の場合はhttps://qiita.com/uyuutosa/items/8de1f7602cb14c29606fなど
        Vector3 center = (vertices[0] + vertices[2]) / 2;
        Vector3[] vertices2 = new Vector3[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 diff = vertices[i] - center;
            diff = margin * new Vector2(Mathf.Sign(diff.x), Mathf.Sign(diff.y));
            vertices2[i] = vertices[i] - diff;
        }
        return vertices2;
    }

    Vector2[] ToVector2Array(Vector3[] vectors)
    {
        Vector2[] vector2s = new Vector2[vectors.Length];
        for (int i = 0; i < vectors.Length; i++)
        {
            vector2s[i] = (Vector2) vectors[i];
        }
        return vector2s;
    }    

    Vector3[] ToVector3Array(Vector2[] vectors)
    {
        Vector3[] vector3s = new Vector3[vectors.Length];
        for (int i = 0; i < vectors.Length; i++)
        {
            vector3s[i] = (Vector3) vectors[i];
        }
        return vector3s;
    }

    void Transform()
    {
        // Meshの変形処理

        for (int i = 0; i < _meshVertices.Length; i++)
        {
            for (int j = 0; j < _meshVertices[0].Length; j++)
            {
                _meshVertices[i][j] = Player.LorentzTransform(defaultVertices[i][j]);
            }  
            _mesh[i].vertices = _meshVertices[i];
        }

        // 次の処理はUnityDocumentationではするべきとあったがするとバグる
        //_mesh.RecalculateBounds();

        // Colliderの変形処理
        // 必要なさそう(?)
        for(int i = 0; i < polygonCollider2D.points.Length; i++)
            _points[i] = _meshVertices[_meshVertices.Length-1][i];
        polygonCollider2D.points = _points;

        if (attribute == Attribute.Player || attribute == Attribute.Solidifying)
        {
            transformed = true;
        }
    }

    void SetColor() {
        for (int i = 0; i < _mesh.Length; i++)
        {
            switch (i % 2)
            {
                // 枠線のMeshに対して
                case 1: 
                    switch (attribute)
                    {
                        case Attribute.Player:
                            // blue
                            SetWallColor(i, new Color32(138, 213, 235, 255));
                            break;
                        case Attribute.Fixed:
                            SetWallColor(i, new Color32(204, 204, 204, 255));
                            break;
                        case Attribute.Shadow:
                            SetWallColor(i, (Color) new Color32(138, 213, 235, 255) - new Color32(0, 0, 0, 224));
                            break;
                        case Attribute.Solidifying:
                            // pink
                            SetWallColor(i, new Color32(235, 138, 235, 255));
                            break;
                    }
                    break;

                // 中塗りのMeshに対して
                case 0:
                    switch (attribute)
                    {
                        case Attribute.Player:
                            // blue
                            SetWallColor(i, new Color32(4, 37, 78, 255));
                            break;
                        case Attribute.Fixed:
                            SetWallColor(i, Color.black);
                            break;
                        case Attribute.Shadow:
                            SetWallColor(i, (Color) new Color32(4, 37, 78, 255) - new Color32(0, 0, 0, 224));
                            break;
                        case Attribute.Solidifying:
                            // pink
                            SetWallColor(i, new Color32(78, 4, 78, 255));
                            break;
                    }
                    break;
            }
        }
    }

    void SetWallColor(int index, Color32 color) {
        transform.GetChild(index).gameObject.GetComponent<MeshRenderer>().material.color = color;
    }

    void TransitToFixed()
    {
        attribute = Attribute.Fixed;
        SetColor();
    }
    
    void TransitToSolidifying()
    {
        attribute = Attribute.Solidifying;
        SetColor();
    }
}

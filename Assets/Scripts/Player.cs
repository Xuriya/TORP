using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;

public class Player : MonoBehaviour
{
    public enum State
    {
        Idol = 0,
        Active,
        Goal,
        GameOver
    }
    public static State CurrentState { get; private set; }
    public static float Gamma { get; private set; }
    public static bool isObserver {get; private set; }

    [SerializeField] int _remainObserve;
    static int remainObserve;
    int initRemainObserve;
    public static int RemainObserve {get => remainObserve; }

    public bool onShadow {get; private set; }

    Rigidbody2D myRigidBody;
    Camera mainCamera;
    GameManager gameManager;
    GameObject[] blackholeObjects;
    Blackhole[] blackholes;
    Vector3[] blackholesPos;
    
    Vector3 initPlayerPosition;
    Vector3 direction;
    [SerializeField] float amp;
    [SerializeField] float saturationSpeed;

    // Start is called before the first frame update

    void Awake()
    {
        myRigidBody = this.gameObject.GetComponent<Rigidbody2D>();
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        // Blackholeの取得
        blackholeObjects = GameObject.FindGameObjectsWithTag("Blackhole");
        Blackhole[] _blackholes = new Blackhole[blackholeObjects.Length];
        Vector3[] _blackholesPos = new Vector3[blackholeObjects.Length];
        for (int i = 0; i < blackholeObjects.Length; i++)
        {
            _blackholes[i] = blackholeObjects[i].GetComponent<Blackhole>();
            _blackholesPos[i] = blackholeObjects[i].transform.position;
        }
        blackholes = _blackholes;
        blackholesPos = _blackholesPos;
    }

    void Start()
    {
        initRemainObserve = _remainObserve;
        initPlayerPosition = myRigidBody.position;
        Initialize();
    }

    void Initialize()
    {
        _remainObserve = initRemainObserve;
        myRigidBody.position = initPlayerPosition;
        Gamma = 1.0f;
        remainObserve = _remainObserve;
        onShadow = false;
        CurrentState = State.Idol;
        TransitToIdol();
        SetObserverMode(false);
    }

    // Update is called once per frame

    void Update()
    {
        switch(GameManager.CurrentState)
        {
            case GameManager.State.Start:
                Initialize();
                break;
            case GameManager.State.Play:
                switch(CurrentState)
                {
                    case State.Idol:
                        // Shadowに入っておらずスペースキーを受け取ったならモードを切り替え
                        if(Input.GetKeyDown(KeyCode.Space) && !onShadow)
                        {
                            // 切り替え不可の仕様
                            if (!isObserver)
                            {
                                SetObserverMode(true);    
                            }
                        }
                        break;
                }
                break;
        }
    }

    void FixedUpdate()
    {
        switch(GameManager.CurrentState)
        {
            case GameManager.State.Play:
                switch(CurrentState)
                {
                    case State.Idol:
                        // 方向を受け取る
                        // 押下した瞬間を受け取るならUpdate()の中で回す必要がある
                        if (Input.GetMouseButton(0))
                        {
                            GetDirection();
                            gameManager.EnterStateActive();
                            CurrentState = State.Active;
                        } else {
                            ResetVelocity();
                        }
                        break;
                        
                    case State.Active:
                        // マウスで指定した方向へと力を加える
                        GetCurrentDirection();
                        myRigidBody.AddForce(amp * direction);

                        // 重力を足す
                        for (int i = 0; i < blackholes.Length; i++)
                        {
                            // BlackholeのIsFixedに応じてblackholeの位置を修正
                            if (!blackholes[i].IsFixed)
                            {
                                blackholesPos[i] = blackholeObjects[i].transform.position;
                            }

                            Vector3 relPos = blackholesPos[i] - transform.position;
                            float dist = relPos.magnitude;
                            Vector3 dirc = UnitVector(relPos);
                            myRigidBody.AddForce(blackholes[i].Gravity * dirc / dist / dist);
                        }

                        // 最高速度を超えないように調整
                        if (myRigidBody.velocity.magnitude >= saturationSpeed)
                        {
                            myRigidBody.velocity = saturationSpeed / myRigidBody.velocity.magnitude * myRigidBody.velocity;
                        }
                        if (isObserver){
                            Gamma = 1 / Mathf.Sqrt(1 - (myRigidBody.velocity.magnitude / Physics.c) * (myRigidBody.velocity.magnitude / Physics.c));
                        }
                        break;
                }
                break;
        }
    }
    void OnCollisionEnter2D(Collision2D collision){
        switch(CurrentState)
        {
            case State.Active:
                if (collision.gameObject.tag == "Wall")
                {
                    if (isObserver)
                    {
                        TransitToIdol();
                        gameManager.ExitObserverMode();
                    } else {
                        TransitToIdol();
                    }
                    gameManager.EnterStateIdol();
                }
                if (collision.gameObject.tag == "Goal")
                {
                    if (remainObserve >= 0) {
                        TransitToGoal();
                    } else {
                        TransitToGameOver();
                    }
                }
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D collider){
        if (collider.tag == "Blackhole") TransitToGameOver();
        if (collider.tag == "Wall")
        {
            onShadow = true;
        }
    }

    void OnTriggerExit2D(Collider2D collider){
        if (collider.tag == "Wall")
        {
            onShadow = false;
        }
    }

    void GetDirection()
    {
        // Playerのワールド座標をスクリーン座標に変換
        Vector3 playerScreenPos = mainCamera.WorldToScreenPoint(myRigidBody.position);
        Vector3 dirc = Input.mousePosition - playerScreenPos;
        dirc = UnitVector(dirc);
        direction = dirc;
    }

    void GetCurrentDirection()
    {
        if (myRigidBody.velocity == Vector2.zero){
            return;
        }
        direction = UnitVector(myRigidBody.velocity);
    }

    public Vector3 LorentzTransform(Vector3 position)
    {
        Vector3 playerDirection = UnitVector(myRigidBody.velocity);
        Vector3 playerPosition = myRigidBody.position;
        float gamma = Player.Gamma;
        Vector3 transformedPosition =  position + (1/gamma - 1) * Vector3.Dot(position - playerPosition, playerDirection) * playerDirection;
        return transformedPosition;
    }

    void ResetVelocity()
    {
        myRigidBody.velocity = Vector2.zero;
    }

    Vector3 UnitVector(Vector3 vec)
    {
        if (vec != Vector3.zero)
        {
            vec = vec / vec.magnitude;
        }
        return vec;
    }

    void SetColorNormal()
    {
        gameObject.GetComponent<SpriteRenderer>().material.color = Color.white;
    }

    void SetColorObserver()
    {
        gameObject.GetComponent<SpriteRenderer>().material.color = Color.red;
    }

    void SetObserverMode(bool ObserverMode)
    {
        isObserver = ObserverMode;
        if (isObserver)
        {
            remainObserve--;
            SetColorObserver();
            gameManager.EnterObserverMode();
        } else {
            SetColorNormal();
            gameManager.ExitObserverMode();
        }
    }
    void TransitToIdol()
    {
        CurrentState = State.Idol;
        ResetVelocity();
        
        // Observerモードに入ってたならObserverモードを抜ける
        if (isObserver) 
        {
            SetObserverMode(false);
        }
    }

    void TransitToGoal()
    {
        ResetVelocity();
        CurrentState = State.Goal;
    }

    void TransitToGameOver()
    {
        ResetVelocity();
        CurrentState = State.GameOver;
    }
}
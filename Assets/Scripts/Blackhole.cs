using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackhole : MonoBehaviour   
{
    [SerializeField] float gravity;
    public float Gravity {get => gravity; }

    [SerializeField] bool isFixed;
    public bool IsFixed {get => isFixed; }

    Rigidbody2D myRigidbody;
    Player Player;

    Vector3 defaultPosition;

    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        Player = GameObject.Find("Player").GetComponent<Player>();
        defaultPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        switch(Player.CurrentState)
        {
            case Player.State.Active:
                // PlayerがObserverの時 Lorentz変換で移動
                if (!isFixed && Player.isObserver) 
                {
                    myRigidbody.position = Player.LorentzTransform(defaultPosition);
                }
                break;
        }
    }
}

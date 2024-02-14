using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    GameObject Player;
    Camera myCamera;

    [SerializeField] bool followX = false;
    [SerializeField] bool followY = true;

    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.Find("Player");
        myCamera = gameObject.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position = Player.transform.position;
        if (!followX) position.x = 0;
        if (!followY) position.y = 0;
        position.z = -10;
        transform.position = position;
    }
}

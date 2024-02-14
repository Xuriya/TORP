using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


/*
public class Wall1 : MonoBehaviour
{
    private Vector2 initPos;
    private Vector3 initScale;
    private Rigidbody2D playerRigidbody;
    private Rigidbody2D myRigidbody;

    // Start is called before the first frame update
    void Start()
    {
        initPos = transform.position;
        initScale = transform.localScale;
        GameObject Player = GameObject.Find("Player");
        playerRigidbody = Player.gameObject.GetComponent<Rigidbody2D>();
        myRigidbody = this.gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 playerVelocity = playerRigidbody.velocity;
        Vector2 playerPosition = playerRigidbody.position;
        float gamma = Mathf.Sqrt(1 - playerVelocity.magnitude / Physics.c);
        myRigidbody.position = initPos / gamma + (gamma - 1) / gamma * playerPosition;

        float xScale = playerVelocity.x / playerVelocity.magnitude / gamma;
        float yScale = playerVelocity.y / playerVelocity.magnitude / gamma;
        ///transform.localScale = new Vector3(initScale.x * xScale, initScale.y * yScale, initScale.z);
        Debug.Log(myRigidbody.position);
    }
}
*/
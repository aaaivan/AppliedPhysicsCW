using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowBall : MonoBehaviour
{
    public Transform ball;

    void Update()
    {//follow the ball along the x direction
        gameObject.transform.position = new Vector3(
            ball.position.x, 
            gameObject.transform.position.y,
            ball.position.z);
    }
}

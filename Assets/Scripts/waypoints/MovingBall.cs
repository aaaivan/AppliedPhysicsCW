using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBall : MonoBehaviour
{
    public float velocity;
    public float rotationRadius;
    public Transform[] waypoints;
    public Material red;
    public Material yellow;
    public Material green;

    //cartesian position relative to the origin
    MyVector3 position;
    //cylindrical position relative to the next waypoint
    MyVector3_Cylindrical rotationPosition;
    MyVector3 movementDirection;
    MyVector3_Cylindrical directionConcesutiveWaypoints;
    bool clockwise;
    int target;
    MyVector3[] waypoints_position;
    bool rotating;
    float angularVelocity;

    // Start is called before the first frame update
    void Start()
    {
        position = ToMyVector3(gameObject.transform.position);
        waypoints_position = new MyVector3[waypoints.Length];
        for (int i = 0; i < waypoints.Length; i++)
        {
            waypoints_position[i] = ToMyVector3(waypoints[i].position);
        }
        target = 0;
        MakeGreen(target);
        MakeYellow(NextTarget());
        rotationPosition = MyVector3.Subtract(position, waypoints_position[target]).ToCylindrical();
        movementDirection = MyVector3.DirectionalUnitVector(position, waypoints_position[target]);
        rotating = false;
    }

    public void FixedUpdate()
    {
        if (rotating)
        {//the ball is rotating around a waypoint
            //angle ball->current waypoint->next waypoint at the end of the previous update (clockwise winding)
            float prevAngle = MyVector3_Cylindrical.DeltaAngleRadiants(rotationPosition, directionConcesutiveWaypoints);
            int sign = clockwise ? -1 : 1;
            float dAngle = angularVelocity * Time.fixedDeltaTime * sign;
            rotationPosition.RotateByAngle(dAngle);
            //angle ball->current waypoint->next waypoint at the current update
            float angle = MyVector3_Cylindrical.DeltaAngleRadiants(rotationPosition, directionConcesutiveWaypoints);
            if ((!clockwise && prevAngle > Mathf.PI && angle < Mathf.PI) 
                || (clockwise && prevAngle < Mathf.PI && angle > Mathf.PI)
                ||dAngle > Mathf.PI)
            {//the ball has moved past the next movement direction on this frame
                SetLinearMovementVariables();
            }
            else
            {//the ball has NOT crossed the next movement direction on this frame
                position = MyVector3.Add(rotationPosition.ToCartesian(), waypoints_position[target]);
            }
        }
        else
        {
            if (MyVector3.CloserThanRadius(position, waypoints_position[target], rotationRadius))
            {//the ball is closer to the waypoint than the specified radius
                SetRotationVariables();
            }
            else
            {
                MyVector3 deltaPos = movementDirection.Scale(Time.fixedDeltaTime * velocity);
                float distanceFromWaypoint = MyVector3.Subtract(waypoints_position[target], position).Magnitude();
                if (distanceFromWaypoint - deltaPos.Magnitude() <= rotationRadius)
                {//the ball has moved closer than the specified radius on this frame
                    position = MyVector3.Subtract(waypoints_position[target], movementDirection.Scale(rotationRadius));
                    SetRotationVariables();
                }
                else
                {//the ball has NOT moved closer than the specified radius on this frame
                    position = MyVector3.Add(position, deltaPos);
                }
            }
        }
        //place object in the scene
        MoveGameObject();
    }

    private MyVector3 ToMyVector3(Vector3 v)
    {
        return new MyVector3(v.x, v.y, v.z);
    }

    private int NextTarget()
    {
        return (target + 1) % waypoints_position.Length;
    }

    private void MoveGameObject()
    {
        gameObject.transform.position = new Vector3(
            position.getX(), position.getY(), position.getZ());
    }

    private void SetRotationVariables()
    {
        directionConcesutiveWaypoints = MyVector3.Subtract(waypoints_position[NextTarget()], waypoints_position[target]).ToCylindrical();
        rotationPosition = MyVector3.Subtract(position, waypoints_position[target]).ToCylindrical();
        if (rotationPosition.getRadius() > 0)
        {
            angularVelocity = velocity / rotationPosition.getRadius();
            rotating = true;
            float angle = MyVector3_Cylindrical.DeltaAngleRadiants(rotationPosition, directionConcesutiveWaypoints);
            //choose the rotation direction so that the bll always travels along the longest arc
            if (angle >= Mathf.PI)
                clockwise = true;
            else
                clockwise = false;
        }
        else
        {//the ball is exactly on top of the waypoint.
         //No rotation occurs in this case and the ball moves towards the next waypoint
            MakeRed(target);
            target = NextTarget();
            MakeGreen(target);
            MakeYellow(NextTarget());
            movementDirection = MyVector3.DirectionalUnitVector(position, waypoints_position[target]);
        }
    }

    private void SetLinearMovementVariables()
    {
        rotationPosition = new MyVector3_Cylindrical(
            rotationPosition.getRadius(),
            directionConcesutiveWaypoints.getAzmuthalRad(), //place ball exctly between the current and the next waypoint
            rotationPosition.getY());
        rotating = false;
        position = MyVector3.Add(rotationPosition.ToCartesian(), waypoints_position[target]);
        MakeRed(target);
        target = NextTarget();
        MakeGreen(target);
        MakeYellow(NextTarget());
        movementDirection = MyVector3.DirectionalUnitVector(position, waypoints_position[target]);
    }

    private void MakeGreen(int index)
    {
        waypoints[index].gameObject.GetComponent<MeshRenderer>().material = green;
    }
    private void MakeRed(int index)
    {
        waypoints[index].gameObject.GetComponent<MeshRenderer>().material = red;
    }
    private void MakeYellow(int index)
    {
        waypoints[index].gameObject.GetComponent<MeshRenderer>().material = yellow;
    }
}

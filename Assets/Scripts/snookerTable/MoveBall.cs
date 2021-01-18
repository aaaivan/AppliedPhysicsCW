using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBall : MonoBehaviour
{
    [Header("Velocities in m/s")]
    public float initialVelocityX;
    public float initialVelocityZ;

    MyVector3 position;
    MyVector3 linearMomentum;
    Plane[] cushions;

    bool startSimulation = false;

    void Start()
    {
        position = ToMyVector3(gameObject.transform.position);
        linearMomentum = new MyVector3(initialVelocityX, 0, initialVelocityZ).Scale(SnookerBall.mass);
        cushions = new Plane[]
        {//four planes representing the boundaries of the pool table
            new Plane(new MyVector3(1,0,0), new MyVector3(-2, 0, 0)),
            new Plane(new MyVector3(0,0,1), new MyVector3(0, 0, -1)),
            new Plane(new MyVector3(-1,0,0), new MyVector3(2, 0, 0)),
            new Plane(new MyVector3(0,0,-1), new MyVector3(0, 0, 1)),
        };
    }

    private void FixedUpdate()
    {
        //simulation starts upon pressing the spacebar
        if (Input.GetKeyDown(KeyCode.Space) == true)
            startSimulation = true;
        if (!startSimulation)
            return;

        float t = Time.fixedDeltaTime;
        MyVector3 previousLinearMomentum = linearMomentum;
        MyVector3 linearMomentumVariation = linearMomentum.UnitVector().Scale(
            -SnookerBall.NormalForce * SnookerBall.frictionCoeff * t);
        if(linearMomentumVariation.Magnitude()>= linearMomentum.Magnitude())
        {//the decrease in linear momentum is bigger than the linear momentum itself
            linearMomentum = MyVector3.Zero();
        }
        else
        {
            linearMomentum = MyVector3.Add(linearMomentum, linearMomentumVariation);
        }
        MyVector3 avgLinearMomentum = MyVector3.Add(linearMomentum, previousLinearMomentum).Scale(0.5f);
        MyVector3 avgVelocity = avgLinearMomentum.Scale(1 / SnookerBall.mass);
        MyVector3 newPosition = MyVector3.Add(position, avgVelocity.Scale(t));
        foreach(Plane plane in cushions){
            float distanceFromPlane= newPosition.DistanceFromPlane(plane) - SnookerBall.radius;
            if (distanceFromPlane < 0)
            {//the ball has gone beyond the boundary
                //collision detected: the collision has happened anytime between the previous frame and this one.
                //find how much time has past since the collision
                float impactTime =
                   ImpactTime(
                       MyVector3.Dot(previousLinearMomentum, plane.Normal()),
                       - MyVector3.Dot(avgLinearMomentum.UnitVector(), plane.Normal()) * SnookerBall.NormalForce * SnookerBall.frictionCoeff,
                       position.DistanceFromPlane(plane) - SnookerBall.radius);
                linearMomentumVariation= previousLinearMomentum.UnitVector().Scale(
                   -SnookerBall.NormalForce * SnookerBall.frictionCoeff * impactTime);
                //reflect the linear momentum along the normal of the plane
                linearMomentum = MyVector3.Add(previousLinearMomentum, linearMomentumVariation).Reflect(plane.Normal());
                //position ball just within the boundaries
                newPosition = MyVector3.Add(newPosition, plane.Normal().Scale(-distanceFromPlane));
            }
        }
        position = newPosition;
        MoveGameObject();
    }

    private MyVector3 ToMyVector3(Vector3 v)
    {
        return new MyVector3(
            v.x,
            v.y,
            v.z);
    }

    private float ImpactTime(float perpendicularMomentum, float perpendicularForce, float perpendicularDistanceFromImpact)
    {
        // 0 = d*m + p°*t + 1/2*F°*(t)^2 kinematic equation: solve for t (time)
        //d= distance from cushion
        //m= mass of the ball
        //p°= component of the momentum perpendicular to the cushion
        //F°= component of the friction force perpendicular to the cushion
        float square = perpendicularMomentum * perpendicularMomentum
            - 2 * perpendicularForce * perpendicularDistanceFromImpact * SnookerBall.mass;
        if (square < 0)
            square = 0;
        float impactTime = (
                -perpendicularMomentum
                -Mathf.Sqrt(square) 
            ) / perpendicularForce;
        return impactTime;
    }

    //reposition game object in the scene
    private void MoveGameObject()
    {
        gameObject.transform.position = new Vector3(
            position.getX(),
            position.getY(),
            position.getZ());
    }

    public MyVector3 GetPosition()
    {
        return position;
    }
    public MyVector3 GetVelocity()
    {
        return linearMomentum.Scale(1 / SnookerBall.mass);
    }
    public MyVector3 GetLinearMomentum()
    {
        return linearMomentum;
    }
    public void SetLinearMomentum(MyVector3 momentum)
    {
        linearMomentum= momentum;
    }

    //relative velocity of ball 2 with respect of ball 1
    public static MyVector3 RelativeVelocity(MoveBall b2, MoveBall b1)
    {
        return MyVector3.Subtract(b2.linearMomentum, b1.linearMomentum).Scale(1 / SnookerBall.mass);
    }
    //relative position of ball 2 with respect of ball 1
    public static MyVector3 RelativePosition(MoveBall b2, MoveBall b1)
    {
        return MyVector3.Subtract(b2.position, b1.position);
    }
    //move the ball by the specified vector
    public void MoveByVector(MyVector3 v)
    {
        position = MyVector3.Add(v, position);
    }
}

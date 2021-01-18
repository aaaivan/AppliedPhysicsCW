using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingBall : MonoBehaviour
{
    public float initialSpeed_X;
    public float initialSpeed_Y;
    public float initialSpeed_Z;
    //[Range(0, 10)]
    //public float initialSpeed;
    //[Range(0, 180)]
    //public float initialAngle;
    [Range(0, 1)]
    public float restitution;
    [Range(0, 20)]
    public float mass;
    [Range(0, 1)]
    public float floorFriction;

    MyVector3 position;
    MyVector3 velocityCurrentFrame;
    MyVector3 velocityPreviousFrame;
    MyVector3 angularMomentumPreviousFrame;
    MyVector3 angularMomentumCurrentFrame;
    bool pureRolling; //true when linearSpeed=ANgularSpeed*radius 
    bool stopBouncing; //true when th ebounces become small enough to be ignored
    bool fullStop; //true when all motions of the ball (rotation, bouncing, sliding) become small enough to be ignored
    float tresholdStopBouncing; //vertical speed below which the ball should be treated as if it is not bouncing
    Plane ground;
    float radius = 0.1f;
    float momentOfInertia; // 2/5 * M * R^2 for a solid sphere
    MyVector3 gravity = new MyVector3(0, -9.81f, 0);

    bool startSimulation = false;

    void Start()
    {
        if (initialSpeed_X==0 && initialSpeed_Z==0)
        {
            pureRolling = true;
        }
        else
        {
            pureRolling = false;
        }
        velocityCurrentFrame = new MyVector3(initialSpeed_X, initialSpeed_Y, initialSpeed_Z);
        velocityPreviousFrame = velocityCurrentFrame;
        position = ToMyVector3(gameObject.transform.position);
        ground = new Plane(new MyVector3(0, 1, 0), new MyVector3(0, -radius, 0));
        angularMomentumPreviousFrame = MyVector3.Zero();
        angularMomentumCurrentFrame = angularMomentumPreviousFrame;
        momentOfInertia = 2.0f / 5.0f * mass * radius * radius;
        stopBouncing = false;
        fullStop = false;
        tresholdStopBouncing = 0.01f;
    }

    private void FixedUpdate()
    {
        //simulation start upon press of the spacebar
        if (Input.GetKeyDown(KeyCode.Space) == true)
            startSimulation = true;
        if (!startSimulation)
            return;

        if (fullStop)
            return; //simulation ended

        float t = Time.fixedDeltaTime;
        if (stopBouncing)
        {
            if (pureRolling)
            {//case 1: the ball is rolling on the floor without sliding
                float dumping = 1 -
                    (5-4/(1+mass)) *
                    (floorFriction / (4 * floorFriction + 6)) *
                    (1 + 49 / (1 + 10 * Mathf.Abs(velocityPreviousFrame.Magnitude()))) *
                    t; //arbitrary dumping factor. No real physical meaning.
                velocityCurrentFrame = velocityPreviousFrame.Scale(dumping);
                if (velocityCurrentFrame.Magnitude() < 0.01f)
                {//the velocity is low enough to be neglectable
                    velocityCurrentFrame = MyVector3.Zero();
                    fullStop = true;
                }
                //calculate the avarage velocity between two consecutive frames and use it to calculate the new position
                MyVector3 avgVelocity = MyVector3.Lerp(velocityPreviousFrame, velocityCurrentFrame, 0.5f);
                MyVector3 displacement = avgVelocity.Scale(t);
                position = MyVector3.Add(position, displacement);
                MoveGameObject();
                angularMomentumCurrentFrame= MyVector3.Cross(velocityCurrentFrame, new MyVector3(0, -1, 0)).Scale(momentOfInertia / radius);
                RotateGameObject(t);
                velocityPreviousFrame = velocityCurrentFrame;
                angularMomentumPreviousFrame = angularMomentumCurrentFrame;
            }
            else
            {//case 2: the ball is rolling on the floor with sliding 
                velocityCurrentFrame = HorizontalVelocityAfterSliding(t);
                //calculate the avarage velocity between two consecutive frames and use it to calculate the new position
                MyVector3 avgVelocity = MyVector3.Lerp(velocityPreviousFrame, velocityCurrentFrame, 0.5f);
                MyVector3 displacement = avgVelocity.Scale(t);
                position = MyVector3.Add(position, displacement);
                MoveGameObject();
                //angularMomentumCurrentFrame set in the X_VelocityAfterSliding() function
                RotateGameObject(t);
                velocityPreviousFrame = velocityCurrentFrame;
                angularMomentumPreviousFrame = angularMomentumCurrentFrame;
            }
        }
        else
        {//the ball is still bouncing
            MyVector3 deltaVel = gravity.Scale(t);
            velocityCurrentFrame = MyVector3.Add(velocityPreviousFrame, deltaVel);
            //calculate the avarage velocity between two consecutive frames and use it to calculate the new position
            MyVector3 avgVelocity = MyVector3.Lerp(velocityPreviousFrame, velocityCurrentFrame, 0.5f);
            MyVector3 displacement = avgVelocity.Scale(t);
            MyVector3 newPosition = MyVector3.Add(position, displacement);
            float distanceFromGround = newPosition.DistanceFromPlane(ground) - radius;
            if (distanceFromGround < 0)
            {//case 3: the ball is bouncing and has partially fallen balow the plane of the ground
                float impactVel_y = Y_ImpactVelocity();
                float velocityCurrentFrame_y = impactVel_y * restitution; //upwards speed after bounce
                if (velocityCurrentFrame_y < tresholdStopBouncing)
                {//the speed after the bounce is small enough to be ignored
                    stopBouncing = true;
                    velocityCurrentFrame_y = 0;
                }
                MyVector3 horzVelocityCurrentFrame = HorizontalVelocityAfterSliding(t + 0.001f * impactVel_y + 8 * t * (1 - 1 / Mathf.Sqrt(1 + mass)));
                //position ball on top of the ground
                position = MyVector3.Add(newPosition, ground.Normal().Scale(-distanceFromGround));
                MoveGameObject();
                //angularMomentumCurrentFrame set in the X_VelocityAfterSliding() function
                RotateGameObject(t);
                angularMomentumPreviousFrame = angularMomentumCurrentFrame;
                velocityPreviousFrame = new MyVector3(horzVelocityCurrentFrame.getX(), velocityCurrentFrame_y, horzVelocityCurrentFrame.getZ());
            }
            else
            {//case 4: the ball is bouncing and is in mid-air
                velocityPreviousFrame = velocityCurrentFrame;
                position = newPosition;
                MoveGameObject();
                RotateGameObject(t);
            }
        }
    }

    //convets Vector3 to a MyVector3
    private MyVector3 ToMyVector3(Vector3 v)
    {
        return new MyVector3(v.x, v.y, v.z);
    }

    //reposition gameObject in the scene
    private void MoveGameObject()
    {
        gameObject.transform.position = new Vector3(
            position.getX(),
            position.getY(),
            position.getZ());
    }

    //reorient the gameObject in the scene
    private void RotateGameObject(float deltaTime)
    {
        MyVector3 avgAngularMomentum= MyVector3.Lerp(angularMomentumPreviousFrame, angularMomentumCurrentFrame, 0.5f);
        MyVector3 dir = avgAngularMomentum.UnitVector();
        float rotationAngle = AngularVelocity(avgAngularMomentum).Magnitude() * deltaTime*(-1);
        Vector3 directionV3 = new Vector3(dir.getX(), dir.getY(), dir.getZ());
        gameObject.transform.Rotate(directionV3, rotationAngle * 180/Mathf.PI, Space.World);
    }

    //calculate the vertical velocity at the moment of impact
    private float Y_ImpactVelocity()
    {// (V)^2 = (V°)^2  - 2 * g * X°
        float Y_impactVelSquared = velocityPreviousFrame.getY() * velocityPreviousFrame.getY() -
            2 * gravity.getY() * (position.DistanceFromPlane(ground)-radius);
        if (Y_impactVelSquared < 0)
            Y_impactVelSquared = 0;
        return Mathf.Sqrt(Y_impactVelSquared);
    }

    //calculate the x velocity resulting from the ball sliding on the floor for a time deltaTime
    private MyVector3 HorizontalVelocityAfterSliding(float deltaTime)
    {
        if (pureRolling)
            return velocityPreviousFrame;
        MyVector3 afterSlidingVel;
        MyVector3 horizontal_velocityPreviousFrame = 
            new MyVector3(velocityPreviousFrame.getX(), 0, velocityPreviousFrame.getZ());
        //calculate the variation in the angular momentum and x component of the linear momentum
        float arbitraryConst1 = 5f;
        MyVector3 frictionDirection = horizontal_velocityPreviousFrame.Scale(-1).UnitVector();
        MyVector3 linearMomentumVariation = frictionDirection.Scale(
            arbitraryConst1 * floorFriction * mass * deltaTime);
        MyVector3 angularMomentumVariation = MyVector3.Cross(
            new MyVector3(0, -radius, 0),
            linearMomentumVariation);
        afterSlidingVel = MyVector3.Add(horizontal_velocityPreviousFrame, linearMomentumVariation.Scale(1/mass));
        angularMomentumCurrentFrame = MyVector3.Add(angularMomentumCurrentFrame, angularMomentumVariation);
        MyVector3 contactPointTangentVel = MyVector3.Cross(AngularVelocity(angularMomentumCurrentFrame), new MyVector3(0, -radius, 0));
        if (contactPointTangentVel.Magnitude() > afterSlidingVel.Magnitude())
        {
            pureRolling = true;
            //calculate the rolling horizintal velocity
            //pure rolling law: speed = (radius) * (angular vel)
            float MRsquared_over_I = (mass * radius * radius) / momentOfInertia;
            afterSlidingVel =
                MyVector3.Subtract(afterSlidingVel.Scale(MRsquared_over_I), contactPointTangentVel).Scale(1/(1 + MRsquared_over_I));
            angularMomentumCurrentFrame = MyVector3.Cross(afterSlidingVel, new MyVector3(0, -1, 0)).Scale(momentOfInertia / radius);
        }
        return afterSlidingVel;
    }

    //calculate the angular velocity from the eangular momentum
    private MyVector3 AngularVelocity(MyVector3 _angularMomentum)
    {// Angular velocity = - (L / I)
        return _angularMomentum.Scale(1/momentOfInertia);
    }
}

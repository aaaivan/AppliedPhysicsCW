using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCollisions : MonoBehaviour
{
    public MoveBall[] balls;

    private void FixedUpdate()
    {
        //check collision between any pair of balls
        for (int i=0;i<balls.Length-1; i++)
        {
            for (int j=i+1; j < balls.Length; j++)
            {
                if(MyVector3.CloserThanRadius(balls[i].GetPosition(), balls[j].GetPosition(), 2 * SnookerBall.radius))
                {//the distance between the centers of the two balls is less than twice the radius
                 //this means there is a collision
                    HandleCollision(balls[i], balls[j]);
                }
            }
        }
    }

    private void HandleCollision(MoveBall b1, MoveBall b2)
    {//collision detected: the collision has happened anytime between the previous frame and this one.
        //find how much time has past since the the collision
        MyVector3 relVelocity = MoveBall.RelativeVelocity(b2, b1); // V° rel velocity (const)
        MyVector3 relPosition = MoveBall.RelativePosition(b2, b1); // P° rel position at t=0
        //  relative position P(t) = P° - V° * t
        //  the collision happens when |P(t)| = 2 * radius
        //  <P(t)|P(t)> = <(P° - V° * t)|(P° - V° * t)> = 4 * radius^2
        //  solve the above quadratic eq. for t
        float term1 = MyVector3.Dot(relVelocity, relPosition);
        float term2 = MyVector3.Dot(relPosition, relPosition);
        float term3 = MyVector3.Dot(relVelocity, relVelocity);
        float rootSquared = term1 * term1 - term3 * (term2 - 4*SnookerBall.radius * SnookerBall.radius);
        if (rootSquared < 0)
            rootSquared = 0;
        float timeAfterCollision = (term1 + Mathf.Sqrt(rootSquared)) / term3;
        b1.MoveByVector(b1.GetVelocity().Scale(-timeAfterCollision));//minus sign because we are moving the ball back in time
        b2.MoveByVector(b2.GetVelocity().Scale(-timeAfterCollision));//minus sign because we are moving the ball back in time
        relPosition = MoveBall.RelativePosition(b2, b1); //vector joining the centers of the 2 balls
        MyVector3 b1_parallel = b1.GetLinearMomentum().ParallelComponent(relPosition);
        MyVector3 b1_perpendicular = b1.GetLinearMomentum().NormalComponent(relPosition);
        MyVector3 b2_parallel = b2.GetLinearMomentum().ParallelComponent(relPosition);
        MyVector3 b2_perpendicular = b2.GetLinearMomentum().NormalComponent(relPosition);
        //the two ball exchange the parallel components of their linear momenta
        //this is only valid in the case of the masses being the same
        b1.SetLinearMomentum(MyVector3.Add(b1_perpendicular, b2_parallel));
        b2.SetLinearMomentum(MyVector3.Add(b2_perpendicular, b1_parallel));
    }
}

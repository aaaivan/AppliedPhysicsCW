using System.Collections;
using System.Collections.Generic;


public class Plane
{
    float xCoeff;
    float yCoeff;
    float zCoeff;
    float constTerm;

    //define plane from the coefficients of its equation
    public Plane(float xCoeff, float yCoeff, float zCoeff, float constTerm)
    {
        MyVector3 normal = new MyVector3(xCoeff, yCoeff, zCoeff);
        float magnitude = normal.Magnitude();
        //normalize coefficients
        this.xCoeff = xCoeff / magnitude;
        this.yCoeff = yCoeff / magnitude;
        this.zCoeff = zCoeff / magnitude;
        this.constTerm = constTerm / magnitude;
    }

    //define plane from its normal and a point in the plane
    public Plane(MyVector3 normal, MyVector3 point)
    {
        normal = normal.UnitVector();
        xCoeff = normal.getX();
        yCoeff = normal.getY();
        zCoeff = normal.getZ();
        constTerm = -MyVector3.Dot(normal, point);
    }

    public float getConstTerm() { return constTerm; }

    public MyVector3 Normal()
    {
        return new MyVector3(xCoeff, yCoeff, zCoeff);
    }

    static public Plane XY()
    {
        return new Plane(new MyVector3(0, 0, 1), MyVector3.Zero());
    }
    static public Plane XZ()
    {
        return new Plane(new MyVector3(0, 1, 0), MyVector3.Zero());
    }
    static public Plane YZ()
    {
        return new Plane(new MyVector3(1, 0, 0), MyVector3.Zero());
    }

}

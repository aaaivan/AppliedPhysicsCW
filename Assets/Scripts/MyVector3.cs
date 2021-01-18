using System;
using System.Diagnostics;

public class MyVector3
{
    float x;
    float y;
    float z;

    public MyVector3(float x, float y, float z)
    {
        this.x= x;
        this.y = y;
        this.z = z;
    }

    public float getX() { return x; }
    public float getY() { return y; }
    public float getZ() { return z; }

    static public MyVector3 Add(MyVector3 v1, MyVector3 v2)
    {
        return new MyVector3(
            v1.x + v2.x,
            v1.y + v2.y,
            v1.z + v2.z);
    }

    static public MyVector3 Subtract(MyVector3 v1, MyVector3 v2)
    {
        return new MyVector3(
            v1.x - v2.x,
            v1.y - v2.y,
            v1.z - v2.z);
    }

    public MyVector3 Scale(float scalar)
    {
        return new MyVector3(
            x * scalar,
            y * scalar,
            z * scalar);
    }

    //dot product
    static public float Dot(MyVector3 v1, MyVector3 v2)
    {
        return v1.x * v2.x + v1.y * v2.y + v1.z * v2.z;
    }

    //cross product for left-handed system
    static public MyVector3 Cross(MyVector3 v1, MyVector3 v2)
    {
        return new MyVector3(
            v1.getZ() * v2.getY() - v1.getY() * v2.getZ(),
            v1.getX() * v2.getZ() - v1.getZ() * v2.getX(),
            v1.getY() * v2.getX() - v1.getX() * v2.getY());
    }

    public float Magnitude()
    {
        float normSquared =
            x * x +
            y * y +
            z * z;
        return (float)Math.Sqrt(normSquared);
    }

    static public MyVector3 Zero()
    {
        return new MyVector3(0, 0, 0);
    }

    public MyVector3 UnitVector()
    {
        float norm = Magnitude();
        try
        {
            if (norm <= 0)
            {
                throw new Exception("Non-positive norm!");
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine(e.Message);
            return Zero();
        }

        return new MyVector3(
            x / norm,
            y / norm,
            z / norm);
    }

    public MyVector3 ReflectX()
    {
        return new MyVector3(
            -x,
            y,
            z);
    }

    public MyVector3 ReflectY()
    {
        return new MyVector3(
            x,
            -y,
            z);
    }

    public MyVector3 ReflectZ()
    {
        return new MyVector3(
            x,
            y,
            -z);
    }

    //reflect along the the specified direction
    public MyVector3 Reflect(MyVector3 normal)
    {
        normal = normal.UnitVector();
        MyVector3 parallel = normal.Scale(Dot(this, normal));
        MyVector3 perpendicular= Subtract(this, parallel);
        return Add(perpendicular, parallel.Scale(-1));
    }

    //calculate the component parallel to the specified direction
    public MyVector3 ParallelComponent(MyVector3 direction)
    {
        direction = direction.UnitVector();
        return direction.Scale(Dot(this, direction));
    }

    //calculate the direction perpendicular to the specified component
    public MyVector3 NormalComponent(MyVector3 direction)
    {
        direction = direction.UnitVector();
        MyVector3 parallel= direction.Scale(Dot(this, direction));
        return Subtract(this, parallel);

    }

    //convert the cartesian vector to cylindrical coordinates
    public MyVector3_Cylindrical ToCylindrical()
    {
        MyVector3 xzProjection = new MyVector3(x, 0, z);
        float radius = xzProjection.Magnitude();
        float azimuthalAngle = 0;
        if (radius > 0)
        {
            azimuthalAngle = (float)Math.Acos(x / radius);
            if (z < 0)
                azimuthalAngle = 2 * (float)Math.PI - azimuthalAngle;
        }

        return new MyVector3_Cylindrical(
            radius,
            azimuthalAngle,
            y);
    }

    //convert the cartesian vector to spherical coordinates
    public MyVector3_Spherical ToSpherical()
    {
        float radius = Magnitude();
        float polarAngle = 0; 
        float azimuthalAngle = 0;
        if (radius > 0)
        {
            polarAngle = (float)Math.Acos(y / radius);
        }
        if (polarAngle > 0)
        {
            azimuthalAngle = (float)Math.Acos(x / (radius * Math.Sin(polarAngle)));
            if (z < 0)
                azimuthalAngle = 2 * (float)Math.PI - azimuthalAngle;
        }

        return new MyVector3_Spherical(
        radius,
        azimuthalAngle,
        polarAngle);
    }

    //direction of the vector going from pos1 to pos 2
    static public MyVector3 DirectionalUnitVector(MyVector3 pos1, MyVector3 pos2)
    {
        MyVector3 directionalVect = Subtract(pos2, pos1);
        return directionalVect.UnitVector();
    }

    //check whether the vectors pos1 and pos2 are closer than the specified radius
    static public bool CloserThanRadius(MyVector3 pos1, MyVector3 pos2, float radius)
    {
        MyVector3 directionalVect = Subtract(pos2, pos1);
        return directionalVect.Magnitude() <= radius;
    }

    //distance from the plane of the point pointed by the vector
    public float DistanceFromPlane(Plane plane)
    {
        float distance = Dot(plane.Normal(), this) + plane.getConstTerm();
        if (distance < 0.000001f)
            distance = 0;
        return distance;
    }

    //check if the point pointed by the vector is closer to the plane than the tolerance
    public bool IsOnPlane(Plane plane, float tolerance)
    {
        return Math.Abs(DistanceFromPlane(plane)) <= tolerance;
    }

    //distance from the line of the point pointed by the vector
    public float DistanceFromLine(Line line)
    {
        MyVector3 relVect = Subtract(this, line.getPointOnLine());
        float leg = Dot(relVect, line.getDirection());
        float dSquared = Dot(relVect, relVect) - (leg * leg);
        if (dSquared < 0.000001f)
            dSquared = 0;
        return (float)Math.Sqrt(dSquared);
    }

    //check if the point pointed by the vector is closer to the line than the tolerance
    public bool IsOnLine(Line line, float tolerance)
    {
        return DistanceFromLine(line) <= tolerance;
    }

    static public MyVector3 Lerp(MyVector3 v1, MyVector3 v2, float f)
    {
        if (f < 0)
            f = 0;
        else if (f > 1)
            f = 1;

        MyVector3 v = Add(v1.Scale(1-f), v2.Scale(f));
        return v;
    }


    public string Print()
    {
        string s = "( ";
        s += x.ToString("0.00");
        s += ", ";
        s += y.ToString("0.00");
        s += ", ";
        s += z.ToString("0.00");
        s += " )";
        return s;
    }

}

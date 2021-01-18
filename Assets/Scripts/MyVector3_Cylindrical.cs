using System;

public class MyVector3_Cylindrical
{
    float radius;
    float azimuthal;
    float y;

    public MyVector3_Cylindrical(float radius, float azimuthal, float y)
    {
        //make sure the angle is between 0 and 2Pi
        while (azimuthal >= 2 * (float)Math.PI)
        {
            azimuthal -= 2 * (float)Math.PI;
        }
        while (azimuthal <0)
        {
            azimuthal += 2 * (float)Math.PI;
        }
        this.radius = radius;
        this.azimuthal = azimuthal;
        this.y = y;
    }

    public float getRadius() { return radius; }
    public float getAzmuthalRad() { return azimuthal; }
    public float getAzmuthalDeg() { return azimuthal*180/(float)Math.PI; }
    public float getY() { return y; }

    //convert cylindrical vector to cartesian
    public MyVector3 ToCartesian()
    {
        return new MyVector3(
            radius * (float)Math.Cos(azimuthal),
            y,
            radius * (float)Math.Sin(azimuthal));
    }

    //convert cylindrical vector to spherical
    public MyVector3_Spherical ToSpherical()
    {
        return ToCartesian().ToSpherical();
    }

    static public MyVector3_Cylindrical Zero()
    {
        return new MyVector3_Cylindrical(0, 0, 0);
    }

    //difference in the azimuthal angles of the two given vectors
    static public float DeltaAngleRadiants(MyVector3_Cylindrical v1, MyVector3_Cylindrical v2)
    {
        float dAngle= v1.azimuthal - v2.azimuthal;
        if (dAngle < 0)
            dAngle = dAngle + (float)Math.PI*2;
        return dAngle;
    }

    public void RotateByAngle(float angle)
    {
        azimuthal += angle;
        while (azimuthal >= 2 * (float)Math.PI)
        {
            azimuthal -= 2 * (float)Math.PI;
        }
        while (azimuthal < 0)
        {
            azimuthal += 2 * (float)Math.PI;
        }
    }
}

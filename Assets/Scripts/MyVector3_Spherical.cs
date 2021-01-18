using System;

public class MyVector3_Spherical
{
    float radius;
    float azimuthal;
    float polar;

    public MyVector3_Spherical(float radius, float azimuthal, float polar)
    {
        //make sure the polar angle is between 0 and Pi
        while (polar >= 2 * (float)Math.PI)
        {
            polar -= 2 * (float)Math.PI;
        }
        while (polar < 0)
        {
            polar += 2 * (float)Math.PI;
        }
        if (polar> (float)Math.PI)
        {
            polar = 2 * (float)Math.PI - polar;
            azimuthal += (float)Math.PI;
        }
        //make sure the polar angle is between 0 and 2Pi
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
        this.polar = polar;
    }

    public float getRadius() { return radius; }
    public float getAzmuthalRad() { return azimuthal; }
    public float getAzmuthalDeg() { return azimuthal * 180 / (float)Math.PI; }
    public float getPolarRad() { return polar; }
    public float getPolarDeg() { return polar * 180 / (float)Math.PI; }

    //convert spherical vector to cartesian
    public MyVector3 ToCartesian()
    {
        return new MyVector3(
            radius * (float)(Math.Sin(polar) * Math.Cos(azimuthal)),
            radius * (float)Math.Cos(polar),
            radius * (float)(Math.Sin(polar) * Math.Sin(azimuthal)));
    }

    //convert spherical vector to cylindrical
    public MyVector3_Cylindrical ToCylindrical()
    {
        return ToCartesian().ToCylindrical();
    }

    static public MyVector3_Spherical Zero()
    {
        return new MyVector3_Spherical(0, 0, 0);
    }
}

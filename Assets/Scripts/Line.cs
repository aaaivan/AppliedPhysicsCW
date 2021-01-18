using System.Collections;
using System.Collections.Generic;

public class Line
{
    MyVector3 direction;
    MyVector3 pointOnLine;

    //define line from its direction and a point on the line
    public Line(MyVector3 direction, MyVector3 pointOnLine)
    {
        this.direction = direction.UnitVector();
        this.pointOnLine = pointOnLine;
    }
    
    public MyVector3 getPointOnLine()
    {
        return pointOnLine;
    }

    public MyVector3 getDirection()
    {
        return direction;
    }
}

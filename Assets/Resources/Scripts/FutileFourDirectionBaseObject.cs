using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class FutileFourDirectionBaseObject : FutilePlatformerBaseObject
{
    public enum Direction
    {
        UP,
        RIGHT,
        DOWN,
        LEFT
    }
    protected Direction _direction = Direction.UP;

    public FutileFourDirectionBaseObject(RXRect hitBox, World world): base(hitBox, world)
    {

    }
}

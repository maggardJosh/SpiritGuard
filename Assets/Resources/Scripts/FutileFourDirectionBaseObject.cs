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

    public override void OnFixedUpdate()
    {
        if (xAcc > 0)
            _direction = Direction.RIGHT;
        else if (xAcc < 0)
            _direction = Direction.LEFT;
        else if (yAcc > 0)
            _direction = Direction.UP;
        else if (yAcc < 0)
            _direction = Direction.DOWN;
        base.OnFixedUpdate();
    }
}

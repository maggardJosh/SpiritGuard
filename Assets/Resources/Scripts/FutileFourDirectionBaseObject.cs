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
    protected Direction _direction = Direction.RIGHT;
    public void SetDirection(Direction newDirection)
    {
        _direction = newDirection;
    }

    public FutileFourDirectionBaseObject(RXRect hitBox, World world): base(hitBox, world)
    {

    }

    public override void OnFixedUpdate()
    {
        if (yAcc > 0)
            _direction = Direction.UP;
        else if (yAcc < 0)
            _direction = Direction.DOWN;
        else if (xAcc > 0)
            _direction = Direction.RIGHT;
        else if (xAcc < 0)
            _direction = Direction.LEFT;
        base.OnFixedUpdate();
    }
}

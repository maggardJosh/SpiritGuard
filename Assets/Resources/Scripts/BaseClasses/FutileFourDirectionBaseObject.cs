using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

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
    public Direction CurrentDirection { get { return _direction; } }
    public void SetDirection(Direction newDirection)
    {
        _direction = newDirection;
    }

    public FutileFourDirectionBaseObject(RXRect hitBox, World world): base(hitBox, world)
    {

    }

    public override void OnFixedUpdate()
    {
        if(Mathf.Abs(yAcc) > Mathf.Abs(xAcc))
        {
        
        if (yAcc > 0)
            _direction = Direction.UP;
        else if (yAcc < 0)
            _direction = Direction.DOWN;
        }
        else if (xAcc > 0)
            _direction = Direction.RIGHT;
        else if (xAcc < 0)
            _direction = Direction.LEFT;
        base.OnFixedUpdate();
        if (_direction == Direction.UP || _direction == Direction.DOWN)
            scaleX = 1;
    }
}

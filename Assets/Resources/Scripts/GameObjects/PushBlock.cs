using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class PushBlock : FutilePlatformerBaseObject
{
    FSprite sprite;
    private float pushCount = 0;
    bool isBeingPushed = false;
    FutileFourDirectionBaseObject.Direction pushDir = FutileFourDirectionBaseObject.Direction.UP;
    public PushBlock(World world)
        : base(new RXRect(0, 0, 16, 16), world)
    {
        sprite = new FSprite("object_block_01");
        this.AddChild(sprite);
    }
    const float PUSH_TIME = .8f;
    public override void OnFixedUpdate()
    {
        if (!isBeingPushed)
            pushCount = 0;
        else
            pushCount += Time.deltaTime;
        isBeingPushed = false;
        if (pushCount > PUSH_TIME)
            TryPush();
        
        base.OnFixedUpdate();
    }

    public void TryPush()
    {
        float newX = this.x;
        float newY = this.y;
        switch(pushDir)
        {
            case FutileFourDirectionBaseObject.Direction.UP: newY += 16; break;
            case FutileFourDirectionBaseObject.Direction.RIGHT: newX += 16; break;
            case FutileFourDirectionBaseObject.Direction.DOWN: newY -= 16; break;
            case FutileFourDirectionBaseObject.Direction.LEFT: newX -= 16; break;
        }
        if (world.isPassable(newX, newY) && world.CheckObjectCollision(this, newX, newY)== null)
        {
            Go.to(this, 1.0f, new TweenConfig().floatProp("x", newX).floatProp("y", newY).setEaseType(EaseType.QuadInOut));
            isBeingPushed = false;
            pushCount = 0;
        }
    }

    public void HandlePlayerCollision(Player p)
    {
        isBeingPushed = true;
        if (p.x + p.hitBox.x > this.x  - 8 && p.x + p.hitBox.x< this.x  + 8)
        {
            if (p.y + p.hitBox.y > this.y)
                pushDir = FutileFourDirectionBaseObject.Direction.DOWN;
            else
                pushDir = FutileFourDirectionBaseObject.Direction.UP;
        }
        else if (p.x + p.hitBox.x < this.x)
            pushDir = FutileFourDirectionBaseObject.Direction.RIGHT;
        else
            pushDir = FutileFourDirectionBaseObject.Direction.LEFT;
        

    }

}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class PushBlock : FutilePlatformerBaseObject
{
    FSprite sprite;
    private float pushCount = 0;
    bool isMoving = false;
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

        if (isMoving)
            SpawnParticles(pushDir, 1);
        
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
        if (world.isPassable(newX, newY) && world.CheckObjectCollision(this, newX, newY)== null && !world.CheckDamageObjectCollision(this, newX, newY))
        {
            C.getCameraInstance().shake(.7f, .5f);
            isMoving = true;
            Go.to(this, 1.0f, new TweenConfig().floatProp("x", newX).floatProp("y", newY).setEaseType(EaseType.QuadInOut).onComplete(() => { isMoving = false; }));
            isBeingPushed = false;
            pushCount = 0;
        }
    }

    private void SpawnParticles(FutileFourDirectionBaseObject.Direction dir, int numParticles = 10)
    {
        RXDebug.Log(dir);
        for (int i = 0; i < numParticles; i++)
        {
            Particle.ParticleOne p = Particle.ParticleOne.getParticle();
            Vector2 vel = new Vector2(-RXRandom.Float() * 20 + 10, -RXRandom.Float() * 20);
            Vector2 acc = new Vector2(-vel.x * (RXRandom.Float() * .5f), -vel.y * -1.0f);
            Vector2 pos = new Vector2(RXRandom.Float() * 16 - 8, 8);
            switch (dir)
            {
                case FutileFourDirectionBaseObject.Direction.DOWN:
                    vel.y *= -1;
                    acc.y *= -1;
                    break;
                case FutileFourDirectionBaseObject.Direction.RIGHT:
                    float tempX = vel.x;
                    vel.x = vel.y;
                    vel.y = tempX;
                    tempX = acc.x;
                    acc.x = acc.y;
                    acc.y = tempX;
                    tempX = pos.x;
                    pos.x = pos.y;
                    pos.y = tempX;
                    pos.x *= -1;
                    break;
                case FutileFourDirectionBaseObject.Direction.UP:
                    pos.y *= -1;
                    break;
                case FutileFourDirectionBaseObject.Direction.LEFT:
                    tempX = vel.x;
                    vel.x = vel.y;
                    vel.y = tempX;
                    tempX = acc.x;
                    acc.x = acc.y;
                    acc.y = tempX;
                    tempX = pos.x;
                    pos.x = pos.y;
                    pos.y = tempX;
                    vel.x *= -1;
                    acc.x *= -1;
                    break;
            }
            p.activate(this.GetPosition() +pos, vel, acc, RXRandom.Bool() ? 180.0f : 0);
            this.container.AddChild(p);
        }
    }

    public void HandlePlayerCollision(Player p)
    {
        if (isMoving)
            return;
        isBeingPushed = true;
        if (Mathf.Round(p.x + p.hitBox.x + p.hitBox.width/2f) > this.x - 8 && Mathf.Round( p.x + p.hitBox.x - p.hitBox.width/2f) < this.x  + 8)
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


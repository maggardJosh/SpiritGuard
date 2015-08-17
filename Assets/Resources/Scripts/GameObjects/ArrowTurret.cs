using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ArrowTurret : FutileFourDirectionBaseObject
{
    private FAnimatedSprite sprite;
    private bool loaded = true;
    private float interval;
    private float initDelay = 0;
    public ArrowTurret(float interval, float initDelay, World world): base(new RXRect(0,-4,12,8), world)
    {
        this.interval = interval;
        handleStateCount = true;
        this.initDelay = initDelay;
        sprite = new FAnimatedSprite("Arrow Turret/turret_arrow");
        sprite.addAnimation(new FAnimation(Direction.DOWN.ToString() + true.ToString(), new int[] { 1}, 150, false));
        sprite.addAnimation(new FAnimation(Direction.UP.ToString() + true.ToString(), new int[] { 3 }, 150, false));
        sprite.addAnimation(new FAnimation(Direction.LEFT.ToString() + true.ToString(), new int[] { 5 }, 150, false));
        sprite.addAnimation(new FAnimation(Direction.RIGHT.ToString() + true.ToString(), new int[] { 5 }, 150, false));

        sprite.addAnimation(new FAnimation(Direction.DOWN.ToString() + false.ToString(), new int[] { 2 }, 150, false));
        sprite.addAnimation(new FAnimation(Direction.UP.ToString() + false.ToString(), new int[] { 4 }, 150, false));
        sprite.addAnimation(new FAnimation(Direction.LEFT.ToString() + false.ToString(), new int[] { 6 }, 150, false));
        sprite.addAnimation(new FAnimation(Direction.RIGHT.ToString() + false.ToString(), new int[] { 6 }, 150, false));

        this.AddChild(sprite);
        PlayAnim();

    }

    private float arrowSpeed = 2f;
    public override void OnFixedUpdate()
    {
        if (C.isTransitioning)
            return;
        if (initDelay > 0)
        {
            if (stateCount > initDelay)
            {
                initDelay = 0;
                stateCount = 0;
            }
            return;
        }
        if (loaded)
        {
            if (stateCount > .5f)
            {
                Arrow a = new Arrow(this, world);
                a.SetDirection(CurrentDirection);
                a.PlayAnim();
                switch(CurrentDirection)
                {
                    case Direction.UP: a.yVel = arrowSpeed; break;
                    case Direction.RIGHT: a.xVel = arrowSpeed; break;
                    case Direction.DOWN: a.yVel = -arrowSpeed; break;
                    case Direction.LEFT: a.xVel = -arrowSpeed; break;
                }
                a.SetPosition(this.GetPosition());
                world.addObject(a);
                loaded = false;
                stateCount = 0;
            }
        }
        else
        {
            if (stateCount > interval)
            {
                loaded = true;
                stateCount = 0;
            }
        }
        base.OnFixedUpdate();
        PlayAnim();
    }

    public void PlayAnim()
    {
        sprite.play(CurrentDirection.ToString() + loaded.ToString());
        switch(CurrentDirection)
        {
            case Direction.DOWN:
                hitBox.x = 0;
                hitBox.y = 0;
                hitBox.width = 12;
                hitBox.height = 12;
                break;
            case Direction.UP:
                hitBox.x = 0;
                hitBox.y = 0;
                hitBox.width = 12;
                hitBox.height = 12;
                break;
            case Direction.LEFT:
                hitBox.x = 0;
                hitBox.y = 0;
                hitBox.width = 12;
                hitBox.height = 12;
                scaleX = 1;
                break;
            case Direction.RIGHT:
                hitBox.x = 0;
                hitBox.y = 0;
                hitBox.width = 12;
                hitBox.height = 12;
                scaleX = -1;
                break;
        }

        UpdateHitBoxSprite();
    }
}


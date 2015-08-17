using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Arrow : FutileFourDirectionBaseObject
{
    public FNode owner;
    FAnimatedSprite sprite;
    bool firstHit = false;
    bool dying = false;
    private FNode stuckInThing;
    private Vector2 stuckInThingDisp;
    public Arrow(FNode owner, World world)
        : base(new RXRect(0, -5, 12, 5), world)
    {
        FSoundManager.PlaySound("arrowShoot");
        this.owner = owner;
        handleStateCount = true;
        this.collidesWithWater = false;
        this.handleDamageObjectCollision = true;
        sprite = new FAnimatedSprite("Arrow/object_arrow");
        sprite.addAnimation(new FAnimation(Direction.UP.ToString(), new int[] { 1 }, 150, false));
        sprite.addAnimation(new FAnimation(Direction.DOWN.ToString(), new int[] { 3 }, 150, false));
        sprite.addAnimation(new FAnimation(Direction.RIGHT.ToString(), new int[] { 2 }, 150, false));
        sprite.addAnimation(new FAnimation(Direction.LEFT.ToString(), new int[] { 2 }, 150, false));
        this.AddChild(sprite);

        PlayAnim();
    }

    public override void OnFixedUpdate()
    {
        if (!firstHit)
        {

            if (HitSomething)
            {
                stateCount = 0;
                firstHit = true;
            }
        }
        else
        {
            if (stuckInThing != null)
            {
                this.SetPosition(stuckInThing.GetPosition() + stuckInThingDisp);
                xVel = 0;
                yVel = 0;
            }
            xVel *= .8f;
            yVel *= .8f;
            if (!dying)
            {

                if (stateCount > 1.0f)
                {
                    dying = true;
                    stateCount = 0;
                }
            }
            else
            {
                isVisible = stateCount * 200 % 10 < 5;
                if (stateCount > 2.0f)
                    world.removeObject(this);
            }
        }
        base.OnFixedUpdate();
    }
    public void HandlePlayerCollision(Player p)
    {
        if (this.isColliding(p))
            HandleDamageObjectCollision(p);
    }
    protected override bool HandleDamageObjectCollision(FutilePlatformerBaseObject damageObject)
    {
        if (firstHit)
            return false;
        if (owner == damageObject)
            return false;
        if (damageObject is Knight)
        {
            Knight k = (Knight)damageObject;
            if (k.State == Knight.KnightState.DYING || k.State == Knight.KnightState.INVULNERABLE)
                return false;
            k.TakeDamage(this.GetPosition());
            stuckInThing = k;
            stuckInThingDisp = this.GetPosition() - k.GetPosition();
            firstHit = true;
            return true;
        }
        else if (damageObject is Player)
        {

            Player p = (Player)damageObject;
            if (p.invulnCount > 0)
                return false;
            p.TakeDamage(this.GetPosition());
            stuckInThing = p;
            stuckInThingDisp = this.GetPosition() - p.GetPosition();
            firstHit = true;
            return true;
        }
        else if (damageObject is Arrow)
        {
            return false;
        }

        return false;
    }
    public void PlayAnim()
    {
        scaleX = CurrentDirection == Direction.RIGHT ? -1 : 1;
        switch (CurrentDirection)
        {
            case Direction.DOWN:
            case Direction.UP:
                hitBox.x = 0;
                hitBox.y = -3;
                hitBox.height = 12;
                hitBox.width = 5;
                break;
            default:
                hitBox.x = 0;
                hitBox.y = -5;
                hitBox.height = 5;
                hitBox.width = 12;
                break;

        }
        UpdateHitBoxSprite();
        sprite.play(CurrentDirection.ToString());
    }
}


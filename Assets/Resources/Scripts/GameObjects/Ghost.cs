using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
public class Ghost : FutileFourDirectionBaseObject
{
    FAnimatedSprite sprite;
    GhostState _state = GhostState.IDLE;
    public GhostState State
    {
        get { return _state; }
        set
        {
            if (value != _state)
            {
                stateCount = 0;
                _state = value;
            }
        }
    }
    public enum GhostState
    {
        IDLE,
        MOVING
    }
    public Ghost(World world)
        : base(new RXRect(1, -2, 10, 10), world)
    {
        handleStateCount = true;
        collidesWithWater = false;
        clearAcc = false;
        useActualMaxVel = true;
        maxVel = .5f;
        sprite = new FAnimatedSprite("ghost");
        sprite.addAnimation(new FAnimation(GhostState.IDLE.ToString() + Direction.DOWN, new int[] { 1 }, 150, true));
        sprite.addAnimation(new FAnimation(GhostState.IDLE.ToString() + Direction.UP, new int[] { 5 }, 150, true));
        sprite.addAnimation(new FAnimation(GhostState.IDLE.ToString() + Direction.LEFT, new int[] { 9}, 150, true));
        sprite.addAnimation(new FAnimation(GhostState.IDLE.ToString() + Direction.RIGHT, new int[] { 9 }, 150, true));

        sprite.addAnimation(new FAnimation(GhostState.MOVING.ToString() + Direction.DOWN, new int[] { 1, 2, 3, 4 }, 150, true));
        sprite.addAnimation(new FAnimation(GhostState.MOVING.ToString() + Direction.UP, new int[] { 5, 6, 7, 8 }, 150, true));
        sprite.addAnimation(new FAnimation(GhostState.MOVING.ToString() + Direction.LEFT, new int[] { 9,10,11,12 }, 150, true));
        sprite.addAnimation(new FAnimation(GhostState.MOVING.ToString() + Direction.RIGHT, new int[] { 9,10,11,12}, 150, true));

        this.AddChild(sprite);
        PlayAnim();

    }
    float minState = .8f;
    float moveSpeed = .01f;
    public override void OnFixedUpdate()
    {
        if (C.isTransitioning)
            return;
        if (State == GhostState.MOVING)
        {

            if (RXRandom.Float() < .2f)
                SpawnParticles();
        }
        else
        {
            if (RXRandom.Float() < .01f)
                SpawnParticles();
        }
        switch (State)
        {
            case GhostState.IDLE:
            case GhostState.MOVING:
                if (stateCount > minState)
                {
                    if (RXRandom.Float() < .03f)
                    {
                        switch (State)
                        {
                            case GhostState.IDLE:
                                float randAngle = RXRandom.Float() * Mathf.PI * 2.0f;
                                xAcc = Mathf.Cos(randAngle) * moveSpeed;
                                yAcc = Mathf.Sin(randAngle) * moveSpeed;
                                State = GhostState.MOVING;
                                break;
                            case GhostState.MOVING:
                                if (RXRandom.Float() < .3f)
                                {
                                    //stop
                                    State = GhostState.IDLE;
                                    PlayAnim(true);
                                    xAcc = 0;
                                    yAcc = 0;

                                }
                                else
                                {
                                  
                                }
                                break;
                        }

                    }
                }
                if (State == GhostState.MOVING)
                {
                    if (hitUp)
                        yAcc = -Mathf.Abs(yAcc);
                    else if (hitDown)
                        yAcc = Mathf.Abs(yAcc);
                    if (hitLeft)
                        xAcc = Mathf.Abs(xAcc);
                    else if (hitRight)
                        xAcc = -Mathf.Abs(xAcc);
                }
                if (xAcc == 0)
                    xVel *= .8f;
                if (yAcc == 0)
                    yVel *= .8f;
                break;
        }
        base.OnFixedUpdate();
        PlayAnim();
    }


    private void SpawnParticles(int numParticles = 1)
    {
        for (int i = 0; i < numParticles; i++)
        {
            Particle.ParticleOne p = Particle.ParticleOne.getParticle();
            Vector2 vel = new Vector2(RXRandom.Float() * 20 - 10, RXRandom.Float() * 20);
            Vector2 acc = new Vector2(-vel.x * (RXRandom.Float() * .5f), -vel.y * -1.0f);
           
            p.activate(this.GetPosition() + new Vector2(RXRandom.Float() * 4 - 2, 1), vel, acc, RXRandom.Bool() ? 180.0f : 0);
            this.container.AddChild(p);
        }
    }

    public void HandlePlayerCollision(Player p)
    {
        if (p.invulnCount > 0)
            return;
        if(p.isColliding(this))
        {
            p.TakeDamage(this.GetPosition());
        }
    }

    public void PlayAnim(bool forced = false)
    {
        sprite.play(_state.ToString() + _direction.ToString(), forced);
    }
}


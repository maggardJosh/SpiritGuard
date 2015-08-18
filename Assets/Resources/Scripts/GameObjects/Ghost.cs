using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
public class Ghost : FutileFourDirectionBaseObject
{
    FAnimatedSprite sprite;
    GhostState _state = GhostState.IDLE;
    private int health = 2;
    public string name;
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
        MOVING,
        INVULNERABLE,
        DYING
    }
    public Ghost(World world, string name)
        : base(new RXRect(1, -2, 10, 10), world)
    {
        this.name = name;
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
    float invulnerableCount = 1f;
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
            case GhostState.INVULNERABLE:

                if (RXRandom.Float() < .2f)
                    SpawnParticles(1);
                this.isVisible = stateCount * 100 % 10 < 5;
                if (stateCount > invulnerableCount)
                {
                    State = GhostState.IDLE;

                    resetMax();
                    this.isVisible = true;
                }
                this.xVel *= .9f;
                this.yVel *= .9f;
                break;
            case GhostState.DYING:
                if (RXRandom.Float() < .4f + .4f * stateCount)
                    SpawnParticles(1 + (int)stateCount);
                this.isVisible = stateCount * 100 % 10 < 5;
                if (stateCount > invulnerableCount)
                {
                    if (!String.IsNullOrEmpty(name))
                        C.Save.requiredEnemyKills.Add(this.name);
                    FSoundManager.PlaySound("enemyDie");
                    if (RXRandom.Float() < Knight.HEART_DROP_CHANCE)
                        world.addObject(new Heart(world, this.GetPosition()));
                    SpawnParticles(25);
                    world.removeObject(this);

                }
                this.xVel *= .9f;
                this.yVel *= .9f;
                break;
        }
        base.OnFixedUpdate();
        PlayAnim();
    }

    private void resetMax()
    {

        maxVel = .5f;
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
        if (p.shouldDamage)
            if (this.isColliding(p.swordCollision))
            {
                this.TakeDamage(p.GetPosition());
                return;
            }
        if (p.invulnCount > 0)
            return;
        
        if(p.isColliding(this))
        {
            p.TakeDamage(this.GetPosition());
        }
    }

    public void TakeDamage(Vector2 pos)
    {
        FSoundManager.PlaySound("enemyHurt");
        Go.killAllTweensWithTarget(this);
        this.health--;
        if (health > 0)
            State = GhostState.INVULNERABLE;
        else
            State = GhostState.DYING;
        Vector2 dist = (this.GetPosition() - pos).normalized * 4;
        maxVel = 50;
        xVel = dist.x;
        yVel = dist.y;
        xAcc = 0;
        yAcc = 0;
    }

    public void PlayAnim(bool forced = false)
    {
        sprite.play(_state.ToString() + _direction.ToString(), forced);
    }
}


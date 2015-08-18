using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Knight : FutileFourDirectionBaseObject
{
    private FAnimatedSprite sprite;
    private float moveSpeed = .5f;
    private KnightState _state = KnightState.IDLE;
    private int health = 2;
    public const float HEART_DROP_CHANCE = .4f;
    private string name;
    public KnightState State
    {
        get
        {
            return _state;
        }
        set
        {
            if (_state != value)
            {
                _state = value;
                stateCount = 0;
            }
        }
    }
    public enum KnightState
    {
        IDLE,
        MOVING,
        ATTACK_START,
        ATTACKING,
        INVULNERABLE,
        DYING
    }
    public Knight(World world, string name)
        : base(new RXRect(0, -6, 10, 8), world)
    {
        this.name = name;

        maxXVel = .5f;
        maxYVel = .5f;
        minYVel = -.5f;
        handleStateCount = true;
        handleDamageObjectCollision = true;
        bounceiness = 0f;
        clearAcc = false;

        sprite = new FAnimatedSprite("knight");
        sprite.addAnimation(new FAnimation(KnightState.MOVING.ToString() + Direction.DOWN, new int[] { 1, 2, 3, 4 }, 150, true));
        sprite.addAnimation(new FAnimation(KnightState.MOVING.ToString() + Direction.UP, new int[] { 5, 6, 7, 8 }, 150, true));
        sprite.addAnimation(new FAnimation(KnightState.MOVING.ToString() + Direction.LEFT, new int[] { 9, 10, 11, 12 }, 150, true));
        sprite.addAnimation(new FAnimation(KnightState.MOVING.ToString() + Direction.RIGHT, new int[] { 9, 10, 11, 12 }, 150, true));

        sprite.addAnimation(new FAnimation(KnightState.IDLE.ToString() + Direction.DOWN, new int[] { 1 }, 150, true));
        sprite.addAnimation(new FAnimation(KnightState.IDLE.ToString() + Direction.UP, new int[] { 5 }, 150, true));
        sprite.addAnimation(new FAnimation(KnightState.IDLE.ToString() + Direction.LEFT, new int[] { 9 }, 150, true));
        sprite.addAnimation(new FAnimation(KnightState.IDLE.ToString() + Direction.RIGHT, new int[] { 9 }, 150, true));

        sprite.addAnimation(new FAnimation(KnightState.DYING.ToString() + Direction.DOWN, new int[] { 1 }, 150, true));
        sprite.addAnimation(new FAnimation(KnightState.DYING.ToString() + Direction.UP, new int[] { 5 }, 150, true));
        sprite.addAnimation(new FAnimation(KnightState.DYING.ToString() + Direction.LEFT, new int[] { 9 }, 150, true));
        sprite.addAnimation(new FAnimation(KnightState.DYING.ToString() + Direction.RIGHT, new int[] { 9 }, 150, true));

        sprite.addAnimation(new FAnimation(KnightState.INVULNERABLE.ToString() + Direction.DOWN, new int[] { 1 }, 150, true));
        sprite.addAnimation(new FAnimation(KnightState.INVULNERABLE.ToString() + Direction.UP, new int[] { 5 }, 150, true));
        sprite.addAnimation(new FAnimation(KnightState.INVULNERABLE.ToString() + Direction.LEFT, new int[] { 9 }, 150, true));
        sprite.addAnimation(new FAnimation(KnightState.INVULNERABLE.ToString() + Direction.RIGHT, new int[] { 9 }, 150, true));


        sprite.addAnimation(new FAnimation(KnightState.ATTACK_START.ToString() + Direction.DOWN, new int[] { 13 }, 350, false));
        sprite.addAnimation(new FAnimation(KnightState.ATTACK_START.ToString() + Direction.UP, new int[] { 15 }, 350, false));
        sprite.addAnimation(new FAnimation(KnightState.ATTACK_START.ToString() + Direction.LEFT, new int[] { 17 }, 350, false));
        sprite.addAnimation(new FAnimation(KnightState.ATTACK_START.ToString() + Direction.RIGHT, new int[] { 17 }, 350, false));

        sprite.addAnimation(new FAnimation(KnightState.ATTACKING.ToString() + Direction.DOWN, new int[] { 13, 14 }, 350, false));
        sprite.addAnimation(new FAnimation(KnightState.ATTACKING.ToString() + Direction.UP, new int[] { 15, 16 }, 350, false));
        sprite.addAnimation(new FAnimation(KnightState.ATTACKING.ToString() + Direction.LEFT, new int[] { 17, 18 }, 350, false));
        sprite.addAnimation(new FAnimation(KnightState.ATTACKING.ToString() + Direction.RIGHT, new int[] { 17, 18 }, 350, false));

        PlayAnim();
        this.AddChild(sprite);

    }

    protected override bool HandleDamageObjectCollision(FutilePlatformerBaseObject damageObject)
    {
        if (damageObject is Arrow)
            return false;   //Handle this in the arrow class
        if (damageObject is Knight)
            return true;
        return base.HandleDamageObjectCollision(damageObject);
    }

    float attackDist = 60f;
    float attackSideDist = 20f;
    float attackTime = .4f;
    float attackDelay = .5f;
    public void CheckDamage(Player p)
    {
        if (this.State == KnightState.INVULNERABLE || this.State == KnightState.DYING)
            return;


        if (p.shouldDamage && (p.State == Player.PlayerState.SWORD || p.State == Player.PlayerState.SWORD_TWO))
        {
            if (isColliding(p.swordCollision))
            {
                TakeDamage(p.GetPosition());
            }
        }
        else
        {
            if (p.invulnCount > 0)// || (p.StateCount > .01f && p.State == Player.PlayerState.JUMP))
                return;
            if (isColliding(p))
            {
                p.TakeDamage(this.GetPosition());
            }
        }

        switch (State)
        {
            case KnightState.IDLE:
            case KnightState.MOVING:
                switch (_direction)
                {
                    case Direction.RIGHT:
                    case Direction.LEFT:
                        if (this.y - attackSideDist < p.y && this.y + attackSideDist > p.y)
                            if (_direction == Direction.LEFT && this.x > p.x && this.x - attackDist < p.x)
                            {
                                FSoundManager.PlaySound("knightAttack");
                                State = KnightState.ATTACK_START;
                                PlayAnim(true);
                                attackTarget = p;
                            }
                            else if (_direction == Direction.RIGHT && this.x < p.x && this.x + attackDist > p.x)
                            {
                                FSoundManager.PlaySound("knightAttack");
                                State = KnightState.ATTACK_START;
                                PlayAnim(true);
                                attackTarget = p;
                            }
                        break;
                    case Direction.UP:
                    case Direction.DOWN:
                        if (this.x - attackSideDist < p.x && this.x + attackSideDist > p.x)
                            if (_direction == Direction.DOWN && this.y > p.y && this.y - attackDist < p.y)
                            {
                                FSoundManager.PlaySound("knightAttack");
                                State = KnightState.ATTACK_START;
                                PlayAnim(true);
                                attackTarget = p;
                            }
                            else if (_direction == Direction.UP && this.y < p.y && this.y + attackDist > p.y)
                            {
                                FSoundManager.PlaySound("knightAttack");
                                State = KnightState.ATTACK_START;
                                PlayAnim(true);
                                attackTarget = p;
                            }
                        break;
                }
                break;
            case KnightState.ATTACKING:
                if (RXRandom.Float() < .3f)
                    SpawnTrailParticles(_direction, 1);
                break;
        }

    }

    public void TakeDamage(Vector2 pos)
    {
        FSoundManager.PlaySound("enemyHurt");
        Go.killAllTweensWithTarget(this);
        this.health--;
        if (health > 0)
            State = KnightState.INVULNERABLE;
        else
            State = KnightState.DYING;
        Vector2 dist = (this.GetPosition() - pos).normalized * 4;
        maxXVel = 50f;
        maxYVel = 50f;
        minYVel = -50f;
        xVel = dist.x;
        yVel = dist.y;
        xAcc = 0;
        yAcc = 0;
    }
    FNode attackTarget = null;
    float invulnerableCount = 1.3f;
    float minState = .8f;
    bool changeDirAfterAttack = false;
    public override void OnFixedUpdate()
    {
        if (C.isTransitioning)
            return;
        switch (State)
        {
            case KnightState.IDLE:
            case KnightState.MOVING:
                if (stateCount > minState)
                {
                    if (RXRandom.Float() < .3f)
                    {
                        switch (State)
                        {
                            case KnightState.IDLE:
                                if (RXRandom.Float() < .5f)
                                {
                                    //left or right
                                    if (RXRandom.Float() < .5f)
                                        xAcc = moveSpeed;
                                    else
                                        xAcc = -moveSpeed;
                                }
                                else
                                {
                                    //up or down
                                    if (RXRandom.Float() < .5f)
                                        yAcc = moveSpeed;
                                    else
                                        yAcc = -moveSpeed;
                                }
                                State = KnightState.MOVING;
                                break;
                            case KnightState.MOVING:
                                if (RXRandom.Float() < .3f)
                                {
                                    //stop
                                    State = KnightState.IDLE;
                                    PlayAnim(true);
                                    xAcc = 0;
                                    yAcc = 0;

                                }
                                else
                                {
                                    //Just turn
                                    if (RXRandom.Float() < .5f)
                                    {
                                        if (RXRandom.Float() < .5f)
                                            xAcc = moveSpeed;
                                        else
                                            xAcc = -moveSpeed;
                                        yAcc = 0;
                                    }
                                    else
                                    {
                                        if (RXRandom.Float() < .5f)
                                            yAcc = moveSpeed;
                                        else
                                            yAcc = -moveSpeed;
                                        xAcc = 0;
                                    }
                                    stateCount = 0;
                                }
                                break;
                        }

                    }
                }
                if (xAcc == 0)
                    xVel *= .8f;
                if (yAcc == 0)
                    yVel *= .8f;
                break;
            case KnightState.ATTACK_START:
                xAcc = 0;
                yAcc = 0;
                xVel = 0;
                yVel = 0;
                if (stateCount > attackDelay)
                {
                    State = KnightState.ATTACKING;
                    Vector2 diff = (attackTarget.GetPosition() - this.GetPosition()).normalized;
                    maxXVel = 100;
                    maxYVel = 100;
                    minYVel = -100;
                    xVel = diff.x * 6;
                    yVel = diff.y * 6;
                }
                break;
            case KnightState.ATTACKING:
                if (stateCount > attackTime * 1.4f)
                {
                    State = KnightState.IDLE;
                    if (changeDirAfterAttack)
                    {
                        switch (CurrentDirection)
                        {
                            case Direction.UP:
                                xAcc = RXRandom.Bool() ? moveSpeed : -moveSpeed;
                                yAcc = 0;
                                break;
                            case Direction.RIGHT:
                                xAcc = 0;
                                yAcc = RXRandom.Bool() ? moveSpeed : -moveSpeed;
                                break;
                            case Direction.DOWN:
                                xAcc = RXRandom.Bool() ? moveSpeed : -moveSpeed;
                                yAcc = 0;
                                break;
                            case Direction.LEFT:
                                changeDirAfterAttack = true;
                                xAcc = 0;
                                yAcc = RXRandom.Bool() ? moveSpeed : -moveSpeed;
                                break;
                        }
                        State = KnightState.MOVING;
                    }
                    changeDirAfterAttack = false;
                    resetMax();
                }
                this.xVel *= .9f;
                this.yVel *= .9f;
                switch (CurrentDirection)
                {
                    case Direction.UP:
                        if (hitUp)
                            changeDirAfterAttack = true;

                        break;
                    case Direction.RIGHT:
                        if (hitRight)
                            changeDirAfterAttack = true;

                        break;
                    case Direction.DOWN:
                        if (hitDown)
                            changeDirAfterAttack = true;

                        break;
                    case Direction.LEFT:
                        if (hitLeft)

                            changeDirAfterAttack = true;

                        break;
                }
                break;
            case KnightState.INVULNERABLE:

                if (RXRandom.Float() < .2f)
                    SpawnParticles(Direction.UP, 1);
                this.isVisible = stateCount * 100 % 10 < 5;
                if (stateCount > invulnerableCount)
                {
                    State = KnightState.IDLE;

                    resetMax();
                    this.isVisible = true;
                }
                this.xVel *= .9f;
                this.yVel *= .9f;
                break;
            case KnightState.DYING:
                if (RXRandom.Float() < .4f + .4f * stateCount)
                    SpawnParticles(Direction.UP, 1 + (int)stateCount);
                this.isVisible = stateCount * 100 % 10 < 5;
                if (stateCount > invulnerableCount)
                {
                    if (!String.IsNullOrEmpty(name))
                        C.Save.requiredEnemyKills.Add(this.name);
                    FSoundManager.PlaySound("enemyDie");
                    if (RXRandom.Float() < Knight.HEART_DROP_CHANCE)
                        world.addObject(new Heart(world, this.GetPosition()));
                    SpawnParticles(Direction.UP, 25);
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
        maxXVel = .5f;
        maxYVel = .5f;
        minYVel = -.5f;
    }

    private void SpawnParticles(Direction dir, int numParticles = 10)
    {
        for (int i = 0; i < numParticles; i++)
        {
            Particle.ParticleOne p = Particle.ParticleOne.getParticle();
            Vector2 vel = new Vector2(RXRandom.Float() * 20 - 10, RXRandom.Float() * 20);
            Vector2 acc = new Vector2(-vel.x * (RXRandom.Float() * .5f), -vel.y * -1.0f);
            switch (dir)
            {
                case Direction.DOWN:
                    vel.y *= -1;
                    acc.y *= -1;
                    break;
                case Direction.RIGHT:
                    float tempX = vel.x;
                    vel.x = vel.y;
                    vel.y = tempX;
                    tempX = acc.x;
                    acc.x = acc.y;
                    acc.y = tempX;
                    break;
                case Direction.UP:

                    break;
                case Direction.LEFT:
                    tempX = vel.x;
                    vel.x = vel.y;
                    vel.y = tempX;
                    tempX = acc.x;
                    acc.x = acc.y;
                    acc.y = tempX;
                    vel.x *= -1;
                    acc.x *= -1;
                    break;
            }
            p.activate(this.GetPosition() + new Vector2(RXRandom.Float() * 16 - 8, RXRandom.Float() * 16 - 8), vel, acc, RXRandom.Bool() ? 180.0f : 0);
            this.container.AddChild(p);
        }
    }
    private void SpawnTrailParticles(Direction dir, int numParticles = 10)
    {
        for (int i = 0; i < numParticles; i++)
        {
            Particle.ParticleOne p = Particle.ParticleOne.getParticle();
            Vector2 vel = new Vector2(RXRandom.Float() * 20 - 10, RXRandom.Float() * 20);
            Vector2 acc = new Vector2(-vel.x * (RXRandom.Float() * .5f), -vel.y * -1.0f);
            switch (dir)
            {
                case Direction.DOWN:
                    vel.y *= -1;
                    acc.y *= -1;
                    break;
                case Direction.RIGHT:
                    float tempX = vel.x;
                    vel.x = vel.y;
                    vel.y = tempX;
                    tempX = acc.x;
                    acc.x = acc.y;
                    acc.y = tempX;
                    break;
                case Direction.UP:

                    break;
                case Direction.LEFT:
                    tempX = vel.x;
                    vel.x = vel.y;
                    vel.y = tempX;
                    tempX = acc.x;
                    acc.x = acc.y;
                    acc.y = tempX;
                    vel.x *= -1;
                    acc.x *= -1;
                    break;
            }
            p.activate(this.GetPosition() + new Vector2(RXRandom.Float() * 4-2, RXRandom.Float() * 4 -2), vel, acc, RXRandom.Bool() ? 180.0f : 0);
            this.container.AddChild(p);
        }
    }
    public void PlayAnim(bool forced = false)
    {
        sprite.play(State.ToString() + _direction.ToString(), forced);
    }
}


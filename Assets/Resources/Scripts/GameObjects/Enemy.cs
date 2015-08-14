using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Enemy : FutileFourDirectionBaseObject
{
    private FAnimatedSprite sprite;
    private float moveSpeed = .5f;
    private EnemyState _state = EnemyState.IDLE;
    public EnemyState State
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
    public enum EnemyState
    {
        IDLE,
        MOVING
    }
    public Enemy(World world)
        : base(new RXRect(0, -6, 10, 8), world)
    {
        maxXVel = .5f;
        maxYVel = .5f;
        minYVel = -.5f;
        handleStateCount = true;
        bounceiness = 0f;
        clearAcc = false;

        sprite = new FAnimatedSprite("villagerA");
        sprite.addAnimation(new FAnimation(EnemyState.MOVING.ToString() + Direction.DOWN, new int[] { 1, 2, 3, 4 }, 150, true));
        sprite.addAnimation(new FAnimation(EnemyState.MOVING.ToString() + Direction.UP, new int[] { 5, 6, 7, 8 }, 150, true));
        sprite.addAnimation(new FAnimation(EnemyState.MOVING.ToString() + Direction.LEFT, new int[] { 9, 10, 11, 12 }, 150, true));
        sprite.addAnimation(new FAnimation(EnemyState.MOVING.ToString() + Direction.RIGHT, new int[] { 9, 10, 11, 12 }, 150, true));

        sprite.addAnimation(new FAnimation(EnemyState.IDLE.ToString() + Direction.DOWN, new int[] { 1 }, 150, true));
        sprite.addAnimation(new FAnimation(EnemyState.IDLE.ToString() + Direction.UP, new int[] { 5 }, 150, true));
        sprite.addAnimation(new FAnimation(EnemyState.IDLE.ToString() + Direction.LEFT, new int[] { 9 }, 150, true));
        sprite.addAnimation(new FAnimation(EnemyState.IDLE.ToString() + Direction.RIGHT, new int[] { 9 }, 150, true));

        PlayAnim();
        this.AddChild(sprite);
    }

    float minState = .8f;
    public override void OnFixedUpdate()
    {
        if (stateCount > minState)
        {
            if (RXRandom.Float() < .3f)
            {
                switch (State)
                {
                    case EnemyState.IDLE:
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
                        State = EnemyState.MOVING;
                        break;
                    case EnemyState.MOVING:
                        if (RXRandom.Float() < .3f)
                        {
                            //stop
                            State = EnemyState.IDLE;
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
        base.OnFixedUpdate();
        if (xAcc == 0)
            xVel *= .8f;
        if (yAcc == 0)
            yVel *= .8f;
        PlayAnim();
    }

    public void PlayAnim(bool forced = false)
    {
        sprite.play(State.ToString() + _direction.ToString(), forced);
    }
}


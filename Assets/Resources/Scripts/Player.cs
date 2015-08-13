using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Player : FutileFourDirectionBaseObject
{
    private float moveSpeed = .1f;
    private FAnimatedSprite player;

    public enum PlayerState
    {
        IDLE,
        MOVE,
        JUMP
    }

    private PlayerState _state = PlayerState.IDLE;
    public PlayerState State
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

    public Player(World world)
        : base(new RXRect(0, 0, 10,10), world)
    {
        maxXVel = 1;
        maxYVel = 1;
        minYVel = -1;
        handleStateCount = true;
        bounceiness = 0f;
        player = new FAnimatedSprite("player");

        player.addAnimation(new FAnimation(PlayerState.IDLE.ToString() + Direction.RIGHT.ToString(), new int[] { 9 }, 100, true));
        player.addAnimation(new FAnimation(PlayerState.IDLE.ToString() + Direction.LEFT.ToString(), new int[] { 9 }, 100, true));
        player.addAnimation(new FAnimation(PlayerState.IDLE.ToString() + Direction.UP.ToString(), new int[] { 5 }, 100, true));
        player.addAnimation(new FAnimation(PlayerState.IDLE.ToString() + Direction.DOWN.ToString(), new int[] { 1 }, 100, true));

        player.addAnimation(new FAnimation(PlayerState.MOVE.ToString() + Direction.RIGHT.ToString(), new int[] { 9,10,9,12 }, 150, true));
        player.addAnimation(new FAnimation(PlayerState.MOVE.ToString() + Direction.LEFT.ToString(), new int[] { 9, 10, 9, 12 }, 150, true));
        player.addAnimation(new FAnimation(PlayerState.MOVE.ToString() + Direction.UP.ToString(), new int[] { 5, 6, 5, 8 }, 150, true));
        player.addAnimation(new FAnimation(PlayerState.MOVE.ToString() + Direction.DOWN.ToString(), new int[] { 1, 2, 1, 4 }, 150, true));

        player.addAnimation(new FAnimation(PlayerState.JUMP.ToString() + Direction.RIGHT.ToString(), new int[] { 18,19 }, 150, false));
        player.addAnimation(new FAnimation(PlayerState.JUMP.ToString() + Direction.LEFT.ToString(), new int[] { 18,19 }, 150, false));
        player.addAnimation(new FAnimation(PlayerState.JUMP.ToString() + Direction.UP.ToString(), new int[] { 16,17}, 150, false));
        player.addAnimation(new FAnimation(PlayerState.JUMP.ToString() + Direction.DOWN.ToString(), new int[] { 14, 15 }, 150, false));
        player.play(State.ToString());
        this.AddChild(player);
    }

    float maxJumpDist = 16 * 2.5f;
    public override void OnFixedUpdate()
    {
        if (C.isTransitioning)
            return;
        switch (State)
        {
            case PlayerState.IDLE:
            case PlayerState.MOVE:
                if (C.getKey(C.JUMP_KEY))
                {
                    State = PlayerState.JUMP;
                    if (C.getKey(C.RIGHT_KEY))
                        _direction = Direction.RIGHT;
                    else if (C.getKey(C.DOWN_KEY))
                        _direction = Direction.DOWN;
                    else if (C.getKey(C.LEFT_KEY))
                        _direction = Direction.LEFT;
                    else if (C.getKey(C.UP_KEY))
                        _direction = Direction.UP;
                    Go.killAllTweensWithTarget(this);
                    float newX = this.x;
                    float newY = this.y;
                    switch(_direction)
                    {
                        case Direction.UP: newY += maxJumpDist; while (!world.isPassable(this.x, newY + hitBox.height / 2f) || !world.isPassable(this.x, newY - hitBox.height/2f)) { newY -= 1f; } break;
                        case Direction.RIGHT: newX += maxJumpDist; while (!world.isPassable(newX + hitBox.width / 2f, this.y) || !world.isPassable(newX - hitBox.width / 2f, this.y)) { newX -= 1f; } break;
                        case Direction.DOWN: newY -= maxJumpDist; while (!world.isPassable(this.x, newY + hitBox.height / 2f) || !world.isPassable(this.x, newY - hitBox.height / 2f)) { newY += 1f; } break;
                        case Direction.LEFT: newX -= maxJumpDist; while (!world.isPassable(newX + hitBox.width / 2f, this.y) || !world.isPassable(newX - hitBox.width / 2f, this.y)) { newX += 1f; } break;
                    }
                    xVel = 0;
                    yVel = 0;
                    float jumpTime = .5f;
                    float jumpDisp = 10f;
                    if (newX == this.x)
                    {
                        Go.to(this, jumpTime / 2f, new TweenConfig().floatProp("x", -jumpDisp, true).setEaseType(EaseType.QuadOut).setIterations(2, LoopType.PingPong));
                        Go.to(this, jumpTime, new TweenConfig().floatProp("y", newY).setEaseType(EaseType.QuadOut).onComplete(() => { State = PlayerState.IDLE; }));

                    }
                    else
                    {
                        Go.to(this, jumpTime / 2f, new TweenConfig().floatProp("y", jumpDisp, true).setEaseType(EaseType.QuadOut).setIterations(2, LoopType.PingPong));
                        Go.to(this, jumpTime, new TweenConfig().floatProp("x", newX).setEaseType(EaseType.QuadOut).onComplete(() => { State = PlayerState.IDLE; }));

                    }
                    return;
                }
                if (C.getKey(C.LEFT_KEY))
                {

                    xAcc = -moveSpeed;
                    if (xVel > 0)
                        xVel *= .8f;

                }
                else
                    if (C.getKey(C.RIGHT_KEY))
                    {

                        xAcc = moveSpeed;
                        if (xVel < 0)
                            xVel *= .8f;

                    }
                    else
                        xVel *= .7f;
                if (C.getKey(C.UP_KEY))
                {
                    yAcc = moveSpeed;
                    if (yVel < 0)
                        yVel *= .8f;

                }
                else if (C.getKey(C.DOWN_KEY))
                {
                    yAcc = -moveSpeed;
                    if (yVel > 0)
                        yVel *= .8f;
                }
                else
                    yVel *= .7f;


                break;
        }

        if (xAcc != 0 || yAcc != 0)
        {
            switch (State)
            {
                case PlayerState.IDLE:
                    State = PlayerState.MOVE;
                    break;
            }
        }
        else
        {
            switch(State)
            {
                case PlayerState.MOVE:
                    State = PlayerState.IDLE;
                    break;
            }
        }

        base.OnFixedUpdate();


        PlayAnim();
    }

    public void PlayAnim(bool forced = false)
    {
        player.play(State.ToString() + _direction.ToString(), forced);
    }

}


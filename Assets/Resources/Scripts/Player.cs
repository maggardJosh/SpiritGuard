using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Player : FutileFourDirectionBaseObject
{
    private float moveSpeed = .1f;
    private FAnimatedSprite player;

    private enum PlayerState
    {
        IDLE,
        MOVE,
        JUMP
    }

    private PlayerState _state = PlayerState.IDLE;
    private PlayerState State
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
        player.addAnimation(new FAnimation(PlayerState.IDLE.ToString() + Direction.RIGHT.ToString(), new int[] { 1 }, 100, true));
        player.addAnimation(new FAnimation(PlayerState.IDLE.ToString() + Direction.LEFT.ToString(), new int[] { 1 }, 100, true));
        player.addAnimation(new FAnimation(PlayerState.IDLE.ToString() + Direction.UP.ToString(), new int[] { 4 }, 100, true));
        player.addAnimation(new FAnimation(PlayerState.IDLE.ToString() + Direction.DOWN.ToString(), new int[] { 7 }, 100, true));
        player.addAnimation(new FAnimation(PlayerState.MOVE.ToString() + Direction.RIGHT.ToString(), new int[] { 2, 1, 3 }, 150, true));
        player.addAnimation(new FAnimation(PlayerState.MOVE.ToString() + Direction.LEFT.ToString(), new int[] { 2, 1, 3 }, 150, true));
        player.addAnimation(new FAnimation(PlayerState.MOVE.ToString() + Direction.UP.ToString(), new int[] { 5, 4, 6 }, 150, true));
        player.addAnimation(new FAnimation(PlayerState.MOVE.ToString() + Direction.DOWN.ToString(), new int[] { 8, 7, 9 }, 150, true));

        player.addAnimation(new FAnimation(PlayerState.JUMP.ToString() + Direction.RIGHT.ToString(), new int[] { 2, 3 }, 50, true));
        player.addAnimation(new FAnimation(PlayerState.JUMP.ToString() + Direction.LEFT.ToString(), new int[] { 2,  3 }, 50, true));
        player.addAnimation(new FAnimation(PlayerState.JUMP.ToString() + Direction.UP.ToString(), new int[] { 5,  6 }, 50, true));
        player.addAnimation(new FAnimation(PlayerState.JUMP.ToString() + Direction.DOWN.ToString(), new int[] { 8,  9 }, 50, true));
        player.play(State.ToString());
        this.AddChild(player);
    }

    public override void OnFixedUpdate()
    {
        switch (State)
        {
            case PlayerState.IDLE:
            case PlayerState.MOVE:
                if (C.getKey(C.JUMP_KEY))
                {
                    State = PlayerState.JUMP;
                    Go.killAllTweensWithTarget(this);
                    float newX = this.x;
                    float newY = this.y;
                    switch(_direction)
                    {
                        case Direction.UP: newY += 32; while (!world.isPassable(this.x, newY + hitBox.height / 2f) || !world.isPassable(this.x, newY - hitBox.height/2f)) { newY -= 1f; } break;
                        case Direction.RIGHT: newX += 32; while (!world.isPassable(newX + hitBox.width / 2f, this.y) || !world.isPassable(newX - hitBox.width/2f,this.y)) { newX -= 1f; } break;
                        case Direction.DOWN: newY -= 32; while (!world.isPassable(this.x, newY + hitBox.height / 2f) || !world.isPassable(this.x, newY - hitBox.height / 2f)) { newY += 1f; } break;
                        case Direction.LEFT: newX -= 32; while (!world.isPassable(newX + hitBox.width / 2f, this.y) || !world.isPassable(newX - hitBox.width / 2f, this.y)) { newX += 1f; } break;
                    }
                    xVel = 0;
                    yVel = 0;
                    Go.to(this, .5f, new TweenConfig().floatProp("x", newX).floatProp("y", newY).setEaseType(EaseType.QuadOut).onComplete(() => { State = PlayerState.IDLE; }));
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


        player.play(State.ToString() + _direction.ToString(), false);
    }

}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Player : FutilePlatformerBaseObject
{
    private float moveSpeed = .5f;
    private FAnimatedSprite player;

    private enum PlayerState
    {
        IDLE,
        MOVE
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
        : base(new RXRect(0, 0, 16, 16), world)
    {
        handleStateCount = true;
        bounceiness = 0f;
        player = new FAnimatedSprite("player");
        player.addAnimation(new FAnimation(PlayerState.IDLE.ToString(), new int[] { 1 }, 100, true));
        player.addAnimation(new FAnimation(PlayerState.MOVE.ToString(), new int[] { 2, 1, 3 }, 100, true));
        player.play(State.ToString());
        this.AddChild(player);
    }
    private bool lastJump = false;
    private float wallSlidePressAwayTimer = 0;
    private float PRESS_AWAY_MAX = .3f;
    public override void OnFixedUpdate()
    {
        player.width = this.hitBox.width;
        player.height = this.hitBox.height;

        switch (State)
        {
            case PlayerState.IDLE:
            case PlayerState.MOVE:
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


        player.play(State.ToString(), false);

        lastJump = C.getKey(C.JUMP_KEY);
    }

}


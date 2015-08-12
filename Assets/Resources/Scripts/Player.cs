using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Player : FutileFourDirectionBaseObject
{
    private float moveSpeed = .4f;
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
        : base(new RXRect(0, 0, 10,10), world)
    {
        handleStateCount = true;
        bounceiness = 0f;
        player = new FAnimatedSprite("player");
        player.addAnimation(new FAnimation(PlayerState.IDLE.ToString() + Direction.RIGHT.ToString(), new int[] { 1 }, 100, true));
        player.addAnimation(new FAnimation(PlayerState.MOVE.ToString() + Direction.RIGHT.ToString(), new int[] { 2, 1, 3 }, 150, true));
        player.addAnimation(new FAnimation(PlayerState.IDLE.ToString() + Direction.LEFT.ToString(), new int[] { 1 }, 100, true));
        player.addAnimation(new FAnimation(PlayerState.MOVE.ToString() + Direction.LEFT.ToString(), new int[] { 2, 1, 3 }, 150, true));
        player.addAnimation(new FAnimation(PlayerState.IDLE.ToString() + Direction.UP.ToString(), new int[] { 4 }, 100, true));
        player.addAnimation(new FAnimation(PlayerState.MOVE.ToString() + Direction.UP.ToString(), new int[] { 5, 4, 6 }, 150, true));
        player.addAnimation(new FAnimation(PlayerState.IDLE.ToString() + Direction.DOWN.ToString(), new int[] { 7 }, 100, true));
        player.addAnimation(new FAnimation(PlayerState.MOVE.ToString() + Direction.DOWN.ToString(), new int[] { 8, 7, 9 }, 150, true));
        player.play(State.ToString());
        this.AddChild(player);
    }

    public override void OnFixedUpdate()
    {
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


        player.play(State.ToString() + _direction.ToString(), false);
    }

}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Player : FutilePlatformerBaseObject
{
    private float jumpStrength = 10f;
    private float wallJumpTowardStrength = 12f;
    private float wallJumpAwayStrength = 10f;
    private float wallJumpTowardX = 4f;
    private float wallJumpAwayX = 12f;
    private float moveSpeed = 1f;
    private FAnimatedSprite player;

    private enum PlayerState
    {
        IDLE,
        WALL_SLIDE,
        WALL_JUMP
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
                switch(value)
                {
                    case PlayerState.WALL_SLIDE:
                        wallSlidePressAwayTimer = 0;
                        break;
                }
                stateCount = 0;
            }
        }
    }

    public Player(World world)
        : base(new RXRect(0, 0, 25, 30), world)
    {
        handleStateCount = true;
        bounceiness = 0f;
        player = new FAnimatedSprite("player");
        player.addAnimation(new FAnimation(PlayerState.IDLE.ToString(), new int[] { 1 }, 100, true));
        player.addAnimation(new FAnimation(PlayerState.WALL_SLIDE.ToString(), new int[] { 2 }, 100, true));
        player.addAnimation(new FAnimation(PlayerState.WALL_JUMP.ToString(), new int[] { 1 }, 100, true));
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
        if (C.getKey(C.JUMP_KEY) && !lastJump)
        {
            if (grounded)
            {
                yVel = jumpStrength;
                grounded = false;
            }

            if (State == PlayerState.WALL_SLIDE)
            {
                if (wallSlidePressAwayTimer > 0)
                {

                    xVel = scaleX == 1 ? -wallJumpAwayX : wallJumpAwayX;
                    yVel = wallJumpAwayStrength;
                }
                else
                {
                    xVel = scaleX == 1 ? -wallJumpTowardX : wallJumpTowardX;
                    yVel = wallJumpTowardStrength;
                }
                grounded = false;
                
                State = PlayerState.WALL_JUMP;
            }
        }
        switch (State)
        {
            case PlayerState.IDLE:
                if (C.getKey(C.LEFT_KEY))
                {
                    if (grounded)
                    {

                        xAcc = -moveSpeed;
                        if (xVel > 0)
                            xVel *= .8f;
                    }
                    else
                    {
                        xAcc = -moveSpeed * .8f;
                    }
                }
                else
                    if (C.getKey(C.RIGHT_KEY))
                    {
                        if (grounded)
                        {

                            xAcc = moveSpeed;
                            if (xVel < 0)
                                xVel *= .8f;
                        }
                        else
                        {
                            xAcc = moveSpeed * .8f;
                        }
                    }
                    else if (grounded)
                        xVel *= .7f;
                    else
                        xVel *= .94f;

                break;
            case PlayerState.WALL_SLIDE:
                xVel = scaleX == 1 ? 1 : -1;
                break;
            case PlayerState.WALL_JUMP:
                if (stateCount > .1f)
                    State = PlayerState.IDLE;
                break;
        }


        float newWidth = hitBox.width;
        float newHeight = hitBox.height;
        float changeSpeed = 2f;
        if (Input.GetKey(KeyCode.U))
            newHeight += changeSpeed;
        else if (Input.GetKey(KeyCode.J))
            newHeight -= changeSpeed;

        if (Input.GetKey(KeyCode.H))
            newWidth -= changeSpeed;
        else if (Input.GetKey(KeyCode.K))
            newWidth += changeSpeed;
        this.UpdateHitBox(newWidth, newHeight);
        if (State == PlayerState.WALL_SLIDE)
            if (yVel < 0)
                yVel *= .7f;
            else
            {
                wallSlidePressAwayTimer = 0;
                if (C.getKey(C.JUMP_KEY))
                    yVel *= 1f;
                else
                    yVel *= .9f;
            }
        base.OnFixedUpdate();

        switch (State)
        {
            case PlayerState.IDLE:
                if (!grounded)
                    if (hitLeft)// && C.getKey(C.LEFT_KEY))
                        State = PlayerState.WALL_SLIDE;
                    else if (hitRight)// && C.getKey(C.RIGHT_KEY))
                        State = PlayerState.WALL_SLIDE;

                break;
            case PlayerState.WALL_SLIDE:
                if (!hitLeft && !hitRight)
                    State = PlayerState.IDLE;
                else
                    if (!grounded)
                    {

                        if ((hitLeft && !C.getKey(C.LEFT_KEY)) || hitRight && !C.getKey(C.RIGHT_KEY))
                            wallSlidePressAwayTimer += Time.deltaTime;
                        else
                            wallSlidePressAwayTimer = 0;
                        if (wallSlidePressAwayTimer > PRESS_AWAY_MAX)
                        {
                            wallSlidePressAwayTimer = 0;
                            State = PlayerState.IDLE;
                        }
                    }
                    else
                        State = PlayerState.IDLE;
          
                break;
            case PlayerState.WALL_JUMP:
                if (Mathf.Abs(xVel) < 1f)
                    State = PlayerState.IDLE;
                if (!grounded)
                    if (hitLeft)// && C.getKey(C.LEFT_KEY))
                        State = PlayerState.WALL_SLIDE;
                    else if (hitRight)// && C.getKey(C.RIGHT_KEY))
                        State = PlayerState.WALL_SLIDE;
                break;
        }


        player.play(State.ToString(), false);

        lastJump = C.getKey(C.JUMP_KEY);
    }

}


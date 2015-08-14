using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Player : FutileFourDirectionBaseObject
{
    private float moveSpeed = .1f;
    private FAnimatedSprite player;
    bool lastActionPress = false;

    public enum PlayerState
    {
        IDLE,
        MOVE,
        JUMP,
        SWORD,
        SWORD_TWO
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
        : base(new RXRect(0, -8, 8,6), world)
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

        int attackSpeed = 250;
        player.addAnimation(new FAnimation(PlayerState.SWORD.ToString() + Direction.RIGHT.ToString(), new int[] { 24,25 }, attackSpeed, false));
        player.addAnimation(new FAnimation(PlayerState.SWORD.ToString() + Direction.LEFT.ToString(), new int[] { 24, 25 }, attackSpeed, false));
        player.addAnimation(new FAnimation(PlayerState.SWORD.ToString() + Direction.UP.ToString(), new int[] { 22, 23 }, attackSpeed, false));
        player.addAnimation(new FAnimation(PlayerState.SWORD.ToString() + Direction.DOWN.ToString(), new int[] { 20, 21 }, attackSpeed, false));


        player.addAnimation(new FAnimation(PlayerState.SWORD_TWO.ToString() + Direction.RIGHT.ToString(), new int[] { 24, 25 }, attackSpeed, false));
        player.addAnimation(new FAnimation(PlayerState.SWORD_TWO.ToString() + Direction.LEFT.ToString(), new int[] { 24, 25 }, attackSpeed, false));
        player.addAnimation(new FAnimation(PlayerState.SWORD_TWO.ToString() + Direction.UP.ToString(), new int[] { 22, 23 }, attackSpeed, false));
        player.addAnimation(new FAnimation(PlayerState.SWORD_TWO.ToString() + Direction.DOWN.ToString(), new int[] { 20, 21 }, attackSpeed, false));
       

        player.play(State.ToString());
        this.AddChild(player);
    }

    float maxJumpDist = 16 * 2.3f;
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
                        case Direction.UP: newY += maxJumpDist; while (!world.isPassable(this.x + hitBox.x, newY +hitBox.y + hitBox.height / 2f) || !world.isPassable(this.x + hitBox.x, newY +hitBox.y - hitBox.height/2f)) { newY -= 1f; } break;
                        case Direction.RIGHT: newX += maxJumpDist; while (!world.isPassable(newX + hitBox.x + hitBox.width / 2f, this.y + hitBox.y) || !world.isPassable(newX + hitBox.x - hitBox.width / 2f, this.y + hitBox.y)) { newX -= 1f; } break;
                        case Direction.DOWN: newY -= maxJumpDist; while (!world.isPassable(this.x + hitBox.x, newY + hitBox.y + hitBox.height / 2f) || !world.isPassable(this.x + hitBox.x, newY + hitBox.y - hitBox.height / 2f)) { newY += 1f; } break;
                        case Direction.LEFT: newX -= maxJumpDist; while (!world.isPassable(newX + hitBox.x + hitBox.width / 2f, this.y + hitBox.y) || !world.isPassable(newX + hitBox.x - hitBox.width / 2f, this.y+ hitBox.y)) { newX += 1f; } break;
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
                    if (_direction == Direction.RIGHT)
                        scaleX = -1;
                    else if (_direction == Direction.LEFT)
                        scaleX = 1;
                    return;
                }
                float attackTime = .4f;
                float attackDist = 16;
                float attackDisp = 3f;
                if (C.getKey(C.ACTION_KEY) && !lastActionPress)
                {
                    State = PlayerState.SWORD;
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
                    switch (_direction)
                    {
                        case Direction.UP: newY += attackDist; while (!world.isPassable(this.x + hitBox.x, newY + hitBox.y + hitBox.height / 2f) || !world.isPassable(this.x + hitBox.x, newY + hitBox.y - hitBox.height / 2f)) { newY -= 1f; } break;
                        case Direction.RIGHT: newX += attackDist; while (!world.isPassable(newX + hitBox.x + hitBox.width / 2f, this.y + hitBox.y) || !world.isPassable(newX + hitBox.x - hitBox.width / 2f, this.y + hitBox.y)) { newX -= 1f; } break;
                        case Direction.DOWN: newY -= attackDist; while (!world.isPassable(this.x + hitBox.x, newY + hitBox.y + hitBox.height / 2f) || !world.isPassable(this.x + hitBox.x, newY + hitBox.y - hitBox.height / 2f)) { newY += 1f; } break;
                        case Direction.LEFT: newX -= attackDist; while (!world.isPassable(newX + hitBox.x + hitBox.width / 2f, this.y + hitBox.y) || !world.isPassable(newX + hitBox.x - hitBox.width / 2f, this.y + hitBox.y)) { newX += 1f; } break;
                    }
                    xVel = 0;
                    yVel = 0;
                    if (newX == this.x)
                    {
                        Go.to(this, attackTime / 2f, new TweenConfig().floatProp("x", -attackDisp, true).setEaseType(EaseType.QuadOut).setIterations(2, LoopType.PingPong));
                        Go.to(this, attackTime, new TweenConfig().floatProp("y", newY).setEaseType(EaseType.BackInOut).onComplete(() => {  }));

                    }
                    else
                    {
                        Go.to(this, attackTime / 2f, new TweenConfig().floatProp("y", attackDisp, true).setEaseType(EaseType.QuadOut).setIterations(2, LoopType.PingPong));
                        Go.to(this, attackTime, new TweenConfig().floatProp("x", newX).setEaseType(EaseType.BackInOut).onComplete(() => {  }));

                    }
                    if (_direction == Direction.RIGHT)
                        scaleX = -1;
                    else if (_direction == Direction.LEFT)
                        scaleX = 1;
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
            case PlayerState.SWORD:
                if (stateCount > .6f)
                    State = PlayerState.IDLE;
                else if (stateCount > .3f)
                {
                    if (C.getKey(C.ACTION_KEY) && !lastActionPress)
                    {
                        float attack2Time = .2f;
                        float attack2Dist = 20;
                        float attack2Disp = 5f;
                        State = PlayerState.SWORD_TWO;
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
                        PlayAnim(true);
                        switch (_direction)
                        {
                            case Direction.UP: newY += attack2Dist; while (!world.isPassable(this.x + hitBox.x, newY + hitBox.y + hitBox.height / 2f) || !world.isPassable(this.x + hitBox.x, newY + hitBox.y - hitBox.height / 2f)) { newY -= 1f; } break;
                            case Direction.RIGHT: newX += attack2Dist; while (!world.isPassable(newX + hitBox.x + hitBox.width / 2f, this.y + hitBox.y) || !world.isPassable(newX + hitBox.x - hitBox.width / 2f, this.y + hitBox.y)) { newX -= 1f; } break;
                            case Direction.DOWN: newY -= attack2Dist; while (!world.isPassable(this.x + hitBox.x, newY + hitBox.y + hitBox.height / 2f) || !world.isPassable(this.x + hitBox.x, newY + hitBox.y - hitBox.height / 2f)) { newY += 1f; } break;
                            case Direction.LEFT: newX -= attack2Dist; while (!world.isPassable(newX + hitBox.x + hitBox.width / 2f, this.y + hitBox.y) || !world.isPassable(newX + hitBox.x - hitBox.width / 2f, this.y + hitBox.y)) { newX += 1f; } break;
                        }
                        xVel = 0;
                        yVel = 0;
                        Go.killAllTweensWithTarget(this);
                        if (newX == this.x)
                        {
                            Go.to(this, attack2Time / 2f, new TweenConfig().floatProp("x", -attack2Disp, true).setEaseType(EaseType.QuadOut).setIterations(2, LoopType.PingPong));
                            Go.to(this, attack2Time, new TweenConfig().floatProp("y", newY).setEaseType(EaseType.BackInOut).onComplete(() => { }));

                        }
                        else
                        {
                            Go.to(this, attack2Time / 2f, new TweenConfig().floatProp("y", attack2Disp, true).setEaseType(EaseType.QuadOut).setIterations(2, LoopType.PingPong));
                            Go.to(this, attack2Time, new TweenConfig().floatProp("x", newX).setEaseType(EaseType.BackInOut).onComplete(() => { }));

                        }
                        if (_direction == Direction.RIGHT)
                            scaleX = -1;
                        else if (_direction == Direction.LEFT)
                            scaleX = 1;
                        return;
                    }
                }
                break;
            case PlayerState.SWORD_TWO:
                if (stateCount > .6f)
                    State = PlayerState.IDLE;
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

        lastActionPress = C.getKey(C.ACTION_KEY);
        PlayAnim();
    }

    public void PlayAnim(bool forced = false)
    {
        player.play(State.ToString() + _direction.ToString(), forced);
    }

}


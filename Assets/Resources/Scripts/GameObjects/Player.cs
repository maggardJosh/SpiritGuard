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
    bool lastSelectPress = false;
    bool lastJumpPress = false;
    public bool shouldDamage = false;
    private int _health = 3;
    public bool hasInteractObject = false;
    public bool hasDamageObject = false;
    private bool canJump = false;
    private bool canSword = false;
    private bool canBow = false;
    public bool CanJump { get { return canJump; } set { world.ui.UpdateHasJump(value); canJump = value; } }
    public int Health
    {
        get { return _health; }
        set
        {
            int oldHealth = _health;
            _health = Mathf.Clamp(value, 0, 3);
            if (oldHealth < _health)
                world.ui.AddHealth(_health, Vector2.zero);
            else if (oldHealth > _health)
                world.ui.TakeDamage(_health);
        }
    }
    public FutilePlatformerBaseObject swordCollision;
    public float invulnCount = 0;
    float invulnerableStunTime = .6f;
    SecondaryItem selectedItem = SecondaryItem.NONE;

    public enum SecondaryItem
    {
        NONE,
        SWORD,
        BOW
    }
    public enum PlayerState
    {
        IDLE,
        MOVE,
        JUMP,
        SWORD,
        SWORD_TWO,
        BOW_DRAWN,
        BOW_SHOOTING,
        INVULNERABLE,
        DYING
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
        : base(new RXRect(0, -8, 8, 6), world)
    {
        swordCollision = new FutilePlatformerBaseObject(new RXRect(6, -8, 15, 10), world);
        CanJump = true;
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

        player.addAnimation(new FAnimation(PlayerState.INVULNERABLE.ToString() + Direction.RIGHT.ToString(), new int[] { 9 }, 100, true));
        player.addAnimation(new FAnimation(PlayerState.INVULNERABLE.ToString() + Direction.LEFT.ToString(), new int[] { 9 }, 100, true));
        player.addAnimation(new FAnimation(PlayerState.INVULNERABLE.ToString() + Direction.UP.ToString(), new int[] { 5 }, 100, true));
        player.addAnimation(new FAnimation(PlayerState.INVULNERABLE.ToString() + Direction.DOWN.ToString(), new int[] { 1 }, 100, true));

        player.addAnimation(new FAnimation(PlayerState.DYING.ToString() + Direction.RIGHT.ToString(), new int[] { 9 }, 100, true));
        player.addAnimation(new FAnimation(PlayerState.DYING.ToString() + Direction.LEFT.ToString(), new int[] { 9 }, 100, true));
        player.addAnimation(new FAnimation(PlayerState.DYING.ToString() + Direction.UP.ToString(), new int[] { 5 }, 100, true));
        player.addAnimation(new FAnimation(PlayerState.DYING.ToString() + Direction.DOWN.ToString(), new int[] { 1 }, 100, true));

        player.addAnimation(new FAnimation(PlayerState.MOVE.ToString() + Direction.RIGHT.ToString(), new int[] { 9, 10, 9, 12 }, 150, true));
        player.addAnimation(new FAnimation(PlayerState.MOVE.ToString() + Direction.LEFT.ToString(), new int[] { 9, 10, 9, 12 }, 150, true));
        player.addAnimation(new FAnimation(PlayerState.MOVE.ToString() + Direction.UP.ToString(), new int[] { 5, 6, 5, 8 }, 150, true));
        player.addAnimation(new FAnimation(PlayerState.MOVE.ToString() + Direction.DOWN.ToString(), new int[] { 1, 2, 1, 4 }, 150, true));

        player.addAnimation(new FAnimation(PlayerState.JUMP.ToString() + Direction.RIGHT.ToString(), new int[] { 18, 19 }, 150, false));
        player.addAnimation(new FAnimation(PlayerState.JUMP.ToString() + Direction.LEFT.ToString(), new int[] { 18, 19 }, 150, false));
        player.addAnimation(new FAnimation(PlayerState.JUMP.ToString() + Direction.UP.ToString(), new int[] { 16, 17 }, 150, false));
        player.addAnimation(new FAnimation(PlayerState.JUMP.ToString() + Direction.DOWN.ToString(), new int[] { 14, 15 }, 150, false));

        int attackSpeed = 250;
        player.addAnimation(new FAnimation(PlayerState.SWORD.ToString() + Direction.RIGHT.ToString(), new int[] { 28, 29 }, attackSpeed, false));
        player.addAnimation(new FAnimation(PlayerState.SWORD.ToString() + Direction.LEFT.ToString(), new int[] { 28, 29 }, attackSpeed, false));
        player.addAnimation(new FAnimation(PlayerState.SWORD.ToString() + Direction.UP.ToString(), new int[] { 24, 25 }, attackSpeed, false));
        player.addAnimation(new FAnimation(PlayerState.SWORD.ToString() + Direction.DOWN.ToString(), new int[] { 20, 21 }, attackSpeed, false));

        player.addAnimation(new FAnimation(PlayerState.SWORD_TWO.ToString() + Direction.RIGHT.ToString(), new int[] { 30, 31 }, attackSpeed, false));
        player.addAnimation(new FAnimation(PlayerState.SWORD_TWO.ToString() + Direction.LEFT.ToString(), new int[] { 30, 31 }, attackSpeed, false));
        player.addAnimation(new FAnimation(PlayerState.SWORD_TWO.ToString() + Direction.UP.ToString(), new int[] { 26, 27 }, attackSpeed, false));
        player.addAnimation(new FAnimation(PlayerState.SWORD_TWO.ToString() + Direction.DOWN.ToString(), new int[] { 22, 23 }, attackSpeed, false));

        player.addAnimation(new FAnimation(PlayerState.BOW_DRAWN.ToString() + Direction.DOWN.ToString(), new int[] { 32, 33 }, 150, true));
        player.addAnimation(new FAnimation(PlayerState.BOW_DRAWN.ToString() + Direction.UP.ToString(), new int[] { 35, 36 }, 150, true));
        player.addAnimation(new FAnimation(PlayerState.BOW_DRAWN.ToString() + Direction.RIGHT.ToString(), new int[] { 38, 39 }, 150, true));
        player.addAnimation(new FAnimation(PlayerState.BOW_DRAWN.ToString() + Direction.LEFT.ToString(), new int[] { 38, 39 }, 150, true));

        player.addAnimation(new FAnimation(PlayerState.BOW_SHOOTING.ToString() + Direction.DOWN.ToString(), new int[] { 34 }, 150, false));
        player.addAnimation(new FAnimation(PlayerState.BOW_SHOOTING.ToString() + Direction.UP.ToString(), new int[] { 37 }, 150, false));
        player.addAnimation(new FAnimation(PlayerState.BOW_SHOOTING.ToString() + Direction.RIGHT.ToString(), new int[] { 40 }, 150, false));
        player.addAnimation(new FAnimation(PlayerState.BOW_SHOOTING.ToString() + Direction.LEFT.ToString(), new int[] { 40 }, 150, false));


        PlayAnim(true);
        this.AddChild(player);
    }

    public override void HandleAddedToContainer(FContainer container)
    {
        container.AddChild(swordCollision);
        base.HandleAddedToContainer(container);
    }

    public void PickupSoul(SoulPickup soul)
    {
        switch (soul.Type)
        {
            case SoulPickup.SoulType.JUMP: CanJump = true; break;
            case SoulPickup.SoulType.SWORD: canSword = true; selectedItem = SecondaryItem.SWORD; world.ui.UpdateSelectedItem(selectedItem); break;
            case SoulPickup.SoulType.BOW: canBow = true; selectedItem = SecondaryItem.BOW; world.ui.UpdateSelectedItem(selectedItem); break;
        }
    }

    public void TakeDamage(Vector2 pos)
    {
        FSoundManager.PlaySound("hurt");
        C.getCameraInstance().shake(.7f, .5f);
        Go.killAllTweensWithTarget(this);
        this.Health--;

        if (this.Health == 0)
        {
            State = PlayerState.DYING;
            hasSpawnedSpiritParticles = false;
        }
        else
        {

            State = PlayerState.INVULNERABLE;
        }
        invulnCount = 3.0f;



        Vector2 dist = (this.GetPosition() - pos).normalized * 4;
        if (dist == Vector2.zero)
            dist = RXRandom.Vector2Normalized() * 4;
        maxXVel = 50f;
        maxYVel = 50f;
        minYVel = -50f;
        xVel = dist.x;
        yVel = dist.y;
        xAcc = 0;
        yAcc = 0;

    }


    bool hasSpawnedSpiritParticles = false;
    float maxJumpDist = 16 * 2.3f;
    public override void OnFixedUpdate()
    {
        swordCollision.SetPosition(this.GetPosition());
        switch (_direction)
        {
            case Direction.RIGHT:
                swordCollision.hitBox.x = 6;
                swordCollision.hitBox.y = -8;
                swordCollision.hitBox.width = 15;
                swordCollision.hitBox.height = 12;
                break;
            case Direction.LEFT:

                swordCollision.hitBox.x = -6;
                swordCollision.hitBox.y = -8;
                swordCollision.hitBox.width = 15;
                swordCollision.hitBox.height = 12;

                break;
            case Direction.DOWN:
                swordCollision.hitBox.x = 0;
                swordCollision.hitBox.y = -12;
                swordCollision.hitBox.width = 12;
                swordCollision.hitBox.height = 15;

                break;
            case Direction.UP:
                swordCollision.hitBox.x = 0;
                swordCollision.hitBox.y = 8;
                swordCollision.hitBox.width = 12;
                swordCollision.hitBox.height = 15;
                break;
        }
        swordCollision.UpdateHitBoxSprite();

        if (C.isTransitioning)
            return;
        switch (State)
        {
            case PlayerState.IDLE:
            case PlayerState.MOVE:
                maxXVel = 1f;
                maxYVel = 1;
                minYVel = -1f;
                if (State == PlayerState.MOVE)
                    if (RXRandom.Float() < .04f)
                        SpawnParticles((Direction)((int)(_direction + 2) % Enum.GetValues(typeof(Direction)).Length), 1);
                shouldDamage = false;
                if (C.getKey(C.JUMP_KEY) && CanJump && !hasInteractObject)
                {
                    FSoundManager.PlaySound("jump");
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

                    SpawnParticles((Direction)((int)(_direction + 2) % Enum.GetValues(typeof(Direction)).Length));

                    float newX = this.x;
                    float newY = this.y;
                    float targetY;
                    float targetX;
                    switch (_direction)
                    {
                        case Direction.UP:
                            targetY = this.y + maxJumpDist;
                            while (newY < targetY && !(world.CheckForJumpObjectCollision(this, this.x + hitBox.x, newY + hitBox.y + hitBox.height / 2f) != null || world.CheckForJumpObjectCollision(this, this.x + hitBox.x, newY + hitBox.y - hitBox.height / 2f) != null))
                                newY += 1;
                            while (newY > this.y && (!world.isAllPassable(this.x + hitBox.x, newY + hitBox.y + hitBox.height / 2f) || !world.isAllPassable(this.x + hitBox.x, newY + hitBox.y - hitBox.height / 2f) || world.CheckObjectCollision(this, this.x + hitBox.x, newY + hitBox.y + hitBox.height / 2f) != null || world.CheckObjectCollision(this, this.x + hitBox.x, newY + hitBox.y - hitBox.height / 2f) != null)) { newY -= 1f; } break;
                        case Direction.RIGHT:
                            targetX = this.x + maxJumpDist;
                            while (newX < targetX && !(world.CheckForJumpObjectCollision(this, newX + hitBox.x + hitBox.width / 2f, this.y + hitBox.y) != null || world.CheckForJumpObjectCollision(this, newX + hitBox.x - hitBox.width / 2f, this.y + hitBox.y) != null))
                                newX += 1;
                            while (newX > this.x && (!world.isAllPassable(newX + hitBox.x + hitBox.width / 2f, this.y + hitBox.y) || !world.isAllPassable(newX + hitBox.x - hitBox.width / 2f, this.y + hitBox.y) || world.CheckObjectCollision(this, newX + hitBox.x + hitBox.width / 2f, this.y + hitBox.y) != null || world.CheckObjectCollision(this, newX + hitBox.x - hitBox.width / 2f, this.y + hitBox.y) != null)) { newX -= 1f; } break;
                        case Direction.DOWN:
                            targetY = this.y - maxJumpDist;
                            while (newY > targetY && !(world.CheckForJumpObjectCollision(this, this.x + hitBox.x, newY + hitBox.y - hitBox.height / 2f) != null || world.CheckForJumpObjectCollision(this, this.x + hitBox.x, newY + hitBox.y + hitBox.height / 2f) != null))
                                newY -= 1;
                            while (newY < this.y && (!world.isAllPassable(this.x + hitBox.x, newY + hitBox.y + hitBox.height / 2f) || !world.isAllPassable(this.x + hitBox.x, newY + hitBox.y - hitBox.height / 2f) || world.CheckObjectCollision(this, this.x + hitBox.x, newY + hitBox.y - hitBox.height / 2f) != null || world.CheckObjectCollision(this, this.x + hitBox.x, newY + hitBox.y + hitBox.height / 2f) != null)) { newY += 1f; } break;
                        case Direction.LEFT:
                            targetX = this.x - maxJumpDist;
                            while (newX > targetX && !(world.CheckForJumpObjectCollision(this, newX + hitBox.x - hitBox.width / 2f, this.y + hitBox.y) != null || world.CheckForJumpObjectCollision(this, newX + hitBox.x + hitBox.width / 2f, this.y + hitBox.y) != null))
                                newX -= 1;
                            while (newX < this.x && (!world.isAllPassable(newX + hitBox.x + hitBox.width / 2f, this.y + hitBox.y) || !world.isAllPassable(newX + hitBox.x - hitBox.width / 2f, this.y + hitBox.y) || world.CheckObjectCollision(this, newX + hitBox.x - hitBox.width / 2f, this.y + hitBox.y) != null || world.CheckObjectCollision(this, newX + hitBox.x + hitBox.width / 2f, this.y + hitBox.y) != null)) { newX += 1f; } break;
                    }
                    xVel = 0;
                    yVel = 0;
                    float jumpTime = .5f;
                    float jumpDisp = 10f;
                    if (_direction == Direction.UP || _direction == Direction.DOWN)
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
                if (C.getKey(C.SELECT_KEY) && !lastSelectPress)
                {
                    if (selectedItem == SecondaryItem.SWORD && canBow)
                    {
                        FSoundManager.PlaySound("swap");
                        selectedItem = SecondaryItem.BOW;
                        world.ui.UpdateSelectedItem(selectedItem);
                    }
                    else if (selectedItem == SecondaryItem.BOW && canSword)
                    {
                        FSoundManager.PlaySound("swap");
                        selectedItem = SecondaryItem.SWORD;
                        world.ui.UpdateSelectedItem(selectedItem);
                    }
                }
                if (C.getKey(C.ACTION_KEY))
                {
                    hasSpawnedSpiritParticles = false;
                    switch (selectedItem)
                    {
                        case SecondaryItem.SWORD:
                            if (lastActionPress)
                                break;
                            FSoundManager.PlaySound("swordOne");
                            State = PlayerState.SWORD;
                            if (C.getKey(C.RIGHT_KEY))
                                _direction = Direction.RIGHT;
                            else if (C.getKey(C.DOWN_KEY))
                                _direction = Direction.DOWN;
                            else if (C.getKey(C.LEFT_KEY))
                                _direction = Direction.LEFT;
                            else if (C.getKey(C.UP_KEY))
                                _direction = Direction.UP;

                            SpawnParticles((Direction)((int)(_direction + 2) % Enum.GetValues(typeof(Direction)).Length));

                            Go.killAllTweensWithTarget(this);
                            float newX = this.x;
                            float newY = this.y;
                            switch (_direction)
                            {
                                case Direction.UP: newY += attackDist; while (newY > this.y && (!world.isAllPassable(this.x + hitBox.x, newY + hitBox.y + hitBox.height / 2f) || !world.isAllPassable(this.x + hitBox.x, newY + hitBox.y - hitBox.height / 2f) || world.CheckObjectCollision(this, this.x + hitBox.x, newY + hitBox.y + hitBox.height / 2f) != null || world.CheckObjectCollision(this, this.x + hitBox.x, newY + hitBox.y - hitBox.height / 2f) != null)) { newY -= 1f; } break;
                                case Direction.RIGHT: newX += attackDist; while (newX > this.x && (!world.isAllPassable(newX + hitBox.x + hitBox.width / 2f, this.y + hitBox.y) || !world.isAllPassable(newX + hitBox.x - hitBox.width / 2f, this.y + hitBox.y) || world.CheckObjectCollision(this, newX + hitBox.x + hitBox.width / 2f, this.y + hitBox.y) != null || world.CheckObjectCollision(this, newX + hitBox.x - hitBox.width / 2f, this.y + hitBox.y) != null)) { newX -= 1f; } break;
                                case Direction.DOWN: newY -= attackDist; while (newY < this.y && (!world.isAllPassable(this.x + hitBox.x, newY + hitBox.y + hitBox.height / 2f) || !world.isAllPassable(this.x + hitBox.x, newY + hitBox.y - hitBox.height / 2f) || world.CheckObjectCollision(this, this.x + hitBox.x, newY + hitBox.y - hitBox.height / 2f) != null || world.CheckObjectCollision(this, this.x + hitBox.x, newY + hitBox.y + hitBox.height / 2f) != null)) { newY += 1f; } break;
                                case Direction.LEFT: newX -= attackDist; while (newX < this.x && (!world.isAllPassable(newX + hitBox.x + hitBox.width / 2f, this.y + hitBox.y) || !world.isAllPassable(newX + hitBox.x - hitBox.width / 2f, this.y + hitBox.y) || world.CheckObjectCollision(this, newX + hitBox.x - hitBox.width / 2f, this.y + hitBox.y) != null || world.CheckObjectCollision(this, newX + hitBox.x + hitBox.width / 2f, this.y + hitBox.y) != null)) { newX += 1f; } break;
                            }
                            xVel = 0;
                            yVel = 0;
                            if (newX == this.x)
                            {
                                Go.to(this, attackTime / 2f, new TweenConfig().floatProp("x", -attackDisp, true).setEaseType(EaseType.QuadOut).setIterations(2, LoopType.PingPong));
                                Go.to(this, attackTime, new TweenConfig().floatProp("y", newY).setEaseType(EaseType.BackInOut).onComplete(() => { }));

                            }
                            else
                            {
                                Go.to(this, attackTime / 2f, new TweenConfig().floatProp("y", attackDisp, true).setEaseType(EaseType.QuadOut).setIterations(2, LoopType.PingPong));
                                Go.to(this, attackTime, new TweenConfig().floatProp("x", newX).setEaseType(EaseType.BackInOut).onComplete(() => { }));

                            }
                            if (_direction == Direction.RIGHT)
                                scaleX = -1;
                            else if (_direction == Direction.LEFT)
                                scaleX = 1;
                            return;
                        case SecondaryItem.BOW:
                            SpawnDeathParticles(Direction.UP, 20);
                            State = PlayerState.BOW_DRAWN;
                            return;
                    }
                }
                if (C.getKey(C.LEFT_KEY))
                {

                    xAcc = -moveSpeed;
                    if (xVel > 0)
                    {

                        SpawnParticles((Direction)((int)(_direction + 2) % Enum.GetValues(typeof(Direction)).Length), 1);
                        xVel *= .8f;
                    }

                }
                else
                    if (C.getKey(C.RIGHT_KEY))
                    {

                        xAcc = moveSpeed;
                        if (xVel < 0)
                        {
                            SpawnParticles((Direction)((int)(_direction + 2) % Enum.GetValues(typeof(Direction)).Length), 1);
                            xVel *= .8f;
                        }

                    }
                    else
                        xVel *= .7f;
                if (C.getKey(C.UP_KEY))
                {
                    yAcc = moveSpeed;
                    if (yVel < 0)
                    {

                        SpawnParticles((Direction)((int)(_direction + 2) % Enum.GetValues(typeof(Direction)).Length), 1);
                        yVel *= .8f;
                    }

                }
                else if (C.getKey(C.DOWN_KEY))
                {
                    yAcc = -moveSpeed;
                    if (yVel > 0)
                    {

                        SpawnParticles((Direction)((int)(_direction + 2) % Enum.GetValues(typeof(Direction)).Length), 1);
                        yVel *= .8f;
                    }
                }
                else
                    yVel *= .7f;


                break;
            case PlayerState.BOW_DRAWN:
                if (!C.getKey(C.ACTION_KEY))
                {
                    State = PlayerState.BOW_SHOOTING;
                    Arrow arrow = new Arrow(this, world);
                    arrow.SetDirection(CurrentDirection);
                    arrow.PlayAnim();
                    arrow.SetPosition(this.GetPosition());
                    switch (CurrentDirection)
                    {
                        case Direction.UP: arrow.yVel = 2; break;
                        case Direction.RIGHT: arrow.xVel = 2; break;
                        case Direction.DOWN: arrow.yVel = -2; break;
                        case Direction.LEFT: arrow.xVel = -2; break;
                    }
                    world.addObject(arrow);
                    return;
                }
                float strafeSpeed = 5f;
                if (C.getKey(C.LEFT_KEY))
                    xVel = -strafeSpeed;
                else if (C.getKey(C.RIGHT_KEY))
                    xVel = strafeSpeed;
                else xVel *= .8f;

                if (C.getKey(C.UP_KEY))
                    yVel = strafeSpeed;
                else if (C.getKey(C.DOWN_KEY))
                    yVel = -strafeSpeed;
                else yVel *= .8f;

                break;
            case PlayerState.BOW_SHOOTING:
                xVel *= .8f;
                yVel *= .8f;
                if (stateCount > .5f)
                {
                    State = PlayerState.IDLE;
                    SpawnDeathParticles(Direction.UP, 20);
                }
                break;
            case PlayerState.SWORD:
                shouldDamage = stateCount > .2f && stateCount < .4f;
                if (stateCount > .6f)
                    State = PlayerState.IDLE;
                else if (stateCount > .3f)
                {

                    if (C.getKey(C.ACTION_KEY) && !lastActionPress)
                    {
                        float attack2Time = .25f;
                        float attack2Dist = 20;
                        float attack2Disp = 5f;
                        State = PlayerState.SWORD_TWO;
                        FSoundManager.PlaySound("swordTwo");
                        hasSpawnedSpiritParticles = false;
                        if (C.getKey(C.RIGHT_KEY))
                            _direction = Direction.RIGHT;
                        else if (C.getKey(C.DOWN_KEY))
                            _direction = Direction.DOWN;
                        else if (C.getKey(C.LEFT_KEY))
                            _direction = Direction.LEFT;
                        else if (C.getKey(C.UP_KEY))
                            _direction = Direction.UP;
                        SpawnParticles((Direction)((int)(_direction + 2) % Enum.GetValues(typeof(Direction)).Length));

                        Go.killAllTweensWithTarget(this);
                        float newX = this.x;
                        float newY = this.y;
                        PlayAnim(true);
                        switch (_direction)
                        {
                            case Direction.UP: newY += attack2Dist; while (newY > this.y && (!world.isAllPassable(this.x + hitBox.x, newY + hitBox.y + hitBox.height / 2f) || !world.isAllPassable(this.x + hitBox.x, newY + hitBox.y - hitBox.height / 2f) || world.CheckObjectCollision(this, this.x + hitBox.x, newY + hitBox.y + hitBox.height / 2f) != null || world.CheckObjectCollision(this, this.x + hitBox.x, newY + hitBox.y - hitBox.height / 2f) != null)) { newY -= 1f; } break;
                            case Direction.RIGHT: newX += attack2Dist; while (newX > this.x && (!world.isAllPassable(newX + hitBox.x + hitBox.width / 2f, this.y + hitBox.y) || !world.isAllPassable(newX + hitBox.x - hitBox.width / 2f, this.y + hitBox.y) || world.CheckObjectCollision(this, newX + hitBox.x + hitBox.width / 2f, this.y + hitBox.y) != null || world.CheckObjectCollision(this, newX + hitBox.x - hitBox.width / 2f, this.y + hitBox.y) != null)) { newX -= 1f; } break;
                            case Direction.DOWN: newY -= attack2Dist; while (newY < this.y && (!world.isAllPassable(this.x + hitBox.x, newY + hitBox.y + hitBox.height / 2f) || !world.isAllPassable(this.x + hitBox.x, newY + hitBox.y - hitBox.height / 2f) || world.CheckObjectCollision(this, this.x + hitBox.x, newY + hitBox.y - hitBox.height / 2f) != null || world.CheckObjectCollision(this, this.x + hitBox.x, newY + hitBox.y + hitBox.height / 2f) != null)) { newY += 1f; } break;
                            case Direction.LEFT: newX -= attack2Dist; while (newX < this.x && (!world.isAllPassable(newX + hitBox.x + hitBox.width / 2f, this.y + hitBox.y) || !world.isAllPassable(newX + hitBox.x - hitBox.width / 2f, this.y + hitBox.y) || world.CheckObjectCollision(this, newX + hitBox.x - hitBox.width / 2f, this.y + hitBox.y) != null || world.CheckObjectCollision(this, newX + hitBox.x + hitBox.width / 2f, this.y + hitBox.y) != null)) { newX += 1f; } break;
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
                else if (stateCount > .25f)
                {
                    if (!hasSpawnedSpiritParticles)
                    {
                        SpawnParticles(_direction);
                        hasSpawnedSpiritParticles = true;
                    }
                }

                break;
            case PlayerState.SWORD_TWO:
                shouldDamage = stateCount > .2f;
                if (stateCount > .6f)
                    State = PlayerState.IDLE;
                else if (stateCount > .19f)
                    if (!hasSpawnedSpiritParticles)
                    {
                        SpawnParticles(_direction, 30);
                        hasSpawnedSpiritParticles = true;
                    }
                break;
            case PlayerState.INVULNERABLE:
                if (RXRandom.Float() < .2f)
                    SpawnParticles(Direction.UP, 1);
                this.isVisible = stateCount * 100 % 10 < 5;
                if (stateCount > invulnerableStunTime)
                {
                    State = PlayerState.IDLE;

                    maxXVel = 1;
                    maxYVel = 1;
                    minYVel = -1;
                    this.isVisible = true;
                }
                this.xVel *= .9f;
                this.yVel *= .9f;

                break;
            case PlayerState.DYING:
                if (!hasSpawnedSpiritParticles)
                {

                    if (RXRandom.Float() < .4f + .4f * stateCount)
                        SpawnDeathParticles(Direction.UP, 1 + (int)stateCount);
                    if (stateCount > invulnerableStunTime * 3)
                    {
                        FSoundManager.PlaySound("death");
                        C.getCameraInstance().shake(1.0f, .5f);
                        SpawnDeathParticles(Direction.UP, 20);
                        this.isVisible = false;
                        hasSpawnedSpiritParticles = true;
                    }
                    else
                        this.isVisible = stateCount * 100 % 10 < 5;
                }
                if (stateCount > invulnerableStunTime * 5)
                {
                    world.Respawn();

                    maxXVel = 1;
                    maxYVel = 1;
                    minYVel = -1;
                }
                this.xVel *= .9f;
                this.yVel *= .9f;
                break;


        }

        if (xAcc != 0 || yAcc != 0)
        {
            switch (State)
            {
                case PlayerState.IDLE:
                    {
                        State = PlayerState.MOVE;
                    }
                    break;
            }
        }
        else
        {
            switch (State)
            {
                case PlayerState.MOVE:
                    State = PlayerState.IDLE;
                    break;
            }
        }

        base.OnFixedUpdate();
        if (stateCount == 0 && State == PlayerState.MOVE)

            SpawnParticles((Direction)((int)(_direction + 2) % Enum.GetValues(typeof(Direction)).Length), 1);
        lastActionPress = C.getKey(C.ACTION_KEY);
        lastSelectPress = C.getKey(C.SELECT_KEY);
        lastJumpPress = C.getKey(C.JUMP_KEY);
        PlayAnim();
    }

    protected override void OnUpdate()
    {
        if (C.isTransitioning)
            return;
        switch (State)
        {
            case PlayerState.DYING:
                base.OnUpdate();
                return;
        }
        if (invulnCount > 0)
        {
            this.isVisible = invulnCount * 50 % 10 < 5;
            invulnCount -= Time.deltaTime;
        }
        else
            this.isVisible = true;
        base.OnUpdate();
    }

    private void SpawnDeathParticles(Direction dir, int numParticles = 10)
    {
        for (int i = 0; i < numParticles; i++)
        {
            Particle.ParticleOne p = Particle.ParticleOne.getParticle();
            Vector2 vel = new Vector2(RXRandom.Float() * 20 - 10, RXRandom.Float() * 10);
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
    private void SpawnParticles(Direction dir, int numParticles = 10)
    {
        for (int i = 0; i < numParticles; i++)
        {
            Particle.ParticleOne p = Particle.ParticleOne.getParticle();
            Vector2 vel = new Vector2(RXRandom.Float() * 60, RXRandom.Float() * 30);
            Vector2 acc = new Vector2(-vel.x * (RXRandom.Float() * .5f), -vel.y * -1.0f);
            Vector2 pos = new Vector2(RXRandom.Float() * 10 - 5, -8);
            switch (dir)
            {
                case Direction.DOWN:
                    //vel.y *= -1;
                    acc.y *= -1;
                    pos.y = -10;

                    break;
                case Direction.RIGHT:
                    float tempX = vel.x;
                    vel.x = vel.y;
                    vel.y = tempX;
                    tempX = acc.x;
                    acc.x = acc.y;
                    acc.y = tempX;
                    tempX = pos.x;
                    pos.x = pos.y;
                    pos.y = tempX;
                    pos.x *= .5f;
                    pos.y = -8;
                    pos.x *= -1;
                    break;
                case Direction.UP:
                    pos.y = 0;
                    break;
                case Direction.LEFT:
                    tempX = vel.x;
                    vel.x = vel.y;
                    vel.y = tempX;
                    tempX = acc.x;
                    acc.x = acc.y;
                    acc.y = tempX;
                    tempX = pos.x;
                    pos.x = pos.y;
                    pos.y = tempX;
                    pos.x *= .5f;
                    pos.y = -8;
                    vel.x *= -1;
                    acc.x *= -1;
                    break;
            }
            p.activate(this.GetPosition() + pos, vel, acc, RXRandom.Bool() ? 180.0f : 0);
            this.container.AddChild(p);
        }
    }
    public void PlayAnim(bool forced = false)
    {
        player.play(State.ToString() + _direction.ToString(), forced);
    }

}


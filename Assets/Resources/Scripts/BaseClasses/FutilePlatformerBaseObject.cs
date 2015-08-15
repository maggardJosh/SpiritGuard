using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class FutilePlatformerBaseObject : FContainer
{
    public RXRect hitBox;
    public float xAcc;
    public float yAcc;
    public float xVel;
    public float yVel;
    protected float maxXVel = 2.5f;
    protected float maxYVel = 2.5f;
    protected float minYVel = -2.5f;
    protected bool collidesWithWalls = true;
    protected bool collidesWithWater = true;
    protected bool handleDamageObjectCollision = false;
    float tileSize;
    protected World world;
    protected bool clearAcc = true;

    protected bool handleStateCount = false;
    protected float stateCount = 0;
    public float StateCount { get { return stateCount; } }
    bool applyGravity = false;
    protected bool grounded = false;
    protected float groundedMargin = 16f;       //Amount off the ground that the player is considered to be grounded (Gives the player some leeway)
    protected float bounceiness = .8f;

    private float[] horizontalYChecks;
    private float[] verticalXChecks;
    protected bool hitLeft = false;
    protected bool hitRight = false;
    protected bool hitDown = false;
    protected bool hitUp = false;
    FSprite collisionDebugSprite;

    public bool HitSomething { get { return hitLeft || hitRight || hitDown || hitUp; } }

    public FutilePlatformerBaseObject(RXRect hitBox, World world)
    {
        this.hitBox = hitBox;
        this.world = world;
        this.tileSize = world.TileSize;

        if (C.isDebug)
        {
            collisionDebugSprite = new FSprite("boundingBox");
            collisionDebugSprite.x = hitBox.x;
            collisionDebugSprite.y = hitBox.y;
            collisionDebugSprite.width = hitBox.width;
            collisionDebugSprite.height = hitBox.height;
            collisionDebugSprite.sortZ = 100;
            this.shouldSortByZ = true;
            this.AddChild(collisionDebugSprite);
        }

        UpdateCollisionChecks();
    }
    public void UpdateHitBoxSprite()
    {
        if (C.isDebug)
        {
            collisionDebugSprite.x = hitBox.x;
            collisionDebugSprite.y = hitBox.y;
            collisionDebugSprite.width = hitBox.width;
            collisionDebugSprite.height = hitBox.height;
        }
    }
    public void UpdateHitBox(float newWidth, float newHeight)
    {
        if (this.hitBox.width == newWidth &&
            this.hitBox.height == newHeight)
            return;
        float oldWidth = hitBox.width;
        float oldHeight = hitBox.height;
        float[] oldXChecks = verticalXChecks;
        float[] oldYChecks = horizontalYChecks;
        newWidth = Mathf.Clamp(newWidth, 0, 1000);
        newHeight = Mathf.Clamp(newHeight, 0, 1000);
        this.hitBox.width = newWidth;
        this.hitBox.height = newHeight;
        if (C.isDebug)
        {
            collisionDebugSprite.x = hitBox.x;
            collisionDebugSprite.y = hitBox.y;
            collisionDebugSprite.width = newWidth;
            collisionDebugSprite.height = newHeight;
        }
        UpdateCollisionChecks();
        if (!TryMoveDown(-.1f) & !TryMoveUp(.1f))
        {
            hitBox.height = oldHeight;
            verticalXChecks = oldXChecks;
        }
        if (!TryMoveLeft(-.1f) & !TryMoveRight(.1f))
        {
            hitBox.width = oldWidth;
            horizontalYChecks = oldYChecks;
        }
    }

    private void UpdateCollisionChecks()
    {
        float sideMargin = 2;
        horizontalYChecks = new float[2 + Mathf.FloorToInt((hitBox.height - (sideMargin * 2)) / (tileSize / 2f))];
        int ind = 0;
        for (float i = -hitBox.height / 2 + sideMargin; i < hitBox.height / 2 - sideMargin; i += tileSize / 2f)
            horizontalYChecks[ind++] = i;
        horizontalYChecks[ind] = hitBox.height / 2f - sideMargin;

        verticalXChecks = new float[2 + Mathf.FloorToInt((hitBox.width - (sideMargin * 2)) / (tileSize / 2f))];
        ind = 0;
        for (float i = -hitBox.width / 2 + sideMargin; i < hitBox.width / 2 - sideMargin; i += tileSize / 2f)
            verticalXChecks[ind++] = i;
        verticalXChecks[ind] = hitBox.width / 2f - sideMargin;
    }

    public override void HandleAddedToStage()
    {
        Futile.instance.SignalFixedUpdate += OnFixedUpdate;
        if (handleStateCount)
            Futile.instance.SignalUpdate += OnUpdate;
        base.HandleAddedToStage();
    }

    public override void HandleRemovedFromStage()
    {
        Futile.instance.SignalFixedUpdate -= OnFixedUpdate;
        if (handleStateCount)
            Futile.instance.SignalUpdate -= OnUpdate;
        base.HandleRemovedFromStage();
    }
    protected virtual void OnUpdate()
    {
        stateCount += Time.deltaTime;
    }
    public virtual void OnFixedUpdate()
    {
        if (C.isTransitioning)
            return;
        hitLeft = false;
        hitRight = false;
        hitDown = false;
        hitUp = false;

        if (xAcc > 0)
        {
            if (scaleX != -1 && collisionDebugSprite != null)
                collisionDebugSprite.x *= -1;
            scaleX = -1;
        }
        else if (xAcc < 0)
        {
            if (scaleX != 1 && collisionDebugSprite != null)
                collisionDebugSprite.x *= -1;
            scaleX = 1;
        }
        if (applyGravity)
            this.yVel += world.gravity;
        this.xVel += xAcc;
        this.yVel += yAcc;
        if (clearAcc)
        {
            xAcc = 0;
            yAcc = 0;
        }

        this.yVel = Mathf.Clamp(this.yVel, minYVel, maxYVel);
        this.xVel = Mathf.Clamp(this.xVel, -maxXVel, maxXVel);

        if (this is Player)
            world.CheckDamageObjectCollision();
        else if (this is Knight)
            world.CheckDamageObjectCollision(this);
        if (yVel > 0)
            hitUp = !TryMoveUp(yVel);
        else if (yVel < 0)
            hitDown = !TryMoveDown(yVel);

        if (xVel > 0)
        {
            hitRight = !TryMoveRight(xVel);
        }
        else if (xVel < 0)
        {

            hitLeft = !TryMoveLeft(xVel);
        }
    }

    
    protected virtual bool HandleDamageObjectCollision(FutilePlatformerBaseObject damageObject)
    {
        return false;
    }

    private bool TryMoveRight(float amount)
    {
        float newX = this.x + amount;
        float lastX = x;
        while (x < newX)
        {
            x += Mathf.Min(tileSize / 2, newX - x);
            foreach (float yCheck in horizontalYChecks)
            {
                if ((collidesWithWater && !world.isPassable(x + hitBox.x + hitBox.width / 2, this.y + hitBox.y + yCheck)) || (collidesWithWalls && !world.isWallPassable(x + hitBox.x + hitBox.width / 2, this.y + hitBox.y + yCheck)))
                {
                    this.x = Mathf.FloorToInt((this.x + hitBox.x + hitBox.width / 2) / tileSize) * tileSize - hitBox.x - hitBox.width / 2;
                    xVel *= -bounceiness;
                    return false;
                }

                if (this.handleDamageObjectCollision)
                {
                    FutilePlatformerBaseObject damageObject = world.CheckDamageObjectCollisionGetRect(this, this.x + hitBox.x + hitBox.width / 2, y + hitBox.y + yCheck);
                    if (damageObject != null)
                    {
                        if (HandleDamageObjectCollision(damageObject))
                        {
                            xVel *= -bounceiness;
                            this.x = damageObject.x + damageObject.hitBox.x - damageObject.hitBox.width/2f - hitBox.x - hitBox.width / 2f;
                            return false;
                            
                        }
                    }
                }

                RXRect objectCollision = world.CheckObjectCollision(this, this.x + hitBox.x + hitBox.width / 2, y + hitBox.y + yCheck);
                if (objectCollision != null)
                {
                    xVel *= -bounceiness;
                    this.x = objectCollision.x - hitBox.x - hitBox.width / 2;
                    return false;
                }
            }
            lastX = x;
        }
        return true;
    }

    private bool TryMoveLeft(float amount)
    {
        float newX = this.x + amount;
        float lastX = x;
        while (x > newX)
        {
            x += Mathf.Max(-tileSize / 2, newX - x);
            foreach (float yCheck in horizontalYChecks)
            {
                if ((collidesWithWater && !world.isPassable(x + hitBox.x - hitBox.width / 2, this.y + hitBox.y + yCheck)) || (collidesWithWalls && !world.isWallPassable(x + hitBox.x - hitBox.width / 2, this.y + hitBox.y + yCheck)))
                {
                    this.x = Mathf.CeilToInt((this.x + hitBox.x - hitBox.width / 2) / tileSize) * tileSize - hitBox.x + hitBox.width / 2;
                    xVel *= -bounceiness;
                    return false;
                }

                if (this.handleDamageObjectCollision)
                {
                    FutilePlatformerBaseObject damageObject = world.CheckDamageObjectCollisionGetRect(this, this.x + hitBox.x - hitBox.width / 2, y + hitBox.y + yCheck);
                    if (damageObject != null)
                    {

                        if (HandleDamageObjectCollision(damageObject))
                        {
                            xVel *= -bounceiness;
                            this.x = damageObject.x + damageObject.hitBox.x + damageObject.hitBox.width/2f - hitBox.x + hitBox.width / 2;
                            return false;

                        }
                    }
                }

                RXRect objectCollision = world.CheckObjectCollision(this, this.x + hitBox.x - hitBox.width / 2, y + hitBox.y + yCheck);
                if (objectCollision != null)
                {
                    xVel *= -bounceiness;
                    this.x = objectCollision.x + objectCollision.width - hitBox.x + hitBox.width / 2;
                    return false;
                }
            }
            lastX = x;
        }
        return true;
    }

    private bool TryMoveDown(float amount)
    {
        float targetY = this.y + amount;
        float lastY = y;
        while (y > targetY)
        {
            y += Mathf.Max(-tileSize / 2, targetY - y);
            foreach (float xCheck in verticalXChecks)
            {
                if ((collidesWithWater && !world.isPassable(this.x + hitBox.x + xCheck, y + hitBox.y - hitBox.height / 2)) || (collidesWithWalls && !world.isWallPassable(this.x + hitBox.x + xCheck, y + hitBox.y - hitBox.height / 2)))
                {
                    grounded = true;
                    yVel *= -bounceiness;
                    this.y = Mathf.CeilToInt((this.y + hitBox.y - hitBox.height / 2f) / tileSize) * tileSize - hitBox.y + hitBox.height / 2;
                    return false;
                }


                if (this.handleDamageObjectCollision)
                {
                    FutilePlatformerBaseObject damageObject = world.CheckDamageObjectCollisionGetRect(this, this.x + hitBox.x + xCheck, y + hitBox.y - hitBox.height / 2);
                    if (damageObject != null)
                    {

                        if (HandleDamageObjectCollision(damageObject))
                        {
                            yVel *= -bounceiness;
                            this.y = damageObject.y + damageObject.hitBox.y + damageObject.hitBox.height / 2f - hitBox.y + hitBox.height / 2f;
                            return false;

                        }
                    }
                }

                RXRect objectCollision = world.CheckObjectCollision(this, this.x + hitBox.x + xCheck, y + hitBox.y - hitBox.height / 2);
                if (objectCollision != null)
                {
                    yVel *= -bounceiness;
                    this.y = objectCollision.y + objectCollision.height - hitBox.y + hitBox.height / 2;
                    return false;
                }
            }
            lastY = y;

        }
        return true;
    }

    private bool TryMoveUp(float amount)
    {
        grounded = false;
        float newY = this.y + amount;
        float lastY = y;
        while (y < newY)
        {
            y += Mathf.Min(tileSize / 2, newY - y);
            foreach (float xCheck in verticalXChecks)
            {
                if ((collidesWithWater && !world.isPassable(this.x + hitBox.x + xCheck, y + hitBox.y + hitBox.height / 2)) || (collidesWithWalls && !world.isWallPassable(this.x + hitBox.x + xCheck, y + hitBox.y + hitBox.height / 2)))
                {
                    this.y = Mathf.FloorToInt((this.y + hitBox.y + hitBox.height / 2f) / tileSize) * tileSize - hitBox.y - hitBox.height / 2;
                    if (bounceiness != 0)
                        yVel *= -bounceiness;

                    return false;
                }

                if (this.handleDamageObjectCollision)
                {
                    FutilePlatformerBaseObject damageObject = world.CheckDamageObjectCollisionGetRect(this, this.x + hitBox.x + xCheck, y + hitBox.y + hitBox.height / 2);
                    if (damageObject != null)
                    {

                        if (HandleDamageObjectCollision(damageObject))
                        {
                            yVel *= -bounceiness;
                            this.y = damageObject.y + damageObject.hitBox.y - damageObject.hitBox.height / 2f - hitBox.y - hitBox.height / 2f;
                            return false;
                        }
                    }
                }

                RXRect objectCollision = world.CheckObjectCollision(this, this.x + hitBox.x + xCheck, y + hitBox.y + hitBox.height / 2);
                if (objectCollision != null)
                {
                    yVel *= -bounceiness;
                    this.y = objectCollision.y - hitBox.y - hitBox.height / 2;
                    return false;
                }
            }
            lastY = y;
        }
        return true;
    }

    public bool isColliding(FutilePlatformerBaseObject other)
    {

        return this.x + hitBox.x - hitBox.width / 2f < other.x + other.hitBox.x + other.hitBox.width / 2f &&
            this.x + hitBox.x + hitBox.width / 2f > other.x + other.hitBox.x - other.hitBox.width / 2f &&
            this.y + hitBox.y + hitBox.height / 2f > other.y + other.hitBox.y - other.hitBox.height / 2f &&
            this.y + hitBox.y - hitBox.height / 2f < other.y + other.hitBox.y + other.hitBox.height / 2f;
    }
}


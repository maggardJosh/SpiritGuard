﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class World : FContainer
{
    FTmxMap map;
    FTilemap wallCollisionTilemap;
    FTilemap collisionTilemap;
    FTilemap backgroundTilemap;
    FTilemap foregroundTilemap;
    public FSprite loadingBG;
    public UI ui;
    public Player player;

    FContainer background = new FContainer();
    FContainer playerLayer = new FContainer();
    FContainer foreground = new FContainer();

    List<SpawnPoint> spawnPoints = new List<SpawnPoint>();
    List<FutilePlatformerBaseObject> collisionObjects = new List<FutilePlatformerBaseObject>();
    List<FutilePlatformerBaseObject> damageObjects = new List<FutilePlatformerBaseObject>();
    List<Sign> signs = new List<Sign>();
    List<Villager> villagers = new List<Villager>();
    List<HitSwitch> hitSwitches = new List<HitSwitch>();

    public float TileSize
    {
        get
        {
            if (collisionTilemap != null)
                return Mathf.Max(collisionTilemap.tileWidth, collisionTilemap.tileHeight);
            else
                return 0;
        }
    }

    public float gravity = 0;

    public World()
    {
        map = new FTmxMap();

        playerLayer.shouldSortByZ = true;

        this.AddChild(background);
        this.AddChild(playerLayer);
        this.AddChild(foreground);
        Futile.instance.SignalUpdate += Update;
        loadingBG = new FSprite("loadingBG");
        loadingBG.isVisible = false;
        loadingBG.x = -Futile.screen.halfWidth - loadingBG.width / 2;

        C.getCameraInstance().AddChild(loadingBG);
        ui = new UI();

    }

    public bool forceWaitLoad = false;
    public void ShowLoading(Action loadAction, Action finishAction = null, bool fromRight = true)
    {
        float midPos;
        if (fromRight)
        {
            loadingBG.x = Futile.screen.halfWidth + loadingBG.width / 2f;
            loadingBG.rotation = 180f;
            midPos = Futile.screen.halfWidth - loadingBG.width / 2f;

        }
        else
        {
            loadingBG.x = -Futile.screen.halfWidth - loadingBG.width / 2;
            loadingBG.rotation = 0;
            midPos = -Futile.screen.halfWidth + loadingBG.width / 2;

        }
        FSoundManager.PlaySound("slideOn");
        loadingBG.isVisible = true;
        C.isTransitioning = true;
        Go.to(loadingBG, .7f, new TweenConfig().floatProp("x", midPos).setDelay(.1f).setEaseType(EaseType.QuadOut).onComplete(() =>
        {
            loadAction.Invoke();
            if (!forceWaitLoad)
                HideLoading(finishAction, fromRight);

        }));
    }
    public void HideLoading(Action doneAction, bool fromRight = true)
    {
        forceWaitLoad = false;
        float midStartPos;
        float finalPos;
        if (fromRight)
        {
            midStartPos = -Futile.screen.halfWidth + loadingBG.width / 2f;
            finalPos = -Futile.screen.halfWidth - loadingBG.width;
        }
        else
        {
            midStartPos = Futile.screen.halfWidth - loadingBG.width / 2;
            finalPos = -Futile.screen.halfWidth - loadingBG.width;
        }
        loadingBG.rotation += 180.0f;
        loadingBG.x = midStartPos;
        FSoundManager.PlaySound("slideOff");
        Go.to(loadingBG, 1.2f, new TweenConfig().floatProp("x", finalPos).setEaseType(EaseType.QuadIn).onComplete(() =>
        {
            C.isTransitioning = false;
            if (doneAction != null)
                doneAction.Invoke();
            loadingBG.isVisible = false;
        }));
    }
    public void LoadMap(string mapName)
    {
        C.Save.lastMap = mapName;
        spawnPoints.Clear();
        signs.Clear();
        villagers.Clear();
        hitSwitches.Clear();
        collisionObjects.Clear();
        damageObjects.Clear();

        playerLayer.RemoveAllChildren();
        background.RemoveAllChildren();
        foreground.RemoveAllChildren();
        this.map = new FTmxMap();
        this.map.LoadTMX("Maps/" + mapName);

        FSoundManager.PlayMusic(map.mapMusic);


        collisionTilemap = (FTilemap)this.map.getLayerNamed("collision");
        wallCollisionTilemap = (FTilemap)this.map.getLayerNamed("walls");
        backgroundTilemap = (FTilemap)this.map.getLayerNamed("background");
        foregroundTilemap = (FTilemap)this.map.getLayerNamed("foreground");
        background.AddChild(backgroundTilemap);
        background.AddChild(collisionTilemap);
        background.AddChild(wallCollisionTilemap);
        foreground.AddChild(foregroundTilemap);

        if (player == null)
        {
            player = new Player(this);
            C.getCameraInstance().follow(player);
            player.LoadLastSave();
        }
        collisionObjects.Add(player);
        playerLayer.AddChild(player);
        this.y = -16;
        C.getCameraInstance().setWorldBounds(new Rect(0, -collisionTilemap.height - 16, collisionTilemap.width, collisionTilemap.height + 16));
        foregroundTilemap.clipNode = C.getCameraInstance();
        collisionTilemap.clipNode = C.getCameraInstance();
        wallCollisionTilemap.clipNode = C.getCameraInstance();
        backgroundTilemap.clipNode = C.getCameraInstance();

        MapLoader.loadObjects(this, map.objects);
    }
    public bool isAllPassable(FutilePlatformerBaseObject self, float x, float y, bool isMovingHorizontal = true)
    {

        float xPos = x + self.hitBox.x + (isMovingHorizontal ? 0 : -1);
        float yPos = y + self.hitBox.y + (isMovingHorizontal ? -1 : 0);
        float width = self.hitBox.width + (isMovingHorizontal ? 0 : -4);
        float height = self.hitBox.height + (isMovingHorizontal ? -4 : 0);
        return
            isWallPassable(xPos - width / 2f, yPos - height / 2f) &&
            isWallPassable(xPos + width / 2f, yPos - height / 2f) &&
            isWallPassable(xPos - width / 2f, yPos + height / 2f) &&
            isWallPassable(xPos + width / 2f, yPos + height / 2f) &&
            isPassable(xPos - width / 2f, yPos - height / 2f) &&
            isPassable(xPos + width / 2f, yPos - height / 2f) &&
            isPassable(xPos - width / 2f, yPos + height / 2f) &&
            isPassable(xPos + width / 2f, yPos + height / 2f);
    }
    public bool isAllPassable(float x, float y)
    {
        return isWallPassable(x, y) && isPassable(x, y);
    }
    public bool isWallPassable(float x, float y)
    {
        return wallCollisionTilemap.IsPassable(x, y);
    }
    public bool isPassable(float x, float y)
    {
        return collisionTilemap.IsPassable(x, y);
    }

    public void SpawnPlayer(string spawnName)
    {
        foreach (SpawnPoint s in spawnPoints)
            if (s.name.ToLower() == spawnName.ToLower())
            {
                player.State = Player.PlayerState.IDLE;
                C.Save.lastDoor = spawnName;
                C.lastSave.copy(C.Save);
                if (s.showMapName)
                {
                    FLabel mapNameLabel = new FLabel(C.largeFontName, map.mapName);
                    C.getCameraInstance().AddChild(mapNameLabel);
                    mapNameLabel.SetPosition(new Vector2(Futile.screen.width, Futile.screen.halfHeight * .8f));
                    Go.to(mapNameLabel, 1.5f, new TweenConfig().floatProp("x", 0).setEaseType(EaseType.BackOut).onComplete(() =>
                    {
                        Go.to(mapNameLabel, .7f, new TweenConfig().floatProp("x", -Futile.screen.halfWidth).setDelay(.6f).setEaseType(EaseType.BackIn).onComplete(() => { mapNameLabel.RemoveFromContainer(); }));
                    }));
                }
                s.SpawnPlayer(player);
                return;
            }
    }

    public void LoadNewMap(string mapName, string spawnName)
    {
        ShowLoading(() => { LoadMap(mapName); SpawnPlayer(spawnName); });
    }

    public void addSpawn(SpawnPoint spawn)
    {
        spawnPoints.Add(spawn);
    }

    public void addObject(FNode objectToAdd)
    {
        if (objectToAdd is Knight || objectToAdd is Arrow || objectToAdd is Heart || objectToAdd is MagicOrb || objectToAdd is Ghost || objectToAdd is SoulPickup)
            damageObjects.Add((FutilePlatformerBaseObject)objectToAdd);
        else
            if (objectToAdd is FutilePlatformerBaseObject)
                collisionObjects.Add((FutilePlatformerBaseObject)objectToAdd);

        if (objectToAdd is Sign)
            signs.Add((Sign)objectToAdd);
        if (objectToAdd is Villager)
            villagers.Add((Villager)objectToAdd);
        if (objectToAdd is HitSwitch)
            hitSwitches.Add((HitSwitch)objectToAdd);
        if (objectToAdd is PushBlock || objectToAdd is Switch)
            background.AddChild(objectToAdd);
        else
            playerLayer.AddChild(objectToAdd);
    }

    public void Respawn()
    {
        ShowLoading(() =>
        {
            player.LoadLastSave();
            LoadMap(C.Save.lastMap);
            SpawnPlayer(C.Save.lastDoor);
            player.invulnCount = 2;
            player.Health = 3;
            this.isVisible = true;

        });
    }

    public void removeObject(FNode objectToRemove)
    {

        if (objectToRemove is Knight || objectToRemove is Arrow || objectToRemove is Heart || objectToRemove is MagicOrb || objectToRemove is Ghost || objectToRemove is SoulPickup)
        {
            damageObjects.Remove((FutilePlatformerBaseObject)objectToRemove);
        }
        else
            if (objectToRemove is FutilePlatformerBaseObject)
                collisionObjects.Remove((FutilePlatformerBaseObject)objectToRemove);

        if (objectToRemove is Sign)
            signs.Remove((Sign)objectToRemove);
        if (objectToRemove is Villager)
            villagers.Remove((Villager)objectToRemove);
        if (objectToRemove is HitSwitch)
            hitSwitches.Remove((HitSwitch)objectToRemove);
        objectToRemove.RemoveFromContainer();
    }

    RXRect worldPos = new RXRect();
    RXRect selfRect = new RXRect();
    public RXRect CheckObjectCollisionHitbox(FutilePlatformerBaseObject self, float x, float y, float sideMargin = 4)
    {
        selfRect.x = x + self.hitBox.x - (self.hitBox.width - sideMargin) / 2f;
        selfRect.y = y + self.hitBox.y - (self.hitBox.height - sideMargin) / 2f;
        selfRect.width = self.hitBox.width - sideMargin;
        selfRect.height = self.hitBox.height - sideMargin;
        foreach (FutilePlatformerBaseObject o in collisionObjects)
        {
            if (!o.blocksOtherObjects && !(o is Switch))
                continue;
            if ((self is Knight && o is Player))
                continue;
            if ((self is Ghost && o is Player))
                continue;
            if (self is Arrow && ((Arrow)self).owner == o)
                continue;
            if (self is MagicOrb && ((MagicOrb)self).owner == o)
                continue;
            if (self is Arrow && o is Player)
                continue;
            if (self is MagicOrb && o is Player)
                continue;

            if (o == self)
                continue;
            worldPos.x = o.x + o.hitBox.x - o.hitBox.width / 2;
            worldPos.y = o.y + o.hitBox.y - o.hitBox.height / 2;
            worldPos.width = o.hitBox.width;
            worldPos.height = o.hitBox.height;
            if (worldPos.CheckIntersect(selfRect))
            {
                if (self is Player && o is PushBlock)
                    ((PushBlock)o).HandlePlayerCollision((Player)self);
                if (self is Player && o is Switch)
                {
                    ((Switch)o).HandlePlayerCollision((Player)self);
                    continue;
                }
                else if (o is Switch)
                    continue;
                if (self is Arrow && o is HitSwitch)
                    ((HitSwitch)o).Activate();
                return worldPos;
            }
        }
        return null;
    }

    public RXRect CheckObjectCollision(FutilePlatformerBaseObject self, float x, float y)
    {
        foreach (FutilePlatformerBaseObject o in collisionObjects)
        {
            if (!o.blocksOtherObjects && !(o is Switch))
                continue;
            if ((self is Knight && o is Player))
                continue;
            if ((self is Ghost && o is Player))
                continue;
            if (self is Arrow && ((Arrow)self).owner == o)
                continue;
            if (self is MagicOrb && ((MagicOrb)self).owner == o)
                continue;
            if (self is Arrow && o is Player)
                continue;
            if (self is MagicOrb && o is Player)
                continue;

            if (o == self)
                continue;
            worldPos.x = o.x + o.hitBox.x - o.hitBox.width / 2;
            worldPos.y = o.y + o.hitBox.y - o.hitBox.height / 2;
            worldPos.width = o.hitBox.width;
            worldPos.height = o.hitBox.height;
            if (worldPos.Contains(x, y))
            {
                if (self is Player && o is PushBlock)
                    ((PushBlock)o).HandlePlayerCollision((Player)self);
                if (self is Player && o is Switch)
                {
                    ((Switch)o).HandlePlayerCollision((Player)self);
                    continue;
                }
                else if (o is Switch)
                    continue;
                if (self is Arrow && o is HitSwitch)
                    ((HitSwitch)o).Activate();
                return worldPos;
            }
        }
        return null;

    }
    public void OpenDoor(string doorName)
    {
        foreach (FutilePlatformerBaseObject o in collisionObjects)
        {
            if (!(o is Door))
                continue;
            Door d = (Door)o;
            if (d.name.ToLower() == doorName.ToLower())
            {
                C.isTransitioning = true;
                FNode tempFollowNode = new FNode();
                tempFollowNode.SetPosition(player.GetPosition());
                C.getCameraInstance().follow(tempFollowNode);
                Go.to(tempFollowNode, 2.0f, new TweenConfig().floatProp("x", d.x).floatProp("y", d.y).setEaseType(EaseType.QuadInOut).onComplete(() =>
                {
                    C.getCameraInstance().shake(.6f, .5f);
                    d.Open();
                    Go.to(tempFollowNode, 1.5f, new TweenConfig().floatProp("x", player.x).floatProp("y", player.y).setEaseType(EaseType.QuadInOut).setDelay(1.3f).onComplete(() =>
                    {
                        C.getCameraInstance().follow(player);
                        C.isTransitioning = false;
                    }));
                }));
            }
        }
    }

    public bool checkForInteractObject(Player p)
    {
        foreach (Villager v in villagers)
            if (v.isColliding(p.swordCollision))
                return true;
        if (p.CurrentDirection == FutileFourDirectionBaseObject.Direction.UP)
            foreach (Sign s in signs)
                if (s.isColliding(p.swordCollision))
                    return true;
        return false;
    }

    public RXRect CheckForJumpObjectCollisionHitbox(Player self, float x, float y)
    {
        selfRect.x = x + self.hitBox.x - self.hitBox.width / 2f;
        selfRect.y = y + self.hitBox.y - self.hitBox.height / 2f;
        selfRect.width = self.hitBox.width;
        selfRect.height = self.hitBox.height;
        foreach (FutilePlatformerBaseObject o in collisionObjects)
        {
            if (!o.blocksJump || !o.blocksOtherObjects)
                continue;
            worldPos.x = o.x + o.hitBox.x - o.hitBox.width / 2;
            worldPos.y = o.y + o.hitBox.y - o.hitBox.height / 2;
            worldPos.width = o.hitBox.width;
            worldPos.height = o.hitBox.height;
            if (worldPos.CheckIntersect(selfRect))
                return worldPos;
        }
        return null;
    }

    public FutilePlatformerBaseObject CheckDamageObjectCollisionGetRect(FutilePlatformerBaseObject self, float x, float y)
    {
        foreach (FutilePlatformerBaseObject o in damageObjects)
        {
            if (o == self)
                continue;
            worldPos.x = o.x + o.hitBox.x - o.hitBox.width / 2;
            worldPos.y = o.y + o.hitBox.y - o.hitBox.height / 2;
            worldPos.width = o.hitBox.width;
            worldPos.height = o.hitBox.height;
            if (worldPos.Contains(x, y))
            {
                return o;
            }
        }

        return null;

    }
    public void CheckDamageObjectCollision()
    {
        for(int i=damageObjects.Count-1; i>=0; i--)
            CheckDamageObjectCollision(damageObjects[i]);
    }
    public void CheckDamageObjectCollision(FutilePlatformerBaseObject o)
    {
        if (o is Knight)
        {
            Knight k = (Knight)o;
            k.CheckDamage(player);
        }
        else if (o is Arrow)
        {
            Arrow a = (Arrow)o;
            a.HandlePlayerCollision(player);
        }
        else if (o is Heart)
        {
            Heart h = (Heart)o;
            h.HandlePlayerCollision(player);
        }
        else if (o is MagicOrb)
        {
            MagicOrb orb = (MagicOrb)o;
            orb.HandlePlayerCollision(player);
        }
        else if (o is Ghost)
        {
            Ghost g = (Ghost)o;
            g.HandlePlayerCollision(player);
        }
        else if (o is SoulPickup)
        {
            SoulPickup s = (SoulPickup)o;
            s.HandlePlayerCollision(player);
        }

    }
    public bool CheckDamageObjectCollision(FutilePlatformerBaseObject self, float x, float y)
    {
        foreach (FutilePlatformerBaseObject o in damageObjects)
        {
            if (o == self)
                continue;
            if (self is PushBlock && o is Arrow)
                continue;
            worldPos.x = o.x + o.hitBox.x - o.hitBox.width / 2;
            worldPos.y = o.y + o.hitBox.y - o.hitBox.height / 2;
            worldPos.width = o.hitBox.width;
            worldPos.height = o.hitBox.height;
            if (worldPos.Contains(x, y))
                return true;
        }
        return false;

    }

    public void Update()
    {
        if (C.isTransitioning)
            return;
        SpawnPoint spawnCollision = null;
        foreach (SpawnPoint p in spawnPoints)
            if (!String.IsNullOrEmpty(p.targetMap))
                if (p.CheckCollision(player))
                {
                    spawnCollision = p;
                    break;
                }
        if (spawnCollision != null)
        {
            Go.to(player, .4f, new TweenConfig().floatProp("x", spawnCollision.pos.x - player.hitBox.x).floatProp("y", spawnCollision.pos.y - player.hitBox.y).setEaseType(EaseType.QuadOut));
            LoadNewMap(spawnCollision.targetMap, spawnCollision.targetSpawn);
            return;
        }
        foreach (Sign s in signs)
            s.CheckCollision(player);
        foreach (Villager v in villagers)
            v.HandlePlayerCollision(player);
        foreach (HitSwitch s in hitSwitches)
            s.HandlePlayerCollision(player);
    }
}

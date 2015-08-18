using System;
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

    string lastMap = "";
    string lastWarpPoint = "";

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
    public void ShowLoading(Action loadAction, bool fromRight = true)
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
                HideLoading(null, fromRight);
            
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
            if (doneAction != null)
                doneAction.Invoke();
            C.isTransitioning = false;
            loadingBG.isVisible = false;
        }));
    }
    public void LoadMap(string mapName)
    {
        lastMap = mapName;
        spawnPoints.Clear();
        signs.Clear();
        collisionObjects.Clear();
        damageObjects.Clear();

        playerLayer.RemoveAllChildren();
        background.RemoveAllChildren();
        foreground.RemoveAllChildren();
        this.map = new FTmxMap();
        this.map.LoadTMX("Maps/" + mapName);


        FLabel mapNameLabel = new FLabel(C.largeFontName, map.mapName);
        C.getCameraInstance().AddChild(mapNameLabel);
        mapNameLabel.SetPosition(new Vector2(Futile.screen.width, Futile.screen.halfHeight * .8f));
        Go.to(mapNameLabel, 1.5f, new TweenConfig().floatProp("x", 0).setEaseType(EaseType.BackOut).onComplete(() =>
        {
            Go.to(mapNameLabel, .7f, new TweenConfig().floatProp("x", -Futile.screen.halfWidth).setDelay(.6f).setEaseType(EaseType.BackIn).onComplete(() => { mapNameLabel.RemoveFromContainer(); }));
        }));

        collisionTilemap = (FTilemap)this.map.getLayerNamed("collision");
        wallCollisionTilemap = (FTilemap)this.map.getLayerNamed("walls");
        backgroundTilemap = (FTilemap)this.map.getLayerNamed("background");
        background.AddChild(backgroundTilemap);
        background.AddChild(collisionTilemap);
        background.AddChild(wallCollisionTilemap);

        if (player == null)
        {
            player = new Player(this);
            C.getCameraInstance().follow(player);
        }
        collisionObjects.Add(player);
        playerLayer.AddChild(player);
        this.y = -16;
        C.getCameraInstance().setWorldBounds(new Rect(0, -collisionTilemap.height-16, collisionTilemap.width, collisionTilemap.height+16));
        collisionTilemap.clipNode = C.getCameraInstance();
        wallCollisionTilemap.clipNode = C.getCameraInstance();
        backgroundTilemap.clipNode = C.getCameraInstance();

        MapLoader.loadObjects(this, map.objects);
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
                lastWarpPoint = spawnName;
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
        if (objectToAdd is PushBlock)
            background.AddChild(objectToAdd);
        else
            playerLayer.AddChild(objectToAdd);
    }

    public void Respawn()
    {
        ShowLoading(() => {
            LoadMap(lastMap);
            SpawnPlayer(lastWarpPoint);
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
        objectToRemove.RemoveFromContainer();
    }

    RXRect worldPos = new RXRect();
    public RXRect CheckObjectCollision(FutilePlatformerBaseObject self, float x, float y)
    {
        foreach (FutilePlatformerBaseObject o in collisionObjects)
        {
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
                return worldPos;
            }
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
        foreach (FutilePlatformerBaseObject o in damageObjects)
            CheckDamageObjectCollision(o);
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
    }
}

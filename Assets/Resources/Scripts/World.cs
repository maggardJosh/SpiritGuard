using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class World : FContainer
{
    FTmxMap map;
    FTilemap collisionTilemap;
    FTilemap backgroundTilemap;
    FSprite loadingBG;
    Player player;

    FContainer background = new FContainer();
    FContainer playerLayer = new FContainer();
    FContainer foreground = new FContainer();

    List<SpawnPoint> spawnPoints = new List<SpawnPoint>();

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

        this.AddChild(background);
        this.AddChild(playerLayer);
        this.AddChild(foreground);
        Futile.instance.SignalUpdate += Update;
        loadingBG = new FSprite("loadingBG");
        loadingBG.isVisible = false;
        loadingBG.x = -Futile.screen.halfWidth - loadingBG.width / 2;
        C.getCameraInstance().AddChild(loadingBG);

    }

    public void ShowLoading(Action loadAction, bool fromRight = true)
    {
        float midPos;
        float midStartPos;
        float finalPos;
        if (fromRight)
        {
            loadingBG.x = Futile.screen.halfWidth + loadingBG.width / 2f;
            loadingBG.rotation = 180f;
            midPos = Futile.screen.halfWidth - loadingBG.width / 2f;
            midStartPos = -Futile.screen.halfWidth + loadingBG.width / 2f;
            finalPos = -Futile.screen.halfWidth - loadingBG.width / 2f;
        }
        else
        {
            loadingBG.x = -Futile.screen.halfWidth - loadingBG.width / 2;
            loadingBG.rotation = 0;
            midPos = -Futile.screen.halfWidth + loadingBG.width / 2;
            midStartPos = Futile.screen.halfWidth - loadingBG.width / 2;
            finalPos = -Futile.screen.halfWidth - loadingBG.width / 2;
        }

        loadingBG.isVisible = true;
        C.isTransitioning = true;
        Go.to(loadingBG, 1.0f, new TweenConfig().floatProp("x", midPos).setDelay(.2f).setEaseType(EaseType.QuadOut).onComplete(() =>
        {
            loadAction.Invoke();
            loadingBG.rotation += 180.0f;
            loadingBG.x = midStartPos;
            Go.to(loadingBG, 1.0f, new TweenConfig().floatProp("x", finalPos).setEaseType(EaseType.QuadIn).onComplete(() =>
            {
                C.isTransitioning = false;
                loadingBG.isVisible = false;
            }));
        }));
    }

    public void LoadMap(string mapName)
    {
        spawnPoints.Clear();
        background.RemoveAllChildren();
        foreground.RemoveAllChildren();

        this.map = new FTmxMap();
        this.map.LoadTMX("Maps/" + mapName);
        collisionTilemap = (FTilemap)this.map.getLayerNamed("collision");
        backgroundTilemap = (FTilemap)this.map.getLayerNamed("background");
        background.AddChild(backgroundTilemap);
        background.AddChild(collisionTilemap);

        if (player == null)
        {

            player = new Player(this);
            playerLayer.AddChild(player);
            C.getCameraInstance().follow(player);
        }
        C.getCameraInstance().setWorldBounds(new Rect(0, -collisionTilemap.height, collisionTilemap.width, collisionTilemap.height));
        collisionTilemap.clipNode = C.getCameraInstance();
        backgroundTilemap.clipNode = C.getCameraInstance();

        MapLoader.loadObjects(this, map.objects);
    }

    public bool isPassable(float x, float y, bool checkBreakableWalls = true)
    {
        bool result = collisionTilemap.IsPassable(x, y);
        int tileX = Mathf.FloorToInt(x / collisionTilemap.tileWidth);
        int tileY = Mathf.FloorToInt(y / collisionTilemap.tileHeight);

        return result;
    }

    public void SpawnPlayer(string spawnName)
    {
        foreach (SpawnPoint s in spawnPoints)
            if (s.name.ToLower() == spawnName.ToLower())
            {
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
            Go.to(player, .4f, new TweenConfig().floatProp("x", spawnCollision.pos.x).floatProp("y", spawnCollision.pos.y).setEaseType(EaseType.QuadOut));
            LoadNewMap(spawnCollision.targetMap, spawnCollision.targetSpawn);
        }
    }
}

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
        loadingBG.isVisible = true;
        Go.to(loadingBG, 1.0f, new TweenConfig().floatProp("x", -Futile.screen.halfWidth + loadingBG.width / 2).setDelay(1.0f).setEaseType(EaseType.QuadOut).onComplete(() =>
        {

            loadingBG.rotation = 180.0f;
            loadingBG.x = Futile.screen.halfWidth - loadingBG.width / 2;
            Go.to(loadingBG, 1.0f, new TweenConfig().floatProp("x", Futile.screen.halfWidth + loadingBG.width / 2).setDelay(.7f).setEaseType(EaseType.QuadIn));
        }));
    }

    public void LoadMap(string mapName)
    {
        background.RemoveAllChildren();
        foreground.RemoveAllChildren();

        this.map = new FTmxMap();
        this.map.LoadTMX("Maps/" + mapName);
        collisionTilemap = (FTilemap)this.map.getLayerNamed("collision");
        backgroundTilemap = (FTilemap)this.map.getLayerNamed("background");
        background.AddChild(backgroundTilemap);
        background.AddChild(collisionTilemap);

        if (player == null)
            player = new Player(this);
        playerLayer.AddChild(player);
        C.getCameraInstance().follow(player);
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

    public void SpawnPlayer(Vector2 spawn)
    {
        player.SetPosition(spawn);
    }

    public void Update()
    {

    }
}

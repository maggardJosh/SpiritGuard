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
        foreground.AddChild(collisionTilemap);

        FutilePlatformerBaseObject player = new Player(this);
        playerLayer.AddChild(player);
        player.SetPosition(16*16,-8*16);
        C.getCameraInstance().follow(player);
        C.getCameraInstance().setWorldBounds(new Rect(0, -collisionTilemap.height, collisionTilemap.width, collisionTilemap.height));
        collisionTilemap.clipNode = C.getCameraInstance();
        backgroundTilemap.clipNode = C.getCameraInstance();
    }

    public bool isPassable(float x, float y, bool checkBreakableWalls = true)
    {
        bool result = collisionTilemap.IsPassable(x, y);
        int tileX = Mathf.FloorToInt(x / collisionTilemap.tileWidth);
        int tileY = Mathf.FloorToInt(y / collisionTilemap.tileHeight);

        return result;
    }

    public void Update()
    {

    }
}

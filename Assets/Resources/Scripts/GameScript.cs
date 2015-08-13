﻿using UnityEngine;
using System.Collections;

public class GameScript : MonoBehaviour
{

    
    // Use this for initialization
    void Start()
    {
        FutileParams futileParams = new FutileParams(true, false, false, false);
        futileParams.AddResolutionLevel(160.0f, 1.0f, 1.0f, "");

        futileParams.origin = new Vector2(0.5f, 0.5f);
        futileParams.backgroundColor = new Color(0, 0, 0);
        futileParams.shouldLerpToNearestResolutionLevel = true;

        Futile.instance.Init(futileParams);

        Futile.atlasManager.LoadAtlas("Atlases/inGameAtlas");

        World world = new World();
        Futile.stage.AddChild(world);
        Futile.stage.AddChild(C.getCameraInstance());
        world.LoadMap("1_1");
        world.SpawnPlayer("spawnpoint");

    }

    // Update is called once per frame
    void Update()
    {

    }
}

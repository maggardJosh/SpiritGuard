using UnityEngine;
using System.Collections;

public class GameScript : MonoBehaviour
{

    
    // Use this for initialization
    void Start()
    {
        FutileParams futileParams = new FutileParams(true, false, false, false);
        futileParams.AddResolutionLevel(160.0f, 1.0f, 1.0f, "");

        futileParams.origin = new Vector2(0.5f, 0.5f);
        futileParams.backgroundColor = new Color(245.0f / 255.0f, 252.0f / 255.0f, 178.0f / 255.0f);
        futileParams.shouldLerpToNearestResolutionLevel = true;

        Futile.instance.Init(futileParams);

        Futile.atlasManager.LoadAtlas("Atlases/inGameAtlas");
        Futile.atlasManager.LoadFont(C.smallFontName, "smallFont_0", "Atlases/smallFont", 0, 0);
        Futile.atlasManager.LoadFont(C.largeFontName, "largeFont_0", "Atlases/largeFont", 0, 0);


        World world = new World();
        Futile.stage.AddChild(world);
        Futile.stage.AddChild(C.getCameraInstance());
        world.ShowLoading(() => { world.LoadMap("1_1"); world.SpawnPlayer("spawnpoint"); });
        FLabel test = new FLabel(C.smallFontName, C.versionNumber);
        Go.to(test, 5, new TweenConfig().floatProp("y", -Futile.screen.halfHeight + test.textRect.height/2f).setEaseType(EaseType.BounceOut));
        C.getCameraInstance().AddChild(test);
        C.getCameraInstance().SetPosition(Futile.screen.halfWidth, -Futile.screen.halfHeight);

    }

    // Update is called once per frame
    void Update()
    {

    }
}

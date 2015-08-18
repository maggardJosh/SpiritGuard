using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TitleScreen : FContainer
{
    FSprite title;
    FSprite english;
    FSprite play;
    FSprite selected;
    FSprite credits;
    FSprite white;

    public TitleScreen()
    {
        FSoundManager.PlayMusic("SpiritGuardMenu");
        title = new FSprite("title_kanji");
        english = new FSprite("title_english");
        play = new FSprite("pressLToStart");
        credits = new FSprite("credits");
        white = new FSprite("white");
        white.width = Futile.screen.width;
        white.height = Futile.screen.height;
        this.AddChild(white);
        this.AddChild(title);
        this.AddChild(english);
        this.AddChild(play);
        this.AddChild(credits);

        FLabel test = new FLabel(C.smallFontName, C.versionNumber);
        test.y = -Futile.screen.halfHeight - 30;
        test.x -= 1;
        
        Go.to(test, 3, new TweenConfig().floatProp("y", -Futile.screen.halfHeight + test.textRect.height/2f + 13).setDelay(4.0f).setEaseType(EaseType.BackOut));
        this.AddChild(test);

    }

    public float volume { get { return FSoundManager.volume; } set { FSoundManager.volume = value; } }
    public override void HandleAddedToStage()
    {
        
        title.y = Futile.screen.halfHeight + title.height / 2f;
        english.x = Futile.screen.halfWidth + english.width / 2f;
        play.x = Futile.screen.halfWidth + play.width / 2f;
        credits.y = -Futile.screen.halfHeight - credits.height / 2f - 30f;
        Go.to(credits, 2.0f, new TweenConfig().floatProp("y", -Futile.screen.halfHeight + credits.height + 1).setEaseType(EaseType.BackOut).setDelay(3.0f));
        Go.to(title, 1.5f, new TweenConfig().floatProp("y", Futile.screen.halfHeight / 2f).setEaseType(EaseType.BounceOut).onComplete(() =>
        {
            english.y = title.y - title.height / 2f - english.height / 2f - 4;
            play.y = english.y - english.height / 2f - play.height / 2f - 20;
            Go.to(english, 2.0f, new TweenConfig().floatProp("x", 0).setEaseType(EaseType.BackOut).onComplete(() =>
            {
                Go.to(title, 2.0f, new TweenConfig().floatProp("y", 2, true).setEaseType(EaseType.BackIn).setIterations(-1, LoopType.PingPong));
                
                //     Go.to(english, 2.0f, new TweenConfig().floatProp("y", 4, true).setEaseType(EaseType.QuadIn).setIterations(-1, LoopType.PingPong));

            }));
            Go.to(play, 2.0f, new TweenConfig().floatProp("x", 0).setDelay(.5f).setEaseType(EaseType.BackOut).onComplete(() => {
                isTransIn = true;
                //Go.to(play , 1.0f, new TweenConfig().floatProp("y", 2, true).setEaseType(EaseType.QuadIn).setIterations(-1, LoopType.PingPong));
            }));
        }));
                Futile.instance.SignalUpdate += Update;
        base.HandleAddedToStage();
    }
    float count = 0;
    bool isTransIn = false;
    public void Update()
    {
        if(isTransIn)
        count += Time.deltaTime;
        if (C.getKeyDown(C.JUMP_KEY))
        {
            World world = new World();

            world.ShowLoading(() =>
            {
                C.getCameraInstance().AddChild(world.ui);
                world.loadingBG.MoveToFront();
                Futile.stage.AddChild(world);

                C.getCameraInstance().MoveToFront();
                world.LoadMap("1_1"); world.SpawnPlayer("spawnpoint");
                
                this.RemoveFromContainer();
            });
            Futile.instance.SignalUpdate -= Update;
        }
        this.play.isVisible = (count * 1500 % 2000 < 1500 );
    }
}


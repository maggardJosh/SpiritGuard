using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class TitleScreen : FContainer
{
    FSprite title;
    FSprite english;
    FSprite play;
    FSprite exit;
    FSprite selected;
    FSprite credits;
    FSprite white;

    public TitleScreen()
    {
        title = new FSprite("title_kanji");
        english = new FSprite("title_english");
        play = new FSprite("button_start");
        exit = new FSprite("button_exit");
        credits = new FSprite("credits");
        white = new FSprite("white");
        white.width = Futile.screen.width;
        white.height = Futile.screen.height;
        this.AddChild(white);
        this.AddChild(title);
        this.AddChild(english);
        this.AddChild(play);
        this.AddChild(exit);
        this.AddChild(credits);
    }

    public override void HandleAddedToStage()
    {
        title.y = Futile.screen.halfHeight + title.height / 2f;
        english.x = Futile.screen.halfWidth + english.width / 2f;
        play.x = Futile.screen.halfWidth + play.width / 2f;
        exit.x = Futile.screen.halfWidth + exit.width / 2f;
        credits.y = -Futile.screen.halfHeight - credits.height / 2f - 30f;
        Go.to(credits, 2.0f, new TweenConfig().floatProp("y", -Futile.screen.halfHeight + credits.height + 1).setEaseType(EaseType.BackOut).setDelay(3.0f));
        Go.to(title, 1.5f, new TweenConfig().floatProp("y", Futile.screen.halfHeight / 2f).setEaseType(EaseType.BounceOut).onComplete(() =>
        {
            english.y = title.y - title.height / 2f - english.height / 2f - 4;
            play.y = english.y - english.height / 2f - play.height / 2f - 15;
            exit.y = play.y - play.height / 2f - exit.height / 2f - 8f;
            Go.to(english, 2.0f, new TweenConfig().floatProp("x", 0).setEaseType(EaseType.BackOut).onComplete(() =>
            {
                Go.to(title, 2.0f, new TweenConfig().floatProp("y", 2, true).setEaseType(EaseType.BackIn).setIterations(-1, LoopType.PingPong));
                //     Go.to(english, 2.0f, new TweenConfig().floatProp("y", 4, true).setEaseType(EaseType.QuadIn).setIterations(-1, LoopType.PingPong));
                Futile.instance.SignalUpdate += Update;

            }));
            Go.to(play, 2.0f, new TweenConfig().floatProp("x", 0).setDelay(.5f).setEaseType(EaseType.BackOut));
            Go.to(exit, 2.0f, new TweenConfig().floatProp("x", 0).setDelay(.7f).setEaseType(EaseType.BackOut));
        }));
        base.HandleAddedToStage();
    }
    public void Update()
    {
        if (C.getKeyDown(C.JUMP_KEY))
        {

            World world = new World();
            world.ShowLoading(() =>
            {

                Futile.stage.AddChild(world);
                C.getCameraInstance().MoveToFront();
                world.LoadMap("1_1"); world.SpawnPlayer("spawnpoint");
                FLabel test = new FLabel(C.smallFontName, C.versionNumber);
                test.y = -Futile.screen.halfHeight - 30;
                Go.to(test, 5, new TweenConfig().floatProp("y", -Futile.screen.halfHeight + test.textRect.height / 2f).setEaseType(EaseType.BackOut));
                C.getCameraInstance().AddChild(test);
                this.RemoveFromContainer();
            });
            Futile.instance.SignalUpdate -= Update;
        }
    }
}


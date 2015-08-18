using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Door : FutilePlatformerBaseObject
{
    private FAnimatedSprite sprite;
    public enum State
    {
        CLOSED,
        OPENING,
    }
    public string name;
    public State currentState = State.CLOSED;
    public Door(string name, World world, float x, float y): base (new RXRect(0,0,16,16),world)
    {
        this.name = name;
        this.SetPosition(x, y);
        this.blocksJump = true;
        sprite = new FAnimatedSprite("Door/door");
        sprite.addAnimation(new FAnimation(State.CLOSED.ToString(), new int[] { 1 }, 150, false));
        sprite.ignoreTransitioning = true;
        sprite.addAnimation(new FAnimation(State.OPENING.ToString(), new int[] { 2,3,4,5,6,7 }, 150, false));

        this.AddChild(sprite);
        PlayAnim();
    }

    public void Open()
    {
        FSoundManager.PlaySound("door");
        currentState = State.OPENING;
        blocksOtherObjects = false;
        PlayAnim();
    }

    public override void OnFixedUpdate()
    {
        
        base.OnFixedUpdate();
    }

    private void PlayAnim()
    {
        sprite.play(currentState.ToString());
    }
}


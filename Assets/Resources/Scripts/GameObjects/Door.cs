using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Door : FutileFourDirectionBaseObject
{
    private FAnimatedSprite sprite;
    public enum State
    {
        CLOSED,
        OPENING,
        OPEN,
        CLOSING
    }
    public string name;
    public State currentState = State.CLOSED;
    public Door(string name, Direction direction, World world, float x, float y): base (new RXRect(0,0,16,16),world)
    {
        this.name = name;
        this.SetPosition(x, y);
        this.blocksJump = true;
        sprite = new FAnimatedSprite("Door/door");
        sprite.addAnimation(new FAnimation(Direction.DOWN.ToString() + State.CLOSED.ToString(), new int[] { 1 }, 150, false));
        sprite.addAnimation(new FAnimation(Direction.UP.ToString() + State.CLOSED.ToString(), new int[] { 1 }, 150, false));
        sprite.addAnimation(new FAnimation(Direction.RIGHT.ToString() + State.CLOSED.ToString(), new int[] { 1 }, 150, false));
        sprite.addAnimation(new FAnimation(Direction.LEFT.ToString() + State.CLOSED.ToString(), new int[] { 1 }, 150, false));

        sprite.addAnimation(new FAnimation(Direction.DOWN.ToString() + State.OPENING.ToString(), new int[] { 2 }, 150, false));
        sprite.addAnimation(new FAnimation(Direction.UP.ToString() + State.OPENING.ToString(), new int[] { 2 }, 150, false));
        sprite.addAnimation(new FAnimation(Direction.RIGHT.ToString() + State.OPENING.ToString(), new int[] { 2 }, 150, false));
        sprite.addAnimation(new FAnimation(Direction.LEFT.ToString() + State.OPENING.ToString(), new int[] { 2 }, 150, false));

        sprite.addAnimation(new FAnimation(Direction.DOWN.ToString() + State.OPEN.ToString(), new int[] { 1 }, 150, false));
        sprite.addAnimation(new FAnimation(Direction.UP.ToString() + State.OPEN.ToString(), new int[] { 1 }, 150, false));
        sprite.addAnimation(new FAnimation(Direction.RIGHT.ToString() + State.OPEN.ToString(), new int[] { 1 }, 150, false));
        sprite.addAnimation(new FAnimation(Direction.LEFT.ToString() + State.OPEN.ToString(), new int[] { 1 }, 150, false));

        sprite.addAnimation(new FAnimation(Direction.DOWN.ToString() + State.CLOSING.ToString(), new int[] { 1 }, 150, false));
        sprite.addAnimation(new FAnimation(Direction.UP.ToString() + State.CLOSING.ToString(), new int[] { 1 }, 150, false));
        sprite.addAnimation(new FAnimation(Direction.RIGHT.ToString() + State.CLOSING.ToString(), new int[] { 1 }, 150, false));
        sprite.addAnimation(new FAnimation(Direction.LEFT.ToString() + State.CLOSING.ToString(), new int[] { 1 }, 150, false));

        this.AddChild(sprite);
        this._direction = direction;
        PlayAnim();
    }

    public void Open()
    {
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
        sprite.play(CurrentDirection.ToString() + currentState.ToString());
    }
}


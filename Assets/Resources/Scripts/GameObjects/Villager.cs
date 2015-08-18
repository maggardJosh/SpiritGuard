using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Villager : FutileFourDirectionBaseObject
{
    private FAnimatedSprite sprite;
    private float moveSpeed = .5f;
    private VillagerState _state = VillagerState.IDLE;
    private InteractInd indicator;
    bool walks = true;
    bool isInteracting = false;
    List<string> dialogue;
    public VillagerState State
    {
        get
        {
            return _state;
        }
        set
        {
            if (_state != value)
            {
                _state = value;
                stateCount = 0;
            }
        }
    }
    public enum VillagerState
    {
        IDLE,
        MOVING
    }
    public Villager(List<string> dialogue, World world, string villagerType = "A")
        : base(new RXRect(0, -6, 10, 8), world)
    {
        this.dialogue = dialogue;
        maxXVel = .5f;
        maxYVel = .5f;
        minYVel = -.5f;
        handleStateCount = true;
        bounceiness = 0f;
        clearAcc = false;

        sprite = new FAnimatedSprite("villager" + villagerType);
        if (villagerType == "A")
        {

            sprite.addAnimation(new FAnimation(VillagerState.MOVING.ToString() + Direction.DOWN, new int[] { 1, 2, 3, 4 }, 150, true));
            sprite.addAnimation(new FAnimation(VillagerState.MOVING.ToString() + Direction.UP, new int[] { 5, 6, 7, 8 }, 150, true));
            sprite.addAnimation(new FAnimation(VillagerState.MOVING.ToString() + Direction.LEFT, new int[] { 9, 10, 11, 12 }, 150, true));
            sprite.addAnimation(new FAnimation(VillagerState.MOVING.ToString() + Direction.RIGHT, new int[] { 9, 10, 11, 12 }, 150, true));

            sprite.addAnimation(new FAnimation(VillagerState.IDLE.ToString() + Direction.DOWN, new int[] { 1 }, 150, true));
            sprite.addAnimation(new FAnimation(VillagerState.IDLE.ToString() + Direction.UP, new int[] { 5 }, 150, true));
            sprite.addAnimation(new FAnimation(VillagerState.IDLE.ToString() + Direction.LEFT, new int[] { 9 }, 150, true));
            sprite.addAnimation(new FAnimation(VillagerState.IDLE.ToString() + Direction.RIGHT, new int[] { 9 }, 150, true));
        }
        else
        {
            walks = false;
            sprite.addAnimation(new FAnimation(VillagerState.MOVING.ToString() + Direction.DOWN, new int[] { 1 }, 150, true));
            sprite.addAnimation(new FAnimation(VillagerState.IDLE.ToString() + Direction.DOWN, new int[] { 1 }, 150, true));
        }

        PlayAnim();
        this.AddChild(sprite);
        indicator = new InteractInd(0, 5);
        this.AddChild(indicator);
    }

    float minState = .8f;
    public override void OnFixedUpdate()
    {
        if (walks && !isInteracting && stateCount > minState)
        {
            if (RXRandom.Float() < .3f)
            {
                switch (State)
                {
                    case VillagerState.IDLE:
                        if (RXRandom.Float() < .5f)
                        {
                            //left or right
                            if (RXRandom.Float() < .5f)
                                xAcc = moveSpeed;
                            else
                                xAcc = -moveSpeed;
                        }
                        else
                        {
                            //up or down
                            if (RXRandom.Float() < .5f)
                                yAcc = moveSpeed;
                            else
                                yAcc = -moveSpeed;
                        }
                        State = VillagerState.MOVING;
                        break;
                    case VillagerState.MOVING:
                        if (RXRandom.Float() < .3f)
                        {
                            //stop
                            State = VillagerState.IDLE;
                            PlayAnim(true);
                            xAcc = 0;
                            yAcc = 0;

                        }
                        else
                        {
                            //Just turn
                            if (RXRandom.Float() < .5f)
                            {
                                if (RXRandom.Float() < .5f)
                                    xAcc = moveSpeed;
                                else
                                    xAcc = -moveSpeed;
                                yAcc = 0;
                            }
                            else
                            {
                                if (RXRandom.Float() < .5f)
                                    yAcc = moveSpeed;
                                else
                                    yAcc = -moveSpeed;
                                xAcc = 0;
                            }
                            stateCount = 0;
                        }
                        break;
                }
            }
        }
        base.OnFixedUpdate();
        this.indicator.scaleX = scaleX;
        if (xAcc == 0)
            xVel *= .8f;
        if (yAcc == 0)
            yVel *= .8f;
        PlayAnim();
    }

    public void HandlePlayerCollision(Player p)
    {
        if (!isInteracting)
        {

            if (this.isColliding(p.swordCollision))
            {
                this._direction = (Direction)((int)(p.CurrentDirection + 2) % Enum.GetValues(typeof(Direction)).Length);
                this.State = VillagerState.IDLE;
                RXDebug.Log(this._direction);
                PlayAnim(true);
                xAcc = 0;
                yAcc = 0;
                xVel = 0;
                yVel = 0;
                scaleX = _direction == Direction.RIGHT ? -1 : 1;
                isInteracting = true;
                p.hasInteractObject = true;
                this.indicator.Show();
            }
        }
        else
        {
            if (C.getKey(C.JUMP_KEY))
            {
                world.ui.dialogue.ShowMessage(new List<string>(dialogue));
            }
            if (!this.isColliding(p.swordCollision))
            {
                stateCount = 0;
                this.indicator.Hide();
                isInteracting = false;
            }
            else
            {
                p.hasInteractObject = true;
            }
        }
    }

    public void PlayAnim(bool forced = false)
    {
        sprite.play(State.ToString() + _direction.ToString(), forced);
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Dialogue : FContainer
{
    FSprite dialogueBG;
    FLabel message;
    private enum State
    {
        TRANS_IN,
        TYPEWRITER,
        WAITING_ON_KEY_PRESS,
        TRANS_OUT
    }
    State currentState = State.TRANS_IN;
    List<string> messages;
    InteractInd indicator;
    public Dialogue()
    {
        dialogueBG = new FSprite("dialogueBG");
        dialogueBG.isVisible = false;
        this.y = -Futile.screen.halfHeight - dialogueBG.height / 2;
        this.AddChild(dialogueBG);

        indicator = new InteractInd(Futile.screen.halfWidth - 12, -7);
        indicator.ignoreTransitioning = true;
        this.AddChild(indicator);

        message = new FLabel(C.smallFontName, "");
        message.alignment = FLabelAlignment.Center;
        this.AddChild(message);
        message.isVisible = false;
    }

    const float TWEEN_IN_TIME = .5f;
    const float TWEEN_OUT_TIME = .6f;
    public void ShowMessage(List<string> messages)
    {
        if (this.dialogueBG.isVisible)
            return;
        stateCount = 0;
        this.dialogueBG.isVisible = true;
        this.messages = messages;
        C.isTransitioning = true;
        Go.to(this, TWEEN_IN_TIME, new TweenConfig().floatProp("y", -Futile.screen.halfHeight + dialogueBG.height / 2f).setEaseType(EaseType.BackOut).onComplete(() =>
        {
            indicator.Show();
            stateCount = 0;
            currentState = State.TYPEWRITER;
            message.isVisible = true;
            message.text = "";
        }));
        currentState = State.TRANS_IN;
        Futile.instance.SignalUpdate += Update;
    }

    float stateCount = 0;
    float typewriterTime = 1.4f;
    public void Update()
    {
        stateCount += Time.deltaTime;
        switch (currentState)
        {
            case State.TRANS_IN:
                break;
            case State.TYPEWRITER:
                int textLength = Mathf.FloorToInt((stateCount / typewriterTime) * (messages[0].Length));
                if (stateCount > .3f && C.getKeyDown(C.JUMP_KEY))
                {
                    textLength = messages[0].Length + 1;
                    stateCount = 0;
                }
                if (textLength > messages[0].Length)
                {
                    textLength = messages[0].Length;
                    currentState = State.WAITING_ON_KEY_PRESS;
                }
                message.text = messages[0].Substring(0, textLength);
                break;
            case State.WAITING_ON_KEY_PRESS:
                if (stateCount > .3f && C.getKeyDown(C.JUMP_KEY))
                {
                    if (messages.Count > 1)
                    {
                        messages.RemoveAt(0);
                        message.text = "";
                        currentState = State.TYPEWRITER;
                        stateCount = 0;
                    }
                    else
                    {
                        message.isVisible = false;
                        currentState = State.TRANS_OUT;
                        Futile.instance.SignalUpdate -= Update;
                        indicator.Hide();
                        Go.to(this, TWEEN_OUT_TIME, new TweenConfig().floatProp("y", -Futile.screen.halfHeight - dialogueBG.height / 2f).setEaseType(EaseType.BackIn).onComplete(() => { dialogueBG.isVisible = false; currentState = State.TRANS_IN; C.isTransitioning = false; }));
                    }
                }
                break;
            case State.TRANS_OUT:
                break;
        }
    }
}

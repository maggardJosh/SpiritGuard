﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class C
{
    public const string versionNumber = "v1.1.1";

    private static FCamObject camera;
    public static FCamObject getCameraInstance()
    {
        if (camera == null)
        {
            camera = new FCamObject();
            camera.shakeHUD = false;
        }
        return camera;
    }

    public static bool isTransitioning = false;
    public static bool isDebug = false;

    public const string smallFontName = "smallFont";
    public const string largeFontName = "largeFont";

    public static readonly KeyCode[] LEFT_KEY = new KeyCode[] { KeyCode.LeftArrow, KeyCode.A, KeyCode.Q };
    public static readonly KeyCode[] RIGHT_KEY = new KeyCode[] { KeyCode.RightArrow, KeyCode.D };
    public static readonly KeyCode[] UP_KEY = new KeyCode[] { KeyCode.UpArrow, KeyCode.W, KeyCode.Z };
    public static readonly KeyCode[] DOWN_KEY = new KeyCode[] { KeyCode.DownArrow, KeyCode.S };
    public static readonly KeyCode[] JUMP_KEY = new KeyCode[] { KeyCode.L, KeyCode.Space };
    public static readonly KeyCode[] ACTION_KEY = new KeyCode[] { KeyCode.K };
    public static readonly KeyCode[] SELECT_KEY = new KeyCode[] { KeyCode.Tab };

    private static float lastVerticalValue = 0;
    public static bool getUpPress()
    {
        bool upPressed = lastVerticalValue >= 0 && Input.GetAxis("Vertical") < 0;
        lastVerticalValue = Input.GetAxis("Vertical");
        return upPressed || Input.GetButtonDown("Action");
    }
    public static bool getJumpPress() { return Input.GetButtonDown("Jump"); }

    public static bool getKey(KeyCode[] keys)
    {
        foreach (KeyCode key in keys)
            if (Input.GetKey(key))
                return true;
        return false;
    }

    public static bool getKeyDown(KeyCode[] keys)
    {
        foreach (KeyCode key in keys)
            if (Input.GetKeyDown(key))
                return true;
        return false;
    }

    internal static bool getStartPressed()
    {
        return Input.GetButtonDown("Start");
    }
    private static Player.SaveState _lastSave;
    public static Player.SaveState lastSave
    {
        get { if (_lastSave == null) _lastSave = new Player.SaveState(); return _lastSave; }
    }
    private static Player.SaveState _currentSave;
    public static Player.SaveState Save
    {
        get { if (_currentSave == null) _currentSave = new Player.SaveState(); return _currentSave; }
    }
}

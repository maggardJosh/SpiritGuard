using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
public class MapLoader
{

    public static void loadObjects(World world, List<XMLNode> objects)
    {
        foreach (XMLNode node in objects)
        {
            if (!node.attributes.ContainsKey("type"))
                continue;
            switch (node.attributes["type"].ToLower())
            {
                case "spawnpoint":
                    world.addSpawn(parseSpawnPoint(node));
                    break;
            }
        }
    }


    private static SpawnPoint parseSpawnPoint(XMLNode node)
    {
        SpawnPoint result;
        string name = "";
        string targetMap = "";
        string targetSpawn = "";
        FutileFourDirectionBaseObject.Direction exitDirection = FutileFourDirectionBaseObject.Direction.DOWN;
        foreach (XMLNode property in ((XMLNode)node.children[0]).children)
        {
            switch (property.attributes["name"].ToLower())
            {
                case "name":
                    name = property.attributes["value"].ToLower();
                    break;
                case "targetmap":
                    targetMap = property.attributes["value"].ToLower();
                    break;
                case "targetspawn":
                    targetSpawn = property.attributes["value"].ToLower();
                    break;
                case "exitdirection":
                    switch (property.attributes["value"].ToLower())
                    {
                        case "up":
                            exitDirection = FutileFourDirectionBaseObject.Direction.UP;
                            break;
                        case "right":
                            exitDirection = FutileFourDirectionBaseObject.Direction.RIGHT;
                            break;
                        case "down":
                            exitDirection = FutileFourDirectionBaseObject.Direction.DOWN;
                            break;
                        case "left":
                            exitDirection = FutileFourDirectionBaseObject.Direction.LEFT;
                            break;
                    }
                    break;
            }
        }

        result = new SpawnPoint((float.Parse(node.attributes["x"]) + 8f), -(float.Parse(node.attributes["y"])-  8f), name, targetMap, targetSpawn, exitDirection);
        return result;
    }

    /*
     *  private static Vector2 parseSpawnPoint(XMLNode node)
    {
        Vector2 result;
        if (node.children[0] != null)
        {

            foreach (XMLNode property in ((XMLNode)node.children[0]).children)
            {
                switch (property.attributes["name"].ToLower())
                {
                    case "easetype":
                        switch (property.attributes["value"].ToLower())
                        {
                            case "quad": easeType = EaseType.QuadInOut; break;
                            case "linear": easeType = EaseType.Linear; break;
                            case "circ": easeType = EaseType.CircInOut; break;
                        }
                        break;
                    case "color":
                        string[] values = property.attributes["value"].Split(',');
                        if (values.Length != 3)
                        {
                            RXDebug.Log("Incorrect value for color on moving light: " + property.attributes["value"]);
                            continue;
                        }
                        color = new Color(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]));
                        break;
                    case "direction":
                        switch (property.attributes["value"].ToLower())
                        {
                            case "u": direction = FIsoSprite.Direction.UP; break;
                            case "r": direction = FIsoSprite.Direction.RIGHT; break;
                            case "d": direction = FIsoSprite.Direction.DOWN; break;
                            case "l": direction = FIsoSprite.Direction.LEFT; break;
                        }
                        break;
                    case "distance":
                        distance = int.Parse(property.attributes["value"]);
                        break;
                    case "flicker":
                        flicker = bool.Parse(property.attributes["value"]);
                        break;
                    case "independent":
                        independentMovement = bool.Parse(property.attributes["value"]);
                        break;
                    case "intensitymax":
                        intensityMax = float.Parse(property.attributes["value"]);
                        break;
                    case "intensitymin":
                        intensityMin = float.Parse(property.attributes["value"]);
                        break;
                    case "lighttype":
                        lightType = property.attributes["value"];
                        break;
                    case "lightdistance":
                        lightDistance = int.Parse(property.attributes["value"]);
                        break;
                    case "time":
                        time = float.Parse(property.attributes["value"]);
                        break;
                    case "startcount":
                        count = float.Parse(property.attributes["value"]);
                        break;
                }
            }
            int startCartX = (int)(float.Parse(node.attributes["x"]) / 32.0f - 1);
            int startCartY = (int)(float.Parse(node.attributes["y"]) / 32.0f - 1);
            int endCartX = startCartX;
            int endCartY = startCartY;
            switch (direction)
            {
                case FIsoSprite.Direction.UP: endCartY -= distance; break;
                case FIsoSprite.Direction.RIGHT: endCartX += distance; break;
                case FIsoSprite.Direction.DOWN: endCartY += distance; break;
                case FIsoSprite.Direction.LEFT: endCartX -= distance; break;
            }
            result = new MovingLight(tilemap, startCartX, startCartY, endCartX, endCartY, time, independentMovement, flicker, intensityMin, intensityMax, lightType, color, lightDistance, easeType, count);
        }
        return result;
    } */

}

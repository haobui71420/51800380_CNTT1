using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{   
    public int colorCode = -1;
    public int typeCode = -1;
    public int x, y;
    public int gCost, hCost, fCost;
    public Tile previousTile;
    public Tile(int x, int y)
    {
        this.x = x;
        this.y = y;
	}
    
    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }
}

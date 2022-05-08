using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    public GameManager manager;
    public Tile[,] grid;
    private List<Tile> openList;
    private List<Tile> closedList;
	public PathFinding()
    {
        grid = new Tile[9, 9];
        for (int x = 0; x < 9; x++)
        {
            for (int y = 0; y < 9; y++)
            {
                grid[x, y] = new Tile(x, y);
            }
        }
    }
	public List<Tile> FindPath(int startX, int startY, int endX, int endY)
    {
        Tile startTile = grid[startX, startY];
        Tile endTile = grid[endX, endY];

        openList = new List<Tile> { startTile };
        closedList = new List<Tile>();

        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                Tile tile = grid[i, j];
                tile.gCost = int.MaxValue;
                tile.CalculateFCost();
                tile.previousTile = null;
            }
        }

        startTile.gCost = 0;
        startTile.hCost = CalculateDistance(startTile, endTile);
        startTile.CalculateFCost();

        while(openList.Count > 0)
        {
            Tile currentTile = GetLowestFCostTile(openList);
            if(currentTile == endTile)
            {
                return CalculatePath(endTile);
			}
            openList.Remove(currentTile);
            closedList.Add(currentTile);

            foreach(Tile neighborTile in GetNeighborTile(currentTile, startTile))
            {
                if (closedList.Contains(neighborTile)) continue;
                int tentativeCost = currentTile.gCost + CalculateDistance(currentTile, neighborTile);
                if(tentativeCost < neighborTile.gCost)
                {
                    neighborTile.previousTile = currentTile;
                    neighborTile.gCost = tentativeCost;
                    neighborTile.hCost = CalculateDistance(neighborTile, endTile);
                    neighborTile.CalculateFCost();

                    if(!openList.Contains(neighborTile))
                    {
                        openList.Add(neighborTile);
					}
				}

            }
		}

        return null;
    }

    private List<Tile> GetNeighborTile(Tile currentTile, Tile startTile)
    {
        List<Tile> neighborList = new List<Tile>();
        if(currentTile.x - 1 >= 0 && (grid[currentTile.x - 1, currentTile.y].colorCode == -1 || startTile.typeCode == 0))
        {
            neighborList.Add(grid[currentTile.x - 1, currentTile.y]);
		}
        if(currentTile.x + 1 < 9 && (grid[currentTile.x + 1, currentTile.y].colorCode == -1 || startTile.typeCode == 0))
        {
            neighborList.Add(grid[currentTile.x + 1, currentTile.y]);
		}
        if (currentTile.y - 1 >= 0 && (grid[currentTile.x, currentTile.y - 1].colorCode == -1 || startTile.typeCode == 0))
        {
            neighborList.Add(grid[currentTile.x, currentTile.y - 1]);
        }
        if (currentTile.y + 1 < 9 && (grid[currentTile.x, currentTile.y + 1].colorCode == -1 || startTile.typeCode == 0))
        {
            neighborList.Add(grid[currentTile.x, currentTile.y + 1]);
        }
        return neighborList;
    }

    private int CalculateDistance(Tile a, Tile b)
    {
        return (int)Vector2.Distance(new Vector2(a.x, a.y), new Vector2(b.x, b.y));

    }

	private List<Tile> CalculatePath(Tile endTile)
	{
        List<Tile> path = new List<Tile>();
        path.Add(endTile);
        Tile currentTile = endTile;
        while(currentTile.previousTile != null)
        {
            path.Add(currentTile.previousTile);
            currentTile = currentTile.previousTile;
		}
        path.Reverse();

        return path;
    }

	private Tile GetLowestFCostTile(List<Tile> tileList)
    {
        Tile lowestfCostTile = tileList[0];
        for (int i = 1; i < tileList.Count; i++)
        {
            if (tileList[i].fCost < lowestfCostTile.fCost)
            {
                lowestfCostTile = tileList[0];
            }
        }

        return lowestfCostTile;
    }

}

    

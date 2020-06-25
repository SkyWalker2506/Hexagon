using System.Collections.Generic;
using UnityEngine;

public class BoardHelper
{

  static Vector4[] singleSingleIndexList = new Vector4[]
  {
    new Vector4(0, 2, 1, 1), new Vector4(1, 1, 1, -1),
    new Vector4(1, -1, 0, -2), new Vector4(0, -2, 0, -1),
    new Vector4(0, -1, 0, 1), new Vector4(0, 1, 0, 2)
  };

  static Vector4[] singleDoubleIndexList = new Vector4[]
  { new Vector4(0, 1, 0, -1), new Vector4(0, -1, 0, -2),
    new Vector4(0, -2, -1, -1), new Vector4(-1, -1, -1, 1),
    new Vector4(-1, 1, 0, 2), new Vector4(0, 2, 0, 1) };

  static Vector4[] doubleSingleIndexList = new Vector4[]
  { new Vector4(0, 2, 1, 1), new Vector4(1, 1, 1, -1),
    new Vector4(1, -1, 0, -2), new Vector4(0, -2, 0, -1),
    new Vector4(0, -1, 0, 1), new Vector4(0, 1, 0, 2) };

  static Vector4[] doubleDoubleIndexList = new Vector4[]
  { new Vector4(0, 1, 0, -1), new Vector4(0, -1, 0, -2),
    new Vector4(0, -2, -1, -1), new Vector4(-1, -1, -1, 1),
    new Vector4(-1, 1, 0, 2), new Vector4(0, 2, 0, 1) };

  public static List<Vector4> ReturnThreeMatches(HexCell[][] boardCells, Vector2 index)
  {
    List<Vector4> result = new List<Vector4>();
    var boardSize = new Vector2(boardCells.Length, boardCells[0].Length);
    var relativeNeighbours = GetNeighbourArray(boardSize, index);

    for (int i = 0; i < relativeNeighbours.Length; i++)
    {
      var mainColor = boardCells[(int)index.x][(int)index.y].HexObject.HexColor;
      var neighbourColor1 = boardCells[(int)relativeNeighbours[i].x][(int)relativeNeighbours[i].y].HexObject.HexColor;
      var neighbourColor2 = boardCells[(int)relativeNeighbours[i].z][(int)relativeNeighbours[i].w].HexObject.HexColor;

      if (mainColor == neighbourColor1 && mainColor == neighbourColor2)
      {
        result.Add(relativeNeighbours[i]);
      }
    }

    return result;
  }


  public static Vector4[] GetNeighbourArray(Vector2 size, Vector2 index)
  {
    List<Vector4> results = new List<Vector4>();
    Vector4[] neighbours;

    if ((index.x % 2 == 1) && (index.y % 2 == 1))
      neighbours = singleSingleIndexList;
    else if ((index.x % 2 == 1) && (index.y % 2 == 0))
      neighbours = singleDoubleIndexList;
    else if ((index.x % 2 == 0) && (index.y % 2 == 1))
      neighbours = doubleSingleIndexList;
    else
      neighbours = doubleDoubleIndexList;


    for (int i = 0; i < neighbours.Length; i++)
    {
      if (
          (index.x + neighbours[i].x) >= 0 &&
          (index.x + neighbours[i].z) >= 0 &&
          (index.y + neighbours[i].y) >= 0 &&
          (index.y + neighbours[i].w) >= 0 &&
          (index.x + neighbours[i].x) < size.x &&
          (index.x + neighbours[i].z) < size.x &&
          (index.y + neighbours[i].y) < size.y &&
          (index.y + neighbours[i].w) < size.y
         )
      {
        results.Add(new Vector4(index.x + neighbours[i].x, index.y + neighbours[i].y, index.x + neighbours[i].z, index.y + neighbours[i].w));
      }
    }

    return results.ToArray();
  }


  public static List<HexCell> GetNearMatchedCells(HexCell[][] boardCells, Vector2 index)
  {
    List<HexCell> result = new List<HexCell>();
    var matches = BoardHelper.ReturnThreeMatches(boardCells, index);
    if (matches.Count == 0)
      return result;

    result.AddUnique(boardCells[(int)index.x][(int)index.y]);
    for (int i = 0; i < matches.Count; i++)
    {
      result.AddUnique(boardCells[(int)matches[i].x][(int)matches[i].y]);
      result.AddUnique(boardCells[(int)matches[i].z][(int)matches[i].w]);
    }
    return result;
  }


  public static List<HexCell> GetAllMatchedCells(HexCell[][] boardCells, Vector2 index)
  {
    List<HexCell> checkedCells = new List<HexCell>();

    var neighbourMatches = GetNearMatchedCells(boardCells, index);
    checkedCells.AddUnique(boardCells[(int)index.x][(int)index.y]);
    while (checkedCells.Count < neighbourMatches.Count)
    {
      for (int i = 0; i < neighbourMatches.Count; i++)
      {
        var cell = neighbourMatches[i];
        if (!checkedCells.Contains(cell))
        {

          neighbourMatches.AddRangeUnique(GetNearMatchedCells(boardCells, cell.HexObject.Index));
          checkedCells.AddUnique(cell);
        }
      }
    }

    return neighbourMatches;
  }

}

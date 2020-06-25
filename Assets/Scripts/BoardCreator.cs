using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardCreator : MonoBehaviour
{
  [SerializeField]
  Transform boardPanel;
  [SerializeField]
  int boardWidth = 8;
  int width { get { return (int)(boardWidth * .5f); } }
  [SerializeField]
  int boardHeight = 9;
  int height { get { return boardHeight * 2; } }
  [SerializeField]
  HexCell[][] boardCells;
  [SerializeField]
  GameObject hexPrefab;
  [SerializeField]
  Vector2 startPosition = new Vector2(50, 1400);
  [SerializeField]
  Vector2 hexSize = new Vector2(150 , 150);
  [SerializeField]
  List<Color> hexColors;
  public static List<Color> HexColors;

  private void Awake()
  {
    HexColors = hexColors;
  }
  private void Start()
  {
    SetBoardData();
    SetBoard();
    ChangeMatchedCells();
  }

  void SetBoardData()
  {
    boardCells = new HexCell[width][];
    hexSize = hexPrefab.GetComponent<RectTransform>().sizeDelta*Screen.width/ 1080 ;

    for (int i = 0; i < width; i++)
    {
      boardCells[i] = new HexCell[height];
    }

    for (int i = 0; i < width; i++)
    {
      for (int j = 0; j < height; j++)
      {
          boardCells[i][j]= new HexCell();
          startPosition.x= (Screen.width- hexSize.x*.83f* (width*2-1))*.5f;
          startPosition.y= (Screen.height+ hexSize.y* height*.4f)*.5f;
          boardCells[i][j].Position = startPosition + j * hexSize.y * .5f * Vector2.down + ((j % 2) * .51f + i) * hexSize.x *1.65f* Vector2.right;
      }
    }
  }

  void ChangeMatchedCells()
  {
    int matchedCells;
    do
    {
      matchedCells = 0;
      for (int i = 0; i < width; i++)
      {
        for (int j = 0; j < height; j++)
        {
          if (BoardHelper.ReturnThreeMatches(boardCells, new Vector2(i, j)).Count > 0)
          {
            matchedCells++;
            boardCells[i][j].HexObject.SetRandomColor();
          }
        }
      }
    } while (matchedCells > 0);
  }

  void SetBoard()
  {
    for (int i = 0; i < width; i++)
    {
      for (int j = 0; j < height; j++)
      {
        var obj = Instantiate(hexPrefab, boardPanel);
        var rect = obj.GetComponent<RectTransform>();
        rect.anchoredPosition= new Vector2( boardCells[i][j].Position.x, boardCells[i][j].Position.y);
        rect.sizeDelta = hexSize;
        var hexObject = obj.GetComponent<HexObject>();
        hexObject.Index = new Vector2(i, j);
        hexObject.Object = obj;
        hexObject.SetRandomColor();
        boardCells[i][j].HexObject = hexObject;
      }
    }

    HexCellController.Instance.BoardCells = boardCells;
    HexCellController.Instance.BoardPanel = boardPanel;
  }

}
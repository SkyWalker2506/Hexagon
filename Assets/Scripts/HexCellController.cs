using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HexCellController : Singleton<HexCellController>
{
  [HideInInspector]
  public Transform BoardPanel;
  public HexCell[][] BoardCells;
  internal static RectTransform chosenParent;
  HexObject[] chosenObjects = new HexObject[3];
  Stack<HexObject> collectedObjects = new Stack<HexObject>();
  bool isRotating = false;
  [SerializeField]
  float rotationSpeed = 5;

  private void Start()
  {
    chosenParent = new GameObject("Chosen Parent").AddComponent<RectTransform>();
    chosenParent.sizeDelta = Vector2.one * 10;
  }

  private void OnEnable()
  {
    ActionSystem.OnSwipeRight += RotateClockwise;
    ActionSystem.OnSwipeLeft += RotateCounterClockwise;
    ActionSystem.OnSwipeUp += RotateClockwise;
    ActionSystem.OnSwipeDown += RotateCounterClockwise;
  }

  private void OnDisable()
  {
    ActionSystem.OnSwipeRight -= RotateClockwise;
    ActionSystem.OnSwipeLeft -= RotateCounterClockwise;
    ActionSystem.OnSwipeUp -= RotateClockwise;
    ActionSystem.OnSwipeDown -= RotateCounterClockwise;
  }

  public void DeselectAllHexObject()
  {
    for (int i = 0; i < BoardCells.Length; i++)
    {
      for (int j = 0; j < BoardCells[i].Length; j++)
      {
        if(BoardCells[i][j].HexObject)
        BoardCells[i][j].HexObject.DeActivateHighlight();
      }
    }
  }

  public void SelectHexObjects(Vector2 index)
  {
    DeselectAllHexObject();
    var neighbourIndex = GetRandomIndex(index);
    var cell1= BoardCells[(int)index.x][(int)index.y];
    var cell2= BoardCells[(int)(neighbourIndex.x)][(int)(neighbourIndex.y)];
    var cell3= BoardCells[(int)(neighbourIndex.z)][(int)(neighbourIndex.w)];
    cell1.HexObject.ActivateHighlight();
    cell2.HexObject.ActivateHighlight();
    cell3.HexObject.ActivateHighlight();
    if(chosenObjects[0]!=null)
    {
      foreach (var obj in chosenObjects)
      {
        obj.transform.SetParent(BoardPanel);
      }
    }
    chosenObjects[0] = cell1.HexObject;
    chosenObjects[1] = cell2.HexObject;
    chosenObjects[2] = cell3.HexObject;
    chosenParent.SetParent(BoardPanel);
    chosenParent.position = (chosenObjects[0].transform.position +
    chosenObjects[1].transform.position + chosenObjects[2].transform.position ) / 3;
    foreach (var obj in chosenObjects)
    {
      obj.transform.SetParent( chosenParent);
    }

  }

  Vector4 GetRandomIndex(Vector2 index)
  {
    var relativeNeighbours = BoardHelper.GetNeighbourArray(new Vector2(BoardCells.Count(), BoardCells[0].Count()), index);
    return relativeNeighbours[UnityEngine.Random.Range(0, relativeNeighbours.Length)];
  }


  public void RotateClockwise()
  {
    StartCoroutine(IERotateClockwiseThreeTimes());
  }

  public void RotateCounterClockwise()
  {
    StartCoroutine(IERotateCounterClockwiseThreeTimes());
  }



  IEnumerator IERotateClockwiseThreeTimes()
  {
    for (int i = 0; i < 3; i++)
    {
      yield return StartCoroutine(IERotateClockwise());
      if (CheckAndSetCollectableObjects())
      {
         StartCoroutine(IERelocateHexCell());
        DeselectAllHexObject();
        yield return IECheckAndCollectAllMatched();
        ActionSystem.OnMoveMade?.Invoke();
        yield break;
      }
    }
  }

  IEnumerator IERotateCounterClockwiseThreeTimes()
  {
    for (int i = 0; i < 3; i++)
    {
      yield return StartCoroutine(IERotateCounterClockwise());
      if (CheckAndSetCollectableObjects())
      {
         StartCoroutine(IERelocateHexCell());
        DeselectAllHexObject();
        yield return IECheckAndCollectAllMatched();
        ActionSystem.OnMoveMade?.Invoke();
        yield break;
      }
    }
  }

  IEnumerator IERotateClockwise()
  {
    if (chosenParent.childCount == 0 || isRotating)
      yield break;
    yield return StartCoroutine(RotateChosenParent(-120));
    List<Vector2> indexList = new List<Vector2>();
    for (int i = 0; i < 3; i++)
    {
      indexList.Add(chosenObjects[i].Index);
    }
    var tempObj = chosenObjects;

    for (int i = 0; i < 3; i++)
    {
      BoardCells[(int)indexList[i].x][(int)indexList[i].y].HexObject = tempObj[(i + 1) % 3];
      chosenObjects[(i + 1) % 3].Index = indexList[i];
    }

    for (int i = 0; i < 3; i++)
    {
      CollectMatched(BoardHelper.GetAllMatchedCells(BoardCells, chosenObjects[i].Index));
    }
    CheckAndSetCollectableObjects();

  }

  IEnumerator IERotateCounterClockwise()
  {
    if (chosenParent.childCount==0 || isRotating)
         yield break;
    yield return StartCoroutine(RotateChosenParent(120));
    List<Vector2> indexList = new List<Vector2>();
    for (int i = 0; i < 3; i++)
    {
      indexList.Add(chosenObjects[i].Index);
    }
    var tempObj = chosenObjects;

    for (int i = 0; i < 3; i++)
    {
      BoardCells[(int)indexList[(i + 1) % 3].x][(int)indexList[(i + 1) % 3].y].HexObject = tempObj[i];
      chosenObjects[i].Index = indexList[(i + 1) % 3];
    }
    for (int i = 0; i < 3; i++)
    {
        CollectMatched(BoardHelper.GetAllMatchedCells(BoardCells, chosenObjects[i].Index));
    }
    CheckAndSetCollectableObjects();

  }

  IEnumerator RotateChosenParent(float value)
  {
    isRotating = true;
    chosenParent.Rotate(Vector3.forward * 360);
    var startTime = Time.time;

    var startRot = chosenParent.rotation;

    while ((Time.time - startTime)* rotationSpeed < 1)
    {
      yield return null;
      chosenParent.rotation=Quaternion.Lerp(startRot, Quaternion.Euler(startRot.eulerAngles +  value * Vector3.forward), (Time.time - startTime)* rotationSpeed);
    }

    chosenParent.rotation = Quaternion.Euler(startRot.eulerAngles + value * Vector3.forward);
    isRotating = false;
  }

  void CollectMatched(List<HexCell> matched)
  {
    for (int i = 0; i < matched.Count; i++)
    {
      HexObject hexObject = matched[i].HexObject;
      var matchedTransform = hexObject.Object.transform;
      matchedTransform.parent = BoardPanel;
      ParticleController.Instance.ShowCollectedParticle(matchedTransform.position);
      matchedTransform.position += Vector3.up * Screen.height*1.5f;
      if(!collectedObjects.Contains(hexObject))
      collectedObjects.Push(hexObject);
      GameManager.IncreaseScore(5);
      hexObject.ResetHexObject();
    }
  }

  IEnumerator IERelocateHexCell()
  {
    List<Tuple<HexObject, Vector3>> replacementObjects=new List<Tuple<HexObject, Vector3>>();
    for (int i = 0; i < BoardCells.Length; i++)
    {
      for (int j = BoardCells[i].Length-1; j >= 0; j--)
      {
        HexObject replacementObject = null;
        if(BoardCells[i][j].HexObject == null)
        {
          if (j > 1)
          {

            int y = j;
            while (y >1)
            {
              y -= 2;
              if (BoardCells[i][y].HexObject != null)
              {
                replacementObject = BoardCells[i][y].HexObject;
                BoardCells[i][y].HexObject = null;
                break;
              }
            }
          }
        
          if (replacementObject == null)
          {
            replacementObject = collectedObjects.Pop();
            replacementObject.transform.position = BoardCells[i][j].Position + Vector3.up * Screen.height*.75f;
          }
          BoardCells[i][j].HexObject = replacementObject;
          replacementObject.Index = new Vector2(i, j);
          replacementObjects.Add(Tuple.Create(replacementObject, BoardCells[i][j].Position));
        }
        
      }
    }
 
      for (int x = 0; x < replacementObjects.Count; x++)
      {
        yield return null;
        var coroutine =  StartCoroutine(replacementObjects[x].Item1.MoveToPosition(replacementObjects[x].Item2));
        if (x == replacementObjects.Count - 1)
         yield return coroutine;
      }
   yield return null;
  }

  IEnumerator IECheckAndCollectAllMatched()
  {
   // yield break;
    for (int i = 0; i < BoardCells.Length; i++)
    {
      for (int j = 0; j < BoardCells[i].Length; j++)
      {
        CollectMatched(BoardHelper.GetAllMatchedCells(BoardCells, new Vector2(i, j)));
                if (CheckAndSetCollectableObjects())
                {
                    yield return StartCoroutine(IERelocateHexCell());
                    yield return IECheckAndCollectAllMatched();
                }
            }
    }
  }

  private bool CheckAndSetCollectableObjects()
  {
    if (collectedObjects.Count > 0)
    {
      for (int j = 0; j < collectedObjects.Count; j++)
      {
        var index = collectedObjects.ToList()[j].Index;
        BoardCells[(int)index.x][(int)index.y].HexObject = null;
      }
      return true;
    }
    else
      return false;
  }
}

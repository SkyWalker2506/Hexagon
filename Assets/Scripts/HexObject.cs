using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HexObject : MonoBehaviour
{
  public GameObject Object;
  public Color HexColor;
  public Vector2 Index;
  [SerializeField]
  Text bombText;

  int bombCountDown;
  bool isBombActive;

  private void OnEnable()
  {
    ActionSystem.OnMoveMade += OnMoveMade;
  }

  private void OnDisable()
  {
    ActionSystem.OnMoveMade -= OnMoveMade;
  }

  public HexObject(GameObject obj, Color hexColor)
  {
    Object= obj;
    HexColor = hexColor;
    Object.GetComponent<Image>().color = HexColor;
  }

  public void Select()
  {
    HexCellController.Instance.SelectHexObjects(Index);
  }

  public void DeActivateHighlight()
  {
    transform.GetChild(0).gameObject.SetActive(false);
  }
  public void ActivateHighlight()
  {
    transform.GetChild(0).gameObject.SetActive(true);
  }
  public void ResetHexObject()
  {
    DeActivateHighlight();
    SetRandomColor();
    if(GameManager.IsReadyToSetBomb)
    {
      ActivateBomb(Random.Range(5, 10));
      GameManager.IsReadyToSetBomb = false;
    }
    else
    {
      DeActivateBomb();
    }
  }


  void ActivateBomb(int time)
  {
    isBombActive = true;
    bombCountDown = time;
    bombText.text = bombCountDown.ToString();
  }
   void DeActivateBomb()
  {
    isBombActive = false;
    bombText.text = "";
  }


  void OnMoveMade()
  {
    if(isBombActive)
    {
       bombCountDown--;
       bombText.text = bombCountDown.ToString();
       if (bombCountDown <= 0)
       {
        DeActivateBomb();
        ActionSystem.OnBombExplode?.Invoke();
       }
    }
  }

  public void SetRandomColor()
  {
    HexColor = BoardCreator.HexColors.GetRandom();
    Object.GetComponent<Image>().color = HexColor;
  }

  public IEnumerator MoveToPosition(Vector3 targetPos)
  {
    var distance = (transform.position - targetPos).magnitude;
    var startTime = Time.time;
    var startPos = transform.position;
    var movementSpeed = 3000;
    while ((Time.time - startTime) * movementSpeed / distance < 1)
    {
      yield return null;
      transform.position = Vector3.Lerp(startPos, targetPos, (Time.time - startTime) * movementSpeed / distance);
    }
    transform.position = targetPos;
  }

}
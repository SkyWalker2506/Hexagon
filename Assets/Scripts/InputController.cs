using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputController : MonoBehaviour
{
  Vector3 startTouchPosition;
  private void Update()
  {
    if (!GameManager.IsGameOn)
      return;

    if (HexCellController.chosenParent == null)
      return;

    if (Input.GetMouseButtonDown(0))
    {
      startTouchPosition = Input.mousePosition;

    }
    if (Input.GetMouseButtonUp(0))
      {
      Vector2 difference = Input.mousePosition - startTouchPosition;
      
      if (difference.magnitude < 100)
        return;
      if (Mathf.Abs(difference.x) > Mathf.Abs(difference.y))
      {
        if (difference.x * (startTouchPosition-HexCellController.chosenParent.position).y > 0)
          ActionSystem.OnSwipeRight?.Invoke();
        else
          ActionSystem.OnSwipeLeft?.Invoke();
      }
      else
      {
        if (difference.y * (HexCellController.chosenParent.position - startTouchPosition).x > 0)
          ActionSystem.OnSwipeUp?.Invoke();
        else
          ActionSystem.OnSwipeDown?.Invoke();
      }
    }
  }
}

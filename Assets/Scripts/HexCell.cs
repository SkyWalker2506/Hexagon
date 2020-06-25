using System;
using UnityEngine;

[Serializable]
public class HexCell
{
  public Vector3 Position=new Vector3(0,0,0);
  public HexObject HexObject = null;

  public HexCell()
  {
    Position = new Vector3(0, 0, 0);
    HexObject = null;
  }
}
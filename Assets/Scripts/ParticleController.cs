using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : Singleton<ParticleController>
{
  [SerializeField]
  GameObject collectedEffect;
  [SerializeField]
  GameObject bombEffect;

  private void Start()
  {
    bombEffect.SetActive(false);
  }


  private void OnEnable()
  {
    ActionSystem.OnBombExplode += ShowBombParticle;
  }

  private void OnDisable()
  {
    ActionSystem.OnBombExplode -= ShowBombParticle;
  }

  void ShowBombParticle()
  {
    bombEffect.SetActive(true);
  }

  public void ShowCollectedParticle(Vector3 position)
  {
    var particle = Instantiate(collectedEffect,position,Quaternion.identity, HexCellController.Instance.BoardPanel);
    particle.SetActive(true);
    Destroy(particle, 1); 

  }


}

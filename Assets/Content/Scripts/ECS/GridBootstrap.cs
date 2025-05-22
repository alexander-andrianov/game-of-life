using Unity.Entities;
using UnityEngine;

namespace Content.Scripts.ECS {
  public class GridBootstrap : MonoBehaviour {
    [SerializeField] private GameSettings gameSettings;
    
    public GameSettings GameSettings => gameSettings;

    private void Start() {
      var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
      var archetype = entityManager.CreateArchetype(typeof(CellData));
      
      for (var y = 0; y < gameSettings.Height; y++) {
        for (var x = 0; x < gameSettings.Width; x++) {
          var entity = entityManager.CreateEntity(archetype);
          var isAlive = Random.value < gameSettings.AliveChance;
          
          entityManager.SetComponentData(entity, new CellData {
            IsAlive = isAlive,
            X = x,
            Y = y
          });
        }
      }
    }
  }
} 
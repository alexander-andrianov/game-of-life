using UnityEngine;

namespace Content.Scripts.ECS {
  [CreateAssetMenu(fileName = "GameSettings", menuName = "GameOfLife/GameSettings")]
  public class GameSettings : ScriptableObject {
    [SerializeField] private int width = 128;
    [SerializeField] private int height = 72;
    [SerializeField] private int simulationSpeed = 4;
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private float aliveChance = 0.2f;
    
    [SerializeField] private Color aliveColor = Color.green;
    [SerializeField] private Color deadColor = Color.black;
    [SerializeField] private Color stableColor = new Color(0.2f, 0.7f, 1f);
    
    public int Width => width;
    public int Height => height;
    public int SimulationSpeed => simulationSpeed;
    public float CellSize => cellSize;
    public float AliveChance => aliveChance;
    
    public Color AliveColor => aliveColor;
    public Color DeadColor => deadColor;
    public Color StableColor => stableColor;
  }
} 
namespace Content.Scripts.Rules
{
    public class SteppersRule : IGameOfLifeRule
    {
        public bool GetNextState(bool isAlive, int aliveNeighbors)
        {
            if (isAlive)
                return aliveNeighbors == 2 || aliveNeighbors == 3 || aliveNeighbors == 4;
            else
                return aliveNeighbors == 3 || aliveNeighbors == 4;
        }
    }
} 
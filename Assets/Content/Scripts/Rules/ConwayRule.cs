namespace Content.Scripts.Rules
{
    public class ConwayRule : IGameOfLifeRule
    {
        public bool GetNextState(bool isAlive, int aliveNeighbors)
        {
            if (isAlive)
                return aliveNeighbors == 2 || aliveNeighbors == 3;
            else
                return aliveNeighbors == 3;
        }
    }
} 
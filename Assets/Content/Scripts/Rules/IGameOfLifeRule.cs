namespace Content.Scripts.Rules
{
    public interface IGameOfLifeRule
    {
        bool GetNextState(bool isAlive, int aliveNeighbors);
    }
} 
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Entities;
using Content.Scripts.Rules;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Content.Scripts.ECS
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class GameOfLifeSystem : SystemBase
    {
        private static GameSettings gameSettings;
        private static bool restartRequested = false;
        private static float simulationSpeed = 0f;

        private IGameOfLifeRule rule;
        private float timer = 0f;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static async Task Initialize()
        {
            gameSettings = await Addressables.LoadAssetAsync<GameSettings>("game-settings-config").Task.ConfigureAwait(true);
            simulationSpeed = gameSettings.SimulationSpeed;
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            rule = new ConwayRule();
        }

        public static void SetSimulationSpeed(float speed)
        {
            simulationSpeed = Mathf.Clamp(speed, 0.01f, 100f);
        }

        public static void RequestRestart()
        {
            restartRequested = true;
        }

        protected override void OnUpdate()
        {
            var entityManager = EntityManager;
            var cellQuery = GetEntityQuery(typeof(CellData));

            if (restartRequested)
            {
                var cellArrayRestart = cellQuery.ToEntityArray(Unity.Collections.Allocator.TempJob);
                var random = new Unity.Mathematics.Random((uint)UnityEngine.Random.Range(1, int.MaxValue));
                
                foreach (var entity in cellArrayRestart)
                {
                    var cell = entityManager.GetComponentData<CellData>(entity);

                    cell.IsAlive = random.NextFloat() < 0.2f;
                    entityManager.SetComponentData(entity, cell);
                }

                cellArrayRestart.Dispose();
                restartRequested = false;

                return;
            }

            timer += SystemAPI.Time.DeltaTime * simulationSpeed;
            if (timer < 1f) return;
            timer = 0f;

            var cellArray = cellQuery.ToEntityArray(Allocator.TempJob);
            var cellDataArray = cellQuery.ToComponentDataArray<CellData>(Allocator.TempJob);

            var width = 128;
            var height = 128;

            var stateGrid = new NativeArray<bool>(width * height, Allocator.Temp);
            foreach (var cell in cellDataArray)
            {
                stateGrid[cell.Y * width + cell.X] = cell.IsAlive;
            }

            var nextStates = new NativeArray<bool>(cellDataArray.Length, Allocator.Temp);

            for (var i = 0; i < cellDataArray.Length; i++)
            {
                var cell = cellDataArray[i];
                var aliveNeighbors = 0;

                for (var dy = -1; dy <= 1; dy++)
                {
                    for (var dx = -1; dx <= 1; dx++)
                    {
                        if (dx == 0 && dy == 0) continue;

                        var nx = cell.X + dx;
                        var ny = cell.Y + dy;

                        if (nx < 0 || ny < 0 || nx >= width || ny >= height) continue;
                        
                        if (stateGrid[ny * width + nx]) aliveNeighbors++;
                    }
                }

                nextStates[i] = rule.GetNextState(cell.IsAlive, aliveNeighbors);
            }

            for (var i = 0; i < cellArray.Length; i++)
            {
                var cell = cellDataArray[i];
                var wasAlive = cell.IsAlive;
                var nextAlive = nextStates[i];

                if (wasAlive == nextAlive)
                {
                    cell.StableCount++;
                }
                else
                {
                    cell.StableCount = 0;
                }

                cell.PreviousAlive = wasAlive;
                cell.IsAlive = nextAlive;
                entityManager.SetComponentData(cellArray[i], cell);
            }

            nextStates.Dispose();
            stateGrid.Dispose();
            cellArray.Dispose();
            cellDataArray.Dispose();
        }
    }
} 
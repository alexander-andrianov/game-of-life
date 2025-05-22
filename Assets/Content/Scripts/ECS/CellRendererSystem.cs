using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.ECS
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial class CellRendererSystem : SystemBase
    {
        private Dictionary<Entity, Image> cellImages = new();
        private GameObject gridUI;
        private RectTransform gridRect;
        private GameObject cellPrefab;
        private GameSettings settings;
        private bool initialized = false;
        private int width;
        private int height;

        protected override void OnUpdate()
        {
            if (!initialized)
            {
                var canvas = Object.FindFirstObjectByType<Canvas>();
                if (canvas == null)
                {
                    var canvasGO = new GameObject("Canvas", typeof(Canvas));
                    canvas = canvasGO.GetComponent<Canvas>();
                    canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    canvasGO.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
                    canvasGO.AddComponent<GraphicRaycaster>();
                }

                gridUI = new GameObject("GridUI", typeof(RectTransform));
                gridRect = gridUI.GetComponent<RectTransform>();
                gridUI.transform.SetParent(canvas.transform, false);
                gridUI.transform.SetAsFirstSibling();

                gridRect.anchorMin = Vector2.zero;
                gridRect.anchorMax = Vector2.one;
                gridRect.offsetMin = Vector2.zero;
                gridRect.offsetMax = Vector2.zero;
                gridRect.pivot = new Vector2(0.5f, 0.5f);

                cellPrefab = new GameObject("CellImage", typeof(RectTransform), typeof(Image));
                var img = cellPrefab.GetComponent<Image>();
                img.color = Color.gray;
                cellPrefab.SetActive(false);

                settings = Object.FindFirstObjectByType<GridBootstrap>()?.GameSettings;
                initialized = true;
            }

            if (gridRect.rect.width == 0 || gridRect.rect.height == 0)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(gridRect);
                if (gridRect.rect.width == 0 || gridRect.rect.height == 0)
                    return;
            }

            int gridWidth = settings?.Width ?? 32;
            int gridHeight = settings?.Height ?? 32;
            bool isPortrait = gridRect.rect.height > gridRect.rect.width;
            
            if (isPortrait)
            {
                (gridWidth, gridHeight) = (gridHeight, gridWidth);
            }

            float cellWidth = gridRect.rect.width / gridWidth;
            float cellHeight = gridRect.rect.height / gridHeight;
            float cellSize = Mathf.Min(cellWidth, cellHeight);
            float totalGridWidth = cellSize * gridWidth;
            float totalGridHeight = cellSize * gridHeight;
            float offsetX = (gridRect.rect.width - totalGridWidth) * 0.5f;
            float offsetY = (gridRect.rect.height - totalGridHeight) * 0.5f;

            var aliveColor = settings?.AliveColor ?? Color.green;
            var deadColor = settings?.DeadColor ?? Color.black;
            var stableColor = settings?.StableColor ?? Color.cyan;

            Entities.ForEach((Entity entity, ref CellData cell) =>
            {
                if (!cellImages.ContainsKey(entity))
                {
                    var go = Object.Instantiate(cellPrefab, gridUI.transform);
                    go.SetActive(true);
                    cellImages[entity] = go.GetComponent<Image>();
                }

                var image = cellImages[entity];
                var rect = image.GetComponent<RectTransform>();

                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.zero;
                rect.pivot = Vector2.zero;
                rect.sizeDelta = new Vector2(cellSize, cellSize);
                if (isPortrait)
                {
                    rect.anchoredPosition = new Vector2(cell.Y * cellSize + offsetX, cell.X * cellSize + offsetY);
                }
                else
                {
                    rect.anchoredPosition = new Vector2(cell.X * cellSize + offsetX, cell.Y * cellSize + offsetY);
                }

                if (cell.IsAlive && cell.StableCount >= 2)
                    image.color = stableColor;
                else if (cell.IsAlive)
                    image.color = aliveColor;
                else
                    image.color = deadColor;
            }).WithoutBurst().Run();
        }
    }
}

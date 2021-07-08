namespace Controller
{
    using Generation;
    using UnityEngine;

    public class HighlightTileController : MonoBehaviour
    {
        public Camera playerCamera;
        public MapManager mapManager;
        public Vector2 offset = new Vector2(0.5f, 0.5f);
        public Color highlightColor;

        private bool selection = false;

        private Transform currentChunk;
        private (int, int) pastPosition;

        private Material currentMaterial;
        private (int, int) currentTilePosition;
        private (int, int) currentChunkTilePositionForMove;

        private void Update()
        {
            (int, int) mouseTilePosition = GetTileMapMousePosition();
            if (mapManager.TransformsMap[mouseTilePosition.Item1, mouseTilePosition.Item2, 0]
                    ?.GetComponent<MeshRenderer>().sharedMaterial.color ==
                mapManager.EartCloneTile.GetComponent<MeshRenderer>().sharedMaterial.color)
            {
                MeshRenderer renderer = mapManager.TransformsMap[mouseTilePosition.Item1, mouseTilePosition.Item2, 0]
                    .GetComponent<MeshRenderer>();
                currentMaterial = renderer.material;
                currentMaterial.color = highlightColor;
                
                if ( selection && pastPosition != mouseTilePosition &&
                    mapManager.TransformsMap[pastPosition.Item1, pastPosition.Item2, 0] != null)
                {
                    MeshRenderer renderer2 = mapManager.TransformsMap[pastPosition.Item1, pastPosition.Item2, 0]
                        .GetComponent<MeshRenderer>();
                    Material currentMaterial2 = renderer2.material;
                    currentMaterial2.color = mapManager.config.colorsByIds[1];
                }
                selection = true;
                pastPosition = mouseTilePosition;
            }


            if ( pastPosition != mouseTilePosition &&
                 mapManager.TransformsMap[pastPosition.Item1, pastPosition.Item2, 0]?.GetComponent<MeshRenderer>()
                     .sharedMaterial.color ==
                 highlightColor && selection)
            {
                MeshRenderer renderer = mapManager.TransformsMap[pastPosition.Item1, pastPosition.Item2, 0]
                    .GetComponent<MeshRenderer>();
                currentMaterial = renderer.material;
                currentMaterial.color = mapManager.config.colorsByIds[1];
                pastPosition = mouseTilePosition;
                selection = false;
            }
        }

        private Vector3 GetWorldMousePosition()
        {
            Vector3 mousePosition = Input.mousePosition;
            Vector3 worldPosition = playerCamera.ScreenToWorldPoint(mousePosition);
            return worldPosition;
        }


        private (int, int) GetTileMapMousePosition()
        {
            Vector3 worldMousePosition = GetWorldMousePosition() + (Vector3) offset;

            return (Mathf.FloorToInt(worldMousePosition.x), Mathf.FloorToInt(worldMousePosition.y));
        }

        private void OnDrawGizmos()
        {
            if (!playerCamera)
                return;
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(GetWorldMousePosition(), 0.125f);
        }
    }
}
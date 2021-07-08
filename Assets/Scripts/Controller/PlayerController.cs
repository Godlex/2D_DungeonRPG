namespace Controller
{
    using Generation;
    using UnityEngine;

    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private GameObject field;
        public MapManager mapManager;
        private HighlightTileController Highlight;

        private void Start()
        {
            field = GameObject.Find("FieldGenerator");
            mapManager = field.GetComponent<MapManager>();
            Highlight = field.GetComponent<HighlightTileController>();
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S) && (mapManager.TransformsMap[(int) gameObject.transform.position.x,
                        (int) gameObject.transform.position.y - 1, 0]
                    ?.GetComponent<MeshRenderer>().sharedMaterial.color ==
                mapManager.EartCloneTile.GetComponent<MeshRenderer>().sharedMaterial.color || mapManager.TransformsMap[
                        (int) gameObject.transform.position.x,
                        (int) gameObject.transform.position.y - 1, 0]
                    ?.GetComponent<MeshRenderer>().sharedMaterial.color == Highlight.highlightColor))
            {
                gameObject.transform.position += Vector3.down;
            }

            if (Input.GetKeyDown(KeyCode.W) && (mapManager
                    .TransformsMap[(int) gameObject.transform.position.x, (int) gameObject.transform.position.y + 1, 0]
                    ?.GetComponent<MeshRenderer>().sharedMaterial.color ==
                mapManager.EartCloneTile.GetComponent<MeshRenderer>().sharedMaterial.color|| mapManager.TransformsMap[
                        (int) gameObject.transform.position.x,
                        (int) gameObject.transform.position.y + 1, 0]
                    ?.GetComponent<MeshRenderer>().sharedMaterial.color == Highlight.highlightColor))
            {
                gameObject.transform.position += Vector3.up;
            }

            if (Input.GetKeyDown(KeyCode.D) && (mapManager
                    .TransformsMap[(int) gameObject.transform.position.x + 1, (int) gameObject.transform.position.y, 0]
                    ?.GetComponent<MeshRenderer>().sharedMaterial.color ==
                mapManager.EartCloneTile.GetComponent<MeshRenderer>().sharedMaterial.color || mapManager.TransformsMap[
                        (int) gameObject.transform.position.x+1,
                        (int) gameObject.transform.position.y, 0]
                    ?.GetComponent<MeshRenderer>().sharedMaterial.color == Highlight.highlightColor))
            {
                gameObject.transform.position += Vector3.right;
            }

            if (Input.GetKeyDown(KeyCode.A) && (mapManager
                    .TransformsMap[(int) gameObject.transform.position.x - 1, (int) gameObject.transform.position.y, 0]
                    ?.GetComponent<MeshRenderer>().sharedMaterial.color ==
                mapManager.EartCloneTile.GetComponent<MeshRenderer>().sharedMaterial.color || mapManager.TransformsMap[
                        (int) gameObject.transform.position.x-1,
                        (int) gameObject.transform.position.y , 0]
                    ?.GetComponent<MeshRenderer>().sharedMaterial.color == Highlight.highlightColor))
            {
                gameObject.transform.position += Vector3.left;
            }
        }
    }
}
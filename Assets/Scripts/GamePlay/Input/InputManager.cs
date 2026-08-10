using Match3.Gameplay.Board;
using UnityEngine;

namespace Match3.Gameplay.Input
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Swap.SwapManager swapManager;

        private GemTile firstGem;

        private void Update()
        {
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                firstGem = RaycastGem();
            }

            if (UnityEngine.Input.GetMouseButtonUp(0))
            {
                if (firstGem == null)
                    return;

                GemTile secondGem = RaycastGem();

                if (secondGem != null &&
                    secondGem != firstGem)
                {
                    swapManager.TrySwap(firstGem, secondGem);
                }

                firstGem = null;
            }
        }

        private GemTile RaycastGem()
        {
            Vector2 mouse =
                mainCamera.ScreenToWorldPoint(UnityEngine.Input.mousePosition);

            RaycastHit2D hit =
                Physics2D.Raycast(mouse, Vector2.zero);

            if (!hit)
                return null;

            return hit.collider.GetComponent<GemTile>();
        }
    }
}
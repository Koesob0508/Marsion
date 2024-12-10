using UnityEngine;

namespace Marsion
{
    public class CanvasOrderHandler
    {
        private int _order = 10;

        public void SetCanvas(GameObject go, bool isPopup)
        {
            Canvas canvas = Util.GetOrAddComponent<Canvas>(go);
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.overrideSorting = true;
            canvas.sortingOrder = isPopup ? _order++ : 0;
        }

        public void DecrementOrder()
        {
            if (_order > 10) _order--;
        }
    }
}
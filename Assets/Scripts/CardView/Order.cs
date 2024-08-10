using UnityEngine;

public class Order : MonoBehaviour
{
    [SerializeField] Renderer[] BackRenderers;
    [SerializeField] Renderer[] FrameRenderers;
    [SerializeField] Renderer[] AbilityRenderers;
    [SerializeField] Renderer[] FrontRenderers;
    [SerializeField] Renderer[] TextRenderers;
    [SerializeField] string SortingLayerName;
    [SerializeField] int OriginOrder;

    public void SetOrder(int order)
    {
        int mulOrder = order * 10;

        foreach(var renderer in BackRenderers)
        {
            renderer.sortingLayerName = SortingLayerName;
            renderer.sortingOrder = mulOrder;
        }

        foreach(var renderer in FrameRenderers)
        {
            renderer.sortingLayerName = SortingLayerName;
            renderer.sortingOrder = mulOrder + 1;
        }
        foreach (var renderer in AbilityRenderers)
        {
            renderer.sortingLayerName = SortingLayerName;
            renderer.sortingOrder = mulOrder + 2;
        }

        foreach (var renderer in FrontRenderers)
        {
            renderer.sortingLayerName = SortingLayerName;
            renderer.sortingOrder = mulOrder + 3;
        }

        foreach (var renderer in TextRenderers)
        {
            renderer.sortingLayerName = SortingLayerName;
            renderer.sortingOrder = mulOrder + 4;
        }
    }

    public void SetOriginOrder(int originOrder)
    {
        OriginOrder = originOrder;
        SetOrder(originOrder);
    }

    public void SetMostFrontOrder(bool isMostFront)
    {
        SetOrder(isMostFront ? 100 : OriginOrder);
    }
}

using UnityEngine;
using UnityEngine.UI;

public class DynamicGridCellSize : MonoBehaviour
{
    public int horizontalCellCount = 3;

    private GridLayoutGroup _adButtonsGridLayout;
    private RectTransform _adButtonsRect;

    // Start is called before the first frame update
    void Start()
    {
        _adButtonsGridLayout = this.GetComponent<GridLayoutGroup>();
        _adButtonsRect = this.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        //Update grid cell size
        float width = _adButtonsRect.rect.width;
        _adButtonsGridLayout.cellSize =
            new Vector2((width / horizontalCellCount) - _adButtonsGridLayout.spacing.x, _adButtonsGridLayout.cellSize.y);
    }
}
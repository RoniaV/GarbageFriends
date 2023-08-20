using System;
using UnityEngine;

public class PreviewSystem : MonoBehaviour
{
    [SerializeField] float minSize = 0.1f;
    [SerializeField] GameObject cellIndicator;
    [SerializeField] Material previewMaterialPrefab;
    [Header("Colors")]
    [SerializeField] Color previewColor = Color.white;
    [SerializeField] Color wrongColor = Color.red;

    private Renderer cellIndicatorRenderer;

    private Vector3Int actualSize = Vector3Int.one;

    private void Start()
    {
        cellIndicator.SetActive(false);
        cellIndicatorRenderer = cellIndicator.GetComponentInChildren<Renderer>();
    }

    public void StartShowingPlacementPreview(Vector3Int size)
    {
        PrepareCursor(size);
        MoveCursor(Vector3.zero, FurnitureLayers.none);
        ApplyFeedbackToCursor(true);

        cellIndicator.SetActive(true);
    }

    private void PrepareCursor(Vector3Int size)
    {
        if (size.x > 0 || size.z > 0)
        {
            actualSize = size;
        }
        else
            actualSize = Vector3Int.one;
    }

    public void StopShowingPreview()
    {
        cellIndicator.SetActive(false);
    }

    public void UpdatePosition(Vector3 position, bool validity, FurnitureLayers furnitureLayer)
    {
        MoveCursor(position, furnitureLayer);
        ApplyFeedbackToCursor(validity);
        FixSize(furnitureLayer, position);
    }

    private void FixSize(FurnitureLayers furnitureLayer, Vector3 position)
    {
        switch (furnitureLayer)
        {
            case FurnitureLayers.floor:
                if(position.y > 0)
                    cellIndicator.transform.localScale =
                    new Vector3(actualSize.x, position.y, actualSize.z);
                else
                cellIndicator.transform.localScale = 
                    new Vector3(actualSize.x, minSize, actualSize.z);
                break;
            case FurnitureLayers.rightWall:
                cellIndicator.transform.localScale =
                    new Vector3(minSize, actualSize.y, actualSize.z);
                break;
            case FurnitureLayers.leftWall:
                cellIndicator.transform.localScale =
                    new Vector3(actualSize.x, actualSize.y, minSize);
                break;
            case FurnitureLayers.none:
                cellIndicator.transform.localScale = Vector3.one;
                break;
            default:
                cellIndicator.transform.localScale = Vector3.one;
                break;
        }
    }

    private void ApplyFeedbackToCursor(bool validity)
    {
        Color c = validity ? previewColor : wrongColor;

        cellIndicatorRenderer.material.SetColor("_Color", c);
    }

    private void MoveCursor(Vector3 position, FurnitureLayers furnitureLayer)
    {
        Vector3 cursorPosition = position;

        if(furnitureLayer == FurnitureLayers.floor && position.y > 0)
        {
            cursorPosition.y = 0;
        }

        cellIndicator.transform.position = cursorPosition;
    }

    internal void StartShowingRemovePreview()
    {
        cellIndicator.SetActive(true);
        PrepareCursor(Vector3Int.one);
        ApplyFeedbackToCursor(false);
    }
}

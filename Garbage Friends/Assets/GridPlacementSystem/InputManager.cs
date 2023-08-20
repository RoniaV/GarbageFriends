using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum FurnitureLayers
{
    floor,
    rightWall,
    leftWall,
    none
}

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private Camera sceneCamera;

    private MapInfo lastMapInfo;

    [SerializeField] LayerMask floorLayermask;
    [SerializeField] LayerMask rightWallLayermask;
    [SerializeField] LayerMask leftWallLayermask;

    public event Action OnClicked;
    public event Action OnExit;

    void Start()
    {
        if (lastMapInfo == null)
            lastMapInfo = new MapInfo(Vector3.zero, FurnitureLayers.none);
    }

    private void Update()
    {
        if(Input.GetMouseButtonUp(0))
            OnClicked?.Invoke();
        if(Input.GetKeyDown(KeyCode.Escape))
            OnExit?.Invoke();
    }

    public bool IsPointerOverUI()
        => EventSystem.current.IsPointerOverGameObject();

    public MapInfo GetSelectedMapInfo()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = sceneCamera.nearClipPlane;
        Ray ray = sceneCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, floorLayermask))
            lastMapInfo = new MapInfo(hit.point, FurnitureLayers.floor);
        else if (Physics.Raycast(ray, out hit, 100, rightWallLayermask))
            lastMapInfo = new MapInfo(hit.point, FurnitureLayers.rightWall);
        else if(Physics.Raycast(ray, out hit, 100, leftWallLayermask))
            lastMapInfo = new MapInfo(hit.point, FurnitureLayers.leftWall);

        return lastMapInfo;
    }

    //public Vector3 GetSelectedMapPositionAtLayer(FurnitureLayers furnitureLayer)
    //{
    //    LayerMask layer = default(LayerMask);

    //    if (furnitureLayer == FurnitureLayers.floor)
    //        layer = floorLayermask;
    //    else if (furnitureLayer == FurnitureLayers.rightWall)
    //        layer = rightWallLayermask;
    //    else if (furnitureLayer == FurnitureLayers.leftWall)
    //        layer = leftWallLayermask;

    //    Vector3 mousePos = Input.mousePosition;
    //    mousePos.z = sceneCamera.nearClipPlane;
    //    Ray ray = sceneCamera.ScreenPointToRay(mousePos);
    //    RaycastHit hit;
    //    if (Physics.Raycast(ray, out hit, 100, layer))
    //    {
    //        lastMapInfo = hit.point;
    //    }
    //    return lastMapInfo;
    //}

    public FurnitureLayers GetSelectedLayer()
    {
        FurnitureLayers selectedLayer = new FurnitureLayers();

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = sceneCamera.nearClipPlane;
        Ray ray = sceneCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100, floorLayermask))
            selectedLayer = FurnitureLayers.floor;

        else if (Physics.Raycast(ray, out hit, 100, rightWallLayermask))
            selectedLayer = FurnitureLayers.rightWall;

        else if(Physics.Raycast(ray,out hit, 100, leftWallLayermask))
            selectedLayer= FurnitureLayers.leftWall;

            return selectedLayer;
    }
}

public class MapInfo
{
    public Vector3 mapPosition { get; private set; }
    public FurnitureLayers layer { get; private set; }

    public MapInfo(Vector3 mapPosition, FurnitureLayers layer)
    {
        this.mapPosition = mapPosition;
        this.layer = layer;
    }
}

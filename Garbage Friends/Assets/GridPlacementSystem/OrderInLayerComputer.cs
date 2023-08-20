using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderInLayerComputer : MonoBehaviour
{
    [SerializeField] Vector3Int startGridPoint;

    public int GetOrderInLayer(Vector3Int objectGridPosition, Vector3Int objectSize)
    {
        int order = 0;

        Vector3Int layerPoint = objectGridPosition;

        if(objectSize.x > 1 || objectSize.z > 1)
        {
            if (objectSize.z >= objectSize.x)
                layerPoint.z += objectSize.z - 1;
            else if (objectSize.x > objectSize.z)
                layerPoint.x += objectSize.x - 1;
        }

        order = ((startGridPoint.x - layerPoint.x) +
            (startGridPoint.z - layerPoint.z)) * 2;

        if (objectGridPosition.y > 0)
            order += objectGridPosition.y;

        //if (objectSize.y > 2)
        //    order += Mathf.FloorToInt(objectSize.y / 2);

        return order;
    }
}

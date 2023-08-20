using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlacementUI : MonoBehaviour
{
    [SerializeField] Image objectPreview;
    [SerializeField] GameObject rotateObjectButton;
    [SerializeField] GameObject deleteObjectButton;
    [SerializeField] ObjectsDatabaseSO database;

    public void SetObjectIcon(int objectID, bool isRotated = false)
    {
        ObjectData objectData = database.GetObjectDataWithID(objectID);

        objectPreview.sprite = objectData.Icon;
        rotateObjectButton.SetActive(objectData.CanBeRotated);
        deleteObjectButton.SetActive(!objectData.CantBeDeleted);

        Vector3 imageScale = new Vector3(isRotated ? -1 : 1, 1, 1);
        objectPreview.transform.localScale = imageScale;
    }
}

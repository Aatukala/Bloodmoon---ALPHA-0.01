using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    [Header("Build Objects")]
    [SerializeField] private List<GameObject> floorObjects = new List<GameObject>();
    [SerializeField] private List<GameObject> wallObjects = new List<GameObject>();
    [SerializeField] private List<GameObject> stairObjects = new List<GameObject>();
    [SerializeField] private List<GameObject> craftingTableObjects = new List<GameObject>();

    [Header("Build Settings")]
    [SerializeField] private SelectedBuildType currentBuildType = SelectedBuildType.floor;
    [SerializeField] private LayerMask connectorLayer;

    [Header("Ghost Settings")]
    [SerializeField] private Material ghostMaterialValid;
    [SerializeField] private Material ghostMaterialInvalid;
    [SerializeField] private float connectorOverlapRadius = 1;
    [SerializeField] private float maxGroundAngle = 45f;

    [Header("Internal State")]
    [SerializeField] private bool isBuilding = false;
    [SerializeField] private int currentBuildingIndex;
    private GameObject ghostBuildGameObject;
    private bool isGhostInValidPosition = false;
    private Transform ModelParent = null;

    private void Update()
    {
        handleBuildTypeInput();
        handleVariantRotationInput();
        handleBuildMode();

        if (isBuilding)
        {
            ghostBuild();

            if (Input.GetMouseButtonDown(0))
            {
                placeBuild();
            }
        }
        else if (ghostBuildGameObject)
        {
            Destroy(ghostBuildGameObject);
            ghostBuildGameObject = null;
        }
    }

    //public GameData LoadData(GameData data)
    //{

    //}

    //public GameData SaveData(ref GameData data)
    //{

    //}

    // ?? Switch build type with number keys
    private void handleBuildTypeInput()
    {
        bool switched = false;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentBuildType = SelectedBuildType.floor;
            switched = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentBuildType = SelectedBuildType.wall;
            switched = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentBuildType = SelectedBuildType.stair;
            switched = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            currentBuildType = SelectedBuildType.craftingTable;
            switched = true;
        }

        if (switched)
        {
            Debug.Log("Switched Build Type: " + currentBuildType);

            // ?? Refresh the ghost when switching build types
            if (ghostBuildGameObject != null)
            {
                Destroy(ghostBuildGameObject);
                ghostBuildGameObject = null;
            }

            if (isBuilding)
            {
                GameObject currentBuild = getCurrentBuild();
                createGhostPrefab(currentBuild);
            }
        }
    }

    private void handleBuildMode()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            isBuilding = !isBuilding;
            Debug.Log(isBuilding ? "Building Mode: ON" : "Building Mode: OFF");

            if (!isBuilding && ghostBuildGameObject != null)
            {
                Destroy(ghostBuildGameObject);
                ghostBuildGameObject = null;
            }
        }
    }

    private void ghostBuild()
    {
        GameObject currentBuild = getCurrentBuild();
        createGhostPrefab(currentBuild);
        moveGhostPrefabToRaycast();
        checkBuildValidity();
    }

    private void createGhostPrefab(GameObject currentBuild)
    {
        if (ghostBuildGameObject == null)
        {
            ghostBuildGameObject = Instantiate(currentBuild);
            ModelParent = ghostBuildGameObject.transform.GetChild(0);

            ghostifyModel(ModelParent, ghostMaterialValid);
            ghostifyModel(ghostBuildGameObject.transform);
        }
    }

    private void moveGhostPrefabToRaycast()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            ghostBuildGameObject.transform.position = hit.point;
        }
    }

    private void checkBuildValidity()
    {
        if (currentBuildType == SelectedBuildType.craftingTable)
        {
            ghostSeperateBuild();
            return;
        }

        Collider[] colliders = Physics.OverlapSphere(ghostBuildGameObject.transform.position, connectorOverlapRadius, connectorLayer);
        if (colliders.Length > 0)
        {
            ghostConnectBuild(colliders);
        }
        else
        {
            ghostSeperateBuild();

            if (isGhostInValidPosition)
            {
                Collider[] overlapColliders = Physics.OverlapBox(ghostBuildGameObject.transform.position, new Vector3(2f, 2f, 2f), ghostBuildGameObject.transform.rotation);
                foreach (Collider overlapCollider in overlapColliders)
                {
                    if (overlapCollider.gameObject != ghostBuildGameObject && overlapCollider.transform.root.CompareTag("Buildables"))
                    {
                        ghostifyModel(ModelParent, ghostMaterialInvalid);
                        isGhostInValidPosition = false;
                        return;
                    }
                }
            }
        }
    }

    private void ghostConnectBuild(Collider[] colliders)
    {
        Connector bestConnector = null;
        foreach (Collider collider in colliders)
        {
            Connector connector = collider.GetComponent<Connector>();
            if (connector != null && connector.canConnectTo)
            {
                bestConnector = connector;
                break;
            }
        }

        if (bestConnector == null)
        {
            ghostifyModel(ModelParent, ghostMaterialInvalid);
            isGhostInValidPosition = false;
            return;
        }

        if ((currentBuildType == SelectedBuildType.floor && bestConnector.isConnectedToFloor) ||
            (currentBuildType == SelectedBuildType.wall && bestConnector.isConnectedToWall) ||
            (currentBuildType == SelectedBuildType.stair && bestConnector.isConnectedToStair))
        {
            ghostifyModel(ModelParent, ghostMaterialInvalid);
            isGhostInValidPosition = false;
            return;
        }

        snapGhostPrefabToConnector(bestConnector);
    }

    private void snapGhostPrefabToConnector(Connector connector)
    {
        Transform ghostConnector = findSnapConnector(connector.transform, ghostBuildGameObject.transform.GetChild(1));
        if (!ghostConnector)
        {
            ghostifyModel(ModelParent, ghostMaterialInvalid);
            isGhostInValidPosition = false;
            return;
        }

        ghostBuildGameObject.transform.position = connector.transform.position - (ghostConnector.position - ghostBuildGameObject.transform.position);

        if (currentBuildType == SelectedBuildType.wall || currentBuildType == SelectedBuildType.stair)
        {
            Quaternion newRotation = ghostBuildGameObject.transform.rotation;
            newRotation.eulerAngles = new Vector3(newRotation.eulerAngles.x, connector.transform.rotation.eulerAngles.y, newRotation.eulerAngles.z);
            ghostBuildGameObject.transform.rotation = newRotation;
        }

        ghostifyModel(ModelParent, ghostMaterialValid);
        isGhostInValidPosition = true;
    }

    private void ghostSeperateBuild()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (currentBuildType == SelectedBuildType.wall || currentBuildType == SelectedBuildType.stair)
            {
                ghostifyModel(ModelParent, ghostMaterialInvalid);
                isGhostInValidPosition = false;
                return;
            }

            // Crafting table = always allowed to free place (if ground angle is valid)
            if (currentBuildType == SelectedBuildType.craftingTable)
            {
                if (Vector3.Angle(hit.normal, Vector3.up) < maxGroundAngle)
                {
                    ghostifyModel(ModelParent, ghostMaterialValid);
                    isGhostInValidPosition = true;
                }
                else
                {
                    ghostifyModel(ModelParent, ghostMaterialInvalid);
                    isGhostInValidPosition = false;
                }
                return;
            }

            if (Vector3.Angle(hit.normal, Vector3.up) < maxGroundAngle)
            {
                ghostifyModel(ModelParent, ghostMaterialValid);
                isGhostInValidPosition = true;
            }
            else
            {
                ghostifyModel(ModelParent, ghostMaterialInvalid);
                isGhostInValidPosition = false;
            }
        }
    }

    private Transform findSnapConnector(Transform snapConnector, Transform ghostConnectorParent)
    {
        ConnectorPosition OppositeConnectorTag = getOppositePosition(snapConnector.GetComponent<Connector>());
        foreach (Connector connector in ghostConnectorParent.GetComponentsInChildren<Connector>())
        {
            if (connector.connectorPosition == OppositeConnectorTag)
                return connector.transform;
        }
        return null;
    }

    private ConnectorPosition getOppositePosition(Connector connector)
    {
        ConnectorPosition position = connector.connectorPosition;

        switch (currentBuildType)
        {
            case SelectedBuildType.wall:
                if (connector.connectorParentType == SelectedBuildType.floor)
                    return ConnectorPosition.bottom;
                break;

            case SelectedBuildType.floor:
                if (connector.connectorParentType == SelectedBuildType.wall && connector.connectorPosition == ConnectorPosition.top)
                    return ConnectorPosition.bottom;
                break;

            case SelectedBuildType.stair:
                if (connector.connectorParentType == SelectedBuildType.floor)
                    return ConnectorPosition.top;
                break;
        }

        switch (position)
        {
            case ConnectorPosition.left: return ConnectorPosition.right;
            case ConnectorPosition.right: return ConnectorPosition.left;
            case ConnectorPosition.top: return ConnectorPosition.bottom;
            case ConnectorPosition.bottom: return ConnectorPosition.top;
            default: return ConnectorPosition.bottom;
        }
    }

    private void ghostifyModel(Transform modelParent, Material ghostMaterial = null)
    {
        if (ghostMaterial != null)
        {
            foreach (MeshRenderer meshRenderer in modelParent.GetComponentsInChildren<MeshRenderer>())
            {
                meshRenderer.material = ghostMaterial;
            }
        }
        else
        {
            foreach (Collider modelCollider in modelParent.GetComponentsInChildren<Collider>())
            {
                modelCollider.enabled = false;
            }
        }
    }

    private GameObject getCurrentBuild()
    {
        switch (currentBuildType)
        {
            case SelectedBuildType.floor:
                return floorObjects[currentBuildingIndex];
            case SelectedBuildType.wall:
                return wallObjects[currentBuildingIndex];
            case SelectedBuildType.stair:
                return stairObjects[currentBuildingIndex];
            case SelectedBuildType.craftingTable:
                return craftingTableObjects[currentBuildingIndex];
        }
        return null;
    }

    private void placeBuild()
    {
        if (!isGhostInValidPosition)
            return;


        GameObject newBuild = Instantiate(getCurrentBuild(), ghostBuildGameObject.transform.position, ghostBuildGameObject.transform.rotation);

        Destroy(ghostBuildGameObject);
        ghostBuildGameObject = null;
        isBuilding = false;

        foreach (Connector connector in newBuild.GetComponentsInChildren<Connector>())
        {
            connector.updateConnectors(true);
        }

        Debug.Log("Build placed! -10 Wood");
    }
    private void handleVariantRotationInput()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            List<GameObject> currentList = getCurrentBuildList();

            if (currentList == null || currentList.Count == 0)
                return;

            currentBuildingIndex = (currentBuildingIndex + 1) % currentList.Count;
            Debug.Log($"Switched to {currentBuildType} variant: {currentList[currentBuildingIndex].name}");

            // Refresh ghost
            if (ghostBuildGameObject != null)
            {
                Destroy(ghostBuildGameObject);
                ghostBuildGameObject = null;
            }

            if (isBuilding)
            {
                GameObject currentBuild = getCurrentBuild();
                createGhostPrefab(currentBuild);
            }
        }
    }
    private List<GameObject> getCurrentBuildList()
    {
        switch (currentBuildType)
        {
            case SelectedBuildType.floor:
                return floorObjects;
            case SelectedBuildType.wall:
                return wallObjects;
            case SelectedBuildType.stair:
                return stairObjects;
            case SelectedBuildType.craftingTable:
                return craftingTableObjects;
            default:
                return null;
        }
    }
}

[System.Serializable]
public enum SelectedBuildType
{
    floor,
    wall,
    stair,
    craftingTable
}
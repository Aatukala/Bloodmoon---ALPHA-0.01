using UnityEngine;

public class Connector : MonoBehaviour
{
    public ConnectorPosition connectorPosition;
    public SelectedBuildType connectorParentType;

    [HideInInspector] public bool isConnectedToFloor = false;
    [HideInInspector] public bool isConnectedToWall = false;
    [HideInInspector] public bool isConnectedToStair = false;
    [HideInInspector] public bool canConnectTo = true;

    [SerializeField] public bool canConnectToFloor = true;
    [SerializeField] public bool canConnectToWall = true;
    [SerializeField] public bool canConnectToStair = true;

    private void OnDrawGizmos()
    {
        // Visualize connections for debugging
        if (isConnectedToFloor && isConnectedToWall && isConnectedToStair)
            Gizmos.color = Color.red;
        else if (isConnectedToFloor)
            Gizmos.color = Color.blue;
        else if (isConnectedToWall)
            Gizmos.color = Color.yellow;
        else if (isConnectedToStair)
            Gizmos.color = Color.magenta;
        else
            Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(transform.position, transform.lossyScale.x / 2f);
    }

    public void updateConnectors(bool rootCall = false)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, transform.lossyScale.x / 2f);

        isConnectedToFloor = !canConnectToFloor;
        isConnectedToWall = !canConnectToWall;
        isConnectedToStair = !canConnectToStair;

        foreach (Collider collider in colliders)
        {
            if (collider.GetInstanceID() == GetComponent<Collider>().GetInstanceID())
                continue;

            if (collider.gameObject.layer == gameObject.layer)
            {
                Connector foundConnector = collider.GetComponent<Connector>();
                if (!foundConnector) continue;

                switch (foundConnector.connectorParentType)
                {
                    case SelectedBuildType.floor:
                        isConnectedToFloor = true;
                        break;
                    case SelectedBuildType.wall:
                        isConnectedToWall = true;
                        break;
                    case SelectedBuildType.stair:
                        isConnectedToStair = true;
                        break;
                }

                if (rootCall)
                    foundConnector.updateConnectors();
            }
        }

        canConnectTo = true;
        if ((isConnectedToFloor && isConnectedToWall) ||
            (isConnectedToWall && isConnectedToStair) ||
            (isConnectedToFloor && isConnectedToStair))
        {
            canConnectTo = false;
        }
    }
}

[System.Serializable]
public enum ConnectorPosition
{
    left,
    right,
    top,
    bottom,
}
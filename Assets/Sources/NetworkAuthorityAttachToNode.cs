using UnityEngine;
using UnityEngine.Networking;

public class NetworkAuthorityAttachToNode : NetworkBehaviour
{
    public string m_NodeName = "";

    private void Update()
    {
        if (!hasAuthority)
            return;

        var node = GameObject.Find(m_NodeName);
        if (node)
        {
            transform.parent = node.transform;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        enabled = false;
    }
}

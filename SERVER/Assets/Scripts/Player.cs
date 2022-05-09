using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class Player : NetworkBehaviour
{
   
    // Update is called once per frame
    void Update()
    {
        if(isLocalPlayer&& Input.GetKeyDown(KeyCode.X))
        {
            Hola();
        }
        if(isServer && Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("Hello was called");
            Hello();
        }
    }
    [ClientRpc]
    void Hello()
    {
        Debug.Log("Client methid");
    }
    [Command]
    void Hola()
    {
        Debug.Log("Recived fomr client");
    }
}

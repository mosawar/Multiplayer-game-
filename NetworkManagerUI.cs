using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    // buttons to start server, host, and client
    [SerializeField] private Button serverBtn;
    [SerializeField] private Button hostBtn;
    [SerializeField] private Button clientBtn;

    // called when the script instance is being loaded
    private void Awake(){
        // add listenr to the server button to start the server
        serverBtn.onClick.AddListener(() => {
            // start the server
            NetworkManager.Singleton.StartServer();
        });
        // start as host
        hostBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
        });
        // start as client
        clientBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
        });

    }
}

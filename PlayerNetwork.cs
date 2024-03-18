using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour 
{

    // [SerializeField] private Transform spawnedObjectPrefab;
    // [SerializeField] private GameObject playerPrefab;

    // network variable to synchronize custom data across network
    private NetworkVariable<MyCustomData> randomNumber = new NetworkVariable<MyCustomData>(
        new MyCustomData{
            _int = 56,
            _bool = true,
        }, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    // struct to define custom data for network serialization
    public struct MyCustomData : INetworkSerializable {
        public int _int;
        public bool _bool;
        public FixedString128Bytes message;

        // method to serialize custom data for netwokr transmission
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
            serializer.SerializeValue(ref _int);
            serializer.SerializeValue(ref _bool);
            serializer.SerializeValue(ref message);

        }
    }

    // called when the network object is spawned
    public override void OnNetworkSpawn() {
        // subscribe to value change event for the randomNumber network variable
        randomNumber.OnValueChanged += (MyCustomData previousValue, MyCustomData newValue) => {
            Debug.Log(OwnerClientId + "; " + newValue._int + "; " + newValue._bool + "; " + newValue.message);
        };

        // if (IsLocalPlayer){
        //     GameObject playerObject = Instantiate(playerPrefab, transform.position, Quaternion.identity);
        //     playerObject.GetComponent<NetworkObject>().Spawn();
        // }
    }

    // update is called once per frame
    private void Update() {
        // only execute owner specific logic
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.T)){
            // Transform spawnedObjectTransform = Instantiate(spawnedObjectPrefab);
            // spawnedObjectTransform.GetComponent<NetworkObject>().Spawn(true);

            // call a client rpc to trigger an action on specific clients
            TestClientRpc(new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new List<ulong> { 1 } } } );

            // update the value of randomNumber network variable
            randomNumber.Value = new MyCustomData {
                _int = 10,
                _bool = false,
                message = "WOWWWWWWWWWWWWWWW"
            };
        }

        // handle player movement based on input
        Vector3 moveDir = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.W)) moveDir.z = +1f;
        if (Input.GetKey(KeyCode.S)) moveDir.z = -1f;
        if (Input.GetKey(KeyCode.A)) moveDir.x = -1f;
        if (Input.GetKey(KeyCode.D)) moveDir.x = +1f;

        float moveSpeed = 3f;
        transform.position += moveDir * moveSpeed * Time.deltaTime;

    }

    // server rpc method, can onyl be called on the server
    [ServerRpc]
    private void TestServerRpc(ServerRpcParams serverRpcParams) {
        Debug.Log("TestServerRpc " + OwnerClientId + "; " + serverRpcParams.Receive.SenderClientId);
    }

    // client repc method, can only be called on the client
    [ClientRpc]
    private void TestClientRpc(ClientRpcParams clientRpcParams) {
        Debug.Log("TestClientRpc");
    }

}

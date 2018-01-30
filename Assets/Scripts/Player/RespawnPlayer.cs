using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class RespawnPlayer : NetworkBehaviour {


	public void Respawn() {
		CmdRespawnSvr();
	}

	[Command]
	void CmdRespawnSvr(){
		var spawn = NetworkManager.singleton.GetStartPosition();
		var newPlayer = (GameObject) Instantiate(NetworkManager.singleton.playerPrefab, spawn.position, spawn.rotation );
		NetworkServer.Destroy(gameObject);
		NetworkServer.ReplacePlayerForConnection(connectionToClient, newPlayer, playerControllerId);

	}
}

//                                  ┌∩┐(◣_◢)┌∩┐
//																				\\
// ServerManager.cs (00/00/0000)													\\
// Autor: Antonio Mateo (.\Moon Antonio) 	antoniomt.moon@gmail.com			\\
// Descripcion:																	\\
// Fecha Mod:		00/00/0000													\\
// Ultima Mod:																	\\
//******************************************************************************\\

#region Librerias
using UnityEngine;
using UnityEngine.Networking;
#endregion

namespace LvlI
{
	public class ServerManager : MonoBehaviour 
	{
		#region Variables Publicas
		public GameObject playerPrefab;
		public Vector3 playerSpawnPos;
		#endregion

		#region Eventos
		public virtual void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
		{
			var player = (GameObject)GameObject.Instantiate(playerPrefab, playerSpawnPos, Quaternion.identity);
			NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
		}
		#endregion
	}
}

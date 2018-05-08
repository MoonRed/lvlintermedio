//                                  ┌∩┐(◣_◢)┌∩┐
//																				\\
// MotorEnemigo.cs (00/00/0000)													\\
// Autor: Antonio Mateo (.\Moon Antonio) 	antoniomt.moon@gmail.com			\\
// Descripcion:																	\\
// Fecha Mod:		00/00/0000													\\
// Ultima Mod:																	\\
//******************************************************************************\\

#region Librerias
using UnityEngine;
using UnityEngine.AI;
#endregion

namespace LvlI
{
	public class MotorEnemigo : MonoBehaviour
	{
		#region Variables Publicas
		public NavMeshAgent agente;
		public Transform objetivo;
		#endregion

		#region Inicializaciones
		private void Start()
		{
			if (agente == null) agente = this.gameObject.GetComponent<NavMeshAgent>();
		}
		#endregion

		#region Actualizadores
		private void Update()
		{
			agente.SetDestination(objetivo.position);
		}
		#endregion
	}
}
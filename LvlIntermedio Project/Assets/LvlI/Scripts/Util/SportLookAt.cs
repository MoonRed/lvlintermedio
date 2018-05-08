//                                  ┌∩┐(◣_◢)┌∩┐
//																				\\
// SportLookAt.cs (11/04/2018)													\\
// Autor: Antonio Mateo (.\Moon Antonio) 	antoniomt.moon@gmail.com			\\
// Descripcion:		Controlador para fijar la luz en el personaje.				\\
// Fecha Mod:		11/04/2018													\\
// Ultima Mod:		Version Inicial												\\
//******************************************************************************\\

#region Librerias
using UnityEngine;
#endregion

namespace MoonAntonio
{
	public class SportLookAt : MonoBehaviour 
	{
		#region Actualizadore
		private void Update()
		{
			if (GameObject.FindGameObjectWithTag("Player"))
			{
				this.transform.LookAt(GameObject.FindGameObjectWithTag("Player").transform);
			}
		}
		#endregion
	}
}

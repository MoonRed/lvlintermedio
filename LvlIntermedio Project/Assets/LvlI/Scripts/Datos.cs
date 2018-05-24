//                                  ┌∩┐(◣_◢)┌∩┐
//																				\\
// Datos.cs (13/04/2018)														\\
// Autor: Antonio Mateo (.\Moon Antonio) 	antoniomt.moon@gmail.com			\\
// Descripcion:		Explicacion de PlayerPref									\\
// Fecha Mod:		13/04/2018													\\
// Ultima Mod:		Version Inicial												\\
//******************************************************************************\\

#region Librerias
using UnityEngine;
#endregion

namespace LvlI
{
	/// <summary>
	/// <para>Explicacion de PlayerPref.</para>
	/// </summary>
	public class Datos : MonoBehaviour 
	{
		public int valor;
		public bool guardar;

		private void Start()
		{
			if (guardar)
			{
				PlayerPrefs.SetInt("Valor", valor);
			}
			else
			{
				valor = PlayerPrefs.GetInt("Valor");
			}

			Debug.Log("El valor es " + valor);
		}
	}
}

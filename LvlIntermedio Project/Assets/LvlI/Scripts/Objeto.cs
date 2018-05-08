//                                  ┌∩┐(◣_◢)┌∩┐
//																				\\
// Objeto.cs (08/05/2018)														\\
// Autor: Antonio Mateo (.\Moon Antonio) 	antoniomt.moon@gmail.com			\\
// Descripcion:		Scriptables													\\
// Fecha Mod:		08/05/2018													\\
// Ultima Mod:		Version Inicial												\\
//******************************************************************************\\

#region Librerias
using UnityEngine;
#endregion

namespace LvlI
{
	/// <summary>
	/// <para>Scriptables.</para>
	/// </summary>
	[CreateAssetMenu(fileName = "Objeto",menuName = "LvlI/Crear/Objeto")]
	public class Objeto : ScriptableObject
	{
		public string nombre = string.Empty;
		public int valor = 0;
	}
}
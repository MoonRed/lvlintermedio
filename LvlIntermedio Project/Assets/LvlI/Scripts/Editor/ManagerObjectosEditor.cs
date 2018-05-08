//                                  ┌∩┐(◣_◢)┌∩┐
//																				\\
// ManagerObjetosEditor.cs (08/05/2018)											\\
// Autor: Antonio Mateo (.\Moon Antonio) 	antoniomt.moon@gmail.com			\\
// Descripcion:		Herramienta													\\
// Fecha Mod:		08/05/2018													\\
// Ultima Mod:		Version Inicial												\\
//******************************************************************************\\

#region Librerias
using UnityEngine;
using UnityEditor;
#endregion

namespace LvlI
{
	/// <summary>
	/// <para>Herramienta.</para>
	/// </summary>
	public class ManagerObjectosEditor : EditorWindow
	{
		#region Menu
		[MenuItem("Creador/Objeto")]
		public static void Windows()
		{
			GetWindow(typeof(ManagerObjectosEditor), true, "Creador de Objetos");
		}
		#endregion

		#region Eventos
		public void OnEnable()
		{
			minSize = new Vector2(300, 53);
			maxSize = new Vector2(4000, 53);
		}
		#endregion

		#region GUI
		private void OnGUI()
		{
			if (GUILayout.Button("Crear Objeto"))
			{
				AssetDatabase.CreateAsset(new Objeto(), "Assets/LvlI/Resources/Objeto.asset");
			}
		}
		#endregion
	}
}
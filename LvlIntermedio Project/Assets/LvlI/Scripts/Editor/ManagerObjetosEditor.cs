//                                  ┌∩┐(◣_◢)┌∩┐
//																				\\
// ManagerObjetosEditor.cs (16/04/2018)											\\
// Autor: Antonio Mateo (.\Moon Antonio) 	antoniomt.moon@gmail.com			\\
// Descripcion:		Herramienta													\\
// Fecha Mod:		16/04/2018													\\
// Ultima Mod:		Version Inicial												\\
//******************************************************************************\\

#region Librerias
using UnityEngine;
using UnityEditor;
#endregion

namespace LvlI
{
	public class ManagerObjetosEditor : EditorWindow 
	{
		#region Menu
		[MenuItem("Creador/Objeto")]
		public static void Windows()
		{
			// Crear window
			GetWindow(typeof(ManagerObjetosEditor), true, "Creador de Objetos");
		}
		#endregion

		#region Eventos
		public void OnEnable()
		{
			// Obtener valores
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

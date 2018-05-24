//                                  ┌∩┐(◣_◢)┌∩┐
//																				\\
// MotorInspector.cs (11/04/2018)												\\
// Autor: Antonio Mateo (.\Moon Antonio) 	antoniomt.moon@gmail.com			\\
// Descripcion:		Inspector de Motor											\\
// Fecha Mod:		11/04/2018													\\
// Ultima Mod:		Version Inicial												\\
//******************************************************************************\\

#region Librerias
using UnityEngine;
using UnityEditor;
#endregion

namespace LvlI
{
	[CustomEditor(typeof(Motor))]
	public class MotorInspector : Editor
	{
		public override void OnInspectorGUI()
		{
			Motor mtr = (Motor)target;

			base.OnInspectorGUI();

			if (GUILayout.Button("Comprobar"))
			{
				GameObject go = mtr.gameObject;

				if (go.GetComponent<CapsuleCollider>())
				{
					Debug.Log("Este objeto tiene un collider.");
				}

				if (go.GetComponent<Rigidbody>())
				{
					Debug.Log("Este objeto tiene un rigidbody.");
				}

				if (go.GetComponent<Animator>())
				{
					Debug.Log("Este objeto tiene un animator.");
				}
			}
		}
	}
}

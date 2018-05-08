//                                  ┌∩┐(◣_◢)┌∩┐
//																				\\
// Motor.cs (08/05/2018)														\\
// Autor: Antonio Mateo (.\Moon Antonio) 	antoniomt.moon@gmail.com			\\
// Descripcion:		Logica del personaje.										\\
// Fecha Mod:		08/05/2018													\\
// Ultima Mod:		Version Inicial												\\
//******************************************************************************\\

#region Librerias
using UnityEngine;
using UnityEngine.Networking;
#endregion

namespace LvlI
{
	/// <summary>
	/// <para>Logica del personaje.</para>
	/// </summary>
	public class Motor : NetworkBehaviour
	{
		#region Constantes
		private const float SPRINT_VEL = 2.0f;
		private const float SPRINT_FOV = 75.0f;
		private const float NORMAL_FOV = 60.0f;
		#endregion

		#region Variables Privadas Serializadas
		[SerializeField] private Animator animator;
		[SerializeField] private CapsuleCollider capCollider;
		[SerializeField] private float velDireccion = 1.5f;
		[SerializeField] private Transform cam;
		[SerializeField] private float velDampTime = 0.05f;
		[SerializeField] private float fovDampTime = 3f;
		[SerializeField] private float dirDampTime = 0.25f;
		[SerializeField] private float multiSalto = 1f;
		[SerializeField] private float distSalto = 1f;
		[SerializeField] private float rotPorSegundo = 120f;
		#endregion

		#region Variables Privadas
		private float capAlturat;
		private AnimatorStateInfo stateInfo;
		private AnimatorTransitionInfo transInfo;
		private float leftX = 0f;
		private float leftY = 0f;
		private float direccion = 0f;
		private float anguloPersonaje = 0f;
		private float velocidad = 0f;
		#endregion

		#region Variables Protegidas
		protected int locomotionId = 0;
		protected int locomotionPivotLId = 0;
		protected int locomotionPivotRId = 0;
		protected int locomotionPivotLTransId = 0;
		protected int locomotionPivotRTransId = 0;
		#endregion

		#region Propiedades
		public Animator Animator { get { return this.animator; } }
		public float Velocidad { get { return this.velocidad; } }
		public float LocomotionLimite { get { return 0.2f; } }
		#endregion

		#region Inicializadores
		private void Start()
		{
			animator = this.GetComponent<Animator>();
			capCollider = this.GetComponent<CapsuleCollider>();
			capAlturat = capCollider.height;

			if (animator.layerCount >= 2)
			{
				animator.SetLayerWeight(1, 1);
			}

			// Hash todos los nombres de animacion para el rendimiento
			locomotionId = Animator.StringToHash("Base Layer.Locomotion");
			locomotionPivotLId = Animator.StringToHash("Base Layer.LocomotionPivotL");
			locomotionPivotRId = Animator.StringToHash("Base Layer.LocomotionPivotR");
			locomotionPivotLTransId = Animator.StringToHash("Base Layer.Locomotion -> Base Layer.LocomotionPivotL");
			locomotionPivotRTransId = Animator.StringToHash("Base Layer.Locomotion -> Base Layer.LocomotionPivotR");
		}
		#endregion

		#region Actualizadores
		private void Update()
		{
			if (!isLocalPlayer)
				return;

			// Si hay un animator
			if (animator)
			{
				// Recogemos la info del animator
				stateInfo = animator.GetCurrentAnimatorStateInfo(0);
				transInfo = animator.GetAnimatorTransitionInfo(0);
			}

			// Evento Saltar
			if (Input.GetButton("Jump"))
			{
				animator.SetBool("Jump", true);
			}
			else
			{
				animator.SetBool("Jump", false);
			}

			// Obtener los valores del controlador/teclado
			leftX = Input.GetAxis("Horizontal");
			leftY = Input.GetAxis("Vertical");

			anguloPersonaje = 0f;
			direccion = 0f;
			float velPersonaje = 0f;

			// Traducir controles de stick en coordenadas del mundo/camara/personaje
			StickLogica(this.transform, cam, ref direccion, ref velPersonaje, ref anguloPersonaje, IsPivotando());

			// Evento Sprint
			/*if (Input.GetButton("Sprint"))
			{
				velocidad = Mathf.Lerp(velocidad, SPRINT_VEL, Time.deltaTime);
				cam.GetComponent<Camera>().fieldOfView = Mathf.Lerp(cam.GetComponent<Camera>().fieldOfView, SPRINT_FOV, fovDampTime * Time.deltaTime);
			}
			else
			{
				velocidad = velPersonaje;
				cam.GetComponent<Camera>().fieldOfView = Mathf.Lerp(cam.GetComponent<Camera>().fieldOfView, NORMAL_FOV, fovDampTime * Time.deltaTime);
			}*/

			// Forzar la direccion y velocidad del personaje
			animator.SetFloat("Speed", velocidad, velDampTime, Time.deltaTime);
			animator.SetFloat("Direction", direccion, dirDampTime, Time.deltaTime);

			// Zona Muerta
			if (velocidad > LocomotionLimite)
			{
				if (!IsPivotando())
				{
					Animator.SetFloat("Angle", anguloPersonaje);
				}
			}

			if (velocidad < LocomotionLimite && Mathf.Abs(leftX) < 0.05f)
			{
				animator.SetFloat("Direction", 0f);
				animator.SetFloat("Angle", 0f);
			}
		}

		private void FixedUpdate()
		{

			if (!isLocalPlayer)
				return;

			// Girar el modelo si la palanca esta inclinada hacia la derecha o izquierda, pero solo si el personaje se mueve en esa direccion
			if (IsInLocomotion() && !IsPivotando() && ((direccion >= 0 && leftX >= 0) || (direccion < 0 && leftX < 0)))
			{
				Vector3 rot = Vector3.Lerp(Vector3.zero, new Vector3(0f, rotPorSegundo * (leftX < 0f ? -1f : 1f), 0f), Mathf.Abs(leftX));
				Quaternion rotDelta = Quaternion.Euler(rot * Time.deltaTime);
				this.transform.rotation = (this.transform.rotation * rotDelta);
			}

			// Mientras el personaje salte, agregar el translado
			if (IsInJump())
			{
				float oldY = transform.position.y;
				transform.Translate(Vector3.up * multiSalto * animator.GetFloat("JumpCurve"));
				if (IsInLocomotionJump())
				{
					transform.Translate(Vector3.forward * Time.deltaTime * distSalto);
				}
				capCollider.height = capAlturat + (animator.GetFloat("CapsuleCurve") * 0.5f);

				cam.transform.Translate(Vector3.up * (transform.position.y - oldY));
			}
		}
		#endregion

		#region Metodos Publicos
		public void StickLogica(Transform root,Transform camara, ref float dir, ref float vel, ref float angulo, bool isGirando)
		{
			// Obtener valores
			Vector3 rootDir = root.forward;
			Vector3 strickDir = new Vector3(leftX, 0, leftY);
			vel = strickDir.sqrMagnitude;

			// Obtener rotacion de camara (Matar Y)
			Vector3 camaraDir = camara.forward;
			camaraDir.y = 0.0f;
			Quaternion refAux = Quaternion.FromToRotation(Vector3.forward, Vector3.Normalize(camaraDir));

			// Convertir input del joystick en las coordenadas del mundo
			Vector3 movDir = refAux * strickDir;
			Vector3 axisAux = Vector3.Cross(movDir, rootDir);

			// Dibujar los debugs
			Debug.DrawRay(new Vector3(root.position.x, root.position.y + 2f, root.position.z), movDir, Color.green);
			Debug.DrawRay(new Vector3(root.position.x, root.position.y + 2f, root.position.z), rootDir, Color.magenta);
			Debug.DrawRay(new Vector3(root.position.x, root.position.y + 2f, root.position.z), strickDir, Color.blue);
			Debug.DrawRay(new Vector3(root.position.x, root.position.y + 2.5f, root.position.z), axisAux, Color.red);

			// Modificar angulo
			float anguloRoot = Vector3.Angle(rootDir, movDir) * (axisAux.y >= 0 ? -1f : 1f);
			if (!isGirando)
			{
				angulo = anguloRoot;
			}
			anguloRoot /= 180f;

			dir = anguloRoot * velDireccion;
		}
		#endregion

		#region Funcionalidad
		public bool IsPivotando()
		{
			return stateInfo.fullPathHash == locomotionPivotLId ||
				stateInfo.fullPathHash == locomotionPivotRId ||
				transInfo.nameHash == locomotionPivotLTransId ||
				transInfo.nameHash == locomotionPivotRTransId;
		}

		public bool IsInJump()
		{
			return (IsInIdleJump() || IsInLocomotionJump());	
		}

		public bool IsInIdleJump()
		{
			return animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.IdleJump");
		}

		public bool IsInLocomotionJump()
		{
			return animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.LocomotionJump");
		}

		public bool IsInLocomotion()
		{
			return stateInfo.fullPathHash == locomotionId;
		}
		#endregion
	}
}
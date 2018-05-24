//                                  ┌∩┐(◣_◢)┌∩┐
//																				\\
// Camara.cs (11/04/2018)														\\
// Autor: Antonio Mateo (.\Moon Antonio) 	antoniomt.moon@gmail.com			\\
// Descripcion:		Controlador de la camara.									\\
// Fecha Mod:		11/04/2018													\\
// Ultima Mod:		Version Inicial												\\
//******************************************************************************\\

#region Librerias
using UnityEngine;
using UnityEngine.Networking;
using MoonAntonio;
#endregion

namespace LvlI
{
	/// <summary>
	/// <para>Controlador de la camara.</para>
	/// </summary>
	[RequireComponent(typeof(BarsEffect))]
	public class Camara : NetworkBehaviour
	{
		#region Variables Privadas Serializadas
		[SerializeField] private Transform camara;
		[SerializeField] private Motor motor;
		[SerializeField] private Transform personaje;
		[SerializeField] private float distanciaArriba;
		[SerializeField] private float distanciaLejana;
		[SerializeField] private float distanciaArribaAux = 5f;
		[SerializeField] private float distanciaLejanaAux = 1.5f;
		[SerializeField] private float sensibilidad = 3.0f;
		[SerializeField] private float tiempoLookAt = 0.1f;
		#endregion

		#region Variables Privadas
		private Vector3 dirFija;
		private Vector3 actualDirFija;
		private BarsEffect barEffect;
		private PosicionCamara camPos;
		private Vector3 offSetPersonaje;
		private float distLejanaLibre;
		private float distArribaLibre;
		private Vector3 dirRigData;
		private Vector3[] vistaDebug;
		private Vector3 dimension = Vector3.zero;
		private Vector3 posicionTarget;
		private float fuerzaLook;
		private Vector3 velActualCam = Vector3.zero;
		private Vector3 velTransicionCam = Vector3.zero;
		private float tiempoTransicionCam = 0.1f;
		#endregion

		#region Propiedades
		public Vector3 DirRig
		{
			get
			{
				// Mueva la altura y la distancia del personaje por separado, ya que RotateAround tiene control de posicion y rotacion
				Vector3 value = Vector3.Normalize(offSetPersonaje - this.transform.position);
				// No se puede calcular la distancia desde un vector con la rotacion del eje Y
				value.y = 0f;

				return value;
			}
		}

		public Motor Motor
		{
			set { motor = value; }
		}

		public Transform Personaje
		{
			set { personaje = value; }
		}
		#endregion

		#region Inicializadores
		private void Start()
		{
			camara = this.transform;
			if (camara == null)
			{
				Debug.LogError("No existe camara en el objeto.", this);
			}

			motor = GetComponentInParent<Motor>();
			personaje = motor.transform;

			dirFija = personaje.forward;
			actualDirFija = personaje.forward;

			barEffect = GetComponent<BarsEffect>();
			if (barEffect == null)
			{
				Debug.LogError("Agrega el efecto BarsEffect al objeto.", this);
			}

			camPos = new PosicionCamara();
			camPos.Init("Camara" + netId, new Vector3(0.0f, 1.6f, 0.2f), new GameObject().transform, motor.transform);

			// Inicializa valores
			offSetPersonaje = personaje.position + new Vector3(0f, distanciaArriba, 0f);
			distArribaLibre = distanciaArriba;
			distLejanaLibre = distanciaLejana;
			dirRigData = DirRig;
		}
		#endregion

		#region Actualizadores
		private void Update()
		{
			if (motor == null) motor = GetComponentInParent<Motor>(); Setup();
			if(personaje == null) personaje = motor.transform; Setup();
		}

		private void LateUpdate()
		{
			vistaDebug = DebugDraw.CalculateViewFrustum(GetComponent<Camera>(), ref dimension);

			// Obtener valores de controller/teclado
			float rightX = Input.GetAxis("RightStickX");
			float rightY = Input.GetAxis("RightStickY");
			float leftX = Input.GetAxis("Horizontal");
			float leftY = Input.GetAxis("Vertical");
			float mouseRuleta = Input.GetAxis("Mouse ScrollWheel");
			float mouseRuletaEscala = mouseRuleta * sensibilidad;
			float leftTrigger = Input.GetAxis("Target");
			bool btnB = Input.GetButton("ExitFPV");
			bool keyQ = Input.GetKey(KeyCode.Q);
			bool keyE = Input.GetKey(KeyCode.E);
			bool keyShift = Input.GetKey(KeyCode.LeftShift);

			// Abstraccion para establecer Y correctamente al usar el raton
			if (mouseRuleta != 0)
			{
				rightY = mouseRuletaEscala;
			}
			if (keyQ)
			{
				rightX = 1;
			}
			if (keyE)
			{
				rightX = -1;
			}
			if (keyShift)
			{
				leftTrigger = 1;
			}

			offSetPersonaje = personaje.position + (distanciaArriba * personaje.up);
			Vector3 lookAt = offSetPersonaje;
			posicionTarget = Vector3.zero;

			// Establece el valor del peso
			motor.Animator.SetLookAtWeight(fuerzaLook);

			// Logica

			// Reseteo de camara
			ResetCamara();

			// Solo actualice la direccion de la camara si se mueve
			if (motor.Velocidad > motor.LocomotionLimite && motor.IsInLocomotion() && !motor.IsPivotando())
			{
				dirFija = Vector3.Lerp(personaje.right * (leftX < 0 ? 1f : -1f), personaje.forward * (leftY < 0 ? -1f : 1f), Mathf.Abs(Vector3.Dot(this.transform.forward, personaje.forward)));
				Debug.DrawRay(this.transform.position, dirFija, Color.white);

				// Calcula la direccion de la camara al jugador, mata Y y normaliza para dar una direccion valida con la magnitud de la unidad
				actualDirFija = Vector3.Normalize(offSetPersonaje - this.transform.position);
				actualDirFija.y = 0;
				Debug.DrawRay(this.transform.position, actualDirFija, Color.green);

				// La interpolacion
				actualDirFija = Vector3.SmoothDamp(actualDirFija, dirFija, ref velActualCam, tiempoLookAt);
			}

			posicionTarget = offSetPersonaje + personaje.up * distanciaArriba - Vector3.Normalize(actualDirFija) * distanciaLejana;
			Debug.DrawLine(personaje.position, posicionTarget, Color.magenta);

			CompensarColisiones(offSetPersonaje, ref posicionTarget);
			TransicionPosicion(camara.position, posicionTarget);
			transform.LookAt(lookAt);
		}
		#endregion

		#region Metodos
		/// <summary>
		/// <para>Restablece la posicion local de la camara.</para>
		/// </summary>
		private void ResetCamara()
		{
			fuerzaLook = Mathf.Lerp(fuerzaLook, 0.0f, Time.deltaTime * 3.0f);
			transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.identity, Time.deltaTime);
		}

		/// <summary>
		/// <para>Transicion de la camara.</para>
		/// </summary>
		/// <param name="fromPos"></param>
		/// <param name="toPos"></param>
		private void TransicionPosicion(Vector3 posActual, Vector3 posDestino)
		{
			// Haciendo una transicion suave entre la posicion actual de la camara y la posicion en la que quiere estar
			camara.position = Vector3.SmoothDamp(posActual, posDestino, ref velTransicionCam, tiempoTransicionCam);
		}

		/// <summary>
		/// <para>Compensa la camara para que no ocurran colisiones.</para>
		/// </summary>
		/// <param name="fromObject"></param>
		/// <param name="toTarget"></param>
		private void CompensarColisiones(Vector3 fromObject, ref Vector3 toTarget)
		{
			// Compensar las paredes entre la camara
			RaycastHit hitCollider = new RaycastHit();
			if (Physics.Linecast(fromObject, toTarget, out hitCollider))
			{
				Debug.DrawRay(hitCollider.point, hitCollider.normal, Color.red);
				toTarget = hitCollider.point;
			}

			// Compensar geo
			Vector3 camPosCache = GetComponent<Camera>().transform.position;
			GetComponent<Camera>().transform.position = toTarget;
			vistaDebug = DebugDraw.CalculateViewFrustum(GetComponent<Camera>(), ref dimension);

			for (int i = 0; i < (vistaDebug.Length / 2); i++)
			{
				RaycastHit cWHit = new RaycastHit();
				RaycastHit cCWHit = new RaycastHit();

				// Reparte lineas en ambas direcciones alrededor de los limites del plano de recorte
				while (Physics.Linecast(vistaDebug[i], vistaDebug[(i + 1) % (vistaDebug.Length / 2)], out cWHit) ||
					   Physics.Linecast(vistaDebug[(i + 1) % (vistaDebug.Length / 2)], vistaDebug[i], out cCWHit))
				{
					Vector3 normal = hitCollider.normal;
					if (hitCollider.normal == Vector3.zero)
					{
						// Si no hay hitCollider disponible, usa la geometria
						if (cWHit.normal == Vector3.zero)
						{
							if (cCWHit.normal == Vector3.zero)
							{
								Debug.LogError("No hay geometria disponible cerca del plano de LineCasts.", this);
							}
							else
							{
								normal = cCWHit.normal;
							}
						}
						else
						{
							normal = cWHit.normal;
						}
					}

					toTarget += (0.2f * normal);
					GetComponent<Camera>().transform.position += toTarget;

					// Recalcular posicion
					vistaDebug = DebugDraw.CalculateViewFrustum(GetComponent<Camera>(), ref dimension);
				}
			}

			GetComponent<Camera>().transform.position = camPosCache;
			vistaDebug = DebugDraw.CalculateViewFrustum(GetComponent<Camera>(), ref dimension);
		}

		private void Setup()
		{
			dirFija = personaje.forward;
			actualDirFija = personaje.forward;

			camPos = new PosicionCamara();
			camPos.Init("Camara", new Vector3(0.0f, 1.6f, 0.2f), new GameObject().transform, motor.transform);

			// Inicializa valores
			offSetPersonaje = personaje.position + new Vector3(0f, distanciaArriba, 0f);
		}
		#endregion

		/// <summary>
		/// <para>Estructura para alinear la camara.</para>
		/// </summary>
		struct PosicionCamara
		{
			private Vector3 posicionCamara;
			private Transform rotacionCamara;

			public Vector3 Position { get { return posicionCamara; } set { posicionCamara = value; } }
			public Transform XForm { get { return rotacionCamara; } set { rotacionCamara = value; } }

			public void Init(string camName, Vector3 pos, Transform transform, Transform parent)
			{
				posicionCamara = pos;
				rotacionCamara = transform;
				rotacionCamara.name = camName;
				rotacionCamara.parent = parent;
				rotacionCamara.localPosition = Vector3.zero;
				rotacionCamara.localPosition = posicionCamara;
			}
		}
	}
}

using UnityEngine;
using System.Collections;

namespace SvenFrankson.Game.SpaceExplorer {

	public class Humanoid : MonoBehaviour {

		public enum HumanoidState {
			Stand,
			PilotMotherShip,
			PilotLightShip
		}

		public HumanoidState state = HumanoidState.Stand;
		public SpaceShip spaceShip = null;
		public LightShip lightShip = null;
		public Transform seat = null;

		public float speed = 1f;
		public float rotateXSpeed = 10f;
		public float rotateYSpeed = 10f;
		public bool ReverseYAxis = false;
		private int reverseY = -1;

		public Transform head;

		public Gravity currentGrav;

		// Cached Components
		private Transform cTransform;
		private Rigidbody cRigidbody;

		void Start () {
			Screen.showCursor = false;

			this.cTransform = this.GetComponent<Transform> ();
			if (ReverseYAxis) {
				this.reverseY = 1;			
			}
			this.cRigidbody = this.GetComponent<Rigidbody> ();
		}

		public void FixedUpdate () {
			if (this.currentGrav != null) {
				if (this.currentGrav.GType == Gravity.GravityType.Cartesian) {
					this.cRigidbody.AddForce (- this.cTransform.parent.up * this.currentGrav.gravity);
					float a = Vector3.Angle (this.cTransform.up, this.cTransform.parent.up);
					if (a > 1f) {
						this.transform.RotateAround (this.cTransform.position, Vector3.Cross (this.cTransform.up, this.cTransform.parent.up), a * Time.deltaTime);
					}
				}
				else if (this.currentGrav.GType == Gravity.GravityType.Spherical) {
					this.cRigidbody.AddForce (- (this.cTransform.position - this.cTransform.parent.position).normalized * this.currentGrav.gravity);
					float a = Vector3.Angle (this.cTransform.up, (this.cTransform.position - this.cTransform.parent.position));
					if (a > 1f) {
						this.transform.RotateAround (this.cTransform.position, Vector3.Cross (this.cTransform.up, (this.cTransform.position - this.cTransform.parent.position)), a * Time.deltaTime);
					}
				}
			}
		}
		
		void Update () {
			float mouseX = Input.GetAxis ("Mouse X");
			float mouseY = Input.GetAxis ("Mouse Y");

			if (this.state == HumanoidState.Stand) {
				this.cTransform.Rotate (0f, mouseX * this.rotateXSpeed, 0f);
				this.head.Rotate (reverseY * mouseY * this.rotateXSpeed, 0f, 0f);
			}
			else if (this.state == HumanoidState.PilotMotherShip) {
				this.cTransform.localPosition = this.seat.localPosition;
				this.cTransform.localRotation = this.seat.localRotation;
				this.head.Rotate (0f, mouseX * this.rotateXSpeed, 0f);
				this.head.Rotate (reverseY * mouseY * this.rotateYSpeed, 0f, 0f);
				this.head.LookAt (this.head.position + this.head.forward, this.cTransform.up);
			}
			else if (this.state == HumanoidState.PilotLightShip) {
				this.cTransform.localPosition = this.seat.localPosition;
				this.cTransform.localRotation = this.seat.localRotation;
				this.head.Rotate (0f, mouseX * this.rotateXSpeed, 0f);
				this.head.Rotate (reverseY * mouseY * this.rotateYSpeed, 0f, 0f);
				this.head.LookAt (this.head.position + this.head.forward, this.cTransform.up);
			}

			if (this.state == HumanoidState.PilotMotherShip) {
				if (this.spaceShip != null) {
					this.spaceShip.engineOn = false;
					this.spaceShip.pitchOn = 0;
					this.spaceShip.rollOn = 0;
				}
			}
			else if (this.state == HumanoidState.PilotLightShip) {
				if (this.lightShip != null) {
					this.lightShip.powerOn = false;
					this.lightShip.pitchOn = 0;
					this.lightShip.rollOn = 0;
				}
			}

			if (Input.GetKey (KeyCode.Z)) {
				if (this.state == HumanoidState.Stand) {
					this.cTransform.position += this.transform.forward * this.speed * Time.deltaTime;
				}
				else if (this.state == HumanoidState.PilotMotherShip) {
					if (this.spaceShip != null) {
						this.spaceShip.pitchOn = 1;
					}
				}
				else if (this.state == HumanoidState.PilotLightShip) {
					if (this.lightShip != null) {
						this.lightShip.pitchOn = 1;
					}
				}
			}
			
			if (Input.GetKey (KeyCode.S)) {
				if (this.state == HumanoidState.Stand) {
					this.cTransform.position += - this.transform.forward * this.speed * Time.deltaTime;
				}
				else if (this.state == HumanoidState.PilotMotherShip) {
					this.spaceShip.pitchOn = -1;
				}
				else if (this.state == HumanoidState.PilotLightShip) {
					this.lightShip.pitchOn = -1;
				}
			}
			
			if (Input.GetKey (KeyCode.Q)) {
				if (this.state == HumanoidState.Stand) {
					this.cTransform.position += - this.transform.right * this.speed * Time.deltaTime;
				}
				else if (this.state == HumanoidState.PilotMotherShip) {
					if (this.spaceShip != null) {
						this.spaceShip.rollOn = 1;
					}
				}
				else if (this.state == HumanoidState.PilotLightShip) {
					if (this.lightShip != null) {
						this.lightShip.rollOn = 1;
					}
				}
			}
			
			if (Input.GetKey (KeyCode.D)) {
				if (this.state == HumanoidState.Stand) {
					this.cTransform.position += this.transform.right * this.speed * Time.deltaTime;
				}
				else if (this.state == HumanoidState.PilotMotherShip) {
					if (this.spaceShip != null) {
						this.spaceShip.rollOn = -1;
					}
				}
				else if (this.state == HumanoidState.PilotLightShip) {
					if (this.lightShip != null) {
						this.lightShip.rollOn = -1;
					}
				}
			}
			
			if (Input.GetKey (KeyCode.A)) {
				if (this.state == HumanoidState.Stand) {

				}
				else if (this.state == HumanoidState.PilotMotherShip) {
					if (this.spaceShip != null) {

					}
				}
				else if (this.state == HumanoidState.PilotLightShip) {
					if (this.lightShip != null) {

					}
				}
			}
			
			if (Input.GetKey (KeyCode.E)) {
				if (this.state == HumanoidState.Stand) {

				}
				else if (this.state == HumanoidState.PilotMotherShip) {
					if (this.spaceShip != null) {

					}
				}
				else if (this.state == HumanoidState.PilotLightShip) {
					if (this.lightShip != null) {

					}
				}
			}
			
			if (Input.GetKey (KeyCode.Space)) {
				if (this.state == HumanoidState.Stand) {

				}
				else if (this.state == HumanoidState.PilotMotherShip) {
					if (this.spaceShip != null) {
						this.spaceShip.engineOn = true;
					}
				}
				else if (this.state == HumanoidState.PilotLightShip) {
					if (this.lightShip != null) {
						this.lightShip.powerOn = true;
					}
				}
			}

			if (Input.GetKeyDown (KeyCode.Space)) {
				if (this.state == HumanoidState.Stand) {
					this.cRigidbody.AddForce (this.cTransform.up * 5f,ForceMode.Impulse);
				}
			}
			
			if (Input.GetKeyDown (KeyCode.LeftAlt)) {
				if (this.state == HumanoidState.Stand) {
					
				}
				else if (this.state == HumanoidState.PilotMotherShip) {
					this.LeaveControl ();
				}
				else if (this.state == HumanoidState.PilotLightShip) {
					if (this.lightShip.state == LightShip.LightShipState.Flying) {
						this.lightShip.Park ();
					}
					else if ((this.lightShip.state == LightShip.LightShipState.Parked) || (this.lightShip.state == LightShip.LightShipState.Landed)) {
						this.LeaveControl ();
					}
				}
			}
			
			if (Input.GetKeyDown (KeyCode.X)) {
				if (this.state == HumanoidState.Stand) {
					
				}
				else if (this.state == HumanoidState.PilotMotherShip) {
					this.LeaveControl ();
				}
				else if (this.state == HumanoidState.PilotLightShip) {
					this.lightShip.SwitchEngine();
				}
			}
			
			if (Input.GetMouseButtonDown (0)) {
				this.Interact ();
			}
		}

		private void Interact () {
			Ray checkAction = new Ray (head.position, head.forward);
			RaycastHit hitInfo = new RaycastHit ();
			Physics.Raycast (checkAction, out hitInfo, 2f);

			if (hitInfo.collider != null) {
				if (hitInfo.collider.gameObject != null) {
					Interactable target = hitInfo.collider.gameObject.GetComponent<Interactable> ();
					if (target == null) {
						if (hitInfo.collider.gameObject.transform.parent != null) {
							target = hitInfo.collider.gameObject.transform.parent.GetComponent<Interactable> ();
						}
					}
					if (target != null) {
						target.OnInteraction (this);
					}
				}
			}
		}

		public void TakeControl (SpaceShip spaceShip, Transform seat) {
			this.seat = seat;
			this.spaceShip = spaceShip;
			this.state = HumanoidState.PilotMotherShip;
		}
		
		public void TakeControl (LightShip lightShip, Transform seat) {
			this.seat = seat;
			this.lightShip = lightShip;
			this.cTransform.parent = this.lightShip.transform;
			this.rigidbody.isKinematic = true;
			this.collider.enabled = false;
			this.state = HumanoidState.PilotLightShip;
		}
		
		public void LeaveControl () {
			if (this.spaceShip != null) {
				this.spaceShip.engineOn = false;
				this.spaceShip.pitchOn = 0;
				this.spaceShip.rollOn = 0;
			}

			if (this.lightShip != null) {
				this.lightShip.engineOn = false;
				this.lightShip.pitchOn = 0;
				this.lightShip.rollOn = 0;
				this.transform.parent = this.lightShip.transform.parent;
				this.rigidbody.isKinematic = false;
				this.collider.enabled = true;
				this.state = HumanoidState.Stand;
				this.currentGrav = this.transform.parent.GetComponent<Gravity> ();
			}

			this.rigidbody.isKinematic = false;
			this.collider.enabled = true;

			this.seat = null;
			this.spaceShip = null;
			this.lightShip = null;
			this.head.localRotation = Quaternion.identity;
			this.state = HumanoidState.Stand;
		}
	}
}

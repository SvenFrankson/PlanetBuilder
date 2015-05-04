using UnityEngine;
using System.Collections;

namespace SvenFrankson.Game.SpaceExplorer {

	public class LightShip : SpaceShip {

		public enum LightShipState {
			Flying,
			Parking,
			Parked,
			Landing,
			Landed
		}

		private Rigidbody cRigidbody;

		public LightShipState state;
		public ParkingSpot parkingSpot = null;
		public MotherShip motherShip = null;

		public bool powerOn = false;
		
		public Gravity currentGrav;
		
		public override void Start () {
			base.Start ();
			this.cRigidbody = this.GetComponent<Rigidbody> ();
		}

		public void OnGUI () {
			//GUILayout.TextField ("LightShip State = " + this.state);
		}

		public void FixedUpdate () {
			
			if (this.state == LightShipState.Flying) {
				if (this.engineOn) {
					this.cRigidbody.AddForce (this.cTransform.forward * this.enginePower / 2f);
				}
				else {
					this.cRigidbody.AddForce (this.cTransform.forward * this.enginePower / 8f);
				}

				if (this.powerOn) {
					this.engineOn = true;
					this.cRigidbody.AddForce (this.cTransform.forward * this.enginePower);
				}
				
				if (this.rollOn != 0) {
					this.cRigidbody.AddTorque (this.rollOn * this.cTransform.forward * this.rollPower);
				}
				
				if (this.pitchOn != 0) {
					this.cRigidbody.AddTorque (this.pitchOn * this.cTransform.right * this.pitchPower);
				}

				this.speed = this.cRigidbody.velocity.magnitude;
			}
			
			if (this.state == LightShipState.Parked) {
				if (this.powerOn) {
					this.TakeOff ();
				}
			}
			
			if (this.state == LightShipState.Parking) {
				if (this.powerOn) {
					this.cTransform.parent = null;
					this.state = LightShipState.Flying;
				}

				if (this.parkingSpot != null) {
					this.cRigidbody.AddForce ((this.parkingSpot.transform.position - this.cTransform.position).normalized);
					this.cRigidbody.MoveRotation (Quaternion.Lerp (this.cTransform.rotation, this.parkingSpot.transform.rotation, Time.fixedDeltaTime));

					if ((this.cTransform.position - this.parkingSpot.transform.position).sqrMagnitude < 0.1f) {
						if (Vector3.Angle (this.cTransform.up, this.parkingSpot.transform.up) < 1f) {
							this.DockOnShip ();
						}
					}
				}
			}
			
			if (this.state == LightShipState.Landed) {
				if (this.powerOn) {
					this.TakeOff ();
				}
			}
			
			if (this.state == LightShipState.Landing) {
				if (this.powerOn) {
					this.cTransform.parent = null;
					this.currentGrav = null;
					this.state = LightShipState.Flying;
				}

				if (this.rollOn != 0) {
					this.cRigidbody.AddTorque (this.rollOn * this.cTransform.forward * this.rollPower);
				}
				
				if (this.pitchOn != 0) {
					this.cRigidbody.AddTorque (this.pitchOn * this.cTransform.right * this.pitchPower);
				}
				
				if (this.currentGrav != null) {
					if (this.currentGrav.GType == Gravity.GravityType.Cartesian) {
						this.cRigidbody.AddForce (- this.cTransform.parent.up * this.currentGrav.gravity / 5f);
					}
					else if (this.currentGrav.GType == Gravity.GravityType.Spherical) {
						this.cRigidbody.AddForce (- (this.cTransform.position - this.cTransform.parent.position).normalized * this.currentGrav.gravity / 5f);
					}
				}

				if (this.cRigidbody.velocity.sqrMagnitude < 0.05f) {
					this.DockOnGround ();
				}
			}
		}

		public void SwitchEngine () {
			this.engineOn = !this.engineOn;
		}

		public void TakeOff () {
			this.parkingSpot = null;
			this.state = LightShipState.Flying;
			this.cRigidbody.isKinematic = false;
			if (this.motherShip != null) {
				this.cRigidbody.velocity = this.motherShip.GetVelocity ();
			}
			this.cTransform.parent = null;
			this.motherShip = null;
			this.currentGrav = null;
		}

		public void Park () {
			if (this.parkingSpot != null) {
				this.state = LightShipState.Parking;
				this.cTransform.parent = this.parkingSpot.transform.parent;
			}
			else {
				this.Land ();
			}
		}

		private void DockOnShip () {
			this.state = LightShipState.Parked;
			this.cRigidbody.isKinematic = true;
			this.motherShip = this.parkingSpot.transform.parent.GetComponent<MotherShip> ();
			this.cTransform.parent = this.motherShip.transform;
		}

		public void Land () {
			Ray checkAction = new Ray (this.cTransform.position, - this.cTransform.up);
			RaycastHit hitInfo = new RaycastHit ();
			Physics.Raycast (checkAction, out hitInfo, 4f);
			
			if (hitInfo.collider != null) {
				if (hitInfo.collider.gameObject != null) {
					Gravity target = hitInfo.collider.gameObject.GetComponent<Gravity> ();
					if (target == null) {
						if (hitInfo.collider.gameObject.transform.parent != null) {
							target = hitInfo.collider.gameObject.transform.parent.GetComponent<Gravity> ();
							if (target == null) {
								if (hitInfo.collider.gameObject.transform.parent.parent != null) {
									target = hitInfo.collider.gameObject.transform.parent.parent.GetComponent<Gravity> ();
								}
							}
						}
					}
					if (target != null) {
						this.currentGrav = target;
						this.cTransform.parent = this.currentGrav.transform;
						this.state = LightShipState.Landing;
					}
				}
			}
		}
		
		private void DockOnGround () {
			this.state = LightShipState.Landed;
			this.cRigidbody.isKinematic = true;
		}

		public void OnTriggerEnter (Collider trigger) {
			ParkingSpot parkingSpot = trigger.GetComponent<ParkingSpot> ();
			if (parkingSpot != null) {
				if (this.parkingSpot == null) {
					this.parkingSpot = parkingSpot;
				}
			}
		}

		public void OnTriggerExit (Collider trigger) {
			if (!(this.state == LightShipState.Parking)) {
				ParkingSpot parkingSpot = trigger.GetComponent<ParkingSpot> ();
				if (parkingSpot != null) {
					if (this.parkingSpot == parkingSpot) {
						this.parkingSpot = null;
					}
				}
			}
		}
	}
}

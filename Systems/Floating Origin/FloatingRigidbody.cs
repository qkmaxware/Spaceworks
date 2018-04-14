using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaceworks.Position {

	[RequireComponent(typeof(Rigidbody))]
	public class FloatingRigidbody : FloatingTransform {

		public bool isKinematic = false;
		private Vector3 cachedVelocity;
		private Vector3 cachedAngularVelocity;

		/// <summary>
		/// Reference to the attached rigidbody
		/// </summary>
		/// <returns></returns>
		public Rigidbody body{
			get{
				if(!_body)
					_body = GetComponent<Rigidbody>();
				return _body;
			}
		}
		private Rigidbody _body;

		/// <summary>
		/// Store the current velocities from the attached rigidbody
		/// </summary>
		public void Cache(){
			cachedVelocity = body.velocity;
			cachedAngularVelocity = body.angularVelocity;
		}

		/// <summary>
		/// Retrieve and set the velocities of the attached rigidbody from the cached values
		/// </summary>
		public void Decache(){
			body.velocity = cachedVelocity;
			body.angularVelocity = cachedAngularVelocity;
		}

		/// <summary>
		/// Enable or disable a rigidbody
		/// </summary>
		/// <param name="enable"></param>
		public void EnableRigidBody(bool enable){
			body.detectCollisions = enable;
			body.isKinematic = (!enable ? true : (isKinematic));
		}

		public override void OnOriginEnter(WorldPosition sceneCenter){
			Decache();
			EnableRigidBody(true);
		}

		public override void OnOriginExit(WorldPosition sceneCenter){
			Cache();
			EnableRigidBody(false);
		}

	}

}
/*
  RaycastVehicle (for Bullet for XNA) Copyright (c) 2007 William Denniss
  Bullet for XNA Copyright (c) 2003-2007 Vsevolod Klementjev http://www.codeplex.com/xnadevru
  Bullet original C++ version Copyright (c) 2003-2007 Erwin Coumans http://bulletphysics.com

  This software is provided 'as-is', without any express or implied
  warranty.  In no event will the authors be held liable for any damages
  arising from the use of this software.

  Permission is granted to anyone to use this software for any purpose,
  including commercial applications, and to alter it and redistribute it
  freely, subject to the following restrictions:

  1. The origin of this software must not be misrepresented; you must not
     claim that you wrote the original software. If you use this software
     in a product, an acknowledgment in the product documentation would be
     appreciated but is not required.
  2. Altered source versions must be plainly marked as such, and must not be
     misrepresented as being the original software.
  3. This notice may not be removed or altered from any source distribution.
*/

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using R3D.MathLib;
using R3D.Collections;

namespace R3D.PhysicsEngine.Dynamics
{
	/// <summary>
	/// RayCast vehicle, very special constraint that turns a rigidbody into a vehicle.
	/// 
	/// Ported from btRaycastVehicle.cpp/.h in bullet-2.53
	/// Ported by William Denniss, 2007-11-25
	/// </summary>
	[Serializable]
	public class RaycastVehicle : TypedConstraint
	{
		[Serializable]
		public struct VehicleTuning
		{
			private float suspensionStiffness;
			private float suspensionCompression;
			private float suspensionDamping;
			private float maxSuspensionTravelCm;
			private float frictionSlip;

			public VehicleTuning(float suspensionStiffness, float suspensionCompression, float suspensionDamping,
				float maxSuspensionTravelCm, float frictionSlip)
			{
				this.suspensionStiffness = suspensionStiffness;
                this.suspensionCompression = suspensionCompression;
                this.suspensionDamping = suspensionDamping;
                this.maxSuspensionTravelCm = maxSuspensionTravelCm;
                this.frictionSlip = frictionSlip;
			}

            public float SuspensionStiffness
            {
                get { return suspensionStiffness; }
                set { suspensionStiffness = value; }
            }
            public float SuspensionCompression
            {
                get { return suspensionCompression; }
                set { suspensionCompression = value; }
            }
            public float SuspensionDamping
            {
                get { return suspensionDamping; }
                set { suspensionDamping = value; }
            }
            public float MaxSuspensionTravelCm
            {
                get { return maxSuspensionTravelCm; }
                set { maxSuspensionTravelCm = value; }
            }
            public float FrictionSlip
            {
                get { return frictionSlip; }
                set { frictionSlip = value; }
            }

            public VehicleTuning Default
            {
                get { return new VehicleTuning(5.88f, 0.83f, 0.88f, 500.0f, 10.5f); }
            }
		};

		float tau;
		float damping;
		IVehicleRaycaster vehicleRaycaster;
		float pitchControl;
		float steeringValue;
		float currentVehicleSpeedKmHour;

		RigidBody chassisBody;

		int indexRightAxis;
		int indexUpAxis;
		int indexForwardAxis;

        FastList<WheelInfo> _wheelInfo = new FastList<WheelInfo>();

		void DefaultInit(VehicleTuning tuning)
		{
			currentVehicleSpeedKmHour = 0;
			steeringValue = 0;
		}

		//constructor to create a car from an existing rigidbody
		public RaycastVehicle(VehicleTuning tuning, RigidBody chassis, IVehicleRaycaster raycaster)
		{
			vehicleRaycaster = raycaster;
			pitchControl = 0;

			chassisBody = chassis;
			indexRightAxis = 0;
			indexUpAxis = 2;
			indexForwardAxis = 1;
			DefaultInit(tuning);
		}

		Matrix GetChassisWorldTransform()
		{
			Matrix chassisWorldTrans;
			this.RigidBody.MotionState.GetWorldTransform(out chassisWorldTrans);
			return chassisWorldTrans;
		}

		public static RigidBody _fixedObject; //= new RigidBody(0.0f, null, null);

		public float RayCast(WheelInfo wheel)
		{
			UpdateWheelTransformsWS(wheel, false);

			float depth = -1;

			float raylen = wheel.SuspensionRestLength + wheel.WheelsRadius;

			Vector3 rayvector = wheel.RaycastInfo.WheelDirectionWS * (raylen);
			Vector3 source = wheel.RaycastInfo.HardPointWS;

			wheel.RaycastInfo.ContactPointWS = source + rayvector;

			Vector3 target = wheel.RaycastInfo.ContactPointWS;

			float param = 0.0f;

			VehicleRaycasterResult rayResults;

			Trace.Assert(this.vehicleRaycaster != null);

			// sets debug info
			wheel.DebugRaycastInfo.From = source;
			wheel.DebugRaycastInfo.To = target;
			wheel.DebugRaycastInfo.Collided = false;

			object obj = vehicleRaycaster.CastRay(source, target, out rayResults);

			wheel.RaycastInfo.GroundObject = null;

			if (obj != null)
			{
				param = rayResults.DistFraction;
				depth = raylen * rayResults.DistFraction;
				wheel.RaycastInfo.ContactNormalWS = rayResults.HitNormalInWorld;
				wheel.RaycastInfo.IsInContact = true;

				wheel.DebugRaycastInfo.Collided = true;
				wheel.DebugRaycastInfo.CollideNormal = rayResults.HitNormalInWorld;
				wheel.DebugRaycastInfo.CollidePoint = rayResults.HitPointInWorld;

				wheel.RaycastInfo.GroundObject = _fixedObject; //todo for driving on dynamic/movable objects!;
				//wheel.m_raycastInfo.m_groundObject = object;


				float hitDistance = param * raylen;
				wheel.RaycastInfo.SuspensionLength = hitDistance - wheel.WheelsRadius;
				//clamp on max suspension travel

				float minSuspensionLength = wheel.SuspensionRestLength - wheel.MaxSuspensionTravelCm * 0.01f;
				float maxSuspensionLength = wheel.SuspensionRestLength + wheel.MaxSuspensionTravelCm * 0.01f;
				if (wheel.RaycastInfo.SuspensionLength < minSuspensionLength)
				{
					wheel.RaycastInfo.SuspensionLength = minSuspensionLength;
				}
				if (wheel.RaycastInfo.SuspensionLength > maxSuspensionLength)
				{
					wheel.RaycastInfo.SuspensionLength = maxSuspensionLength;
				}

				wheel.RaycastInfo.ContactPointWS = rayResults.HitPointInWorld;

				float denominator = Vector3.Dot(wheel.RaycastInfo.ContactNormalWS, wheel.RaycastInfo.WheelDirectionWS);

				Vector3 chassis_velocity_at_contactPoint;
				Vector3 relpos = wheel.RaycastInfo.ContactPointWS - this.RigidBody.CenterOfMassPosition;

				chassis_velocity_at_contactPoint = this.RigidBody.GetVelocityInLocalPoint(relpos);

				float projVel = Vector3.Dot(wheel.RaycastInfo.ContactNormalWS, chassis_velocity_at_contactPoint);

				if (denominator >= -0.1f)
				{
					wheel.SuspensionRelativeVelocity = 0.0f;
					wheel.ClippedInvContactDotSuspension = 1.0f / 0.1f;
				}
				else
				{
					float inv = -1.0f / denominator;
					wheel.SuspensionRelativeVelocity = projVel * inv;
					wheel.ClippedInvContactDotSuspension = inv;
				}

			}
			else
			{
				//put wheel info as in rest position
				wheel.RaycastInfo.SuspensionLength = wheel.SuspensionRestLength;
				wheel.SuspensionRelativeVelocity = 0.0f;
				wheel.RaycastInfo.ContactNormalWS = -wheel.RaycastInfo.WheelDirectionWS;
				wheel.ClippedInvContactDotSuspension = 1.0f;
			}

			return depth;
		}

		public void UpdateVehicle(float step)
		{
			{
				for (int i = 0; i < GetNumWheels(); i++)
				{
					UpdateWheelTransform(i, false);
				}
			}

			currentVehicleSpeedKmHour = 3.6f * this.RigidBody.LinearVelocity.Length();

			Matrix chassisTrans = GetChassisWorldTransform();

			float[][] chassisTransa = MatrixToArray(chassisTrans);	// TODO: optimise
			Vector3 forwardW = new Vector3(
				chassisTransa[0][indexForwardAxis],
				chassisTransa[1][indexForwardAxis],
				chassisTransa[2][indexForwardAxis]);

			if (Vector3.Dot(forwardW, this.RigidBody.LinearVelocity) < 0.0f)
			{
				currentVehicleSpeedKmHour *= -1.0f;
			}

			//
			// simulate suspension
			//

			for (int i = 0; i < _wheelInfo.Count; i++)
			{
				float depth = RayCast(_wheelInfo[i]);
			}

			UpdateSuspension(step);

			for (int i = 0; i < _wheelInfo.Count; i++)
			{
				//apply suspension force
				WheelInfo wheel = _wheelInfo[i];

				float suspensionForce = wheel.WheelsSuspensionForce;

				// will: this was 6000 in Bullet, but that causes car to flip out
				float gMaxSuspensionForce = 1000.0f;
				if (suspensionForce > gMaxSuspensionForce)
				{
					suspensionForce = gMaxSuspensionForce;
				}
				Vector3 impulse = wheel.RaycastInfo.ContactNormalWS * suspensionForce * step;
				Vector3 relpos = wheel.RaycastInfo.ContactPointWS - this.RigidBody.CenterOfMassPosition;

				this.RigidBody.ApplyImpulse(impulse, relpos);

			}

			UpdateFriction(step);

			for (int i = 0; i < _wheelInfo.Count; i++)
			{
				WheelInfo wheel = _wheelInfo[i];
				Vector3 relpos = wheel.RaycastInfo.HardPointWS - this.RigidBody.CenterOfMassPosition;
				Vector3 vel = this.RigidBody.GetVelocityInLocalPoint(relpos);

				if (wheel.RaycastInfo.IsInContact)
				{
					Matrix chassisWorldTransform = GetChassisWorldTransform();

					float[][] chassisWorldTransformA = MatrixToArray(chassisWorldTransform);  // TODO: optimise
					Vector3 fwd = new Vector3(
						chassisWorldTransformA[0][indexForwardAxis],
						chassisWorldTransformA[1][indexForwardAxis],
						chassisWorldTransformA[2][indexForwardAxis]);

					float proj = Vector3.Dot(fwd, wheel.RaycastInfo.ContactNormalWS);
					fwd -= wheel.RaycastInfo.ContactNormalWS * proj;

					float proj2 = Vector3.Dot(fwd, vel);

					wheel.DeltaRotation = (proj2 * step) / (wheel.WheelsRadius);
					wheel.Rotation += wheel.DeltaRotation;

				}
				else
				{
					wheel.Rotation += wheel.DeltaRotation;
				}

				wheel.DeltaRotation *= 0.99f;//damping of rotation when not in contact

			}
		}

		public void ResetSuspension()
		{

			for (int i = 0; i < _wheelInfo.Count; i++)
			{
				WheelInfo wheel = _wheelInfo[i];

				wheel.RaycastInfo.SuspensionLength = wheel.SuspensionRestLength;
				wheel.SuspensionRelativeVelocity = 0.0f;

				wheel.RaycastInfo.ContactNormalWS = wheel.RaycastInfo.WheelDirectionWS;
				wheel.ClippedInvContactDotSuspension = 1.0f;
			}
		}


		public float GetSteeringValue(int wheel)
		{
			return GetWheelInfo(wheel).Steering;
		}

		public void SetSteeringValue(float steering, int wheel)
		{
			Trace.Assert(wheel >= 0 && wheel < GetNumWheels());

			WheelInfo wheelInfo = GetWheelInfo(wheel);
			wheelInfo.Steering = steering;
		}

		public void ApplyEngineForce(float force, int wheel)
		{
			Trace.Assert(wheel >= 0 && wheel < GetNumWheels());

			WheelInfo wheelInfo = GetWheelInfo(wheel);
			wheelInfo.EngineForce = force;
		}

		public Matrix GetWheelTransformWS(int wheelIndex)
		{
			Trace.Assert(wheelIndex < GetNumWheels());
			WheelInfo wheel = _wheelInfo[wheelIndex];
			return wheel.WorldTransform;
		}

		public void UpdateWheelTransform(int wheelIndex)
		{
			UpdateWheelTransform(wheelIndex, true);
		}

		public void UpdateWheelTransform(int wheelIndex, bool interpolatedTransform)
		{
			WheelInfo wheel = _wheelInfo[wheelIndex];
			UpdateWheelTransformsWS(wheel, interpolatedTransform);
			Vector3 up = -wheel.RaycastInfo.WheelDirectionWS;
			Vector3 right = wheel.RaycastInfo.WheelAxleWS;
			Vector3 fwd = Vector3.Cross(up, right);
			fwd.Normalize();
			//	up = right.cross(fwd);
			//	up.normalize();

			//rotate around steering over de wheelAxleWS
			float steering = wheel.Steering;

			Quaternion steeringOrn = new Quaternion(up, steering);
			Matrix steeringMat = Matrix.CreateFromQuaternion(steeringOrn);

			Quaternion rotatingOrn = new Quaternion(right, -wheel.Rotation);
			Matrix rotatingMat = Matrix.CreateFromQuaternion(rotatingOrn);

			Matrix m2 = new Matrix();
			m2.M11 = right.X; m2.M12 = fwd.X; m2.M13 = up.X;
			m2.M21 = right.Y; m2.M22 = fwd.Y; m2.M23 = up.Y;
			m2.M31 = right.Z; m2.M32 = fwd.Z; m2.M33 = up.Z;

			//MathHelper.SetElement(

			//	  btMatrix3x3 basis2(
			//	  right[0],fwd[0],up[0],
			//	  right[1],fwd[1],up[1],
			//	  right[2],fwd[2],up[2]
			//);

			//m2.Right = right;
			//m2.Forward= fwd;
			//m2.Up = up;

			Matrix m = Matrix.Identity;
			m = m * steeringMat * rotatingMat * m2;
			m.Translation = wheel.RaycastInfo.HardPointWS + wheel.RaycastInfo.WheelDirectionWS * wheel.RaycastInfo.SuspensionLength;
			wheel.WorldTransform = m;
			//wheel.m_worldTransform.setBasis(steeringMat * rotatingMat * basis2);
			//wheel.m_worldTransform.setOrigin(
			//	  wheel.m_raycastInfo.m_hardPointWS + wheel.m_raycastInfo.m_wheelDirectionWS * wheel.m_raycastInfo.m_suspensionLength
			//);
		}

		public WheelInfo AddWheel(Vector3 connectionPointCS, Vector3 wheelDirectionCS0, Vector3 wheelAxleCS, float suspensionRestLength, float wheelRadius, VehicleTuning tuning, bool isFrontWheel)
		{

			WheelInfoConstructionInfo ci = new WheelInfoConstructionInfo();

			ci.ChassicConnectionCS = connectionPointCS;
			ci.WheelDirectionCS = wheelDirectionCS0;
			ci.WheelAxleCS = wheelAxleCS;
			ci.SuspensionRestLength = suspensionRestLength;
			ci.WheelRadius = wheelRadius;
			ci.SuspensionStiffness = tuning.SuspensionStiffness;
			ci.WheelsDampingCompression = tuning.SuspensionCompression;
			ci.WheelsDampingRelaxation = tuning.SuspensionDamping;
			ci.FrictionSlip = tuning.FrictionSlip;
			ci.IsFrontWheel = isFrontWheel;
			ci.MaxSuspensionTravelCm = tuning.MaxSuspensionTravelCm;

			WheelInfo wheel = new WheelInfo(ci);
			_wheelInfo.Add(wheel);


			UpdateWheelTransformsWS(wheel, false);
			UpdateWheelTransform(GetNumWheels() - 1, false);
			return wheel;
		}

		public int GetNumWheels()
		{
			return _wheelInfo.Count;
		}

		public void UpdateWheelTransformsWS(WheelInfo wheel)
		{
			UpdateWheelTransformsWS(wheel, true);
		}

		public Vector3 BracketTrans(Matrix m, Vector3 x)
		{
			return new Vector3(
			Vector3.Dot(m.Right, x) + m.Translation.X,
			Vector3.Dot(m.Up, x) + m.Translation.Y,
			Vector3.Dot(m.Forward, x) + m.Translation.Z);


			// SIMD_FORCE_INLINE btVector3 operator()(const btVector3& x) const
			//{
			//	  return btVector3(m_basis[0].dot(x) + m_origin.x(), 
			//		  m_basis[1].dot(x) + m_origin.y(), 
			//		  m_basis[2].dot(x) + m_origin.z());
			//}
		}


		public Vector3 Bracket3x3(Matrix m, Vector3 x)
		{
			return new Vector3(
			Vector3.Dot(m.Right, x),
			Vector3.Dot(m.Up, x),
			Vector3.Dot(m.Forward, x));


			//SIMD_FORCE_INLINE btVector3 operator()(const btVector3& x) const
			//{
			//	  return btVector3(m_basis[0].dot(x) + m_origin.x(), 
			//		  m_basis[1].dot(x) + m_origin.y(), 
			//		  m_basis[2].dot(x) + m_origin.z());
			//}

		}

		public void UpdateWheelTransformsWS(WheelInfo wheel, bool interpolatedTransform)
		{
			wheel.RaycastInfo.IsInContact = false;

			Matrix chassisTrans = GetChassisWorldTransform();
			if (interpolatedTransform && this.RigidBody.MotionState != null)
			{
				this.RigidBody.MotionState.GetWorldTransform(out chassisTrans);
			}
			Matrix chassisTransRot = Matrix.CreateFromQuaternion(Quaternion.CreateFromRotationMatrix(chassisTrans));
			//chassisTransRot.Translation = Matrix.Identity.Translation;
			// chassisTransRot.Translation = new Vector3(0, 0, 0);

			// TODO
			// wheel.RaycastInfo.HardPointWS = chassisTrans(wheel.m_chassisConnectionPointCS); // ?
			wheel.RaycastInfo.HardPointWS = Vector3.Transform(wheel.ChassicConnectionPointCS, chassisTrans);
			wheel.RaycastInfo.WheelDirectionWS = Vector3.Transform(wheel.WheelDirectionCS, chassisTransRot);
			wheel.RaycastInfo.WheelAxleWS = Vector3.Transform(wheel.WheelAxleCS, chassisTransRot);
		}

		public void SetBrake(float brake, int wheelIndex)
		{
			Trace.Assert(wheelIndex >= 0 && wheelIndex < GetNumWheels());

			WheelInfo wheel = GetWheelInfo(wheelIndex);
			wheel.Brake = brake;
		}

		public void UpdateSuspension(float deltaTime)
		{
			//(void)deltaTime;

			float chassisMass = 1.0f / chassisBody.InverseMass;

			for (int w_it = 0; w_it < GetNumWheels(); w_it++)
			{
				WheelInfo wheel_info = _wheelInfo[w_it];

				if (wheel_info.RaycastInfo.IsInContact)
				{
					float force;
					//	Spring
					{
						float susp_length = wheel_info.SuspensionRestLength;
						float current_length = wheel_info.RaycastInfo.SuspensionLength;

						float length_diff = (susp_length - current_length);

						force = wheel_info.SuspensionStiffness
							* length_diff * wheel_info.ClippedInvContactDotSuspension;
					}

					// Damper
					{
						float projected_rel_vel = wheel_info.SuspensionRelativeVelocity;
						{
							float susp_damping;
							if (projected_rel_vel < 0.0f)
							{
								susp_damping = wheel_info.WheelsDampingCompression;
							}
							else
							{
								susp_damping = wheel_info.WheelsDampingRelaxation;
							}
							force -= susp_damping * projected_rel_vel;
						}
					}

					// RESULT
					wheel_info.WheelsSuspensionForce = force * chassisMass;
					if (wheel_info.WheelsSuspensionForce < 0.0f)
					{
						wheel_info.WheelsSuspensionForce = 0.0f;
					}
				}
				else
				{
					wheel_info.WheelsSuspensionForce = 0.0f;
				}
			}

		}

		[Serializable]
		struct WheelContactPoint
		{
			private RigidBody _bodyA;
			private RigidBody _bodyB;
			private Vector3 _frictionPositionWorld;
			private Vector3 _frictionDirectionWorld;
			private float _jacDiagABInv;
			private float _maxImpulse;

			public WheelContactPoint(RigidBody bodyA, RigidBody bodyB, Vector3 frictionPosWorld, Vector3 frictionDirectionWorld, float maxImpulse)
			{
				_bodyA = bodyA;
				_bodyB = bodyB;
				_frictionPositionWorld = frictionPosWorld;
				_frictionDirectionWorld = frictionDirectionWorld;
				_maxImpulse = maxImpulse;

				float denomA = bodyA.ComputeImpulseDenominator(frictionPosWorld, frictionDirectionWorld);
				float denomB = bodyB.ComputeImpulseDenominator(frictionPosWorld, frictionDirectionWorld);
				float relaxation = 1.0f;
				_jacDiagABInv = relaxation / (denomA + denomB);
			}

			public RigidBody BodyA { get { return _bodyA; } set { _bodyA = value; } }
			public RigidBody BodyB { get { return _bodyB; } set { _bodyB = value; } }
			public Vector3 FrictionPositionWorld { get { return _frictionPositionWorld; } set { _frictionPositionWorld = value; } }
			public Vector3 FrictionDirectionWorld { get { return _frictionDirectionWorld; } set { _frictionDirectionWorld = value; } }
			public float JacDiagABInv { get { return _jacDiagABInv; } set { _jacDiagABInv = value; } }
			public float MaxImpulse { get { return _maxImpulse; } set { _maxImpulse = value; } }
		};

		private float CalcRollingFriction(WheelContactPoint contactPoint)
		{
			float j1 = 0.0f;

			Vector3 contactPosWorld = contactPoint.FrictionPositionWorld;

			Vector3 rel_pos1 = contactPosWorld - contactPoint.BodyA.CenterOfMassPosition;
			Vector3 rel_pos2 = contactPosWorld - contactPoint.BodyB.CenterOfMassPosition;

			float maxImpulse = contactPoint.MaxImpulse;

			Vector3 vel1 = contactPoint.BodyA.GetVelocityInLocalPoint(rel_pos1);
			Vector3 vel2 = contactPoint.BodyB.GetVelocityInLocalPoint(rel_pos2);
			Vector3 vel = vel1 - vel2;

			float vrel = Vector3.Dot(contactPoint.FrictionDirectionWorld, vel);

			// calculate j that moves us to zero relative velocity
			j1 = -vrel * contactPoint.JacDiagABInv;

			j1 = Math.Min(j1, maxImpulse);	//GEN_set_min(j1, maxImpulse);
			j1 = Math.Max(j1, -maxImpulse);	 //GEN_set_max(j1, -maxImpulse);


			return j1;
		}

		// TODO:  This is a very slow method, should be fixed
		private float[][] MatrixToArray(Matrix matrix)
		{
#warning "highly unoptimised"

			float[][] ma = new float[4][];
			for (int i = 0; i < ma.Length; i++)
			{
				ma[i] = new float[4];
			}
			ma[0][0] = matrix.M11;
			ma[0][1] = matrix.M12;
			ma[0][2] = matrix.M13;
			ma[0][3] = matrix.M14;
			ma[1][0] = matrix.M21;
			ma[1][1] = matrix.M22;
			ma[1][2] = matrix.M23;
			ma[1][3] = matrix.M24;
			ma[2][0] = matrix.M31;
			ma[2][1] = matrix.M32;
			ma[2][2] = matrix.M33;
			ma[2][3] = matrix.M44;
			ma[3][0] = matrix.M41;
			ma[3][1] = matrix.M42;
			ma[3][2] = matrix.M43;
			ma[3][3] = matrix.M44;
			return ma;
		}

		private float _sideFrictionStiffness2 = 1.0f;

		public void UpdateFriction(float timeStep)
		{

			//calculate the impulse, so that the wheels don't move sidewards
			int numWheel = GetNumWheels();
			if (numWheel == 0)
				return;

			Vector3[] forwardWS = new Vector3[numWheel];
			Vector3[] axle = new Vector3[numWheel];
			float[] forwardImpulse = new float[numWheel];
			float[] sideImpulse = new float[numWheel];

			int numWheelsOnGround = 0;


			//collapse all those loops into one!
			for (int i = 0; i < GetNumWheels(); i++)
			{
				WheelInfo wheelInfo = _wheelInfo[i];
				RigidBody groundObject = (RigidBody)wheelInfo.RaycastInfo.GroundObject;
				if (groundObject != null)
					numWheelsOnGround++;
				sideImpulse[i] = 0.0f;
				forwardImpulse[i] = 0.0f;

			}

			for (int i = 0; i < GetNumWheels(); i++)
			{

				WheelInfo wheelInfo = _wheelInfo[i];

				RigidBody groundObject = (RigidBody)wheelInfo.RaycastInfo.GroundObject;

				if (groundObject != null)
				{

					Matrix wheelTrans = GetWheelTransformWS(i);

					Matrix wheelBasis0 = wheelTrans;
					float[][] wheelBasis0a = MatrixToArray(wheelTrans);	 // TODO: optimise
					axle[i] = new Vector3(
						wheelBasis0a[0][indexRightAxis],
						wheelBasis0a[1][indexRightAxis],
						wheelBasis0a[2][indexRightAxis]);

					Vector3 surfNormalWS = wheelInfo.RaycastInfo.ContactNormalWS;
					float proj = Vector3.Dot(axle[i], surfNormalWS);
					axle[i] -= surfNormalWS * proj;
					Trace.Assert(axle[i] != new Vector3(0.0f, 0.0f, 0.0f));
					axle[i].Normalize();


					forwardWS[i] = Vector3.Cross(surfNormalWS, axle[i]);

					// wd-hack
					if (forwardWS[i] != new Vector3(0.0f, 0.0f, 0.0f))
					{
						forwardWS[i].Normalize();
					}
                    //else
                    //{
                    //    int x = 0;
                    //}


					ContactConstraint.ResolveSingleBilateral(chassisBody, wheelInfo.RaycastInfo.ContactPointWS,
							  groundObject, wheelInfo.RaycastInfo.ContactPointWS,
							  0.0f, axle[i], out sideImpulse[i], timeStep);

					sideImpulse[i] *= _sideFrictionStiffness2;

				}
			}

			float sideFactor = 1.0f;
			float fwdFactor = 0.5f;

			bool sliding = false;
			{
				for (int wheel = 0; wheel < GetNumWheels(); wheel++)
				{
					WheelInfo wheelInfo = _wheelInfo[wheel];
					RigidBody groundObject = (RigidBody)wheelInfo.RaycastInfo.GroundObject;

					float rollingFriction = 0.0f;

					if (groundObject != null)
					{
						if (wheelInfo.EngineForce != 0.0f)
						{
							rollingFriction = wheelInfo.EngineForce * timeStep;
						}
						else
						{
							float defaultRollingFrictionImpulse = 0.0f;
							float maxImpulse = wheelInfo.Brake != 0.0f ? wheelInfo.Brake : defaultRollingFrictionImpulse;
							WheelContactPoint contactPt = new WheelContactPoint(chassisBody, groundObject, wheelInfo.RaycastInfo.ContactPointWS, forwardWS[wheel], maxImpulse);
							rollingFriction = CalcRollingFriction(contactPt);
						}
					}

					//switch between active rolling (throttle), braking and non-active rolling friction (no throttle/break)

					forwardImpulse[wheel] = 0.0f;
					_wheelInfo[wheel].SkidInfo = 1.0f;

					if (groundObject != null)
					{
						_wheelInfo[wheel].SkidInfo = 1.0f;

						float maximp = wheelInfo.WheelsSuspensionForce * timeStep * wheelInfo.FrictionSlip;
						float maximpSide = maximp;

						float maximpSquared = maximp * maximpSide;


						forwardImpulse[wheel] = rollingFriction;//wheelInfo.m_engineForce* timeStep;

						float x = (forwardImpulse[wheel]) * fwdFactor;
						float y = (sideImpulse[wheel]) * sideFactor;

						float impulseSquared = (x * x + y * y);

						if (impulseSquared > maximpSquared)
						{
							sliding = true;

							float factor = maximp / (float)Math.Sqrt((double)impulseSquared);

							_wheelInfo[wheel].SkidInfo *= factor;
						}
					}
				}
			}

			if (sliding)
			{
				for (int wheel = 0; wheel < GetNumWheels(); wheel++)
				{
					if (sideImpulse[wheel] != 0.0f)
					{
						if (_wheelInfo[wheel].SkidInfo < 1.0f)
						{
							forwardImpulse[wheel] *= _wheelInfo[wheel].SkidInfo;
							sideImpulse[wheel] *= _wheelInfo[wheel].SkidInfo;
						}
					}
				}
			}

			// apply the impulses
			{
				for (int wheel = 0; wheel < GetNumWheels(); wheel++)
				{
					WheelInfo wheelInfo = _wheelInfo[wheel];

					Vector3 rel_pos = wheelInfo.RaycastInfo.ContactPointWS -
							chassisBody.CenterOfMassPosition;
					if (forwardImpulse[wheel] != 0.0f)
					{
						chassisBody.ApplyImpulse(forwardWS[wheel] * (forwardImpulse[wheel]), rel_pos);
					}
					if (sideImpulse[wheel] != 0.0f)
					{
						RigidBody groundObject = (RigidBody)_wheelInfo[wheel].RaycastInfo.GroundObject;

						Vector3 rel_pos2 = wheelInfo.RaycastInfo.ContactPointWS -
							groundObject.CenterOfMassPosition;


						Vector3 sideImp = axle[wheel] * sideImpulse[wheel];

						rel_pos.Z *= wheelInfo.RollInfluence;
						chassisBody.ApplyImpulse(sideImp, rel_pos);

						//apply friction impulse on the ground
						groundObject.ApplyImpulse(-sideImp, rel_pos2);
					}
				}
			}
		}

		public RigidBody RigidBody
		{
			get
			{
				return chassisBody;
			}
		}

		public int RightAxis
		{
			get
			{
				return indexRightAxis;
			}
		}

		public int UpAxis
		{
			get
			{
				return indexUpAxis;
			}
		}

		public int ForwardAxis
		{
			get
			{
				return indexForwardAxis;
			}
		}

		//Velocity of vehicle (positive if velocity vector has same direction as foward vector)
		public float CurrentSpeedKmHour { get { return currentVehicleSpeedKmHour; } }

		public virtual void SetCoordinateSystem(int rightIndex, int upIndex, int forwardIndex)
		{
			indexRightAxis = rightIndex;
			indexUpAxis = upIndex;
			indexForwardAxis = forwardIndex;
		}

		public override void BuildJacobian()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void SolveConstraint(float timeStep)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public WheelInfo GetWheelInfo(int v)
		{
			return _wheelInfo[v];
		}
	}

	[Serializable]
	public class DefaultVehicleRaycaster : IVehicleRaycaster
	{
		DynamicsWorld _dynamicsWorld;

		public DefaultVehicleRaycaster(DynamicsWorld world)
		{
			_dynamicsWorld = world;
		}

		public object CastRay(Vector3 from, Vector3 to, out VehicleRaycasterResult result)
		{
			CollisionWorld.ClosestRayResultCallback rayCallback = new CollisionWorld.ClosestRayResultCallback(from, to);
			_dynamicsWorld.RayTest(from, to, rayCallback, (short)BroadphaseProxy.CollisionFilterGroups.All);

			result = new VehicleRaycasterResult();

			if (!rayCallback.HasHit)
			{
				return null;
			}
			RigidBody body = RigidBody.Upcast(rayCallback.CollisionObject);
			if (body == null)
			{
				return null;
			}

			result.HitPointInWorld = rayCallback.HitPointWorld;
			result.HitNormalInWorld = rayCallback.HitNormalWorld;
			result.HitNormalInWorld.Normalize();
			result.DistFraction = rayCallback.ClosestHitFraction;
			return body;
		}
	}
}

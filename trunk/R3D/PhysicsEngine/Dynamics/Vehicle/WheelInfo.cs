/*
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
using System.Collections.Generic;
using System.Text;
using R3D.MathLib;

namespace R3D.PhysicsEngine.Dynamics
{
	[Serializable]
	public struct WheelInfoConstructionInfo
	{
		private Vector3 _chassicConnectionCS;
		private Vector3 _wheelDirectionCS;
		private Vector3 _wheelAxisCS;

		private float _suspensionRestLength;
		private float _maxSuspensionTravelCm;
		private float _wheelRadius;
		private float _suspensionStiffness;
		private float _wheelsDampingCompression;
		private float _wheelsDampingRelaxation;
		private float _frictionSlip;

		private bool _isFrontWheel;

		#region Basic Properties
		public Vector3 ChassicConnectionCS
		{
			get { return _chassicConnectionCS; }
			set { _chassicConnectionCS = value; }
		}

		public Vector3 WheelDirectionCS
		{
			get { return _wheelDirectionCS; }
			set { _wheelDirectionCS = value; }
		}

		public Vector3 WheelAxleCS
		{
			get { return _wheelAxisCS; }
			set { _wheelAxisCS = value; }
		}


		public Single SuspensionRestLength
		{
			get { return _suspensionRestLength; }
			set { _suspensionRestLength = value; }
		}

		public Single MaxSuspensionTravelCm
		{
			get { return _maxSuspensionTravelCm; }
			set { _maxSuspensionTravelCm = value; }
		}

		public Single WheelRadius
		{
			get { return _wheelRadius; }
			set { _wheelRadius = value; }
		}


		public Single SuspensionStiffness
		{
			get { return _suspensionStiffness; }
			set { _suspensionStiffness = value; }
		}

		public Single WheelsDampingCompression
		{
			get { return _wheelsDampingCompression; }
			set { _wheelsDampingCompression = value; }
		}

		public Single WheelsDampingRelaxation
		{
			get { return _wheelsDampingRelaxation; }
			set { _wheelsDampingRelaxation = value; }
		}

		public Single FrictionSlip
		{
			get { return _frictionSlip; }
			set { _frictionSlip = value; }
		}


		public Boolean IsFrontWheel
		{
			get { return _isFrontWheel; }
			set { _isFrontWheel = value; }
		}
		#endregion
	}

	[Serializable]
	public class DebugRaycastInfo
	{
		public Vector3 _from;
		public Vector3 _to;
		public bool _collided;
		public Vector3 _collidePoint;
		public Vector3 _collideNormal;

		public Vector3 From { get { return _from; } set { _from = value; } }
		public Vector3 To { get { return _to; } set { _to = value; } }
		public bool Collided { get { return _collided; } set { _collided = value; } }
		public Vector3 CollidePoint { get { return _collidePoint; } set { _collidePoint = value; } }
		public Vector3 CollideNormal { get { return _collideNormal; } set { _collideNormal = value; } }
	}

	[Serializable]
	public class RaycastInfo
	{
		private Vector3 _contractNormalWS;
		private Vector3 _contractPointWS;

		private Vector3 _hardPointWS;
		private Vector3 _wheelDirectionWS;
		private Vector3 _wheelAxleWS;

		private float _suspensionLength;
		private bool _isInContract;

		private object _groundObject;

		#region Basic Properties
		public Single SuspensionLength
		{
			get { return _suspensionLength; }
			set { _suspensionLength = value; }
		}

		public Boolean IsInContact
		{
			get { return _isInContract; }
			set { _isInContract = value; }
		}

		public Vector3 ContactNormalWS
		{
			get { return _contractNormalWS; }
			set { _contractNormalWS = value; }
		}

		public Vector3 ContactPointWS
		{
			get { return _contractPointWS; }
			set { _contractPointWS = value; }
		}

		public Vector3 HardPointWS
		{
			get { return _hardPointWS; }
			set { _hardPointWS = value; }
		}

		public Vector3 WheelDirectionWS
		{
			get { return _wheelDirectionWS; }
			set { _wheelDirectionWS = value; }
		}

		public Vector3 WheelAxleWS
		{
			get { return _wheelAxleWS; }
			set { _wheelAxleWS = value; }
		}

		public object GroundObject
		{
			get { return _groundObject; }
			set { _groundObject = value; }
		}
		#endregion
	}

	[Serializable]
	public class WheelInfo
	{
		private RaycastInfo _raycastInfo;

		private Matrix _worldTransform;

		private Vector3 _chassicConnectionPointCS;
		private Vector3 _wheelDirectionCS;
		private Vector3 _wheelAxleCS;

		private float _suspensionRestLength;
		private float _maxSuspensionTravelCm;

		private float _wheelsRadius;
		private float _rollInfluence;
		private float _suspensionStiffness;
		private float _wheelsDampingCompression;
		private float _wheelsDampingRelaxation;
		private float _frictionSlip;
		private float _steering;
		private float _rotation;
		private float _deltaRotation;

		private float _engineForce;
		private float _brake;
		private bool _isFrontWheel;


		private float _clippedInvContactDotSuspension;
		private float _skidInfo;
		private float _wheelsSuspensionForce;
		private float _suspensionRelativeVelocity;
		//can be used to store pointer to sync transforms...
		private object _clientInfo;

		private DebugRaycastInfo _debugRayInfo = new DebugRaycastInfo();

		#region Constructor
		public WheelInfo(WheelInfoConstructionInfo constructionInfo)
		{
			_suspensionRestLength = constructionInfo.SuspensionRestLength;
			_maxSuspensionTravelCm = constructionInfo.MaxSuspensionTravelCm;

			_wheelsRadius = constructionInfo.WheelRadius;
			_wheelsDampingCompression = constructionInfo.WheelsDampingCompression;
			_wheelsDampingRelaxation = constructionInfo.WheelsDampingRelaxation;
			_wheelDirectionCS = constructionInfo.WheelDirectionCS;

			_suspensionStiffness = constructionInfo.SuspensionStiffness;
			_chassicConnectionPointCS = constructionInfo.ChassicConnectionCS;

			_wheelAxleCS = constructionInfo.WheelAxleCS;
			_frictionSlip = constructionInfo.FrictionSlip;

			_clippedInvContactDotSuspension = 0;
			_suspensionRelativeVelocity = 0;
			_wheelsSuspensionForce = 0;
			_skidInfo = 0;

			_steering = 0;
			_engineForce = 0;
			_rotation = 0;
			_rotation = 0;
			_deltaRotation = 0;
			_brake = 0;
			_rollInfluence = 0.1f;
			_brake = 0;
			_rollInfluence = 0.1f;

			_isFrontWheel = constructionInfo.IsFrontWheel;

			_raycastInfo = new RaycastInfo();
			_worldTransform = default(Matrix);
			_clientInfo = null;
		}
		#endregion

		#region BasicProperties
		public object ClientInfo { get { return _clientInfo; } set { _clientInfo = value; } }

		public RaycastInfo RaycastInfo
		{
			get { return _raycastInfo; }
			set { _raycastInfo = value; }
		}
		public DebugRaycastInfo DebugRaycastInfo
		{
			get { return _debugRayInfo; }
		}

		public Matrix WorldTransform
		{
			get { return _worldTransform; }
			set { _worldTransform = value; }
		}

		public Vector3 ChassicConnectionPointCS
		{
			get { return _chassicConnectionPointCS; }
			set { _chassicConnectionPointCS = value; }
		}
		public Vector3 WheelDirectionCS
		{
			get { return _wheelDirectionCS; }
			set { _wheelDirectionCS = value; }
		}
		public Vector3 WheelAxleCS
		{
			get { return _wheelAxleCS; }
			set { _wheelAxleCS = value; }
		}

		public Single SuspensionRestLength
		{
			get { return _suspensionRestLength; }
			set { _suspensionRestLength = value; }
		}


		public Single MaxSuspensionTravelCm
		{
			get { return _maxSuspensionTravelCm; }
			set { _maxSuspensionTravelCm = value; }
		}

		public Single WheelsRadius
		{
			get { return _wheelsRadius; }
			set { _wheelsRadius = value; }
		}

		public Single SuspensionStiffness
		{
			get { return _suspensionStiffness; }
			set { _suspensionStiffness = value; }
		}

		public Single WheelsDampingCompression
		{
			get { return _wheelsDampingCompression; }
			set { _wheelsDampingCompression = value; }
		}

		public Single WheelsDampingRelaxation
		{
			get { return _wheelsDampingRelaxation; }
			set { _wheelsDampingRelaxation = value; }
		}

		public Single FrictionSlip
		{
			get { return _frictionSlip; }
			set { _frictionSlip = value; }
		}

		public Single Steering
		{
			get { return _steering; }
			set { _steering = value; }
		}

		public Single Rotation
		{
			get { return _rotation; }
			set { _rotation = value; }
		}

		public Single DeltaRotation
		{
			get { return _deltaRotation; }
			set { _deltaRotation = value; }
		}

		public Single RollInfluence
		{
			get { return _rollInfluence; }
			set { _rollInfluence = value; }
		}

		public Single EngineForce
		{
			get { return _engineForce; }
			set { _engineForce = value; }
		}

		public Single Brake
		{
			get { return _brake; }
			set { _brake = value; }
		}

		public Boolean IsFrontWheel
		{
			get { return _isFrontWheel; }
			set { _isFrontWheel = value; }
		}

		public Single ClippedInvContactDotSuspension
		{
			get { return _clippedInvContactDotSuspension; }
			set { _clippedInvContactDotSuspension = value; }
		}

		public Single SuspensionRelativeVelocity
		{
			get { return _suspensionRelativeVelocity; }
			set { _suspensionRelativeVelocity = value; }
		}

		public Single WheelsSuspensionForce
		{
			get { return _wheelsSuspensionForce; }
			set { _wheelsSuspensionForce = value; }
		}

		public Single SkidInfo
		{
			get { return _skidInfo; }
			set { _skidInfo = value; }
		}
		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="chassis"></param>
		/// <param name="paramRaycastInfo">Not used!</param>
		public void UpdateWheel(RigidBody chassis, RaycastInfo paramRaycastInfo)
		{
			if (_raycastInfo.IsInContact)
			{
				float project = Vector3.Dot(_raycastInfo.ContactNormalWS, _raycastInfo.WheelDirectionWS);

                //Vector3 chassisVelocityAtContactPoint = new Vector3();
				Vector3 relpos = _raycastInfo.ContactPointWS - chassis.CenterOfMassPosition;
                Vector3 chassisVelocityAtContactPoint = chassis.GetVelocityInLocalPoint(relpos);
				float projVel = Vector3.Dot(_raycastInfo.ContactNormalWS, chassisVelocityAtContactPoint);

				if (project >= -0.1f)
				{
					_suspensionRelativeVelocity = 0;
					_clippedInvContactDotSuspension = 1.0f / 0.1f;
				}
				else
				{
					float inv = -1 / project;
					_suspensionRelativeVelocity = projVel * inv;
					_clippedInvContactDotSuspension = inv;
				}
			}
			else
			{
				_raycastInfo.SuspensionLength = _suspensionRestLength;
				_suspensionRelativeVelocity = 0.0f;
				_raycastInfo.ContactNormalWS = -_raycastInfo.WheelDirectionWS;
				_clippedInvContactDotSuspension = 1.0f;
			}
		}
	}
}

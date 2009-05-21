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
using R3D.PhysicsEngine.LinearMath;

namespace R3D.PhysicsEngine
{
	[Serializable]
	public class HeightFieldTerrainShape : ConcaveShape
	{
		private Vector3 _localScaling;
		private static readonly string _objectName = "Heightfield";
		private Vector3 _localAabbMin;
		private Vector3 _localAabbMax;

		//terrain data
		private float _maxHeight;
		private int _width;
		private int _length;

		private bool _useFloatData;
		private bool _flipQuadEdges;
		private bool _useDiamondSubdivision;
		private int _upAxis;
		private byte[] _heightfieldDataByte;
		private float[] _heightfieldDataFloat;
		//private ITriangleCallback m_callback;

		public HeightFieldTerrainShape(int width, int length, object heightFieldData, float maxHeight,
										int upAxis, bool useFloatData, bool flipQuadEdges)
		{
			_width = width;
			_length = length;
			_maxHeight = maxHeight;
			_upAxis = upAxis;
			_useDiamondSubdivision = false;
			_useFloatData = useFloatData;
			_flipQuadEdges = flipQuadEdges;
			_localScaling = new Vector3(1f, 1f, 1f);

			// sort out the data.
			if (_useFloatData)
			{
				BulletDebug.Assert(heightFieldData is float[]);
				_heightfieldDataFloat = (float[])heightFieldData;
			}
			else
			{
				BulletDebug.Assert(heightFieldData is byte[]);
				_heightfieldDataByte = (byte[])heightFieldData;
			}

			float quantizationMargin = 1f;

			//enlarge the AABB to avoid division by zero when initializing the quantization values
			Vector3 clampValue = new Vector3(quantizationMargin, quantizationMargin, quantizationMargin);
			Vector3 halfExtents = new Vector3(0f, 0f, 0f);

			switch (_upAxis)
			{
				case 0:
					{
						halfExtents.X = _maxHeight;
						halfExtents.Y = _width;
						halfExtents.Z = _length;
						break;
					}
				case 1:
					{
						halfExtents.X = _width;
						halfExtents.Y = _maxHeight;
						halfExtents.Z = _length;
						break;
					};
				case 2:
					{
						halfExtents.X = _width;
						halfExtents.Y = _length;
						halfExtents.Z = _maxHeight;
						break;
					}
				default:
					{
						//need to get valid m_upAxis
						BulletDebug.Assert(false);
						break;
					}
			}

			halfExtents *= 0.5f;

			_localAabbMin = -halfExtents - clampValue;
			_localAabbMax = halfExtents + clampValue;
			Vector3 aabbSize = _localAabbMax - _localAabbMin;
		}

		public int UpAxis
		{
			get { return _upAxis; }
			set { _upAxis = value; }
		}

		public override string Name
		{
			get { return _objectName; }
		}

		public override Vector3 LocalScaling
		{
			get { return _localScaling; }
			set { _localScaling = value; }
		}

		public override BroadphaseNativeTypes ShapeType
		{
			get { return BroadphaseNativeTypes.Terrain; }
		}

		public override void GetAabb(Matrix transform,
									out Vector3 aabbMin,
									out Vector3 aabbMax)
		{
			Vector3 halfExtents = (_localAabbMax - _localAabbMin) * _localScaling * 0.5f;

			Matrix abs_b = MathHelper.Absolute(transform);
			Vector3 center = transform.Translation;
			Vector3 row1 = new Vector3(abs_b.M11, abs_b.M12, abs_b.M13);
			Vector3 row2 = new Vector3(abs_b.M21, abs_b.M22, abs_b.M23);
			Vector3 row3 = new Vector3(abs_b.M31, abs_b.M32, abs_b.M33);
			Vector3 extent = new Vector3(Vector3.Dot(row1, halfExtents),
										 Vector3.Dot(row2, halfExtents),
										 Vector3.Dot(row3, halfExtents));
			extent += new Vector3(Margin, Margin, Margin);

			aabbMin = center - extent;
			aabbMax = center + extent;
		}

		private float GetHeightFieldValue(int x, int y)
		{
			float val = 0f;
			if (_useFloatData)
			{
				val = _heightfieldDataFloat[(y * _width) + x];
			}
			else
			{
				//assume unsigned short int
				byte heightFieldValue = _heightfieldDataByte[(y * _width) + x];
				val = heightFieldValue * (_maxHeight / 65535f);
			}
			return val;
		}

		private void GetVertex(int x, int y, ref Vector3 vertex)
		{
			BulletDebug.Assert(x >= 0);
			BulletDebug.Assert(y >= 0);
			BulletDebug.Assert(x < _width);
			BulletDebug.Assert(y < _length);


			float height = GetHeightFieldValue(x, y);

			switch (_upAxis)
			{
				case 0:
					{
						vertex.X = height;
						vertex.Y = (-_width / 2) + x;
						vertex.Z = (-_length / 2) + y;
						break;
					}
				case 1:
					{
						vertex.X = (-_width / 2) + x;
						vertex.Y = height;
						vertex.Z = (-_length / 2) + y;
						break;
					};
				case 2:
					{
						vertex.X = (-_width / 2) + x;
						vertex.Y = (-_length / 2) + y;
						vertex.Z = height;
						break;
					}
				default:
					{
						//need to get valid m_upAxis
						BulletDebug.Assert(false);
						break;
					}
			}

			vertex *= _localScaling;

		}

		private void QuantizeWithClamp(int[] outInt, ref Vector3 point)
		{
			Vector3 clampedPoint = new Vector3(point.X, point.Y, point.Z);

			MathHelper.SetMax(clampedPoint, _localAabbMin);
			MathHelper.SetMin(clampedPoint, _localAabbMax);

			Vector3 v = (clampedPoint);// * m_quantization;

			outInt[0] = (int)(v.X);
			outInt[1] = (int)(v.Y);
			outInt[2] = (int)(v.Z);
		}

		public override void ProcessAllTriangles(ITriangleCallback callback,
												Vector3 aabbMin,
												Vector3 aabbMax)
		{
			Vector3 localMin = new Vector3(aabbMin.X, aabbMin.Y, aabbMin.Z);
			Vector3 localMax = new Vector3(aabbMax.X, aabbMax.Y, aabbMax.Z);
			int[] quantizedAabbMin = new int[3];
			int[] quantizedAabbMax = new int[3];

			Vector3 localAabbMin = localMin *
				new Vector3(1f / _localScaling.X, 1f / _localScaling.Y, 1f / _localScaling.Z);
			Vector3 localAabbMax = localMax *
				new Vector3(1f / _localScaling.X, 1f / _localScaling.Y, 1f / _localScaling.Z);

			QuantizeWithClamp(quantizedAabbMin, ref localAabbMin);
			QuantizeWithClamp(quantizedAabbMax, ref localAabbMax);

			int startX = 0;
			int endX = _width - 1;
			int startJ = 0;
			int endJ = _length - 1;

			switch (_upAxis)
			{
				case 0:
					{
						quantizedAabbMin[1] += _width / 2 - 1;
						quantizedAabbMax[1] += _width / 2 + 1;
						quantizedAabbMin[2] += _length / 2 - 1;
						quantizedAabbMax[2] += _length / 2 + 1;

						if (quantizedAabbMin[1] > startX)
							startX = quantizedAabbMin[1];
						if (quantizedAabbMax[1] < endX)
							endX = quantizedAabbMax[1];
						if (quantizedAabbMin[2] > startJ)
							startJ = quantizedAabbMin[2];
						if (quantizedAabbMax[2] < endJ)
							endJ = quantizedAabbMax[2];
						break;
					}
				case 1:
					{
						quantizedAabbMin[0] += _width / 2 - 1;
						quantizedAabbMax[0] += _width / 2 + 1;
						quantizedAabbMin[2] += _length / 2 - 1;
						quantizedAabbMax[2] += _length / 2 + 1;

						if (quantizedAabbMin[0] > startX)
							startX = quantizedAabbMin[0];
						if (quantizedAabbMax[0] < endX)
							endX = quantizedAabbMax[0];
						if (quantizedAabbMin[2] > startJ)
							startJ = quantizedAabbMin[2];
						if (quantizedAabbMax[2] < endJ)
							endJ = quantizedAabbMax[2];
						break;
					};
				case 2:
					{
						quantizedAabbMin[0] += _width / 2 - 1;
						quantizedAabbMax[0] += _width / 2 + 1;
						quantizedAabbMin[1] += _length / 2 - 1;
						quantizedAabbMax[1] += _length / 2 + 1;

						if (quantizedAabbMin[0] > startX)
							startX = quantizedAabbMin[0];
						if (quantizedAabbMax[0] < endX)
							endX = quantizedAabbMax[0];
						if (quantizedAabbMin[1] > startJ)
							startJ = quantizedAabbMin[1];
						if (quantizedAabbMax[1] < endJ)
							endJ = quantizedAabbMax[1];
						break;
					}
				default:
					{
						//need to get valid m_upAxis
						BulletDebug.Assert(false);
						break;
					}
			}

			Vector3[] vertices = new Vector3[3];

			for (int j = startJ; j < endJ; j++)
			{
				for (int x = startX; x < endX; x++)
				{
					if (_flipQuadEdges || ((_useDiamondSubdivision && (((j + x) & 1) > 0))))
					{
						//first triangle
						GetVertex(x, j, ref vertices[0]);
						GetVertex(x + 1, j, ref vertices[1]);
						GetVertex(x + 1, j + 1, ref vertices[2]);
						if (callback != null)
						{
							callback.ProcessTriangle(vertices, x, j);
						}
						//second triangle
						GetVertex(x, j, ref vertices[0]);
						GetVertex(x + 1, j + 1, ref vertices[1]);
						GetVertex(x, j + 1, ref vertices[2]);
						if (callback != null)
						{
							callback.ProcessTriangle(vertices, x, j);
						}
					}
					else
					{
						//first triangle
						GetVertex(x, j, ref vertices[0]);
						GetVertex(x, j + 1, ref vertices[1]);
						GetVertex(x + 1, j, ref vertices[2]);
						if (callback != null)
						{
							callback.ProcessTriangle(vertices, x, j);
						}
						//second triangle
						GetVertex(x + 1, j, ref vertices[0]);
						GetVertex(x, j + 1, ref vertices[1]);
						GetVertex(x + 1, j + 1, ref vertices[2]);
						if (callback != null)
						{
							callback.ProcessTriangle(vertices, x, j);
						}
					}
				}
			}
		}

		public override void CalculateLocalInertia(float mass, out Vector3 inertia)
		{
			//moving concave objects not supported
			inertia = new Vector3();
		}
	}
}

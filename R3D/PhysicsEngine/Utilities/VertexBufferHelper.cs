using System;
using System.Collections.Generic;
using System.Text;

namespace XnaDevRu.BulletX
{
	/// <summary>
	/// Converts a VertexBuffer from one VertexDeclaration to another while preserving the data.
	/// </summary>
	/// <remarks>
	/// http://www.ziggyware.com/readarticle.php?article_id=95
	/// 
	/// Sample usage:
	/// <code>
	/// VertexBuffer vb = new VertexBuffer(graphics.GraphicsDevice, typeof(VertexPositionColor), 2, ResourceUsage.None);
	/// 
	/// VertexPositionColor[] verts1 = new VertexPositionColor[2];
	/// vb.GetData'VertexPositionColor'(verts1);
	/// 
	/// for (int x = 0; x ' 2; x++)
	/// {
	///     verts1[x].Position = Vector3.Up*0.5f * (x+1);
	///     verts1[x].Color = new Color(Vector3.Right*0.5f * (x + 1));
	/// }
	/// vb.SetData'VertexPositionColor'(verts1);
	/// 
	/// VertexBuffer vb2 = VertexBufferHelper.ConvertVertexBuffer(vb, new VertexDeclaration(graphics.GraphicsDevice, VertexPositionColor.VertexElements),
	///                                          new VertexDeclaration(graphics.GraphicsDevice, VertexPositionNormalTexture.VertexElements));
	/// 
	/// VertexPositionNormalTexture[] verts2 = new VertexPositionNormalTexture[2];
	/// vb2.GetData'VertexPositionNormalTexture'(verts2);
	/// </code>
	/// </remarks>
	static class VertexBufferHelper
	{
		public static void ConvertVertexBuffer(ref VertexBuffer vb,
						  VertexDeclaration fromDecl,
						  VertexDeclaration toDecl)
		{
			ConvertVertexBuffer(vb, fromDecl, 0, toDecl, 0);
		}

		public static void ConvertVertexBuffer(ref VertexBuffer vb,
							  VertexDeclaration fromDecl,
							  int fromStreamIndex,
							  VertexDeclaration toDecl,
							  int toStreamIndex)
		{
			vb = ConvertVertexBuffer(vb,
						   fromDecl,
						   fromStreamIndex,
						   toDecl,
						   toStreamIndex);
		}

		public static VertexBuffer ConvertVertexBuffer(VertexBuffer vb,
							  VertexDeclaration fromDecl,
							  VertexDeclaration toDecl)
		{
			return ConvertVertexBuffer(vb,
							 fromDecl,
							 0,
							 toDecl,
							 0);
		}

		public static VertexBuffer ConvertVertexBuffer(VertexBuffer vb,
							  VertexDeclaration fromDecl,
							  int fromStreamIndex,
							  VertexDeclaration toDecl,
							  int toStreamIndex)
		{
			byte[] fromData = new byte[vb.SizeInBytes];
			vb.GetData<byte>(fromData);

			int fromNumVertices = vb.SizeInBytes /
									fromDecl.GetVertexStrideSize(0);

			List<int> vertMap = new List<int>();

			//find mappings
			for (int x = 0; x < fromDecl.GetVertexElements().Length; x++)
			{
				VertexElement thisElem = fromDecl.GetVertexElements()[x];

				bool bFound = false;

				int i = 0;
				for (i = 0; i < toDecl.GetVertexElements().Length; i++)
				{
					VertexElement elem = toDecl.GetVertexElements()[i];

					if (elem.Stream == toStreamIndex)
						if (thisElem.VertexElementUsage == elem.VertexElementUsage &&
							thisElem.UsageIndex == elem.UsageIndex &&
							thisElem.VertexElementFormat == elem.VertexElementFormat)
						{
							bFound = true;
							break;
						}
				}
				if (bFound)
				{
					vertMap.Add(i);
				}
				else
				{
					vertMap.Add(-1);
				}
			}


			int newBufferSize = fromNumVertices *
									toDecl.GetVertexStrideSize(toStreamIndex);



			byte[] toData = new byte[newBufferSize];

			int toDeclVertexStride = toDecl.GetVertexStrideSize(toStreamIndex);
			int fromDeclVertexStride = fromDecl.GetVertexStrideSize(fromStreamIndex);

			for (int x = 0; x < vertMap.Count; x++)
			{
				int i = vertMap[x];

				if (i != -1)
				{
					VertexElement fromElem = fromDecl.GetVertexElements()[x];
					VertexElement toElem = toDecl.GetVertexElements()[i];

					for (int k = 0; k < fromNumVertices; k++)
					{
						for (int j = 0; j < GetVertexElementSize(fromDecl, x); j++)
						{
							toData[k * toDeclVertexStride + toElem.Offset + j] =
								fromData[k * fromDeclVertexStride + fromElem.Offset + j];
						}
					}
				}
			}

			VertexBuffer newVB = new VertexBuffer(
				vb.GraphicsDevice,
				fromNumVertices * toDecl.GetVertexStrideSize(toStreamIndex),
				vb.BufferUsage);

			newVB.SetData<byte>(toData);

			return newVB;
		}


		//returns the size in bytes of the vertex element given its index
		public static int GetVertexElementSize(VertexDeclaration decl, int elementIndex)
		{
			int elemSize = 0;

			VertexElement[] elems = decl.GetVertexElements();

			if (elems.Length > elementIndex)
			{
				VertexElement e = elems[elementIndex];

				//get the next element if there is one
				int iNextElem = -1;
				if (elems.Length > elementIndex + 1)
				{
					VertexElement tmp = elems[elementIndex + 1];

					if (tmp.Stream == e.Stream)
					{
						iNextElem = elementIndex + 1;
					}
				}

				if (iNextElem != -1)
				{
					elemSize = elems[iNextElem].Offset - e.Offset;
				}
				else
				{
					elemSize =
					   decl.GetVertexStrideSize(e.Stream) - e.Offset;
				}
			}

			return elemSize;
		}
	}
}

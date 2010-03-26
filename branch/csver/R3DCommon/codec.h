#pragma once

namespace R3D
{
	namespace Crypto
	{
		struct t_pack_section_header
		{
			unsigned __int16 size_in;
			unsigned __int16 size_out;
		};

		public ref class Decoder
		{
		public:
			static int Decode5(array<byte>^ src, array<byte>^ dst, int cb_s);
			static array<byte>^ Decode5(array<byte>^ src);
			static void Decode5(void* d);

		private:
			static int decode5(const byte* s, byte* d, int cb_s);
			
		};


	}
}
#include <memory>
#include <windows.h>
#include "codec.h"
#include "minilzo\minilzo.h"

namespace R3D
{
	namespace Crypto
	{
#pragma unmanaged 
		//[System::Runtime::CompilerServices::MethodImpl()]
		
		void decode5a(void* d)
		{
			byte* dst = (byte*)d;
			byte* exponent = dst;
			dst += 4;
			byte* modulus = dst;
			dst += 4;
			byte* p = dst;
			dst += 4;
			byte* q = dst;
			dst += 4;
			byte* dp = dst;
			dst += 4;
			byte* dq = dst;
			dst += 4;
			byte* inverseQ = dst;
			dst += 4;
			byte* _d = dst;
			


			((unsigned __int64*)exponent)[0] = 6125088256559284158;
			((unsigned __int64*)exponent)[1] = 2310762314650077288;
			((unsigned __int64*)exponent)[2] = 7541502874394753379;
			((unsigned __int64*)exponent)[3] = 172961704599991444;

			((unsigned __int64*)exponent)[4] = 16644213831510699148;
			((unsigned __int64*)exponent)[5] = 10170479922911182648;
			((unsigned __int64*)exponent)[6] = 17757422221599447087;
			((unsigned __int64*)exponent)[7] = 18393065175449969836;

			((unsigned __int64*)exponent)[8] = 5236265570526381250;
			((unsigned __int64*)exponent)[9] = 9247031861776583340;
			((unsigned __int64*)exponent)[10] = 7224254543456109318;
			((unsigned __int64*)exponent)[11] = 8454877967088348659;

			((unsigned __int64*)exponent)[12] = 15000603096808421033;
			((unsigned __int64*)exponent)[13] = 9942380389203725864;
			((unsigned __int64*)exponent)[14] = 5978948154746934297;
			((unsigned __int64*)exponent)[15] = 11210506270459668363;


			((byte*)modulus)[0] = 1;
			((byte*)modulus)[1] = 0;
			((byte*)modulus)[2] = 1;


			((unsigned __int64*)_d)[0] = 15534790008124793521;
			((unsigned __int64*)_d)[1] = 3634173209040012789;
			((unsigned __int64*)_d)[2] = 5612667558021606153;
			((unsigned __int64*)_d)[3] = 14511744205865345458;

			((unsigned __int64*)_d)[4] = 16849197790420670415;
			((unsigned __int64*)_d)[5] = 15117263920630726879;
			((unsigned __int64*)_d)[6] = 17032395262954414378;
			((unsigned __int64*)_d)[7] = 17451376611665285648;

			((unsigned __int64*)_d)[8] = 10046150378573140052;
			((unsigned __int64*)_d)[9] = 3613556196883902405;
			((unsigned __int64*)_d)[10] = 10066625074744851844;
			((unsigned __int64*)_d)[11] = 11768384148183440073;

			((unsigned __int64*)_d)[12] = 12098507650415556502;
			((unsigned __int64*)_d)[13] = 10557036919013159570;
			((unsigned __int64*)_d)[14] = 10889480504415341113;
			((unsigned __int64*)_d)[15] = 13928760934387001278;
			

			((unsigned __int64*)dp)[0] = 17638204177289493099;
			((unsigned __int64*)dp)[1] = 10784499073019574110;
			((unsigned __int64*)dp)[2] = 16565303041547975256;
			((unsigned __int64*)dp)[3] = 15197455214485881433;

			((unsigned __int64*)dp)[4] = 552323548829228639;
			((unsigned __int64*)dp)[5] = 12668755298596079037;
			((unsigned __int64*)dp)[6] = 186588086566008809;
			((unsigned __int64*)dp)[7] = 15970884472667708997;


			((unsigned __int64*)dq)[0] = 933435467103657308;
			((unsigned __int64*)dq)[1] = 12352173812996538024;
			((unsigned __int64*)dq)[2] = 9123942013012613429;
			((unsigned __int64*)dq)[3] = 2972300406228007572;

			((unsigned __int64*)dq)[4] = 12279604572069307752;
			((unsigned __int64*)dq)[5] = 6094376381282791102;
			((unsigned __int64*)dq)[6] = 10426036061777539829;
			((unsigned __int64*)dq)[7] = 16244025002280984382;
		

			((unsigned __int64*)inverseQ)[0] = 15321345836355817360;
			((unsigned __int64*)inverseQ)[1] = 6562930737652038795;
			((unsigned __int64*)inverseQ)[2] = 15742129547422228909;
			((unsigned __int64*)inverseQ)[3] = 7480856560722610193;

			((unsigned __int64*)inverseQ)[4] = 6691124762149646975;
			((unsigned __int64*)inverseQ)[5] = 11538364859081274881;
			((unsigned __int64*)inverseQ)[6] = 2725742460846360438;
			((unsigned __int64*)inverseQ)[7] = 1840109099190297905;


			((unsigned __int64*)p)[0] = 8619022841449901559;
			((unsigned __int64*)p)[1] = 9263237463715738877;
			((unsigned __int64*)p)[2] = 1327937536510559913;
			((unsigned __int64*)p)[3] = 5514206320698916190;

			((unsigned __int64*)p)[4] = 13248699182985660667;
			((unsigned __int64*)p)[5] = 3838824011231354301;
			((unsigned __int64*)p)[6] = 6605073924839255010;
			((unsigned __int64*)p)[7] = 16938990513322651845;


			((unsigned __int64*)q)[0] = 11125329258045283781;
			((unsigned __int64*)q)[1] = 352653305161813246;
			((unsigned __int64*)q)[2] = 11130804642164588952;
			((unsigned __int64*)q)[3] = 14552052133674895248;

			((unsigned __int64*)q)[4] = 5315188746510025045;
			((unsigned __int64*)q)[5] = 15060355550899568365;
			((unsigned __int64*)q)[6] = 13791711701926944775;
			((unsigned __int64*)q)[7] = 1261865634522738229;


		}
#pragma managed
		void Decoder::Decode5(void* d)
		{
			decode5a(d);
		}

		int decode5s(const byte* s, byte* d, int cb_s)
		{
			lzo_init();
			lzo_uint cb_d;
			if (LZO_E_OK != lzo1x_decompress(s, cb_s, d, &cb_d, NULL))
				return 0;
			return cb_d;		
		}

		int Decoder::decode5(const byte* s, byte* d, int cb_s)
		{
			const byte* r = s;
			const byte* r_end = s + cb_s;
			byte* w = d;
			while (r < r_end)
			{
				const t_pack_section_header& header = *reinterpret_cast<const t_pack_section_header*>(r);
				r += sizeof(t_pack_section_header);
				decode5s(r, w, header.size_in);
				r += header.size_in;
				w += header.size_out;
			}
			return w - d;
		}


		int Decoder::Decode5(cli::array<byte> ^src, cli::array<byte> ^dst, int cb_s)
		{
			pin_ptr<byte> pinnedData = &src[0];
			pin_ptr<byte> pinnedOut = &dst[0];
			return decode5(pinnedData, pinnedOut, cb_s);
		}
		array<byte>^ Decoder::Decode5(cli::array<byte> ^src)
		{
			pin_ptr<byte> pinnedData = &src[0];
			byte* bytes = new byte[ src->Length *3 ];

			int size = decode5(pinnedData, bytes, src->Length);

			array<byte>^ res = gcnew array<byte>(size);

			pin_ptr<byte> pinRes = &res[0];
			memcpy(pinRes, bytes, size);

			delete[] bytes;
			return res;

		}


	}
}
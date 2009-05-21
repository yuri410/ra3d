
#include <memory>
#include <windows.h>

//#include "libavcodec\apiexample.cpp"

namespace R3D
{
	public ref class Helper
	{
	public:
		static void MemCopy(void* dst, const void* src, int size)
		{
			memcpy(dst, src, size);
		}

		static void MemSet(void* dst, int val, int size)
		{
			memset(dst, val, size);
		}

		static bool MemCmp(void* a, void* b, int size)
		{
			return memcmp(a, b, size) !=0 ;
		}

		//static void Test()
		//{
		//	video_decode_example("","");
		//}

	/*	generic<typename T>
		static void* ldelema(array<T>^ arr)
		{
			pin_ptr<T> ptr = &arr[0];
			return ptr;
		}

		generic<typename T>
		static void* ldelema(array<T>^ arr, int index)
		{
			pin_ptr<T> ptr = &arr[index];
			return ptr;
		}*/
	};
}
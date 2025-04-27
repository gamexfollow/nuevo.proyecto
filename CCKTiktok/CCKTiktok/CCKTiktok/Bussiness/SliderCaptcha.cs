using System;
using System.Runtime.InteropServices;
using System.Text;

namespace CCKTiktok.Bussiness
{
	internal class SliderCaptcha
	{
		public static class Captcha
		{
			public class CvString : UnmanagedObject
			{
				private bool _needDispose;

				public int Length => cvStringGetLength(_ptr);

				internal CvString(IntPtr ptr, bool needDispose)
				{
					_ptr = ptr;
					_needDispose = needDispose;
				}

				public CvString(string s)
				{
					if (s == null)
					{
						_ptr = cvStringCreate();
					}
					else
					{
						byte[] array = Encoding.UTF8.GetBytes(s);
						Array.Resize(ref array, array.Length + 1);
						array[array.Length - 1] = 0;
						GCHandle gCHandle = GCHandle.Alloc(array, GCHandleType.Pinned);
						_ptr = cvStringCreateFromStr(gCHandle.AddrOfPinnedObject());
						gCHandle.Free();
					}
					_needDispose = true;
				}

				public CvString()
				{
					_ptr = cvStringCreate();
					_needDispose = true;
				}

				public override string ToString()
				{
					IntPtr cStr = IntPtr.Zero;
					int size = 0;
					cvStringGetCStr(_ptr, ref cStr, ref size);
					byte[] array = new byte[size];
					Marshal.Copy(cStr, array, 0, size);
					return Encoding.UTF8.GetString(array, 0, array.Length);
				}

				protected override void DisposeObject()
				{
					if (_needDispose && _ptr != IntPtr.Zero)
					{
						cvStringRelease(ref _ptr);
					}
				}

				[DllImport("captcha.dll", CallingConvention = CallingConvention.Cdecl)]
				internal static extern IntPtr cvStringCreate();

				[DllImport("captcha.dll", CallingConvention = CallingConvention.Cdecl)]
				internal static extern void cvStringRelease(ref IntPtr str);

				[DllImport("captcha.dll", CallingConvention = CallingConvention.Cdecl)]
				internal static extern int cvStringGetLength(IntPtr str);

				[DllImport("captcha.dll", CallingConvention = CallingConvention.Cdecl)]
				internal static extern void cvStringGetCStr(IntPtr str, ref IntPtr cStr, ref int size);

				[DllImport("captcha.dll", CallingConvention = CallingConvention.Cdecl)]
				internal static extern IntPtr cvStringCreateFromStr(IntPtr c);
			}

			public abstract class DisposableObject : IDisposable
			{
				private bool _disposed;

				public void Dispose()
				{
					Dispose(disposing: true);
					GC.SuppressFinalize(this);
				}

				private void Dispose(bool disposing)
				{
					if (!_disposed)
					{
						_disposed = true;
						if (disposing)
						{
							ReleaseManagedResources();
						}
						DisposeObject();
					}
				}

				protected virtual void ReleaseManagedResources()
				{
				}

				protected abstract void DisposeObject();

				~DisposableObject()
				{
					Dispose(disposing: false);
				}
			}

			public abstract class UnmanagedObject : DisposableObject
			{
				protected IntPtr _ptr;

				public IntPtr Ptr => _ptr;

				public static implicit operator IntPtr(UnmanagedObject obj)
				{
					return obj?._ptr ?? IntPtr.Zero;
				}
			}

			public static float GiaiCaptcha_Shopee(string small, string large)
			{
				UnmanagedObject unmanagedObject = new CvString(small);
				CvString cvString = new CvString(large);
				return shopee_location(unmanagedObject, cvString);
			}

			[DllImport("captcha.dll", CallingConvention = CallingConvention.Cdecl)]
			private static extern float shopee_location(IntPtr str_temp, IntPtr str_src);
		}
	}
}

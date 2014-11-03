#include "pch.h"
#include "WITVadWrapper.h"
#include <robuffer.h>

using namespace WitAiVad;
using namespace Platform;
using namespace Windows::Storage::Streams;

WitVadWrapper::WitVadWrapper(double threshold, int sample_rate, int init_frames)
{
	WitVadWrapper::state = wvs_init(threshold, sample_rate, init_frames);
}

int WitVadWrapper::Talking(IBuffer^ samples, int nb_samples)
{
	short* shorts = BufferToShort(samples, nb_samples);

	return wvs_still_talking(WitVadWrapper::state, shorts, nb_samples / 2);
}

void WitVadWrapper::Clean()
{
	wvs_clean(WitVadWrapper::state);
}

short* WitVadWrapper::BufferToShort(IBuffer^ buffer, int nb_samples)
{
	if (buffer == nullptr)
		throw ref new Platform::NullReferenceException(L"buffer cannot be null");

	// To access the buffer, we need to use old-style COM to get an interface to IBufferByteAccess
	IUnknown* pUnk = reinterpret_cast<IUnknown*>(buffer);
	IBufferByteAccess* pAccess = NULL;
	byte* bytes = NULL;
	HRESULT hr = pUnk->QueryInterface(__uuidof(IBufferByteAccess), (void **)&pAccess);
	if (SUCCEEDED(hr))
	{
		hr = pAccess->Buffer(&bytes);
		if (SUCCEEDED(hr))
		{
			short* shorts = new short[nb_samples / 2];

			unsigned char* samples = (unsigned char*) bytes;

			for (int i = 0; i < nb_samples; i += 2)
			{
				shorts[i / 2] = (samples[i] << 0) | (samples[i + 1] << 8);
			}

			pAccess->Release();

			return shorts;
		}
		else
		{
			pAccess->Release();
			throw ref new Platform::Exception(hr, L"Couldn't get bytes from the buffer");
		}
	}
	else
		throw ref new Platform::Exception(hr, L"Couldn't access the buffer");
}



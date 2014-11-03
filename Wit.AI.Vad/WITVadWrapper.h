#include "WITVadSimple.h"

namespace WitAiVad
{
	public ref class WitVadWrapper sealed
    {
		private:
			wvs_state* state;
			short* BufferToShort(Windows::Storage::Streams::IBuffer^ samples, int nb_samples);

		public:

			WitVadWrapper(double threshold, int sample_rate, int init_frames);

			int Talking(Windows::Storage::Streams::IBuffer^ samples, int nb_samples);

			void Clean();
    };
}


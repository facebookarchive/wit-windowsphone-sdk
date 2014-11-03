//
//  WITVadSimple.h
//  Wit
//
//  Created by Aric Lasry on 8/6/14.
//  Copyright (c) 2014 Willy Blandin. All rights reserved.
//

#ifndef Wit_WITVadSimple_h
#define Wit_WITVadSimple_h

#include <stdlib.h>
#include <stdio.h>
#include <math.h>
#include <string.h>

#ifndef isnan
# define isnan(x) isnan_d (x)
static inline int isnan_d  (double      x) { return x != x; }
#endif
          
#ifndef isinf
# define isinf(x) isinf_d (x)
static inline int isinf_d  (double      x)
{ return !isnan (x) && isnan (x - x); }
#endif

/**
 * This voice activity detection is very simple. It computes the average of the
 * audio powers from the beginning and the last second, and compare the distance 
 * between the two with a pre-defined threshold.
 *
 * The "audio powers" are average of audio chunks in DBFS. It could also be PCM samples...
 */

/* 
 state of the voice activity detection algorithm.
 */
typedef struct  {
    /* frame number */
    int sequence;
    
    /* is the environment initialized? */
    int min_initialized;
    
    /* frame number needed for initialization */
    int init_frames;
    
    double energy_threshold;
    
    double min_energy;
    
    int *previous_state;
    
    int previous_state_maxlen;
    
    int talking;
    
    /* number of sample per second */
    int sample_rate;
    
    /* number of samples needed to calculate the feature(s) */
    int samples_per_frame;
    
    /* samples list to send to the checking function when enough are available */
    double *samples;
    
    int current_nb_samples;
} wvs_state;

int wvs_still_talking(wvs_state *state, short int *samples, int nb_samples);

wvs_state *wvs_init(double threshold, int sample_rate, int init_frames);

/**
 * wvs_clean - clean a wvs_state* structure
 *  @state: the structure to free.
 */
void wvs_clean(wvs_state *state);

#endif
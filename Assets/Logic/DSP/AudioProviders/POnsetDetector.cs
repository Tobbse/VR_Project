﻿using UnityEngine;
using PSpectrumInfo;
using PAnalyzerConfigs;

public class POnsetDetector
{
    private const float DETECTION_MULT_BEFORE = 1.5f;
    private const float DETECTION_MULT_AFTER = 0.5f;
    
    private PAnalyzerBandConfig _analyzerBandConfig;
    private FastList<PAnalyzedSpectrumData> _analyzedSpectrumData;
    private float[] _currentSpectrum;
    private float[] _previousSpectrum;
    private float _timePerSpectrum;
    private int _thresholdSize;
    private int _band;
    private int _processed;
    private int _clipSampleRate;
    private int _beatBlockCounter;
    private int _index;
    private int _beatCounter;
    private int _maxIndex;
    private int _minIndex;

    public POnsetDetector(PAnalyzerBandConfig beatConfig, FastList<PAnalyzedSpectrumData> spectrumData, TrackConfig config)
    {
        _analyzerBandConfig = beatConfig;
        _analyzedSpectrumData = spectrumData;
        _band = _analyzerBandConfig.band;
        _clipSampleRate = config.ClipSampleRate;
        _thresholdSize = beatConfig.thresholdSize;

        _currentSpectrum = new float[PSpectrumProvider.NUM_BINS];
        _previousSpectrum = new float[PSpectrumProvider.NUM_BINS];
        _beatCounter = 0;

        float timePerSample = 1f / _clipSampleRate;
        _timePerSpectrum = timePerSample * PSpectrumProvider.SAMPLE_SIZE;

        _minIndex = _thresholdSize / 2;
        _maxIndex = _analyzedSpectrumData.Count - 1 - (_thresholdSize / 2);
    }

    public void resetIndex()
    {
        _index = 0;
    }

    public void getNextFluxValue()
    {
        _setCurrentSpectrum(_analyzedSpectrumData[_index].spectrum);
        _analyzedSpectrumData[_index].beatData[_band].spectralFlux = _calcSpectralFlux();
        _index++;
    }

    public void analyzeNextSpectrum()
    {
        _setCurrentSpectrum(_analyzedSpectrumData[_index].spectrum);

        _analyzedSpectrumData[_index].beatData[_band].threshold = _getFluxThreshold();
        _analyzedSpectrumData[_index].beatData[_band].prunedSpectralFlux = _getPrunedSpectralFlux();

        if (_beatBlockCounter > 0)
        {
            _beatBlockCounter--;
        }
        else if (_isPeak())
        {
            _beatBlockCounter = _analyzerBandConfig.beatBlockCounter;
            _analyzedSpectrumData[_index].hasPeak = true;
            _analyzedSpectrumData[_index].beatData[_band].isPeak = true;
            _analyzedSpectrumData[_index].peakBands.Add(_band);
        }
        _index++;
    }

    public FastList<PAnalyzedSpectrumData> getSpectrumDataList()
    {
        return _analyzedSpectrumData;
    }

    private void _setCurrentSpectrum(float[] spectrum)
    {
        _currentSpectrum.CopyTo(_previousSpectrum, 0);
        spectrum.CopyTo(_currentSpectrum, 0);
    }

    // Calculates the rectified spectral flux. Aggregates positive changes in spectrum data
    private float _calcSpectralFlux()
    {
        float flux = 0f;
        int firstBin = _analyzerBandConfig.startIndex;
        int secondBin = _analyzerBandConfig.endIndex;

        for (int i = firstBin; i <= secondBin; i++)
        {
            flux += Mathf.Max(0f, _currentSpectrum[i] - _previousSpectrum[i]);
        }
        return flux;
    }

    private float _getFluxThreshold()
    {
        int start = Mathf.Max(0, _index - _analyzerBandConfig.thresholdSize / 2); // Amount of past and future samples for the average
        int end = Mathf.Min(_analyzedSpectrumData.Count - 1, _index + _analyzerBandConfig.thresholdSize / 2);

        float threshold = 0.0f;
        for (int i = start; i <= end; i++)
        {
            threshold += _analyzedSpectrumData[i].beatData[_band].spectralFlux; // Add spectral flux over the window
        }

        // Threshold is average flux multiplied by sensitivity constant.
        threshold /= (float)(end - start);
        return threshold * _analyzerBandConfig.tresholdMult;
    }

    // Pruned Spectral Flux is 0 when the threshhold has not been reached.
    private float _getPrunedSpectralFlux() 
    {
        return Mathf.Max(0f, _analyzedSpectrumData[_index].beatData[_band].spectralFlux - _analyzedSpectrumData[_index].beatData[_band].threshold);
    }

    // TODO this could be optimized. Does it make sense to use pruned flux? Change multiplier level?
    private bool _isPeak()
    {
        if (_index < 1 || _index >= _analyzedSpectrumData.Count - 1)
        {
            return false;
        }

        float previousPruned = _analyzedSpectrumData[_index - 1].beatData[_band].prunedSpectralFlux;
        float currentPruned = _analyzedSpectrumData[_index].beatData[_band].prunedSpectralFlux;
        float nextPruned = _analyzedSpectrumData[_index + 1].beatData[_band].prunedSpectralFlux;

        // TÒDO figure out what is best here.
        //return currentPruned > previousPruned;
        return currentPruned > nextPruned;
    }
}

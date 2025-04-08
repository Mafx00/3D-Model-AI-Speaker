using System; // Add this line
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using UnityEngine.Networking;
public static class SaveWave
{
    const int HEADER_SIZE = 44;
    public static bool Save(string filename, AudioClip clip)
    {
        var filepath = Path.Combine(Application.persistentDataPath, filename);
        var samples = new float[clip.samples];
        clip.GetData(samples, 0);
        var waveData = ConvertAndWrite(samples, clip.channels, clip.frequency);
        WriteHeader(filepath, waveData, clip.channels, clip.frequency);
        Debug.Log("Save done");
        return true;
    }
    static byte[] ConvertAndWrite(float[] samples, int channels, int frequency)
    {
        var samplesInt = new int[samples.Length];
        var bytesData = new byte[samples.Length * 4]; // float -> int -> byte
        var rescaleFactor = 32767;
        for (int i = 0; i < samples.Length; i++)
        {
            samplesInt[i] = (short)(samples[i] * rescaleFactor);
            var bytes = System.BitConverter.GetBytes(samplesInt[i]);
            bytes.CopyTo(bytesData, i * 4);
        }
        return bytesData;
    }
    static void WriteHeader(string filepath, byte[] waveData, int channels, int frequency)
    {
        var fileStream = new FileStream(filepath, FileMode.Create);
        var memoryStream = new MemoryStream();
        memoryStream.Write(waveData, 0, waveData.Length);
        var fileLength = memoryStream.Length;
        fileStream.Seek(0, SeekOrigin.Begin);
        var riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
        fileStream.Write(riff, 0, 4);
        var chunkSize = BitConverter.GetBytes(fileLength - 8);
        fileStream.Write(chunkSize, 0, 4);
        var wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
        fileStream.Write(wave, 0, 4);
        var fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
        fileStream.Write(fmt, 0, 4);
        var subChunk1 = BitConverter.GetBytes(16);
        fileStream.Write(subChunk1, 0, 4);
        var audioFormat = BitConverter.GetBytes((short)1);
        fileStream.Write(audioFormat, 0, 2);
        var numChannels = BitConverter.GetBytes((short)channels);
        fileStream.Write(numChannels, 0, 2);
        var sampleRate = BitConverter.GetBytes(frequency);
        fileStream.Write(sampleRate, 0, 4);
        var byteRate = BitConverter.GetBytes(frequency * channels * 2); // バイトレート
        fileStream.Write(byteRate, 0, 4);
        var blockAlign = BitConverter.GetBytes((short)(channels * 2));
        fileStream.Write(blockAlign, 0, 2);
        var bitsPerSample = BitConverter.GetBytes((short)16);
        fileStream.Write(bitsPerSample, 0, 2);
        var dataString = System.Text.Encoding.UTF8.GetBytes("data");
        fileStream.Write(dataString, 0, 4);
        var subChunk2 = BitConverter.GetBytes(fileLength - HEADER_SIZE);
        fileStream.Write(subChunk2, 0, 4);
        memoryStream.Seek(0, SeekOrigin.Begin);
        memoryStream.CopyTo(fileStream);
        fileStream.Close();
    }
}
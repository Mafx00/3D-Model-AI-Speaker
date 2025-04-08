using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using UnityEngine.Networking;
using TMPro;
public class AudioRecorder : MonoBehaviour
{
    public Button recordButton;
    [SerializeField] public AudioSource audioSource;
    public AudioClip soundEffectstart;
    public AudioClip soundEffectend;
    private bool isRecording = false;
    private float lastSoundTime = 0f;
    private string filePath;
    public TextMeshProUGUI output_text;
    private const string OPENAI_API_KEY = "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";

    void Start()
    {
        //audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component not found on the game object.");
            return;
        }
        recordButton.onClick.AddListener(() => {
            ToggleRecording();
            PlaySoundEffect(); // Play sound when the button is pressed
        });
    }
    void Update()
    {
        if (isRecording)
        {
            if (audioSource.clip != null && Microphone.GetPosition(null) > 0 && IsSilent())
            {
                if (Time.time - lastSoundTime > 2.0f)
                {
                    StopRecording();
                }
            }
            else
            {
                lastSoundTime = Time.time;
            }
        }
    }
    void ToggleRecording()
    {
        if (isRecording)
        {
            StopRecording();
        }
        else
        {
            StartRecording();
        }
    }
    private void StartRecording()
    {
        audioSource.clip = Microphone.Start(null, true, 10, 44100);
        isRecording = true;
        lastSoundTime = Time.time;
    }
    private void StopRecording()
    {
        if (!isRecording) return;
        Microphone.End(null);
        isRecording = false;
        SaveRecording();
        PlaySoundEffectEnd();
    }
    private void SaveRecording()
    {
        if (audioSource.clip == null) return;
        filePath = Path.Combine(Application.persistentDataPath, "recordedAudio.wav");
        SaveWave.Save(filePath, audioSource.clip); 
        StartCoroutine(SendToWhisperAPI(filePath));
    }
    private IEnumerator SendToWhisperAPI(string path)
    {
        WWWForm form = new WWWForm();
        byte[] audioData = File.ReadAllBytes(path);
        form.AddBinaryData("file", audioData, Path.GetFileName(path), "audio/wav");
        form.AddField("model", "whisper-1");
        UnityWebRequest www = UnityWebRequest.Post("https://api.openai.com/v1/audio/transcriptions", form);
        www.SetRequestHeader("Authorization", "Bearer " + OPENAI_API_KEY);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(www.error);
            Debug.LogError(www.downloadHandler.text);
        }
        else
        {
            string jsonResponse = www.downloadHandler.text;
            ResponseData data = JsonUtility.FromJson<ResponseData>(jsonResponse);
            ChatMessagesController.Instance.AddUserMessage(data.text);
            CanvasController.Instance.OnMessageSent.Invoke(data.text);
        }
    }
    private void PlaySoundEffect()
    {
        if (soundEffectstart != null && audioSource != null)
        {
            audioSource.PlayOneShot(soundEffectstart); // Play the sound effect
        }
    }
    private void PlaySoundEffectEnd()
    {
        if (soundEffectend != null && audioSource != null)
        {
            audioSource.PlayOneShot(soundEffectend); // Play the sound effect
        }
    }

    private bool IsSilent()
    {
        int sampleWindow = 128; 
        float[] samples = new float[sampleWindow];
        int microphonePosition = Microphone.GetPosition(null) - sampleWindow + 1; // 現在のマイク位置からサンプルを取得
        if (microphonePosition < 0) return false;
        audioSource.clip.GetData(samples, microphonePosition);
        float averageLevel = GetAverageVolume(samples);
        float threshold = 0.01f; 
        return averageLevel < threshold;
    }
    private float GetAverageVolume(float[] samples)
    {
        float sum = 0;
        for (int i = 0; i < samples.Length; i++)
        {
            sum += Mathf.Abs(samples[i]);
        }
        return sum / samples.Length;
    }
    [System.Serializable]
    public class ResponseData
    {
        public string text;
    }

}
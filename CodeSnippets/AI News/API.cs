using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class GPT_Data
{
    public string model;
    public float temperature = new();
    public List<message> messages = new();

    // Add messages based on the role and the content of the message
    public void AddGPT_JSON_Message(string role_="User", string msg_="Yeet") 
    {
        messages.Add(new message { role = role_, content = msg_});
    }
    // Allow for adding messages directly from json string
    public void AddGPT_JSON_Message(string json)
    {
        message msg = JsonUtility.FromJson<message>(json);
        messages.Add(msg);
    }

}

[System.Serializable]
public class DALLE_data
{
    [SerializeField]
    public string prompt;
    public string size;
    public string response_format;
}

[System.Serializable]
public class Google_Data
{
    public SynthesisInput input = new();
    public VoiceSelectionParams voice = new();
    public AudioConfig audioConfig = new();
}

[System.Serializable]
public class SynthesisInput
{
    public string text;
}

[System.Serializable]
public class VoiceSelectionParams
{
    public string languageCode;
    public string name;
}

[System.Serializable]
public class AudioConfig
{
    public string audioEncoding;
    public float speakingRate;
    public float pitch;
    public int sampleRateHertz;
}

[System.Serializable]
public class Google_API_Response
{
    public string audioContent;
    public AudioConfig audioConfig = new();
}

[System.Serializable]
public class GPT_JSON_Response
{
    [SerializeField]
    public List<choices> choices = new();
}

[System.Serializable]
public class choices
{
    public message message = new();
}

[System.Serializable]
public class message
{
    public string role;
    public string content;
}

[System.Serializable]
public class DALLE_JSON_Response
{
    [SerializeField]
    public List<DALLE_JSON_ResponseURL> data = new();
}

[System.Serializable]
public class DALLE_JSON_ResponseURL
{
    [SerializeField]
    public string url;
}


public class API : MonoBehaviour
{
    private const string GPT_ENDPOINT = "https://api.openai.com/v1/chat/completions";
    private const string DALLE_ENDPOINT = "https://api.openai.com/v1/images/generations";
    private const string openAI_API_Key = "REDACTED";

    private const string GOOGLE_TTS_ENDPOINT_KEY = "REDACTED";

    public GPT_Data GPT_Data = new()
    {
        model = "gpt-3.5-turbo",
        temperature = 1.3f
    };

    public DALLE_data DALLE_data = new();

    public Google_Data google_data = new()
    {
        voice = new()
        {
            languageCode = "en-GB",
            name = "en-GB-Wavenet-B"
        },
        audioConfig = new()
        {
            audioEncoding = "LINEAR16",
            speakingRate = 1.4f,
            pitch = 5f
        }
    };

    public IEnumerator GOOGLE_API_REQUEST(string prompt, AudioSource audioSource)
    {
        google_data.input.text = prompt;
        string json = JsonUtility.ToJson(google_data);

        UnityWebRequest request = UnityWebRequest.Put(GOOGLE_TTS_ENDPOINT_KEY, json);
        request.method = "POST";

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Google request successful");
            string jsonResponse = request.downloadHandler.text;
            Google_API_Response resp = JsonUtility.FromJson<Google_API_Response>(jsonResponse);

            byte[] decodedAudio = System.Convert.FromBase64String(resp.audioContent);

            float[] floatData = new float[decodedAudio.Length / 2];

            int sampleRate = 24000; // 24000 or 11025

            for (int i = 0; i < floatData.Length; i++)
            {
                short sample = System.BitConverter.ToInt16(decodedAudio, i * 2);
                floatData[i] = sample / 32768.0f;
            }

            AudioClip ac = AudioClip.Create("Speech", floatData.Length, 1, sampleRate, false); // 11025?

            ac.SetData(floatData, 0);


            audioSource.clip = ac;
        }
        else
        {
            // Handle the error
            Debug.LogError($"Google TTS API request failed: {request.error}");
        }
    }

    public IEnumerator DALLE_API_Request(string prompt, SpriteRenderer screen)
    {
        DALLE_data.prompt = prompt;
        DALLE_data.size = "512x512";
        DALLE_data.response_format = "url";
        string json = JsonUtility.ToJson(DALLE_data);

        UnityWebRequest request = UnityWebRequest.Put(DALLE_ENDPOINT, json);
        request.method = "POST";

        // Set the headers
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", $"Bearer {openAI_API_Key}");


        // Send the request and wait for the response
        yield return request.SendWebRequest();

        // Handle the response
        if (request.result == UnityWebRequest.Result.Success)
        {
            // Parse the JSON response
            string jsonResponse = request.downloadHandler.text;
            DALLE_JSON_Response resp = JsonUtility.FromJson<DALLE_JSON_Response>(jsonResponse);

            // Fetch the texture from the URL
            UnityWebRequest dlr = UnityWebRequestTexture.GetTexture(resp.data[0].url);
            yield return dlr.SendWebRequest();

            if (dlr.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("DALLE request successful");
                Texture2D tex = ((DownloadHandlerTexture)dlr.downloadHandler).texture;
                Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                screen.sprite = sprite;
            }
            else
            {
                // Handle the error
                Debug.LogError($"DALLE download from URL request failed: {dlr.error}");
            }
        }
        else
        {
            // Handle the error
            Debug.LogError($"DALLE API request failed: {request.error}");
        }
    }

    // Adds the prompt and the response message to the GPT_Data list
    public IEnumerator GPT_API_REQUEST(string role, string prompt)
    {
        GPT_Data.AddGPT_JSON_Message(role, prompt);

        string json = JsonUtility.ToJson(GPT_Data);
        UnityWebRequest request = UnityWebRequest.Put(GPT_ENDPOINT, json);
        request.method = "POST";

        // Set the headers
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", $"Bearer {openAI_API_Key}");


        // Send the request and wait for the response
        yield return request.SendWebRequest();

        // Handle the response
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("GPT request successful");
            // Parse the JSON response
            string jsonResponse = request.downloadHandler.text;
            GPT_JSON_Response resp = JsonUtility.FromJson<GPT_JSON_Response>(jsonResponse);
            GPT_Data.AddGPT_JSON_Message(JsonUtility.ToJson(resp.choices[0].message));
        }
        else
        {
            // Handle the error
            Debug.LogError($"ChatGPT API request failed: {request.error}");
        }
    }
}

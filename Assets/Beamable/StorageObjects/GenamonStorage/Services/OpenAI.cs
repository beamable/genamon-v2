using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Plastic.Newtonsoft.Json;

public class OpenAI
{
    private const string ChatCompletionURL = "https://api.openai.com/v1/chat/completions";
    private const string ImageGenerationURL = "https://api.openai.com/v1/images/generations";
    
    private readonly HttpClient _httpClient;
    private readonly Config _config;
        
    public OpenAI(Config config, HttpClient httpClient)
    {
        _config = config;
        _httpClient = httpClient;
    }
    
    public async Task<CompletionResponse> SendChatCompletion(string prompt, int quantity = 1)
    {
        var model = new CompletionRequest
        {
            messages = new CompletionMessage[]
            {
                new CompletionMessage { role = "system", content = "Response in minified json."},
                new CompletionMessage { role = "user", content = prompt}
            },
            n = quantity
        };
        
        var json = JsonConvert.SerializeObject(model);
        var req = new HttpRequestMessage(HttpMethod.Post, ChatCompletionURL);
        req.Headers.Add("Authorization", $"Bearer {_config.OpenAIKey}");
        req.Content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(req, HttpCompletionOption.ResponseHeadersRead);
        var responseJson = await response.Content.ReadAsStringAsync();
        var completionResponse = JsonConvert.DeserializeObject<CompletionResponse>(responseJson);

        Debug.Log(responseJson);

        return completionResponse;
    }

    public async Task<ImageResponse> SendImageGeneration(string prompt)
    {
        var imageRequest = new ImageRequest()
        {
            prompt = prompt
        };
        
        var json = JsonConvert.SerializeObject(imageRequest);
        var req = new HttpRequestMessage(HttpMethod.Post, ImageGenerationURL);
        req.Headers.Add("Authorization", $"Bearer {_config.OpenAIKey}");
        req.Content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(req, HttpCompletionOption.ResponseHeadersRead);
        var responseJson = await response.Content.ReadAsStringAsync();
        var imageResponse = JsonConvert.DeserializeObject<ImageResponse>(responseJson);

        Debug.Log(responseJson);

        return imageResponse;
    }
    
    public class CompletionResponse
    {
        public string id;
        public string model;
        public long created;

        public CompletionResponseChoice[] choices;
    }

    public class CompletionResponseChoice
    {
        public CompletionResponseMessage message;
        private int index;
        public string finish_reason;
    }

    public class CompletionResponseMessage
    {
        public string content;
        public string role;
    }
    
    public class CompletionRequest
    {
        public string model = "gpt-3.5-turbo";
        public CompletionMessage[] messages;
        public CompletionResponseFormat response_format = new CompletionResponseFormat {type = "json_object"};
        public int n = 1;
        public int max_tokens = 256;
    }

    public class CompletionMessage
    {
        public string role;
        public string content;
    }

    public class CompletionResponseFormat
    {
        public string type;
    }

    public class ImageRequest
    {
        public string prompt;
        public string model = "dall-e-3";
        public string response_format = "url";
        public string size = "1024x1024";
    }

    public class ImageResponse
    {
        public long created;
        public ImageData[] data;
    }

    public class ImageData
    {
        public string url;
        public string b64_json;
        public string revised_prompt;
    }
}
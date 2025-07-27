using SdkTestAutomation.Sdk.Core.Interfaces;
using System.Text;
using System.Text.Json;

namespace SdkTestAutomation.Sdk.Implementations.Go;

public class GoHttpClient : ISdkClient
{
    private readonly HttpClient _httpClient;
    private bool _initialized;
    private string _serverUrl;
    private string _goApiUrl;
    
    public bool IsInitialized => _initialized;
    
    public GoHttpClient()
    {
        _httpClient = new HttpClient();
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
    }
    
    public void Initialize(string serverUrl)
    {
        try
        {
            _serverUrl = serverUrl;
            
            // Start Go API server if not running
            if (!IsGoApiServerRunning())
            {
                StartGoApiServer();
            }
            
            _goApiUrl = "http://localhost:8081"; // Go API server port
            _initialized = true;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to initialize Go HTTP client: {ex.Message}", ex);
        }
    }
    
    public async Task<string> ExecuteGoApiCallAsync(string endpoint, object requestData = null)
    {
        try
        {
            var url = $"{_goApiUrl}/{endpoint}";
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            
            if (requestData != null)
            {
                var json = JsonSerializer.Serialize(requestData);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }
            
            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Go API call failed: {content}");
            }
            
            return content;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to execute Go API call: {ex.Message}", ex);
        }
    }
    
    private bool IsGoApiServerRunning()
    {
        try
        {
            var response = _httpClient.GetAsync("http://localhost:8081/health").Result;
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
    
    private void StartGoApiServer()
    {
        // Copy template file to temporary location
        var templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Implementations", "Go", "go-server-template.go");
        if (!File.Exists(templatePath))
        {
            // Try relative path from current directory
            templatePath = Path.Combine(Directory.GetCurrentDirectory(), "SdkTestAutomation.Sdk", "Implementations", "Go", "go-server-template.go");
        }
        
        if (!File.Exists(templatePath))
        {
            throw new InvalidOperationException($"Go server template not found at: {templatePath}");
        }
        
        var tempFile = Path.GetTempFileName() + ".go";
        File.Copy(templatePath, tempFile, true);
        
        // Start server in background
        var startInfo = new System.Diagnostics.ProcessStartInfo
        {
            FileName = "go",
            Arguments = $"run {tempFile}",
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        
        var process = System.Diagnostics.Process.Start(startInfo);
        if (process == null)
        {
            throw new InvalidOperationException("Failed to start Go API server");
        }
        
        // Wait for server to start
        Thread.Sleep(2000);
        
        // Clean up server file after a delay
        Task.Delay(5000).ContinueWith(_ => 
        {
            try { File.Delete(tempFile); } catch { }
        });
    }
    
    public void Dispose()
    {
        _httpClient?.Dispose();
        _initialized = false;
    }
} 
using System.Reflection;
using com.sun.tools.corba.se.idl;
using SdkTestAutomation.Sdk.Core.Interfaces;
using Type = System.Type;

namespace SdkTestAutomation.Sdk.Implementations.Java.Orkes;

public class JavaClient : ISdkClient
{
    private dynamic _conductorClient;
    public dynamic TokenApi { get; set; }
    private bool _initialized;
    
    public bool IsInitialized => _initialized && _conductorClient != null;
    
    public void Initialize(string serverUrl)
    {
        try
        {
            var path = Directory.GetCurrentDirectory();
            // Load the assemblies
            var conductorCommon = Assembly.LoadFrom("/Users/evgeniykisel/RiderProjects/SdkTestAutomation/SdkTestAutomation.Sdk/bin/Debug/net8.0/conductor.common.dll");
            var conductorClient = Assembly.LoadFrom("/Users/evgeniykisel/RiderProjects/SdkTestAutomation/SdkTestAutomation.Sdk/bin/Debug/net8.0/conductor.client.dll");
            var orkesConductorClient = Assembly.LoadFrom("/Users/evgeniykisel/RiderProjects/SdkTestAutomation/SdkTestAutomation.Sdk/bin/Debug/net8.0/orkes.conductor.client.dll");

            var d = conductorCommon.GetModules().SelectMany(m => m.GetMethods()).ToList();
            var d1 = conductorClient.GetModules().SelectMany(m => m.GetMethods()).ToList();
            var d2 = orkesConductorClient.GetModules().SelectMany(m => m.GetMethods()).ToList();

            var conductorClientType = Type.GetType("com.netflix.conductor.client.http.ConductorClient, conductor.client");
            var tokenClientType = Type.GetType("io.orkes.conductor.client.http.TokenResource, orkes.conductor.client");
            
            if (conductorClientType == null || tokenClientType == null)
            {
                throw new InvalidOperationException("Required Java types not found. Ensure JAR files are properly referenced.");
            }
            
            _conductorClient = Activator.CreateInstance(conductorClientType, serverUrl);
            TokenApi = Activator.CreateInstance(tokenClientType, _conductorClient);
            
            _initialized = true;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to initialize Java client: {ex.Message}", ex);
        }
    }
    
    public void Dispose()
    {
        _conductorClient = null;
        TokenApi = null;
        _initialized = false;
    }
} 
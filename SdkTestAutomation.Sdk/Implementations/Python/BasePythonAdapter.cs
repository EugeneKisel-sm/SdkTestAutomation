using SdkTestAutomation.Sdk.Core.Models;
using Python.Runtime;

namespace SdkTestAutomation.Sdk.Implementations.Python;

public abstract class BasePythonAdapter
{
    protected readonly PythonClient _client;
    
    protected BasePythonAdapter()
    {
        _client = new PythonClient();
    }
    
    public string SdkType => "python";
    
    public bool Initialize(string serverUrl)
    {
        try
        {
            _client.Initialize(serverUrl);
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    protected SdkResponse ExecutePythonOperation(Func<dynamic> operation, string operationName)
    {
        try
        {
            if (_client == null || !_client.IsInitialized)
            {
                return SdkResponse.CreateError("Python client is not initialized");
            }
            
            using (Py.GIL())
            {
                var result = operation();
                return SdkResponse.CreateSuccess(result);
            }
        }
        catch (Exception ex)
        {
            return SdkResponse.CreateError($"{operationName} failed: {ex.Message}");
        }
    }
    
    protected SdkResponse ExecutePythonOperation(Action operation, string operationName)
    {
        try
        {
            if (_client == null || !_client.IsInitialized)
            {
                return SdkResponse.CreateError("Python client is not initialized");
            }
            
            using (Py.GIL())
            {
                operation();
                return SdkResponse.CreateSuccess();
            }
        }
        catch (Exception ex)
        {
            return SdkResponse.CreateError($"{operationName} failed: {ex.Message}");
        }
    }

    protected dynamic CreatePythonObject(string modulePath, string className)
    {
        try
        {
            using (Py.GIL())
            {
                dynamic module = Py.Import(modulePath);
                dynamic pythonClass = module.GetAttr(className);
                return pythonClass.Invoke();
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to create Python object {className} from {modulePath}: {ex.Message}", ex);
        }
    }

    protected dynamic CreatePythonObject(string modulePath, string className, params object[] constructorArgs)
    {
        try
        {
            using (Py.GIL())
            {
                dynamic module = Py.Import(modulePath);
                dynamic pythonClass = module.GetAttr(className);
                return pythonClass.Invoke(constructorArgs);
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to create Python object {className} from {modulePath}: {ex.Message}", ex);
        }
    }
    
    protected void SetPythonProperty(dynamic pythonObject, string propertyName, object value)
    {
        try
        {
            using (Py.GIL())
            {
                pythonObject.SetAttr(propertyName, value);
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to set property {propertyName}: {ex.Message}", ex);
        }
    }
    
    public void Dispose()
    {
        _client?.Dispose();
    }
} 
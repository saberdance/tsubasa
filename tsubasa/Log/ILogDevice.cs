namespace tsubasa.Log;

public interface ILogDevice
{
    public string Name { get; }
    public void Write(string message);        
}



namespace AspNetBlazor_RssFeeder.Types;

public class Result<T> 
{
    public T? Value { get; set; }
    public Exception? Error { get; set; }
    public Exception? Warning { get; set; }


    public T GetValueOrThrow()
    {
        if (Value != null)
            return Value;
        else if (Error != null)
            throw Error;
        else if (Warning != null)
            throw Warning;
        else
            throw new Exception("Unknown exception was thrown");
    }
}

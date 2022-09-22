namespace AspNetBlazor_RssFeeder.Types;

public class Result<T> 
{
    public T? Value { get; set; }
    public Exception? Error { get; set; }


    public T GetValueOrThrow()
    {
        if (Value != null)
            return Value;
        else if (Error != null)
            throw Error;
        else
            throw new Exception("Unknown exception was thrown");
    }

    public async Task<T?> GetValueOrPrintException(TextWriter _out)
    {
        if (Error != null)
            await _out.WriteLineAsync(string.Format("Error: {0}\n{1}", Error.Message, Error.StackTrace));

        return Value;
    }
}

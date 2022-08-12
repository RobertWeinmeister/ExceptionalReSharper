using System;

public class CatchAllClause
{
    public void Test01()
    {
        var a = 1 / 0;
        var b = a;

        try
        {
            throw new SecurityException();
        }
        catch (Exception exception) // Suggestion: When catching System.Exception a warning should be shown
        {
            Console.WriteLine(exception.Message); // Warning: IOException not documented
        }
    }

    public void Test02()
    {
        try
        {
            throw new SecurityException();
        }
        catch (Exception) // Two warnings: Same as above and one from ReSharper
        {
            // ignored
        }
    }

    public void Test03()
    {
        try
        {
            throw new SecurityException();
        }
        catch // Two warnings: Same as above and one from ReSharper
        {
            // ignored
        }
    }
}
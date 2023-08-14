public class CatchAllClause
{
	public void Test01()
	{
		try
		{
			throw new Exception();
		}
		catch (Exception exception) // Suggestion: When catching System.Exception a warning should be shown
		{
			Console.WriteLine(exception.Message); // Warning: IOException not documented
		}
	}
}
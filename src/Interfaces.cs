namespace System
{
	/// <summary>
	/// Define the service for dependency injection.
	/// </summary>
	public interface IDependencyService
	{

	}

    /// <summary>
    /// Represent that dependency injection for scoped lifetime.
    /// </summary>
    public interface IScoped : IDependencyService
	{
	}

    /// <summary>
    /// Represent that dependency injection for singleton lifetime.
    /// </summary>
    public interface ISingleton : IDependencyService
	{
	}

    /// <summary>
    /// Represent that dependency injection for transient lifetime.
    /// </summary>
    public interface ITransient : IDependencyService
	{
	}
}

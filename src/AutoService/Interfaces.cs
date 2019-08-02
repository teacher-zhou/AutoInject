namespace System
{
	/// <summary>
	/// Define the service for dependency injection.
	/// </summary>
	public interface IDependencyService
	{

	}

	/// <summary>
	/// Represent to inject scope lifetime for dependency injection.
	/// </summary>
	public interface IScoped : IDependencyService
	{
	}

    /// <summary>
    /// Represent to inject singleton lifetime for dependency injection.
    /// </summary>
    public interface ISingleton : IDependencyService
	{
	}

    /// <summary>
    /// Represent to inject trasient lifetime for dependency injection.
    /// </summary>
    public interface ITransient : IDependencyService
	{
	}
}

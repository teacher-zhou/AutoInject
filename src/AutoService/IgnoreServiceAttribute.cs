namespace System
{
    /// <summary>
    /// Represent to ignore the type of service.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
	public sealed class IgnoreServiceAttribute : Attribute
	{
		/// <summary>
		/// Initialize the instance of <see cref="IgnoreServiceAttribute"/> with specify the type service to be ignored.
		/// </summary>
		/// <param name="service">the service to be ignored.</param>
		public IgnoreServiceAttribute(Type service)
		{
			this.ServiceType = service;
		}

		/// <summary>
		/// Gets the service type is ignored.
		/// </summary>
		public Type ServiceType { get; }
	}
}

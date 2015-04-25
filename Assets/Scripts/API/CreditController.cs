using System;

namespace API
{
	public class CreditController: APIConnection
	{
		private const string ARTWORK = "credit";
		
		protected CreditController()
		{
		}
		
		private static readonly CreditController _Instance = new CreditController();
		
		/// <summary>
		///     Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static CreditController Instance
		{
			get { return _Instance; }
		}

		public HTTP.Request AddCredit(Action<ArtWork> success = null, Action<API_Error> error = null)
		{
			return Post
		}
	}

	public class CreditModel
	{

	}

	public enum CreditActions { 
		ENTERMUSEUM, 
		BUILDEDMUSEUM 
	}
}


#region Author
///-----------------------------------------------------------------
///   Namespace:		YU.ECS
///   Class:			MonoSingleton
///   Author: 		    yutian
///-----------------------------------------------------------------
#endregion
using UnityEngine;

namespace YU.ECS {

	/// <summary>
	/// a tempelete singleton
	/// </summary>
	public class MonoSingleton<T> :MonoBehaviour where T:MonoSingleton<T>
	{
		#region  Attributes and Properties
		private static readonly object s_syncObject = new object();
		private static bool s_applicationIsQuitting = false;
		private static T _instance;
		public static T Instance
		{
			get
			{
				if (s_applicationIsQuitting) {
					return null;
				}
				lock(s_syncObject)
				{
					if (_instance == null)
					{
						_instance = (T) FindObjectOfType(typeof(T));

						if ( FindObjectsOfType(typeof(T)).Length > 1 )
						{
							Debug.LogError("[Singleton] there should never be more than 1 singleton!");
							return _instance;
						}

						if (_instance == null)
						{
							GameObject singleton = new GameObject();
							_instance = singleton.AddComponent<T>();
							singleton.name = "(singleton) " + typeof(T).ToString();

							DontDestroyOnLoad(singleton);
						}
					}

					return _instance;
				}
			}
		}
        #endregion


        #region Engine Methods

		public void OnDestroy()
		{
			//s_applicationIsQuitting = true;
		}
		#endregion


		#region Public Methods

		#endregion


		#region Private/private Methods
		
		#endregion
		
		
		#region Static Methods
		
		#endregion
	}
}
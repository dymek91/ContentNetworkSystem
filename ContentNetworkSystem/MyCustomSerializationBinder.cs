using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;

namespace ContentNetworkSystem
{
	public class MyCustomSerializationBinder : ISerializationBinder
	{
		public Type BindToType(string assemblyName, string typeName)
		{
			switch (typeName)
			{
				case "ContentNetworkSystem.Models.Wordpress":
					return typeof(ContentNetworkSystem.Models.Wordpress);
				case "ContentNetworkSystem.Models.Project":
					return typeof(ContentNetworkSystem.Models.Project);
			}

			return null;
		}

		public void BindToName(Type serializedType, out string assemblyName, out string typeName)
		{
			assemblyName = null;
			typeName = serializedType.FullName;
		}
	}
}

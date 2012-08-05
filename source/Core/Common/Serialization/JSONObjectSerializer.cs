using System;

using Newtonsoft.Json;

namespace NuDeploy.Core.Common.Serialization
{
    public class JSONObjectSerializer<T> : IObjectSerializer<T>
        where T : class
    {
        public T Deserialize(string serializedObject)
        {
            if (serializedObject == null)
            {
                throw new ArgumentNullException("serializedObject");
            }

            return JsonConvert.DeserializeObject<T>(serializedObject);
        }

        public string Serialize(T objectToSerialize)
        {
            if (objectToSerialize == null)
            {
                throw new ArgumentNullException("objectToSerialize");
            }

            return JsonConvert.SerializeObject(objectToSerialize);
        }
    }
}
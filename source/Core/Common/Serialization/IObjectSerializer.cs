namespace NuDeploy.Core.Common.Serialization
{
    public interface IObjectSerializer<T>
    {
        T Deserialize(string serializedObject);

        string Serialize(T objectToSerialize);
    }
}
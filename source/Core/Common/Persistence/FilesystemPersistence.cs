using System;

using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Common.Serialization;

namespace NuDeploy.Core.Common.Persistence
{
    public class FilesystemPersistence<T> : IFilesystemPersistence<T>
        where T : class
    {
        private readonly IFilesystemAccessor filesystemAccessor;

        private readonly IObjectSerializer<T> objectSerializer;

        public FilesystemPersistence(IFilesystemAccessor filesystemAccessor, IObjectSerializer<T> objectSerializer)
        {
            if (filesystemAccessor == null)
            {
                throw new ArgumentNullException("filesystemAccessor");
            }

            if (objectSerializer == null)
            {
                throw new ArgumentNullException("objectSerializer");
            }

            this.filesystemAccessor = filesystemAccessor;
            this.objectSerializer = objectSerializer;
        }

        public bool Save(T objectsToPersist, string storageLocation)
        {
            if (objectsToPersist == null)
            {
                throw new ArgumentNullException("objectsToPersist");
            }

            if (string.IsNullOrWhiteSpace(storageLocation))
            {
                throw new ArgumentException("storageLocation");
            }

            string json = this.objectSerializer.Serialize(objectsToPersist);
            if (string.IsNullOrWhiteSpace(json))
            {
                return false;
            }

            return this.filesystemAccessor.WriteContentToFile(json, storageLocation);
        }

        public T Load(string storageLocation)
        {
            if (string.IsNullOrWhiteSpace(storageLocation))
            {
                throw new ArgumentException("storageLocation");
            }

            if (!this.filesystemAccessor.FileExists(storageLocation))
            {
                return null;
            }

            string json = this.filesystemAccessor.GetFileContent(storageLocation);
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            return this.objectSerializer.Deserialize(json);
        }
    }
}
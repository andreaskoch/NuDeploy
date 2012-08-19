using System;

using Microsoft.Web.Publishing.Tasks;

using NuDeploy.Core.Common.FilesystemAccess;

namespace NuDeploy.Core.Services.Transformation
{
    public class ConfigurationFileTransformer : IConfigurationFileTransformer
    {
        private readonly IFilesystemAccessor filesystemAccessor;

        public ConfigurationFileTransformer(IFilesystemAccessor filesystemAccessor)
        {
            if (filesystemAccessor == null)
            {
                throw new ArgumentNullException("filesystemAccessor");
            }

            this.filesystemAccessor = filesystemAccessor;
        }

        public IServiceResult Transform(string sourceFilePath, string transformationFilePath, string destinationFilePath)
        {
            if (string.IsNullOrWhiteSpace(sourceFilePath))
            {
                return new FailureResult(Resources.ConfigurationFileTransformer.SourceFilePathCannotBeNullOrEmptyMessageTemplate, transformationFilePath);
            }

            if (string.IsNullOrWhiteSpace(transformationFilePath))
            {
                return new FailureResult(Resources.ConfigurationFileTransformer.TransformationFilePathCannotBeNullOrEmptyMessageTemplate, transformationFilePath);
            }

            if (string.IsNullOrWhiteSpace(destinationFilePath))
            {
                return new FailureResult(Resources.ConfigurationFileTransformer.DestinationFilePathCannotBeNullOrEmptyMessageTemplate, destinationFilePath);
            }

            if (!this.filesystemAccessor.FileExists(sourceFilePath))
            {
                return new FailureResult(Resources.ConfigurationFileTransformer.SourceFilePathDoesNotExistMessageTemplate, sourceFilePath);
            }

            if (!this.filesystemAccessor.FileExists(transformationFilePath))
            {
                return new FailureResult(Resources.ConfigurationFileTransformer.TransformationFilePathDoesNotExistMessageTemplate, transformationFilePath);
            }

            // read source document
            XmlTransformableDocument transformableDocument = this.GetSourceFile(sourceFilePath);
            if (transformableDocument == null)
            {
                return new FailureResult(Resources.ConfigurationFileTransformer.TransformationFailedBecauseSourceFileCouldNotBeReadMessageTemplate, sourceFilePath);
            }

            // read transformation document
            XmlTransformation transformationFile = this.GetTransformationFile(transformationFilePath);
            if (transformationFile == null)
            {
                return new FailureResult(Resources.ConfigurationFileTransformer.TransformationFailedBecauseTransformationFileCouldNotBeReadMessageTemplate, sourceFilePath);                
            }

            // transform
            try
            {
                transformationFile.Apply(transformableDocument);
            }
            catch (Exception transformationException)
            {
                return new FailureResult(
                    Resources.ConfigurationFileTransformer.TransformationExceptionMessageTemplate,
                    sourceFilePath,
                    transformationFilePath,
                    destinationFilePath,
                    transformationException);
            }

            // save
            IServiceResult saveResult = this.SaveTransformedFile(transformableDocument, destinationFilePath);
            if (saveResult.Status == ServiceResultType.Failure)
            {
                return new FailureResult(
                    Resources.ConfigurationFileTransformer.TransformationFailedMessageTemplate, sourceFilePath, transformationFilePath, destinationFilePath)
                    {
                        InnerResult = saveResult
                    };
            }

            return new SuccessResult(
                Resources.ConfigurationFileTransformer.TransformationSuccessMessageTemplate, sourceFilePath, transformationFilePath, destinationFilePath);
        }

        private XmlTransformableDocument GetSourceFile(string filePath)
        {
            try
            {
                using (var textReader = this.filesystemAccessor.GetTextReader(filePath))
                {
                    var transformableDocument = new XmlTransformableDocument { PreserveWhitespace = true };
                    transformableDocument.Load(textReader);
                    return transformableDocument;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        private XmlTransformation GetTransformationFile(string filePath)
        {
            try
            {
                string transformationFileContent = this.filesystemAccessor.GetFileContent(filePath);
                return new XmlTransformation(transformationFileContent, false, null);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private IServiceResult SaveTransformedFile(XmlTransformableDocument transformedDocument, string destinationFilePath)
        {
            try
            {
                this.filesystemAccessor.EnsureParentDirectoryExists(destinationFilePath);
                using (var textWriter = this.filesystemAccessor.GetTextWriter(destinationFilePath))
                {
                    transformedDocument.Save(textWriter);
                }

                return new SuccessResult(Resources.ConfigurationFileTransformationService.SaveSucceededMessageTemplate, destinationFilePath);
            }
            catch (Exception saveException)
            {
                return new FailureResult(
                    Resources.ConfigurationFileTransformationService.SaveFailedWithExceptionMessageTemplate, destinationFilePath, saveException.Message);
            }
        }
    }
}
using System;
using System.Xml;

using Microsoft.Web.Publishing.Tasks;

using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Common.UserInterface;

namespace NuDeploy.Core.Services.Transformation
{
    public class ConfigurationFileTransformer : IConfigurationFileTransformer
    {
        private readonly IUserInterface userInterface;

        private readonly IFilesystemAccessor filesystemAccessor;

        public ConfigurationFileTransformer(IUserInterface userInterface, IFilesystemAccessor filesystemAccessor)
        {
            if (userInterface == null)
            {
                throw new ArgumentNullException("userInterface");
            }

            if (filesystemAccessor == null)
            {
                throw new ArgumentNullException("filesystemAccessor");
            }

            this.userInterface = userInterface;
            this.filesystemAccessor = filesystemAccessor;
        }

        public bool Transform(string sourceFilePath, string transformationFilePath, string destinationFilePath)
        {
            if (string.IsNullOrWhiteSpace(sourceFilePath))
            {
                this.userInterface.WriteLine(
                    string.Format(Resources.ConfigurationFileTransformer.SourceFilePathCannotBeNullOrEmptyMessageTemplate, transformationFilePath));

                return false;
            }

            if (string.IsNullOrWhiteSpace(transformationFilePath))
            {
                this.userInterface.WriteLine(
                    string.Format(Resources.ConfigurationFileTransformer.TransformationFilePathCannotBeNullOrEmptyMessageTemplate, transformationFilePath));

                return false;
            }

            if (string.IsNullOrWhiteSpace(destinationFilePath))
            {
                this.userInterface.WriteLine(
                    string.Format(Resources.ConfigurationFileTransformer.DestinationFilePathCannotBeNullOrEmptyMessageTemplate, destinationFilePath));

                return false;
            }

            if (!this.filesystemAccessor.FileExists(sourceFilePath))
            {
                this.userInterface.WriteLine(string.Format(Resources.ConfigurationFileTransformer.SourceFilePathDoesNotExistMessageTemplate, sourceFilePath));
                return false;
            }

            if (!this.filesystemAccessor.FileExists(transformationFilePath))
            {
                this.userInterface.WriteLine(
                    string.Format(Resources.ConfigurationFileTransformer.TransformationFilePathDoesNotExistMessageTemplate, transformationFilePath));

                return false;
            }

            this.userInterface.WriteLine(
                string.Format(
                    Resources.ConfigurationFileTransformer.TransformationStartMessageTemplate,
                    sourceFilePath,
                    transformationFilePath,
                    destinationFilePath));

            XmlTransformableDocument transformableDocument = this.GetSourceFile(sourceFilePath);

            bool transformationWasSuccessfull = this.GetTransformationFile(transformationFilePath).Apply(transformableDocument);
            if (transformationWasSuccessfull)
            {
                if (this.SaveTransformedFile(transformableDocument, destinationFilePath))
                {
                    this.userInterface.WriteLine(
                        string.Format(
                            Resources.ConfigurationFileTransformer.TransformationSuccessMessageTemplate,
                            sourceFilePath,
                            transformationFilePath,
                            destinationFilePath));

                    return true;
                }
            }

            this.userInterface.WriteLine(Resources.ConfigurationFileTransformer.TransformationFailedMessage);
            return false;
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
            catch (XmlException xmlException)
            {
                this.userInterface.WriteLine(
                    string.Format(
                        Resources.ConfigurationFileTransformer.GetSourceFileXmlExceptionMessageTemplate,
                        filePath,
                        xmlException));
            }
            catch (Exception generalException)
            {
                this.userInterface.WriteLine(
                    string.Format(
                        Resources.ConfigurationFileTransformer.GetSourceFileGeneralExceptionMessageTemplate,
                        filePath,
                        generalException));
            }

            return null;
        }

        private XmlTransformation GetTransformationFile(string filePath)
        {
            try
            {
                string transformationFileContent = this.filesystemAccessor.GetFileContent(filePath);
                return new XmlTransformation(transformationFileContent, false, null);
            }
            catch (XmlException xmlException)
            {
                this.userInterface.WriteLine(
                    string.Format(
                        Resources.ConfigurationFileTransformer.GetTransformationFileXmlExceptionMessageTemplate,
                        filePath,
                        xmlException));
            }
            catch (Exception generalException)
            {
                this.userInterface.WriteLine(
                    string.Format(
                        Resources.ConfigurationFileTransformer.GetTransformationFileGeneralExceptionMessageTemplate,
                        filePath,
                        generalException));
            }

            return null;
        }

        private bool SaveTransformedFile(XmlTransformableDocument transformedDocument, string destinationFilePath)
        {
            if (transformedDocument == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(destinationFilePath))
            {
                return false;
            }

            try
            {
                if (this.filesystemAccessor.FileExists(destinationFilePath))
                {
                    this.userInterface.WriteLine(string.Format(Resources.ConfigurationFileTransformer.DestinationFileAlreadyExistsMessageTemplate, destinationFilePath));
                }

                using (var textWriter = this.filesystemAccessor.GetTextWriter(destinationFilePath))
                {
                    transformedDocument.Save(textWriter);
                }

                return true;
            }
            catch (XmlException xmlException)
            {
                this.userInterface.WriteLine(
                    string.Format(
                        Resources.ConfigurationFileTransformer.SaveTransformedFileXmlExceptionMessageTemplate,
                        destinationFilePath,
                        xmlException));
            }
            catch (Exception generalException)
            {
                this.userInterface.WriteLine(
                    string.Format(
                        Resources.ConfigurationFileTransformer.SaveTransformedFileGeneralExceptionMessageTemplate,
                        destinationFilePath,
                        generalException));
            }

            return false;
        }
    }
}
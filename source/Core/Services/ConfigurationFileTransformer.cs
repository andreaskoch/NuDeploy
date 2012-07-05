using System;
using System.IO;
using System.Xml;

using Microsoft.Web.Publishing.Tasks;

using NuDeploy.Core.Common;

namespace NuDeploy.Core.Services
{
    public class ConfigurationFileTransformer : IConfigurationFileTransformer
    {
        private readonly IUserInterface userInterface;

        public ConfigurationFileTransformer(IUserInterface userInterface)
        {
            this.userInterface = userInterface;
        }

        public bool Transform(string sourceFilePath, string transformationFilePath, string destinationFilePath)
        {
            if (string.IsNullOrWhiteSpace(sourceFilePath))
            {
                this.userInterface.WriteLine(string.Format("The specified source file path \"{0}\" for the configuration file transformation cannot be null or empty.", transformationFilePath));

                return false;
            }

            if (string.IsNullOrWhiteSpace(transformationFilePath))
            {
                this.userInterface.WriteLine(string.Format("The specified transformation file path \"{0}\" for the configuration file transformation cannot be null or empty.", transformationFilePath));

                return false;
            }

            if (string.IsNullOrWhiteSpace(destinationFilePath))
            {
                this.userInterface.WriteLine(string.Format("The specified destination file path \"{0}\" for the configuration file transformation cannot be null or empty.", destinationFilePath));

                return false;
            }

            if (!File.Exists(sourceFilePath))
            {
                this.userInterface.WriteLine(
                    string.Format("The specified source file \"{0}\" for the configuration file transformation was not found.", sourceFilePath));

                return false;
            }

            if (!File.Exists(transformationFilePath))
            {
                this.userInterface.WriteLine(
                    string.Format("The specified transformation file \"{0}\" for the configuration file transformation was not found.", transformationFilePath));

                return false;
            }

            this.userInterface.WriteLine(
                string.Format(
                    "Starting the transformation of the specified configuration file \"{0}\" with the transformation file \"{1}\" into the new file \"{2}\".",
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
                            "The configuration file has been transformed successfully ({0} + {1} => {2}).",
                            sourceFilePath,
                            transformationFilePath,
                            destinationFilePath));

                    return true;
                }
            }

            this.userInterface.WriteLine("The configuration file transformation failed.");
            return false;
        }

        private XmlTransformableDocument GetSourceFile(string filePath)
        {
            try
            {
                var transformableDocument = new XmlTransformableDocument { PreserveWhitespace = true };
                transformableDocument.Load(filePath);
                return transformableDocument;
            }
            catch (XmlException xmlException)
            {
                this.userInterface.WriteLine(
                    string.Format(
                        "Could not load the source file \"{0}\" for the configuration file transformation because the file contains invalid XML: {1}",
                        filePath,
                        xmlException));
            }
            catch (Exception generalException)
            {
                this.userInterface.WriteLine(
                    string.Format(
                        "Opening the the source file \"{0}\" for the configuration file transformation failed with the following exception: {1}",
                        filePath,
                        generalException));
            }

            return null;
        }

        private XmlTransformation GetTransformationFile(string filePath)
        {
            try
            {
                return new XmlTransformation(filePath);
            }
            catch (XmlException xmlException)
            {
                this.userInterface.WriteLine(
                    string.Format(
                        "Could not load the transformation file \"{0}\" for the configuration file transformation because the file contains invalid XML: {1}",
                        filePath,
                        xmlException));
            }
            catch (Exception generalException)
            {
                this.userInterface.WriteLine(
                    string.Format(
                        "Opening the the transformation file \"{0}\" for the configuration file transformation failed with the following exception: {1}",
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
                if (File.Exists(destinationFilePath))
                {
                    this.userInterface.WriteLine(string.Format("The specified destination file for the configuration file transformation does already exist. Please note that the file will be overidden."));
                    File.Delete(destinationFilePath);
                }

                transformedDocument.Save(destinationFilePath);
                return true;
            }
            catch (XmlException xmlException)
            {
                this.userInterface.WriteLine(
                    string.Format(
                        "Could not save the transformed configuration file to the specified path \"{0}\" because the file contains invalid XML: {1}",
                        destinationFilePath,
                        xmlException));
            }
            catch (Exception generalException)
            {
                this.userInterface.WriteLine(
                    string.Format(
                        "Saving the transformed configuration file to the specified path \"{0}\" failed with the following exception: {1}",
                        destinationFilePath,
                        generalException));
            }

            return false;
        }
    }
}
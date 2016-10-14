//----------------------------------------------------------------------------------
// Microsoft Developer & Platform Evangelism
//
// Copyright (c) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
//----------------------------------------------------------------------------------
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
//----------------------------------------------------------------------------------
/*
 * Azure Client Side Encryption Sample - Demonstrates how to use encryption along with Azure Key Vault integration for the Azure Blob service.
 *
 * Documentation References:
 *  - Get started with Azure Key Vault - https://azure.microsoft.com/en-us/documentation/articles/key-vault-get-started/
 *  - Client-Side Encryption and Azure Key Vault for Microsoft Azure Storage - https://azure.microsoft.com/en-us/documentation/articles/storage-client-side-encryption/
 *  - Tutorial: Encrypt and decrypt blobs in Microsoft Azure Storage using Azure Key Vault - https://azure.microsoft.com/en-us/documentation/articles/storage-encrypt-decrypt-blobs-key-vault/
 *
 * Instructions:
 *          1.  Create a Storage Account through the Azure Portal and set up your Key Vault following the instructions on this post: https://azure.microsoft.com/en-us/documentation/articles/key-vault-get-started/
 *          2.  Open the App.config file and set "StorageConnectionString", "KVClientId", "KVClientKey", "VaultUri" and optionally "KeyID"
 *          3.  Set breakpoints and run the project
 */
namespace KVGettingStarted
{
    using System;
    using System.IO;
    using System.Threading;
    using Microsoft.Azure;
    using Microsoft.Azure.KeyVault;
    using Microsoft.Azure.KeyVault.Core;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    /// <summary>
    /// Demonstrates how to use encryption along with Azure Key Vault integration for the Azure Blob service.
    /// </summary>
    public class Program
    {
        const string DemoContainer = "democontainer";
        const string DefaultSecretName = "KVGettingStartedSecret";

        static void Main(string[] args)
        {
            Console.WriteLine("Blob encryption with Key Vault integration");
            Console.WriteLine();

            // Get the key ID from App.config if it exists.
            string keyID = CloudConfigurationManager.GetSetting("KeyID");

            // If no key ID was specified, we will create a new secret in Key Vault.
            // To create a new secret, this client needs full permission to Key Vault secrets.
            // Once the secret is created, its ID can be added to App.config. Once this is done,
            // this client only needs read access to secrets.
            if (string.IsNullOrEmpty(keyID))
            {
                Console.WriteLine("No secret specified in App.config.");
                Console.WriteLine("Please enter the name of a new secret to create in Key Vault.");
                Console.WriteLine("WARNING: This will delete any existing secret with the same name.");
                Console.Write("Name of the new secret to create [{0}]: ", DefaultSecretName);
                string newSecretName = Console.ReadLine().Trim();

                if (string.IsNullOrEmpty(newSecretName))
                {
                    newSecretName = DefaultSecretName;
                }

                // Although it is possible to use keys (rather than secrets) stored in Key Vault, this prevents caching.
                // Therefore it is recommended to use secrets along with a caching resolver (see below).
                keyID = EncryptionShared.KeyVaultUtility.SetUpKeyVaultSecret(newSecretName);

                Console.WriteLine();
                Console.WriteLine("Created a secret with ID: {0}", keyID);
                Console.WriteLine("Copy the secret ID to App.config to reuse.");
                Console.WriteLine();
            }

            // Retrieve storage account information from connection string
            // How to create a storage connection string - https://azure.microsoft.com/en-us/documentation/articles/storage-configure-connection-string/
            CloudStorageAccount storageAccount = EncryptionShared.Utility.CreateStorageAccountFromConnectionString();

            CloudBlobClient client = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = client.GetContainerReference(DemoContainer + Guid.NewGuid().ToString("N"));

            // Construct a resolver capable of looking up keys and secrets stored in Key Vault.
            KeyVaultKeyResolver cloudResolver = new KeyVaultKeyResolver(EncryptionShared.KeyVaultUtility.GetAccessToken);

            // To demonstrate how multiple different types of key can be used, we also create a local key and resolver.
            // This key is temporary and won't be persisted.
            RsaKey rsaKey = new RsaKey("rsakey");
            LocalResolver resolver = new LocalResolver();
            resolver.Add(rsaKey);

            // If there are multiple key sources like Azure Key Vault and local KMS, set up an aggregate resolver as follows.
            // This helps users to define a plug-in model for all the different key providers they support.
            AggregateKeyResolver aggregateResolver = new AggregateKeyResolver()
                .Add(resolver)
                .Add(cloudResolver);

            // Set up a caching resolver so the secrets can be cached on the client. This is the recommended usage
            // pattern since the throttling targets for Storage and Key Vault services are orders of magnitude
            // different.
            CachingKeyResolver cachingResolver = new CachingKeyResolver(2, aggregateResolver);

            // Create a key instance corresponding to the key ID. This will cache the secret.
            IKey cloudKey = cachingResolver.ResolveKeyAsync(keyID, CancellationToken.None).GetAwaiter().GetResult();

            try
            {
                container.Create();
                int size = 5 * 1024 * 1024;
                byte[] buffer = new byte[size];

                Random rand = new Random();
                rand.NextBytes(buffer);

                // The first blob will use the key stored in Azure Key Vault.
                CloudBlockBlob blob = container.GetBlockBlobReference("blockblob1");

                // Create the encryption policy using the secret stored in Azure Key Vault to be used for upload.
                BlobEncryptionPolicy uploadPolicy = new BlobEncryptionPolicy(cloudKey, null);

                // Set the encryption policy on the request options.
                BlobRequestOptions uploadOptions = new BlobRequestOptions() { EncryptionPolicy = uploadPolicy };

                Console.WriteLine("Uploading the 1st encrypted blob.");

                // Upload the encrypted contents to the blob.
                using (MemoryStream stream = new MemoryStream(buffer))
                {
                    blob.UploadFromStream(stream, size, null, uploadOptions);
                }

                // Download the encrypted blob.
                BlobEncryptionPolicy downloadPolicy = new BlobEncryptionPolicy(null, cachingResolver);

                // Set the decryption policy on the request options.
                BlobRequestOptions downloadOptions = new BlobRequestOptions() { EncryptionPolicy = downloadPolicy };

                Console.WriteLine("Downloading the 1st encrypted blob.");

                // Download and decrypt the encrypted contents from the blob.
                using (MemoryStream outputStream = new MemoryStream())
                {
                    blob.DownloadToStream(outputStream, null, downloadOptions);
                }

                // Upload second blob using the local key.
                blob = container.GetBlockBlobReference("blockblob2");

                // Create the encryption policy using the local key.
                uploadPolicy = new BlobEncryptionPolicy(rsaKey, null);

                // Set the encryption policy on the request options.
                uploadOptions = new BlobRequestOptions() { EncryptionPolicy = uploadPolicy };

                Console.WriteLine("Uploading the 2nd encrypted blob.");

                // Upload the encrypted contents to the blob.
                using (MemoryStream stream = new MemoryStream(buffer))
                {
                    blob.UploadFromStream(stream, size, null, uploadOptions);
                }

                // Download the encrypted blob. The same policy and options created before can be used because the aggregate resolver contains both
                // resolvers and will pick the right one based on the key ID stored in blob metadata on the service.
                Console.WriteLine("Downloading the 2nd encrypted blob.");

                // Download and decrypt the encrypted contents from the blob.
                using (MemoryStream outputStream = new MemoryStream())
                {
                    blob.DownloadToStream(outputStream, null, downloadOptions);
                }

                Console.WriteLine("Press enter key to exit");
                Console.ReadLine();
            }
            finally
            {
                container.DeleteIfExists();
            }
        }
    }
}

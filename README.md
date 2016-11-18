---
services: storage, keyvault
platforms: dotnet
author: woodp
---

# Getting Started with Azure Client Side Encryption in Java and C#

Azure Client Side Encryption Sample - Demonstrates how to use encryption along with Azure Key Vault integration for the Azure Blob service.

Note: If you don't have a Microsoft Azure subscription you can get a FREE trial account [here](http://go.microsoft.com/fwlink/?LinkId=330212)

## Running this sample

Instructions:

1.  Create a Storage Account through the Azure Portal and set up your Key Vault following the instructions on this post: https://azure.microsoft.com/en-us/documentation/articles/key-vault-get-started/

2. For the .net sample, open the App.config file and set "StorageConnectionString", "KVClientId", "KVClientKey", "VaultUri" and optionally "KeyID"

3. Set breakpoints and run the project


## More information

[What is a Storage Account](http://azure.microsoft.com/en-us/documentation/articles/storage-whatis-account/)

[Get started with Azure Key Vault](https://azure.microsoft.com/en-us/documentation/articles/key-vault-get-started/)

[Client-Side Encryption and Azure Key Vault for Microsoft Azure Storage](https://azure.microsoft.com/en-us/documentation/articles/storage-client-side-encryption/)

[Client-Side Encryption with Java for Microsoft Azure Storage](https://azure.microsoft.com/en-us/documentation/articles/storage-client-side-encryption-java/)

[Tutorial: Encrypt and decrypt blobs in Microsoft Azure Storage using Azure Key Vault](https://azure.microsoft.com/en-us/documentation/articles/storage-encrypt-decrypt-blobs-key-vault/)

[Getting Started with Blobs](http://azure.microsoft.com/en-us/documentation/articles/storage-java-how-to-use-blob-storage/)

[Blob Service Concepts](http://msdn.microsoft.com/en-us/library/dd179376.aspx)

[Blob Service REST API](http://msdn.microsoft.com/en-us/library/dd135733.aspx)

[Get started with Azure Blob storage using .NET](https://azure.microsoft.com/en-us/documentation/articles/storage-dotnet-how-to-use-blobs/)

[Blob Service Java API](http://azure.github.io/azure-storage-java/)

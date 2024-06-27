using Amazon.S3.Transfer;

namespace SecretsSharing.Infrastructure;

public class S3StorageHandler
{
    private const string bucketName = "sharing-secrets-drive";
    private readonly TransferUtility transferUtility;

    public S3StorageHandler(TransferUtility transferUtility)
    {
        this.transferUtility = transferUtility;
    }

    public Task Upload(Stream fileStream, string fileKey)
    {
        return transferUtility.UploadAsync(fileStream, bucketName, fileKey);
    }
}
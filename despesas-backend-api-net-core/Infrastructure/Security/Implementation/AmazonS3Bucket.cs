﻿using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using despesas_backend_api_net_core.Domain.VM;

namespace despesas_backend_api_net_core.Infrastructure.Security.Implementation
{
    public class AmazonS3Bucket : IAmazonS3Bucket
    {
        private static IAmazonS3Bucket? Instance;
        private readonly S3CannedACL fileCannedACL = S3CannedACL.PublicRead;
        private readonly RegionEndpoint bucketRegion = RegionEndpoint.SAEast1;
        private IAmazonS3 client;
        private readonly string AccessKey;
        private readonly string SecretAccessKey;
        private readonly string S3ServiceUrl;
        private readonly string BucketName;

        private AmazonS3Bucket()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();
            AccessKey = configuration.GetSection("AmazonS3Bucket:accessKey").Value;
            SecretAccessKey = configuration.GetSection("AmazonS3Bucket:secretAccessKey").Value;
            S3ServiceUrl = configuration.GetSection("AmazonS3Bucket:s3ServiceUrl").Value;
            BucketName = configuration.GetSection("AmazonS3Bucket:bucketName").Value;
        }
        public static IAmazonS3Bucket GetInstance
        {
            get
            {
                return Instance == null ? new AmazonS3Bucket() : Instance;
            }
        }
        public async Task<string> WritingAnObjectAsync(ImagemPerfilVM perfilFile)
        {
            try
            {
                string fileContentType = perfilFile.ContentType;
                AmazonS3Config config = new AmazonS3Config();
                config.ServiceURL = S3ServiceUrl;

                client = new AmazonS3Client(AccessKey, SecretAccessKey, config);

                var putRquest = new PutObjectRequest
                {
                    CannedACL = fileCannedACL,
                    BucketName = BucketName,
                    Key = perfilFile.Name,
                    ContentType = perfilFile.ContentType,
                    InputStream = new MemoryStream(perfilFile.Arquivo)
                };
                PutObjectResponse response = await client.PutObjectAsync(putRquest);
                var url = $"https://{BucketName}.s3.amazonaws.com/{perfilFile.Name}";
                return url;

            }
            catch (Exception ex)
            {
                throw new Exception("AmazonS3Bucket_WritingAnObjectAsync_Errro ", ex);
            }
        }
        public async Task<bool> DeleteObjectNonVersionedBucketAsync(ImagemPerfilVM perfilFile)
        {
            try
            {
                AmazonS3Config config = new AmazonS3Config();
                config.ServiceURL = S3ServiceUrl;

                client = new AmazonS3Client(AccessKey, SecretAccessKey, config);


                var deleteObjectRequest = new DeleteObjectRequest
                {
                    BucketName = BucketName,
                    Key = perfilFile.Name,
                };

                Console.WriteLine("Deleting an object");
                await client.DeleteObjectAsync(deleteObjectRequest);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
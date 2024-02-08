using Amazon.Runtime;
using System.Drawing;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using AwsS3.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;

namespace AwsS3.Services;
public class AwsS3StorageService : IAwsS3StorageService
{
    public AmazonS3Client GetAmazonS3Client(AwsCredentials awsCredentials)
    {
        // Add AWS Credentials
        var credentials = new BasicAWSCredentials(awsCredentials.AwsKey, awsCredentials.AwsSecretKey);

        // Specify the region
        var config = new AmazonS3Config()
        {
            RegionEndpoint = Amazon.RegionEndpoint.EUCentral1
        };
        return new AmazonS3Client(credentials, config);
    }

    public async Task<List<AwsS3FileDetails>> GetAwsS3AllFilesAsync(AwsS3Object AwsS3Obj, AwsCredentials awsCredentials)
    {
        var result = new List<AwsS3FileDetails>();
        try
        {
            // Create S3 client
            using var client = GetAmazonS3Client(awsCredentials);
            var response = await client.ListObjectsAsync(AwsS3Obj.BucketName, AwsS3Obj.Prefix);

            var imageFileExtensions = new String[] { ".jpg", ".jpeg", ".png", ".gif", ".tiff", ".bmp", ".svg" }; 

            result = response.S3Objects.Count == 0 ? new List<AwsS3FileDetails>() :
                response.S3Objects.Select(o => new AwsS3FileDetails {BucketName=o.BucketName,FolderName=o.Key.Split("/")[0] ,Key = o.Key,
                FileName = (o.Key.Split("/")[1]), fileExtension = Path.GetExtension(o.Key), Size = (Decimal.Round(((decimal)o.Size) / (1024 * 1024),2)).ToString()+"MB" })
                .Where(x => x.FileName != "").Where(x => imageFileExtensions.Contains(x.fileExtension)).ToList();

        }
        catch (AmazonS3Exception ex)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw;
        }
        return result;
    }
    public async Task<AwsS3Response> DownloadAwsS3FileAsync(AwsS3Object AwsS3Obj, AwsCredentials awsCredentials, string key,string consumeFilePath, string apiFilePath)
    {
        var response = new AwsS3Response();
        try
        {
            // Create S3 client
            using var client = GetAmazonS3Client(awsCredentials);
            using (TransferUtility fileTransferUtility = new TransferUtility(client))
            {
                var request = new GetObjectRequest();
                request.BucketName = AwsS3Obj.BucketName;
                request.Key = key;

                //var file = await fileTransferUtility.S3Client.GetObjectAsync(request);
                //var size = file.ContentLength;
              
                fileTransferUtility.Download(apiFilePath, AwsS3Obj.BucketName, key);
                fileTransferUtility.Download(consumeFilePath, AwsS3Obj.BucketName, key);
                var sizeInMB =(decimal) (new FileInfo(apiFilePath).Length)/(1024 * 1024);

                response.StatusCode = 200;
                response.Message = $"{key} of size {Decimal.Round(sizeInMB,2)}MB downloaded successfully.";
            }
        }
        catch (AmazonS3Exception ex)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw;
        }
        return response;
    }

    public async Task<AwsS3Response> SaveCroppedImage(IFormFile file,string filePath)
    {
        AwsS3Response response = new AwsS3Response();
        try 
        {
            string newfilePath =filePath + $@"\{file.FileName}";
            Image image = Image.FromStream(file.OpenReadStream(), true, true);
            image.Save(newfilePath);

            var sizeInMB = (decimal)(new FileInfo(newfilePath).Length) / (1024 * 1024);

            response.StatusCode = 200;
            response.Message = $"{file.FileName} of size {Decimal.Round(sizeInMB, 2)}MB saved successfully.";
        }
        catch (Exception ex) { throw; }
        return response;
    }

    public async Task<AwsS3Response> ResizeAwsS3AllFileAsync(AwsS3Object AwsS3Obj, AwsCredentials awsCredentials, string key, string apiFilePath)
    {
        var response = new AwsS3Response();
        try
        {
            // Create S3 client
            using var client = GetAmazonS3Client(awsCredentials);
            using (TransferUtility fileTransferUtility = new TransferUtility(client))
            {
                var request = new GetObjectRequest();
                request.BucketName = AwsS3Obj.BucketName;
                request.Key = key;
                
                //Download File
                fileTransferUtility.Download(apiFilePath + "\\" + key, AwsS3Obj.BucketName, key);

                var originalSizeInMB = (decimal)(new FileInfo(apiFilePath + "\\" + key).Length) / (1024 * 1024);


                // Get saved image details
                FileStream stream = File.OpenRead(apiFilePath+ "\\" + key);
                MemoryStream ms = new MemoryStream();
                stream.CopyTo(ms);
                byte[] imgBytes= ms.ToArray();

                // ResizeImage
                byte[] resizedBytes = ScaleImage(imgBytes, 512, 512);
                
                // Save resized image
                MemoryStream resizedMemoryStream = new MemoryStream();
                resizedMemoryStream.Write(resizedBytes, 0, resizedBytes.Length);
                Image image = Image.FromStream(resizedMemoryStream, true, true);
                string currentDate = GetCurrentDate();
                string newImageName = key.Split("/")[0]+"/" + (key.Split("/")[1]).Split(".")[0] + "_Rthumb" + currentDate + "."+ (key.Split("/")[1]).Split(".")[1];
                image.Save(apiFilePath + "\\" + newImageName);

                var resizedSizeInMB = (decimal)(new FileInfo(apiFilePath + "\\" + newImageName).Length) / (1024 * 1024);

                response.StatusCode = 200;
                response.Message = $"{key} of size {Decimal.Round(originalSizeInMB, 2)}MB resized to {Decimal.Round(resizedSizeInMB, 2)}MB successfully.";
            }
        }
        catch (AmazonS3Exception ex)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw;
        }
        return response;
    }

    public static byte[] ScaleImage(byte[] imageBytes, int maxWidth, int maxHeight)
    {
        SKBitmap image = SKBitmap.Decode(imageBytes);

        var ratioX = (double)maxWidth / image.Width;
        var ratioY = (double)maxHeight / image.Height;
        var ratio = Math.Min(ratioX, ratioY);

        var newWidth = (int)(image.Width * ratio);
        var newHeight = (int)(image.Height * ratio);

        var info = new SKImageInfo(newWidth, newHeight);
        image = image.Resize(info, SKFilterQuality.High);

        using var ms = new MemoryStream();
        image.Encode(ms, SKEncodedImageFormat.Png, 100);
        return ms.ToArray();
    }

    public string GetCurrentDate()
    {
        string currentDate = "";
        try 
        {
           currentDate= DateTime.Now.Date.Day+""+ DateTime.Now.Date.Month+""+ DateTime.Now.Date.Year
                +""+ DateTime.Now.TimeOfDay.Hours+""+ DateTime.Now.TimeOfDay.Minutes+""+ DateTime.Now.TimeOfDay.Seconds;
        }
        catch (Exception ex)
        {
            throw;
        }
        return currentDate;
    }
}


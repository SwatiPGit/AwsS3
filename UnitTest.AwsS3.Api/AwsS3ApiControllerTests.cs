using Amazon.Runtime.Internal.Transform;
using AwsS3.Api.Controllers;
using AwsS3.Models;
using AwsS3.Services;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;

namespace UnitTest.AwsS3.Api
{
    public class AwsS3ApiControllerTests
    {
        AwsS3ApiController _controller;
        IAwsS3StorageService _storageService;
        private readonly IConfiguration _configuration;
        private readonly Mock<IWebHostEnvironment> _hostingEnvironment = new Mock<IWebHostEnvironment>();
        public AwsS3ApiControllerTests()
        {
            //Arrange
            var inMemorySettings = new Dictionary<string, string> {
            {"AwsConfiguration:AWSAccessKey", "AKIASO5OP424BFQMJPOL"},
            {"AwsConfiguration:AWSSecretKey","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"} };
            _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

            _storageService = new AwsS3StorageService();
            _controller = new AwsS3ApiController(_storageService, _configuration, _hostingEnvironment.Object);
        }


        [Fact]
        public async Task GetAllAGetAwsS3AllFiles_Success()
        {
            //Arrange

            //Act
            var result = await _controller.GetAwsS3AllFiles();
            var resultType = result as OkObjectResult;
            var resultList = resultType.Value as AwsS3FileDetails;

            //Assert
            Assert.NotNull(result);
            Assert.IsType<AwsS3FileDetails>(resultType.Value);
        }

        [Fact]
        public async Task DownloadAwsS3AllFile_Success()
        {
            //Arrange
            string key = "profiles/1772475.jpg";
            string mockFilePath = AppDomain.CurrentDomain.BaseDirectory + "\\" + "MockImages" + "\\" + key;

            //Act
            var result = await _controller.DownloadAwsS3AllFile(key, mockFilePath);
            var resultType = result as OkObjectResult;

            //Assert
            Assert.NotNull(result);
            Assert.IsType<AwsS3Response>(resultType.Value);
            Assert.Equal(200, resultType.StatusCode);

        }

        [Fact]
        public async Task SaveCroppedImage_Success()
        {
            //Arrange
            string mockFilePath = AppDomain.CurrentDomain.BaseDirectory + "\\" + "MockImages" + "\\" + @"profiles\1772475.jpg";
            FileStream stream = File.OpenRead(mockFilePath);
            IFormFile file = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name));

            //Act
            var result = await _controller.SaveCroppedImage(file);
            var resultType = result as OkObjectResult;

            //Assert
            Assert.NotNull(result);
            Assert.IsType<AwsS3Response>(resultType.Value);
        }
        [Fact]
        public async Task ResizeAwsS3AllFile_Success()
        {
            //Arrange
            string key = "profiles/1772475.jpg";

            //Act
            var result = await _controller.ResizeAwsS3AllFile(key);
            var resultType = result as OkObjectResult;

            //Assert
            Assert.NotNull(result);
            Assert.IsType<AwsS3Response>(resultType.Value);
        }
    }
}
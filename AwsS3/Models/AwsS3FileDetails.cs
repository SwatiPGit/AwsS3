using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwsS3.Models;
public class AwsS3FileDetails
{
    public string? BucketName { get; set; } = "";
    public string? FolderName { get; set; } = "";
    public string? Key { get; set; } = "";
    public string? FileName { get; set; } = "";
    public string? Size { get; set; } = "";
    public string? FilePath { get; set; } = "";
    public string? fileExtension { get; set; } = "";

    public string? Response { get; set; } = "";

}

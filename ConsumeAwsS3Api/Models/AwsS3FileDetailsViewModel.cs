using System.ComponentModel;

namespace ConsumeAwsS3Api.Models;
public class AwsS3FileDetailsViewModel
{
    [DisplayName("Bucket Name")]
    public string? BucketName { get; set; } = "";

    [DisplayName("Folder Name")]
    public string? FolderName { get; set; } = "";

    [DisplayName("File")]
    public string? Key { get; set; } = "";

    [DisplayName("File Name")]
    public string? FileName { get; set; } = "";

    [DisplayName("Size")]
    public string? Size { get; set; } = "";

    public string? FilePath { get; set; } = "";

    public string? fileExtension { get; set; } = "";

    [DisplayName("Response")]
    public string? Response { get; set; } = "";

}

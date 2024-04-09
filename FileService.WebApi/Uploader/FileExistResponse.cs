namespace FileService.WebApi.Uploader
{
    public record FileExistResponse(bool isExists, Uri? url);
}

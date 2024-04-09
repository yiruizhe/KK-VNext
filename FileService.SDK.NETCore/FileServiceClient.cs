using KK.Commons;
using KK.JWT;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace FileService.SDK.NETCore;

public class FileServiceClient
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly ITokenService tokenService;
    private readonly JwtOptions jwtOptions;
    private readonly Uri serverRoot;

    public FileServiceClient(Uri serverRoot, IHttpClientFactory httpClientFactory,
        ITokenService tokenService, JwtOptions jwtOptions)
    {
        this.httpClientFactory = httpClientFactory;
        this.tokenService = tokenService;
        this.jwtOptions = jwtOptions;
        this.serverRoot = serverRoot;
    }

    public async Task<FileExistsResponse> FileExistsAsync(long fileSize, string fileSHA256)
    {
        string relativeUrl = $"Upload/FileExist?fileSize={fileSize}&sha256Hash={fileSHA256}";
        Uri requestUrl = new Uri(serverRoot, relativeUrl);
        var httpClient = httpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", BuildToken());
        return (await httpClient.GetStringAsync(requestUrl)).ParseJson<FileExistsResponse>()!;
    }

    public async Task<Uri> UploadAsync(FileInfo file, CancellationToken ct)
    {
        string token = BuildToken();
        Uri requestUri = new Uri(this.serverRoot, "Upload/Upload");
        using var content = new MultipartFormDataContent();
        using FileStream fs = file.OpenRead();
        using var fileContent = new StreamContent(fs);
        content.Add(fileContent, "file", file.Name);

        var httpClient = httpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await httpClient.PostAsync(requestUri, content, ct);
        if (response.IsSuccessStatusCode)
        {
            string url = await response.Content.ReadAsStringAsync(ct);
            return url.ParseJson<Uri>()!;
        }
        else
        {
            string respMsg = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"上传失败，响应状态码:{response.StatusCode},响应报文:{respMsg}");
        }
    }

    private string BuildToken()
    {
        var claims = new List<Claim>() { new Claim(ClaimTypes.Role, "Admin") };
        return tokenService.BuildToken(claims, jwtOptions);
    }
}

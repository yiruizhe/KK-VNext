using FluentValidation;

namespace FileService.WebApi.Uploader
{
    public class FileUploadRequest
    {
        public IFormFile File { get; set; }

    }

    public class FileUploadValidator : AbstractValidator<FileUploadRequest>
    {
        public FileUploadValidator()
        {
            RuleFor(x => x).NotNull().When(x => x != null);
            long maxFileSize = 50 * 1024 * 1024;
            RuleFor(x => x.File).NotNull().Must(f => f.Length > 0 && f.Length < maxFileSize)
                .WithMessage("上传文件大小超过限制").When(x => x != null);
        }
    }
}

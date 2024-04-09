using FFmpeg.NET;
using MediaEncoder.Domain;

namespace MediaEncoder.Infrustructure
{
    public class ToM4AEncoer : IMediaEncoder
    {
        public bool Accept(string outputFormat)
        {
            return "m4a".Equals(outputFormat, StringComparison.OrdinalIgnoreCase);
        }

        public async Task EncodeAsync(FileInfo sourceFile, FileInfo destFile, string destFormat, string[]? args, CancellationToken ck)
        {
            var inputFile = new InputFile(sourceFile);
            var outputFile = new OutputFile(destFile);
            string baseDir = AppContext.BaseDirectory;
            string ffmpegPath = Path.Combine(baseDir, "ffmpeg.exe");
            var ffmpeg = new Engine(ffmpegPath);
            string? errorMsg = null;
            ffmpeg.Error += (o, e) =>
            {
                errorMsg = e.Exception.Message;
            };
            await ffmpeg.ConvertAsync(inputFile, outputFile, ck);//进行转码
            if (errorMsg != null)
            {
                throw new ApplicationException(errorMsg);
            }
        }
    }
}

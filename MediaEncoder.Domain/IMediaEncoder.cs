

namespace MediaEncoder.Domain
{
    public interface IMediaEncoder
    {
        /// <summary>
        /// 是否可以处理目标类型位outputFormat的文件
        /// </summary>
        /// <param name="outputFormat"></param>
        /// <returns></returns>
        bool Accept(string outputFormat);

        /// <summary>
        /// 进行转码
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="destFile"></param>
        /// <param name="destFormat"></param>
        /// <param name="args"></param>
        /// <param name="ck"></param>
        /// <returns></returns>
        Task EncodeAsync(FileInfo sourceFile, FileInfo destFile,
            string destFormat, string[]? args, CancellationToken ck);
    }
}

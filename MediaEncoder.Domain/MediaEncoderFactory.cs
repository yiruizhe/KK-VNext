
namespace MediaEncoder.Domain
{
    public class MediaEncoderFactory
    {
        private IEnumerable<IMediaEncoder> encoders;

        public MediaEncoderFactory(IEnumerable<IMediaEncoder> encoders)
        {
            this.encoders = encoders;
        }

        public IMediaEncoder? Crete(string outputFormat)
        {
            foreach (var encoder in encoders)
            {
                if (encoder.Accept(outputFormat))
                {
                    return encoder;
                }
            }
            return null;
        }
    }
}

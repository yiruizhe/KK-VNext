using KK.DomainCommons;

namespace Listening.Domain.Entities
{
    public class Category : AggregateRootEntity
    {
        private Category() { }

        /// <summary>
        /// 序列号，数字越小越靠前
        /// </summary>
        public int SequenceNumber { get; private set; }

        /// <summary>
        /// 类型名称
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 封面url
        /// </summary>
        public Uri CoverUrl { get; private set; }

        public Category ChangeSequenceNumber(int number)
        {
            this.SequenceNumber = number;
            return this;
        }

        public Category ChangeCategoryName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("categoryName is null", nameof(name));
            }
            Name = name;
            return this;
        }

        public Category ChangeCover(Uri url)
        {
            CoverUrl = url;
            return this;
        }

        public static Category Create(string name, Uri url, int sequenceNumber)
        {
            return new Category()
            {
                SequenceNumber = sequenceNumber,
                Name = name,
                CoverUrl = url
            };
        }

    }
}

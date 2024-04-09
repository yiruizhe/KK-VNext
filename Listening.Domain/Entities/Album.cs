using KK.DomainCommons;

namespace Listening.Domain.Entities
{
    public class Album : AggregateRootEntity
    {
        private Album() { }

        public bool IsVisible { get; private set; }

        public int SequenceNumber { get; private set; }

        public string Name { get; private set; }

        public string CategoryId { get; set; }

        public static Album Create(int sequenceNumber, string name, string categoryId)
        {
            var album = new Album();
            album.SequenceNumber = sequenceNumber;
            album.Name = name;
            album.CategoryId = categoryId;
            album.IsVisible = false;
            return album;
        }

        public Album ChangeSequenceNumber(int number)
        {
            this.SequenceNumber = number;
            return this;
        }

        public Album ChangeName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("categoryName is null", nameof(name));
            }
            Name = name;
            return this;
        }

        public Album Hide()
        {
            IsVisible = false;
            return this;
        }

        public Album Show()
        {
            IsVisible = true;
            return this;
        }
    }
}

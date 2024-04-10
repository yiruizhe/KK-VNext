using Listening.Domain.Entities;

namespace Listening.Main.WebApi.Category
{
    public record CategoryVM(string Id, string Name, Uri coverUrl)
    {
        public static CategoryVM? Create(Domain.Entities.Category? category)
        {
            if (category == null)
            {
                return null;
            }
            return new CategoryVM(category.Id, category.Name, category.CoverUrl);
        }

        public static CategoryVM[] Create(Domain.Entities.Category[] categories)
        {
            return categories.Select(c => Create(c)!).ToArray();
        }
    }
}

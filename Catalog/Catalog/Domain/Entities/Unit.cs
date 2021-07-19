namespace Catalog.Domain.Entities
{
    public class Unit
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Code { get; set; }
    }
}
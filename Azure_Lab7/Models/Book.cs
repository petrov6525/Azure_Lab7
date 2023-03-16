namespace Azure_Lab7.Models
{
    public class Book
    {
        public int? Id { get; set; }
        public string BookName { get; set; }
        public string AuthorFirstName { get; set; }
        public string AuthorLastName { get; set; }
        public string PublishingName { get; set; }
        public string DateOfPublish { get; set; }
        public int PublishingAddressId { get; set; }
    }
}

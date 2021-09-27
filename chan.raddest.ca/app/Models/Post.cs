using System;

namespace app.Models
{
    public class Post
    {
        public ulong Id { get; set; }
        public DateTime Created { get; set; }

        public string AuthorName { get; set; }
        public string AuthorHash { get; set; }

        public string Content { get; set; }

        public ulong? FileId { get; set; }
    }
}
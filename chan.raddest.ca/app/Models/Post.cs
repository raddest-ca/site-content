using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;

namespace app.Models
{
    public class Post
    {
        public ulong Id { get; set; }
        public ulong? ParentId {get; set;}
        public DateTime Created { get; set; }

        public string AuthorName { get; set; }
        public string AuthorHash { get; set; }
        public string AuthorIp { get; set; }

        public string Content { get; set; }

        [ForeignKey(nameof(Models.File.Id))]
        public string? FileId { get; set; }

        public virtual File File {get; set;}

        public static string ComputeHash(string input)
        {
            if (input is null)
            {
                return null;
            }
            var rtn = new StringBuilder();
            using var algo = SHA256.Create();
            var bytes = algo.ComputeHash(Encoding.UTF8.GetBytes(input));
            foreach (byte b in bytes)
                rtn.Append(b.ToString("X2"));
            return rtn.ToString();
        }
    }
}
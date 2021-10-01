using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using app.Data;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace app.Models
{
    public class Submission
    {
        [Required]
        public string Content { get; set; }
        public string Name { get; set; }

        [DataType(DataType.Password)]
        public string Hash { get; set; }

        public IFormFile File { get; set; }

        public int? ParentId { get; set; }

        public static Submission FromRequest(HttpRequest request)
        {
            var name = request.Cookies["LastName"];
            var hash = request.Cookies["LastHash"];
            return new()
            {
                Name = name,
                Hash = hash
            };
        }

        public bool Validate(ModelStateDictionary ModelState)
        {
            if (File != null && File.Length > 104857600)
            {
                ModelState.AddModelError(nameof(Submission.File), "File too large, 100mb max");
                return false;
            }
            return true;
        }

        public void SaveInfoToCookies(HttpResponse Response)
        {
            if (Name != null)
            {
                Response.Cookies.Append("LastName", Name);
            }

            if (Hash != null)
            {
                Response.Cookies.Append("LastHash", Hash);
            }
        }

        public async Task Commit(
            AppDbContext context,
            BlobContainerClient blobClient,
            string author,
            int? parentId
        )
        {
            var post = new Post()
            {
                ParentId = parentId,
                AuthorName = this.Name,
                AuthorHash = Post.ComputeHash(this.Hash),
                AuthorIp = author,
                Created = DateTime.Now,
                Content = this.Content,
            };
            if (this.File is not null)
            {
                var file = new File()
                {
                    Id = Guid.NewGuid(),
                    Created = DateTime.Now,
                    FileName = this.File.FileName,
                };
                // add file id to post
                post.FileId = file.Id;
                // add file to db
                await context.Files.AddAsync(file);

                // upload to azure
                using var stream = this.File.OpenReadStream();
                await blobClient.UploadBlobAsync(file.BlobName, stream);
                // update file URI
                file.Uri = blobClient.GetBlobClient(file.BlobName).Uri.ToString();
            }
            await context.Posts.AddAsync(post);
            await context.SaveChangesAsync();
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using app.Data;
using app.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Collections.Specialized;
using System.Collections;
using Microsoft.AspNetCore.Http;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;

namespace app.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly AppDbContext _context;
        private readonly BlobContainerClient _blob;

        public IndexModel(ILogger<IndexModel> logger, AppDbContext context, BlobContainerClient blobClient)
        {
            _logger = logger;
            _context = context;
            _blob = blobClient;
        }

        public Dictionary<ulong, List<Post>> Threads { get; set; } = new();

        public void OnGet()
        {
            var posts = _context.Posts
                .Include(p => p.File)
                .OrderBy(p => (int)p.Id)
                .ToList();
            foreach (var post in posts)
            {
                var threadId = post.ParentId ?? post.Id;
                if (!Threads.ContainsKey(threadId))
                {
                    Threads[threadId] = new();
                }
                Threads[threadId].Add(post);
            }

            var name = Request.Cookies["LastName"];
            var hash = Request.Cookies["LastHash"];
            Submission = new()
            {
                Name = name,
                Hash = hash
            };
        }

        public class PostVM
        {
            [Required]
            public string Content { get; set; }
            public string Name { get; set; }
            public string Hash { get; set; }

            public IFormFile File { get; set; }
        }

        [BindProperty]
        public PostVM Submission { get; set; }

        public async Task<IActionResult> OnPost()
        {
            Console.WriteLine($"Got post with content {Submission.Content}");
            if (!ModelState.IsValid)
            {
                OnGet();
                return Page();
            }
            var post = new Post()
            {
                AuthorName = Submission.Name,
                AuthorHash = Post.ComputeHash(Submission.Hash),
                AuthorIp = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                Created = DateTime.Now,
                Content = Submission.Content,
            };
            if (Submission.File is not null)
            {
                var file = new File()
                {
                    Id = Guid.NewGuid().ToString(),
                    Created = DateTime.Now,
                    FileName = Submission.File.FileName,
                };
                // add file id to post
                post.FileId = file.Id;
                // add file to db
                await _context.Files.AddAsync(file);

                // upload to azure
                using var stream = Submission.File.OpenReadStream();
                await _blob.UploadBlobAsync(file.Id, stream);
                
                // update file URI
                file.Uri = _blob.GetBlobClient(file.Id).Uri.ToString();
            }
            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();

            Response.Cookies.Append("LastName", Submission.Name);
            Response.Cookies.Append("LastHash", Submission.Hash);
            return RedirectToPage("./Index");
        }
    }
}

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

namespace app.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly AppDbContext _context;

        public IndexModel(ILogger<IndexModel> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public Dictionary<ulong, List<Post>> Threads { get; set; } = new();

        public void OnGet()
        {
            var posts = _context.Posts.OrderBy(p => (int) p.Id).ToList();
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

            public IFormFile File {get; set;}
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
                Content = Submission.Content
            };
            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();
            Response.Cookies.Append("LastName", Submission.Name);
            Response.Cookies.Append("LastHash", Submission.Hash);
            return RedirectToPage("./Index");
        }
    }
}

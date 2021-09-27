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

namespace app.Pages
{
    public class ThreadModel : PageModel
    {
        private readonly ILogger<ThreadModel> _logger;
        private readonly AppDbContext _context;

        public ThreadModel(ILogger<ThreadModel> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public ulong ParentId {get; set;}

        public IList<Post> Posts {get; set;}

        public void OnGet()
        {
            Posts = _context.Posts
                .Where(post => post.ParentId == ParentId || post.Id == ParentId)
                .ToList();
            var name = Request.Cookies["LastName"];
            var hash = Request.Cookies["LastHash"];
            Submission = new() {
                Name = name,
                Hash = hash
            };
        }

        public class PostVM
        {
            [Required]
            public string Content { get; set; }
            public string Name {get; set;}
            public string Hash {get; set;}
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
                ParentId = ParentId,
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
            return RedirectToPage("./Thread", new { ParentId = ParentId });
        }
    }
}

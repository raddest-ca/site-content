using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using app.Data;
using app.Models;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace app.Pages
{
    public class ThreadModel : PageModel
    {
        private readonly ILogger<ThreadModel> _logger;
        private readonly AppDbContext _context;
        private readonly BlobContainerClient _blob;

        public ThreadModel(ILogger<ThreadModel> logger, AppDbContext context, BlobContainerClient blobClient)
        {
            _logger = logger;
            _context = context;
            _blob = blobClient;
        }

        [BindProperty(SupportsGet = true)]
        public ulong ParentId {get; set;}

        [BindProperty]
        public Submission Submission { get; set; }

        public IList<Post> Posts {get; set;}

        public async Task OnGetAsync()
        {
            Posts = await _context.GetThread(ParentId);
            Submission = Submission.FromRequest(Request);
        }

        public async Task<IActionResult> OnPost()
        {
            Console.WriteLine($"Got post with content {Submission.Content}");
            if (!ModelState.IsValid) 
            {
                await OnGetAsync();
                return Page();
            }
            
            await Submission.Commit(
                _context,
                _blob,
                Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                ParentId
            );

            Response.Cookies.Append("LastName", Submission.Name);
            Response.Cookies.Append("LastHash", Submission.Hash);
            return RedirectToPage("./Thread", new { ParentId = ParentId });
        }
    }
}

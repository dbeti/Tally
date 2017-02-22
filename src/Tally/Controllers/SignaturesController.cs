using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.IO;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Tally.Data;
using Microsoft.EntityFrameworkCore;

namespace Tally.Controllers
{
    [Produces("application/json")]
    public class SignaturesController : Controller
    {

        private Config configuration;
        private ApplicationDbContext context;

        public SignaturesController(IOptions<Config> _configuration, ApplicationDbContext _context)
        {
            configuration = _configuration.Value;
            context = _context;
        }

        public async Task<IActionResult> Details(string student, int course)
        {
            var signatures = context.Signature.Include(s => s.Student).Include(s => s.Lecture).Include(s => s.Lecture.Course).ToList().Where(s => s.Student.Id == student && s.Lecture.Course.CourseId == course);
            ViewData["Student"] = context.Users.FirstOrDefault(u => u.Id == student);
            ViewData["CourseId"] = course;
            return View(signatures.ToList());
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Post([FromBody] dynamic request)
        {
            string value = request.img ?? "";
            int lecture = request.lecture ?? "";
            string student = request.student ?? "";

            var base64Data = Regex.Match(value, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
            var binData = Convert.FromBase64String(base64Data);

            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(configuration.AzureStorageConnectionString);
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("signatures");

            string imageName = $"{lecture}_{student}.jpg";

            CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(imageName);
            cloudBlockBlob.Properties.ContentType = "image/jpg";
            await cloudBlockBlob.UploadFromByteArrayAsync(binData, 0, binData.Length);

            var imageUri = cloudBlockBlob.Uri.ToString();
            var signature = context.Signature.FirstOrDefault(s => s.Lecture.LectureId == lecture && s.Student.Id == student);
            if (signature == null)
            {
                signature = new Models.Signature()
                {
                    Lecture = context.Lecture.FirstOrDefault(l => l.LectureId == lecture),
                    Student = context.Users.FirstOrDefault(u => u.Id == student)
                };
                context.Signature.Add(signature);
            }

            signature.Location = imageUri;
            await context.SaveChangesAsync();

            return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
        }
    }
}

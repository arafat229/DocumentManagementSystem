using Microsoft.Identity.Client;

namespace DocumentManagementSystem.Helpers
{
    public class UploadHandler
    {
        public  string Upload(IFormFile file)
        {
            //extension 
            List<string> validExtension = new List<string> { ".pdf", ".docx", ".jpg" };

            string extension = Path.GetExtension(file.FileName);
            if (!validExtension.Contains(extension))
                return $"Extension not valid {string.Join(",", validExtension)}";

            //Size
            long size = file.Length;
            if (size > 10 * 1024 * 1024)
                return "Maximum size is 10MB";

            //name Changing
            string filename = Guid.NewGuid().ToString()+ extension;
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
          using  FileStream stream = new FileStream(Path.Combine(path, filename), FileMode.Create);
            file.CopyTo(stream);

            return filename;

        }
    }
}


namespace MR.Tech.Storage.Src.Shared.Helpers
{
    public static class MimeHelper
    {
        private static bool IsApplicationPdf(byte[] fileBytes)
        {
            if (fileBytes[0] == 0x25 && fileBytes[1] == 0x50 &&
            fileBytes[2] == 0x44 && fileBytes[3] == 0x46)
             return true;

            return false;
        }

        private static bool IsImagePng(byte[] fileBytes)
        {
            if (fileBytes.Length >= 8 &&
            fileBytes[0] == 0x89 && fileBytes[1] == 0x50 &&
            fileBytes[2] == 0x4E && fileBytes[3] == 0x47 &&
            fileBytes[4] == 0x0D && fileBytes[5] == 0x0A &&
            fileBytes[6] == 0x1A && fileBytes[7] == 0x0A)
                return true;
            return false;
        }

        private static bool IsImageJpeg(byte[] fileBytes)
        {
            if (fileBytes[0] == 0xFF && fileBytes[1] == 0xD8)
                return true;
            return false;
        }



        public static string MimeFinder( byte[] fileBytes )
        {
             if(IsImageJpeg(fileBytes) == true)
             {
                return "image/jpeg";
             }

             if(IsApplicationPdf(fileBytes) == true)
             {
                return "application/pdf";
             }

            if (IsImagePng(fileBytes) == true)
                return "image/png";
            return "";
        }


    }
}

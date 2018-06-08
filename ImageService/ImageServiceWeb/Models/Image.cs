using System.ComponentModel.DataAnnotations;

namespace ImageServiceWeb.Models
{
    public class Image
    {
        static int count = 0;
        public Image()
        {
            count++;
            ID = count;
        }

        public Image(string name, string year, string month, string path, string srcPath)
        {
            Name = name;
            Year = year;
            Month = month;
            Path = path;
            SrcPath = srcPath;
            if (year == "1")
                Date = "Unknown";
            else
                Date = month + "." + year;
        }

        public void copy(Image img)
        {
            Name = img.Name;
            Date = img.Date;
            Path = img.Path;
        }

        [Required]
        [Display(Name = "ID")]
        public int ID { get; set; }
 
        #region for Display only
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Date")]
        public string Date { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Path")]
        public string Path { get; set; }
        #endregion

        // identifier - path to Thums
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "SrcPath")]
        public string SrcPath { get; set; }

        public string NameInWeb { get; set; }
        public string Year { get; set; }
        public string Month { get; set; }

    }
}
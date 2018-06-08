using System.ComponentModel.DataAnnotations;

namespace ImageServiceWeb.Models
{
    /// <summary>
    /// represents a photo in the website. Every photo in 'Images' directory
    /// has an Image object.
    /// </summary>
    public class Image
    {
        static int count = 0;
        public Image()
        {
            count++;
            ID = count;
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="name">real(original) name of the photo</param>
        /// <param name="year">the year that photo was taken</param>
        /// <param name="month">the month that photo was taken</param>
        /// <param name="path">relative path to the photo in Images directory</param>
        /// <param name="srcPath">path of the original thumbnail photo in outputDir</param>
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

        /// <summary>
        /// copies the photo
        /// </summary>
        /// <param name="img">photo</param>
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

        /// <summary>
        /// used as an identifier of a photo- the path of the original thumbnail photo
        /// </summary>
        public string SrcPath { get; set; }
        /// <summary>
        /// the name of the photo as displays in Images directory in this project.
        /// </summary>
        public string NameInWeb { get; set; }
        public string Year { get; set; }
        public string Month { get; set; }

    }
}
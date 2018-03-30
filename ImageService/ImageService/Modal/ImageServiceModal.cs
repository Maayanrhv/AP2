﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace ImageService.Modal
{
    public class ImageServiceModal : IImageServiceModal
    {
        #region Members
        private string m_OutputFolder;            // The Output Folder
        private int m_thumbnailSize;              // The Size Of The Thumbnail Size

        #endregion

        public string AddFile(string path, out bool result)
        {
            //get the file name only
            string fileName = System.IO.Path.GetFileName(path);
            //use Path class to manipulate file and directory path.
            string destFile = System.IO.Path.Combine(m_OutputFolder, fileName);

            Image im = Image.FromFile(path);
            Size size = new Size(m_thumbnailSize, m_thumbnailSize);
            Bitmap bit = new Bitmap(im, size);
            try
            {
                bit.Save(destFile);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                result = true;
                return "failed";
            }
            result = false;
            return "succeeded";
        }

    }
}
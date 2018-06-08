using System.Collections.Generic;
namespace ImageServiceWeb.Models
{
    interface IPhotosModel
    {
        /// <summary>
        /// all the displayed photos that in Imaged Directory
        /// </summary>
        List<Models.Image> Photos { get; }

        /// <summary>
        /// a photo that the user chose to delete
        /// </summary>
        Models.Image PhotoToDelete { get; set; }

        /// <summary>
        /// how many photos are there
        /// </summary>
        int NumOfPhotos { get; }

        /// <summary>
        /// copy the full photo from OutputDir to Images and return the relative path
        /// </summary>
        /// <param name="photo">some photo that in the Images directory</param>
        /// <returns>the relative path to the full photo</returns>
        string GetFullImagePath(Models.Image photo);

        /// <summary>
        /// deletes a photo from the photos list, from Images dir, and from outputDir.
        /// if the deletions are leaving an empty folder - it is being deleted.
        /// </summary>
        /// <param name="photo">a photo to delete.</param>
        void RemovePhoto(Models.Image photo);

        /// <summary>
        /// copy photos from outputDir to Images and add them to Photos list.
        /// </summary>
        void LoadPhotos();
    }
}

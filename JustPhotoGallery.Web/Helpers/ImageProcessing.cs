using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using JustPhotoGallery.Domain.Entities;

namespace JustPhotoGallery.Web.Helpers
{
    public class ImageProcessing
    {
        public void CreateThumbnail(String filename, String directory, int width, int height, String prefix = "thumb_")
        {
            var image = new WebImage(Path.Combine(directory, filename));

            int newWidth = image.Width < image.Height ? image.Width*height/image.Height : width;
            int newHeight = image.Width < image.Height ? height : image.Height*width/image.Width;

            image.Resize(newWidth, newHeight, true).Crop(1, 1);
            image.Save(Path.Combine(directory, String.Format("{0}{1}", prefix, filename)), image.ImageFormat);
        }

        public void FilterImage(String filename, String directory, FilterType filterType, String prefix = "filtered_")
        {
            var original = Image.FromFile(Path.Combine(directory, filename));
            
            var newBitmap = new Bitmap(original.Width, original.Height);

            using (var g = Graphics.FromImage(newBitmap))
            {
                var colorMatrix = new ColorMatrix();

                switch (filterType)
                {
                    case FilterType.BlackWhite:
                        colorMatrix = new ColorMatrix(new float[][]
                        {
                            new float[] {.3f, .3f, .3f, 0, 0},
                            new float[] {.59f, .59f, .59f, 0, 0},
                            new float[] {.11f, .11f, .11f, 0, 0},
                            new float[] {0, 0, 0, 1, 0},
                            new float[] {0, 0, 0, 0, 1}
                        });
                        break;
                    case FilterType.Sepia:
                        colorMatrix = new ColorMatrix(new float[][]
                        {
                            new float[] {.393f, .349f, .272f, 0, 0},
                            new float[] {.769f, .686f, .534f, 0, 0},
                            new float[] {.189f, .168f, .131f, 0, 0},
                            new float[] {0, 0, 0, 1, 0},
                            new float[] {0, 0, 0, 0, 1}
                        });
                        break;
                    case FilterType.Autocontrast:
                        colorMatrix = new ColorMatrix(new float[][]
                        {
                            new float[] {1.438f, -0.062f, -0.062f, 0, 0},
                            new float[] {-0.122f, 1.378f, -0.122f, 0, 0},
                            new float[] {-0.016f, -0.016f, 1.483f, 0, 0},
                            new float[] {0, 0, 0, 1, 0},
                            new float[] {-0.03f, 0.05f, -0.02f, 0, 1}
                        });
                        break;
                }

                var attributes = new ImageAttributes();
                attributes.SetColorMatrix(colorMatrix);
                g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
                    0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);
            }
            newBitmap.Save(Path.Combine(directory, String.Format("{0}{1}", prefix, filename)));
        }
    }
}
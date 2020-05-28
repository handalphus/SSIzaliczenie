using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
namespace GrafikaBayes
{
    class Grafika
    {
        public static byte[][][] Macierz(string pathToFile)
        {
            byte[][][] picture = new byte[3][][];
            
            
            if (File.Exists(pathToFile))
            {
                Bitmap bitmap = new Bitmap(pathToFile);
                picture[0] = new byte[bitmap.Width][];
                picture[1] = new byte[bitmap.Width][];
                picture[2] = new byte[bitmap.Width][];

                for (int i = 0; i < bitmap.Width; i++)
                {
                    picture[0][i] =new  byte[bitmap.Height];
                    picture[1][i]=new byte[bitmap.Height];
                    picture[2][i]=new byte[bitmap.Height];
                    for (int j = 0; j < bitmap.Height; j++)
                    {

                        picture[0][i][j] =(bitmap.GetPixel(i, j).R);
                        picture[1][i][j]=(bitmap.GetPixel(i, j).G);
                        picture[2][i][j]=(bitmap.GetPixel(i, j).B);

                    }
                }
            }
            return picture;

        }
        public static byte[][] GetGrayPicture(string path)
        {
            Bitmap bitmap = new Bitmap(path);
           
            byte[][] picture = new byte[bitmap.Width][];
            
            for (int i = 0; i < bitmap.Width; i++)
            {
                picture[i] = new byte[bitmap.Height];
               

                for (int j = 0; j < bitmap.Height; j++)
                {
                    picture[i][j] = (byte)((bitmap.GetPixel(i, j).R + bitmap.GetPixel(i, j).G+ bitmap.GetPixel(i, j).B)/3);                 
                }
            }
            return picture;
        }
        private static byte[][] GetExtremaFromPicture(string pathToExtrema)
        {
            Bitmap bitmapExtrema = new Bitmap(pathToExtrema);

            byte[][] extrema = new byte[bitmapExtrema.Width][];
            for (int i = 0; i < bitmapExtrema.Width; i++)
            {

                extrema[i] = new byte[bitmapExtrema.Height];

                for (int j = 0; j < bitmapExtrema.Height; j++)
                {
                    if (bitmapExtrema.GetPixel(i, j).R == 255)
                    {
                        extrema[i][j] = bitmapExtrema.GetPixel(i, j).R;
                    }
                    else
                    {
                        extrema[i][j] = 0;
                    }
                }
            }

            return extrema;
        }

        private static byte[][] GrayScale(byte[][][] image)
        {
            int width = image[0].Length;
            int height = image[0][0].Length;
            byte[][] gray = new byte[width][];
            for (int i = 0; i < width; i++)
            {
                gray[i]=new byte[height];
            }
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    gray[i][j]=(byte)((image[0][i][j] + image[1][i][j] + image[2][i][j]) / 3);
                }
            }
            return gray;
        }
        public static void SaveImage(byte[][] gray, string pathToFile)
        {
            int width = gray.Length;
            int height = gray[0].Length;
            Bitmap bitmap = new Bitmap(width, height);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    bitmap.SetPixel(i, j, Color.FromArgb(gray[i][j], gray[i][j], gray[i][j]));
                }
            }
            bitmap.Save(pathToFile);

        }
        private static void DrawKeyPoints(string pathToImage, byte[][] extrema, string pathToFile)
        {
           
            int width = extrema.Length;
            int height = extrema[0].Length;
            Bitmap bitmap = new Bitmap(width, height);
            Bitmap image = new Bitmap(pathToImage);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (extrema[i][j]==255)
                    {
                        bitmap.SetPixel(i, j, Color.FromArgb(255,0, 0));
                    }
                    else
                    {
                        bitmap.SetPixel(i, j, image.GetPixel(i, j));
                    }
                    
                }
            }
            bitmap.Save(pathToFile);

        }

        private static byte[][] FilterImage(byte[][] grayImage, double[][] kernel)
        {
            int width = grayImage.Length;
            int height = grayImage[0].Length;
            byte[][] filtered = new byte[width][];
            for (int i = 0;i< width; i++)
            {
                filtered[i] = new byte[height];
            }
            int margin = (int)Math.Floor((double)kernel.Length / 2);
            double rate = 0;
            for (int i = 0; i < kernel.Length; i++)
            {
                for (int j = 0; j < kernel[0].Length; j++)
                {
                    rate += kernel[i][j];
                }
            }
            if (rate == 0)
            {
                rate = 1;
            }
            for (int i = margin; i < width - margin; i++)
            {
               
                for (int j = margin; j < height - margin; j++)
                {
                    double value = 0;
                    for (int k = 0; k < kernel.Length; k++)
                    {
                        for (int l = 0; l < kernel[0].Length; l++)
                        {
                            value += kernel[k][l] * grayImage[i + k - margin][j + l - margin];
                        }
                    }
                    value /= rate;
                    
                    if (value < 0)
                    {
                        value = 0;
                    }
                    else if (value > 255)
                    {
                        value = 255;
                    }
                    filtered[i ][j] = ((byte)value);

                }
            }
            return filtered;
        }
        private static double[][]GaussianMask(double omega, int radius)
        {
            double[][] mask = new double[2*radius+1][];
            for (int i = 0; i < 2*radius+1; i++)
            {
                mask[i] = new double[2*radius+1];
               
            }
            for (int i = -radius; i < radius+1; i++)
            {
                for (int j  = -radius; j < radius + 1; j++)
                {
                    mask[i + radius][j + radius] = 1 / (2 * Math.PI * Math.Pow(omega, 2)) * Math.Exp(-(Math.Pow(i, 2) + Math.Pow(i, 2)) / (2 * Math.Pow(omega, 2)));
                }
            }


            return mask;
        }
        private static double[][] DifferenceOfGaussianMask(double[][] GaussianMask1, double[][] GaussianMask2)
        {
            int n = GaussianMask1.Length;
           double[][] mask = new double[n][];
            for (int i = 0; i < n; i++)
            {
                mask[i] = new double[n];
                
            }
            if (n==GaussianMask2.Length)
            {
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        mask[i][j] = GaussianMask1[i][j] - GaussianMask2[i][j];
                    }
                }
            }
            return mask;
        }
        private static void Filter(string pathToFile, double[][] kernel, string name)
        {

            string[] a = pathToFile.Split('.');
            a[a.Length - 1] = $"{name}.{a[a.Length - 1]}";
            string pathToSave = string.Concat(a);

           byte[][][] image = Macierz(pathToFile);
            byte[][] szary = GrayScale(image);
            SaveImage(FilterImage(szary, kernel), $@"{pathToSave}");
        }
        public static byte[][] Filtr_DOG(string pathToFile, double omega)
        {
            double[][] kernel = DifferenceOfGaussianMask(GaussianMask(Math.Sqrt(2) *omega, 4),GaussianMask(omega,4));
            byte[][][] image = Macierz(pathToFile);
            byte[][] szary = GrayScale(image);
            return FilterImage(szary, kernel);
            //Filter(pathToFile, kernel, $"_DOG{(int)omega}");
        }
        private static byte[][] FindExtrema(byte[][] picture, byte[][] pictureNext, byte[][] picturePrevious)
        {
            
      

            for (int i = 1; i < picture.Length-1; i++)
            {
                for (int j = 1; j < picture[0].Length-1; j++)
                {
                    bool isMinimum = true;
                    bool isMaximum = true;
                    for (int k = -1; k < 2 && (isMinimum || isMaximum); k++)
                    {
                        for (int l = -1; l < 2 && (isMinimum || isMaximum); l++)
                        {
                            if(picture[i][j]<picture[i+k][j+l] || picture[i][j] < pictureNext[i + k][j + l]|| picture[i][j] < picturePrevious[i + k][j + l])
                            {
                                isMaximum = false;
                            }
                            if(picture[i][j]>picture[i+k][j+l] || picture[i][j] > pictureNext[i + k][j + l]|| picture[i][j] > picturePrevious[i + k][j + l])
                            {
                                isMinimum = false;
                            }

                        }
                    }
                    if (isMaximum || isMinimum)
                    {
                        picture[i][j] = 255;
                    }
                }
            }
            return picture;
            
        }
       private static byte[][] CheckExtrema(byte[][] picture,double currentOmega, byte[][] pictureNext, byte[][] picturePrevious, byte[][] extrema)
        {
            
            for (int i = 0; i < extrema.Length; i++)
            {
                for (int j = 0; j < extrema[0].Length; j++)
                {
                    if (extrema[i][j] == 255)
                    {
                        double[] grad = Gradient3D(i, j, picture, pictureNext, picturePrevious);
                        double[][] hess = Hessian3D(i, j, picture, pictureNext, picturePrevious);
                        double[] hessHelp = new double[3];
                        double[][] hessInv = Inverse3x3(hess);
                        double[] xHat = new double[3];

                        for (int k = 0; k < 3; k++)
                        {
                            hessHelp[k] = (i * hess[k][0] + j * hess[k][1] + currentOmega * hess[k][2]);
                            xHat[k] = (-(hessInv[k][0] * grad[0] + hessInv[k][1] * grad[1] + hessInv[k][2] * grad[2]));
                        }
                        double diff = Math.Abs(i * (grad[0] + 0.5 * xHat[0]) +
                            j * (grad[1] + 0.5 * xHat[1]) +
                            currentOmega * (grad[2] + 0.5 * xHat[2]));
                        if (diff < 0.03)
                        {
                            picture[i][j] = 255;
                        }
                       
                    }
                }
            }
            return picture;
           
        }

       
        private static double[] Gradient3D(int i, int j, byte[][] picture,
    byte[][] pictureNext,
   byte[][] picturePrevious)
        {
            double[] grad = new double[3];
            grad[0]=(picture[i + 1][j] - picture[i - 1][j]) / 2;
            grad[1] = (picture[i][j + 1] - picture[i][j - 1]) / 2;
            grad[2] = (pictureNext[i][j] - picturePrevious[i][j]) / 2;
           
            return grad;
        }
        private static double[][] Hessian3D(int i, int j, byte[][] picture,
    byte[][] pictureNext,
   byte[][] picturePrevious)
        {
            double[][] hess = new double[3][];

            hess[0] = new double[3];
            hess[1] = new double[3];
            hess[2] = new double[3];
            
            hess[0][0] = picture[i + 1][j] + picture[i - 1][j] - 2 * picture[i ][j];
            hess[1][1] = picture[i ][j+1] + picture[i][j-1] - 2 * picture[i][j];
            hess[2][2] = pictureNext[i ][j] + picturePrevious[i - 1][j] - 2 * picture[i ][j];
            int h01 = (picture[i + 1][j + 1] - picture[i + 1][j - 1] - picture[i - 1][j + 1] + picture[i - 1][j - 1]) / 4;
            int h02 = (pictureNext[i + 1][j] - pictureNext[i - 1][j] - picturePrevious[i + 1][j] + picturePrevious[i - 1][j]) / 4;
            int h12 = (pictureNext[i ][j+1] - pictureNext[i ][j -1] - picturePrevious[i ][j +1] + picturePrevious[i ][j -1]) / 4;
            hess[0][1] = h01;
            hess[0][2] = h02;
            hess[1][2] = h12;
            hess[1][0] = h01;
            hess[2][0] = h02;
            hess[2][1] = h12;

            return hess;
        }
        private static double[][] Inverse3x3(double[][] A)
        {
            double[][] Inverse = new double[3][];

            Inverse[0] = new double[3];
            Inverse[1] = new double[3];
            Inverse[2] = new double[3];
            
            double a = A[0][0]; double b =A[0][1]; double c = A[0][2]; double d = A[1][0]; double e = A[1][1]; double f= A[1][2]; double g = A[2][0]; double h = A[2][1]; double i = A[2][2];
            double k = 1 / (a * (e * i - f * h) - b * (d * i - f * g) + c * (d * h - e * g));
            Inverse[0][0] = (e * i - f * h) * k;
            Inverse[0][1] = (c * h - b * i) * k;
            Inverse[0][2] = (b * f - c *e) * k;
             Inverse[1][0] = (g * f - d * i) * k;
            Inverse[1][1] = (a * i - c * g) * k;
            Inverse[1][2] = (c * d - a *f) * k;
             Inverse[2][0] = (d * h - e * g) * k;
            Inverse[2][1] = (b * g - a * h) * k;
            Inverse[2][2] = (a * e - b *d) * k;

            return Inverse;
        }
        private static double[][] Hessian2D(int i, int j, byte[][] picture)
        {
            double[][] hess = new double[2][];

            hess[0] = new double[2];
            hess[1] = new double[2];
            
            hess[0][0] = picture[i + 1][j] + picture[i - 1][j] - 2 * picture[i][j];
            hess[1][1] = picture[i][j + 1] + picture[i][j - 1] - 2 * picture[i][j];
            int h01 = (picture[i + 1][j + 1] - picture[i + 1][j - 1] - picture[i - 1][j + 1] + picture[i - 1][j - 1]) / 4;
            return hess;
        }
        private static byte[][] CheckEdges(byte[][] picture, byte[][] extrema)
        {
            
       
            for (int i = 0; i < picture.Length; i++)
            {
                for (int j = 0; j < picture[0].Length; j++)
                {
                    if (extrema[i][j]==255)
                    {
                        double[][] hess = Hessian2D(i, j, picture);
                        double edgeness = Math.Pow(hess[0][0] + hess[1][1], 2) / ((hess[0][0] * hess[1][1]) - (hess[0][1] * hess[1][0]));
                        double EdgeTreshold = 10;
                        if (edgeness <= Math.Pow(EdgeTreshold + 1, 2) / EdgeTreshold)
                        {
                            picture[i][j] = 255;
                        }
                        
                    }
                }
            }
            return picture;
        }
        public static void PrawieSIFT(string pathToImage)
        {
            string[] name = pathToImage.Split('.');
            name[name.Length - 1] = $"_Extrema.{name[name.Length-1]}";
            string pathToSave = string.Concat(name);
            byte[][] omega1Image = Filtr_DOG(pathToImage, 2.5);
            byte[][] omega2Image = Filtr_DOG(pathToImage, 5);
            byte[][] omega3Image = Filtr_DOG(pathToImage, 7.5);

           byte[][] extrema = FindExtrema(omega2Image,omega3Image,omega1Image);
           extrema= CheckExtrema(omega2Image, 5,omega3Image,omega1Image,extrema );
            extrema = CheckEdges(omega2Image,extrema);
           DrawKeyPoints(pathToImage,extrema,pathToSave);
            Console.WriteLine($"Obrazek zapisany w {pathToSave}");

        }
    }

}

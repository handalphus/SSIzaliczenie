using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSIzaliczenie
{
    class KeyPoints
    {
        public static int[][][] Macierz(string addr)
        {
            Bitmap image = new Bitmap(addr);
            int[][][] matrix = new int[3][][];
            int[][] R = new int[image.Width][];
            int[][] G = new int[image.Width][];
            int[][] B = new int[image.Width][];
            for (int i = 0; i < image.Width; i++)
            {
                R[i] = new int[image.Height];
                G[i] = new int[image.Height];
                B[i] = new int[image.Height];


            }




            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    Color pxl = image.GetPixel(i, j);
                    R[i][j] = pxl.R;
                    G[i][j] = pxl.G;
                    B[i][j] = pxl.B;
                }
            }
            matrix[0] = R;
            matrix[1] = G;
            matrix[2] = B;
            return matrix;

        }


        public static byte[][] Binarize(int[][][] matrix)
        {
            Bitmap image2 = new Bitmap(matrix[0].Length, matrix[0][0].Length);
            byte[][] binarized = new byte[matrix[0].Length][];
            for (int i = 0; i < image2.Width; i++)
            {
                binarized[i] = new byte[matrix[0][0].Length];
                for (int j = 0; j < image2.Height; j++)
                {
                    double sR = 0;
                    double sG = 0;
                    double sB = 0;
                    for (int k = -1; k < 2; k++)
                    {
                        for (int n = -1; n < 2; n++)
                        {
                            if (i + k >= 0 && j + n >= 0 && i + k < image2.Width && j + n < image2.Height && (k != 0 || n != 0))
                            {
                                sR += matrix[0][i + k][j + n];
                                sG += matrix[1][i + k][j + n];
                                sB += matrix[2][i + k][j + n];
                            }

                        }

                    }

                    int sRn = (int)sR / 8;
                    int sGn = (int)sG / 8;
                    int sBn = (int)sB / 8;

                    if (sRn > matrix[0][i][j] && sGn > matrix[1][i][j] && sBn > matrix[2][i][j])
                    {
                        image2.SetPixel(i, j, Color.FromArgb(0, 0, 255));
                        binarized[i][j] = 1;
                    }
                    else
                    {
                        image2.SetPixel(i, j, Color.FromArgb(255, 255, 255));
                        binarized[i][j] = 0;
                    }

                }
               
            }

            return binarized;

            //image2.Save(addr);

        }
        public static  byte[] BuildBinarizedVector(byte[][] image)
        {
            byte[] result = new byte[image.Length * image[0].Length];
            for (int i = 0; i < image.Length; i++)
            {
                for (int j = 0; j < image[0].Length; j++)
                {
                    result[i * image[0].Length + j] = image[i][j];
                }
            }

            return result;
        }
    }
}

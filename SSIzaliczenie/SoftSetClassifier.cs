using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSIzaliczenie
{
    class SoftSetClassifier
    {
        private string pathToPicture;
        private string pathToPattern;
        public SoftSetClassifier(string pathToPicture, string pathToPattern)
        {
            this.pathToPicture = pathToPicture;
            this.pathToPattern = pathToPattern;
        }
        public void FindSimilarPatterns(string pathToSave)
        {
            Bitmap result = new Bitmap(pathToPicture);
            int[][][] matrixD = KeyPoints.Macierz(pathToPicture);
            int[][][] matrixM = KeyPoints.Macierz(pathToPattern);
            //binaryzacja duzego

            byte[][] sourceImage = KeyPoints.Binarize(matrixD);

            //binaryzacja malego
            //byte[][] malyObrazek = KeyPoints.Binarize(matrixM);
            byte[][] pattern = GrafikaBayes.Grafika.GetGrayPicture(pathToPattern);
            double mean = 0;
            for (int i = 0; i < pattern.Length; i++)
            {
                for (int j = 0; j < pattern[0].Length; j++)
                {
                    mean += pattern[i][j];
                }
            }

            mean /= pattern.Length * pattern[0].Length;
            for (int i = 0; i < sourceImage.Length - pattern.Length; i++)
            {
                if (i % 100 == 0)
                {
                    Console.WriteLine(i);
                }
                for (int j = 0; j < sourceImage[i].Length - pattern[0].Length; j++)
                {
                    //Console.WriteLine($"{i}, {j}");
                    //w malym prostokacie na duzymObrazku szukamy podobienstwa do malyObrazek
                    double[] grayVector = new double[pattern.Length * pattern[0].Length];
                    int[] binarizedVector = new int[pattern.Length * pattern[0].Length];
                    double sumOfImage = 0;
                    //utworzenie wektorow do malego obrazka
                    for (int k = 0; k < pattern.Length; k++)
                    {
                        for (int l = 0; l < pattern[k].Length; l++)
                        {
                            if (pattern[k][l] < mean)
                            {
                                grayVector[k * pattern[k].Length + l] = (255.0 - pattern[k][l]) / 255.0;
                                //grayVector[k *malyObrazek[k].Length + l] = malyObrazek[k][l];

                            }
                            else
                            {
                                grayVector[k * pattern[k].Length + l] = 0;
                            }


                            binarizedVector[k * pattern[k].Length + l] = sourceImage[i + k][j + l];
                        }
                    }
                    //klasyfikacja malego obrazka
                    double resultClassification = 0;
                    for (int k = 0; k < binarizedVector.Length; k++)
                    {
                        resultClassification += grayVector[k] * (double)binarizedVector[k];
                        sumOfImage += grayVector[k];
                    }
                    //tym parametrem się reguluje jakość dopasowan - 0.6 dobre do druku, 0.4 dobre do pisma recznego
                    if (resultClassification / sumOfImage > 0.6)
                    {
                        Console.WriteLine($"{i} {j}");
                        for (int k = 0; k < pattern.Length; k++)
                        {

                            result.SetPixel(i + k, j, Color.FromArgb(0, 255, 0));
                           result.SetPixel(i + k, j + pattern[0].Length, Color.FromArgb(0, 255, 0));

                        }
                        for (int k = 0; k < pattern[0].Length; k++)
                        {
                            result.SetPixel(i, j + k, Color.FromArgb(0, 255, 0));
                            result.SetPixel(i + pattern.Length, j + k, Color.FromArgb(0, 255, 0));
                        }
                        Console.WriteLine(resultClassification / sumOfImage);
                        Console.WriteLine();
                    }


                }
            }
            Console.WriteLine($"zapisano w lokalizacji: {pathToSave}");
            result.Save(pathToSave);
        }
    }
}

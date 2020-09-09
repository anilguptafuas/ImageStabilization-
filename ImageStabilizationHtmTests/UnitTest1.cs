using ImageBinarizer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoCortexApi;
using NeoCortexApi.Entities;
using NeoCortexApi.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ImageStabilizationHtmTests
{
    /// <summary>
    /// Class for testing of Stabilization of Similar Images
    /// </summary>
    [TestClass]
    public class UnitTest1
    {
        /// <summary>
        /// Test Method for testing of Stabilization of Similar Images
        /// </summary>
        [TestMethod]
        public void TestMethod1()
        {
            var parameters = Helpers.GetDefaultParams(); ;

            var imageWidth = 30;
            var imageHeight = 30;

            int outputWidth = 60;
            int outputHeight = 60;

            String trainingImageName = "Lamp25Shift.PNG";
            String predictionImageNameShift = "Lamp25Shift.PNG";
            String predictionImageNameRotate = "LampRotate.PNG";            
            String predictionImageNameDifferent= "Fish.PNG";

            bool withTraining = true;

            StringBuilder reportFile = new StringBuilder();

            reportFile.Append($"Given Image Width: {imageWidth}");
            reportFile.AppendLine();
            reportFile.Append($"Given Image Height: {imageHeight}");
            reportFile.AppendLine();
            reportFile.Append($"Given Output Column Width: {outputWidth}");
            reportFile.AppendLine();
            reportFile.Append($"Given Output Column Height: {outputHeight}");
            reportFile.AppendLine();

            parameters.setInputDimensions(new int[] { imageWidth, imageHeight });
            parameters.setColumnDimensions(new int[] { outputWidth, outputHeight });
            
            parameters.setNumActiveColumnsPerInhArea(0.02 * outputWidth * outputHeight);

            var sp = new SpatialPooler();

            var mem = new Connections();
            parameters.apply(mem);
            sp.init(mem);

            int[] activeArray = new int[outputWidth * outputHeight];

            int[] inputVector = ReadImageData(trainingImageName, imageHeight, imageWidth);

            int[] newActiveArray = new int[outputWidth * outputHeight];

            if (withTraining)
            {
                //Training 
                sp.compute(mem, inputVector, activeArray, true);

                var activeCols = ArrayUtils.IndexWhere(activeArray, (el) => el == 1);

                int[] oldActiveArray = activeArray;

                int flag = 0;

                while (flag == 0)
                {
                    sp.compute(mem, inputVector, newActiveArray, true);

                    activeCols = ArrayUtils.IndexWhere(newActiveArray, (el) => el == 1);

                    if (GetHammingDistance(oldActiveArray, newActiveArray) == 0)
                    {
                        flag = 1;
                    }
                    else
                    {
                        flag = 0;
                        oldActiveArray = newActiveArray;
                    }
                }

                var str = Helpers.StringifyVector(activeCols);

                reportFile.AppendLine();
                reportFile.Append($"Active Columns of Trained Image({trainingImageName}):");
                reportFile.AppendLine();
                reportFile.Append(str);
                reportFile.AppendLine();
                reportFile.Append($"Number of Active Columns of Trained Image({trainingImageName}): {activeCols.Length}");
                reportFile.AppendLine();
            }
            else
            {
                //Without Training 
                sp.compute(mem, inputVector, newActiveArray, false);

                var activeCols = ArrayUtils.IndexWhere(newActiveArray, (el) => el == 1);

                var str = Helpers.StringifyVector(activeCols);

                reportFile.AppendLine();
                reportFile.Append($"Active Columns of Untrained Image({trainingImageName}):");
                reportFile.AppendLine();
                reportFile.Append(str);
                reportFile.AppendLine();
                reportFile.Append($"Number of Active Columns of Untrained Image({trainingImageName}): {activeCols.Length}");
                reportFile.AppendLine();
            }         
                       

            //Prediction with Shift
            int[] inputVectorShift = ReadImageData(predictionImageNameShift, imageHeight, imageWidth);

            int[] activeArrayShift = new int[outputWidth * outputHeight];

            sp.compute(mem, inputVectorShift, activeArrayShift, false);

            var resActiveColsShift = ArrayUtils.IndexWhere(activeArrayShift, (el) => el == 1);

            var resStrShift = Helpers.StringifyVector(resActiveColsShift);

            int hamDistShift = GetHammingDistance(newActiveArray, activeArrayShift);

            reportFile.AppendLine();
            reportFile.Append($"Active Columns of Prediction of Trained Image with Shift({predictionImageNameShift}):");
            reportFile.AppendLine();
            reportFile.Append(resStrShift);
            reportFile.AppendLine();
            reportFile.Append($"Number of Active Columns of Prediction of Trained Image with Shift({predictionImageNameShift}): {resActiveColsShift.Length}");
            reportFile.AppendLine();
            reportFile.Append($"Hamming Distance between Trained Image({trainingImageName}) and Prediction of same Image with Shift({predictionImageNameShift}): {hamDistShift}");
            reportFile.AppendLine();
            reportFile.Append($"Hamming Distance in % between Trained Image({trainingImageName}) and Prediction of same Image with Shift({predictionImageNameShift}): {Math.Round((double)(hamDistShift * 100) / (outputWidth * outputHeight), 4)}%");
            reportFile.AppendLine();

            //Prediction with 90 degree Rotate
            int[] inputVectorR90 = ReadImageData(predictionImageNameRotate, imageHeight, imageWidth);

            int[] activeArrayR90 = new int[outputWidth * outputHeight];

            sp.compute(mem, inputVectorR90, activeArrayR90, false);

            var resActiveColsR90 = ArrayUtils.IndexWhere(activeArrayR90, (el) => el == 1);

            var resStrR90 = Helpers.StringifyVector(resActiveColsR90);

            int hamDistR90 = GetHammingDistance(newActiveArray, activeArrayR90);

            reportFile.AppendLine();
            reportFile.Append($"Active Columns of Prediction of Trained Image with 90 degrees rotate({predictionImageNameRotate}):");
            reportFile.AppendLine();
            reportFile.Append(resStrR90);
            reportFile.AppendLine();
            reportFile.Append($"Number of Active Columns of Prediction of Trained Image with 90 degrees rotate({predictionImageNameRotate}): {resActiveColsR90.Length}");
            reportFile.AppendLine();
            reportFile.Append($"Hamming Distance between Trained Image({trainingImageName}) and Prediction of same Image with 90 degrees rotate({predictionImageNameRotate}): {hamDistR90}");
            reportFile.AppendLine();
            reportFile.Append($"Hamming Distance in % between Trained Image({trainingImageName}) and Prediction of same Image with 90 degrees rotate({predictionImageNameRotate}): {Math.Round((double)(hamDistR90 * 100) / (outputWidth * outputHeight), 4)}%");
            reportFile.AppendLine();

            //Prediction with Different Image
            int[] inputVectorDifferent = ReadImageData(predictionImageNameDifferent, imageHeight, imageWidth);

            int[] activeArrayDifferent = new int[outputWidth * outputHeight];

            sp.compute(mem, inputVectorDifferent, activeArrayDifferent, false);

            var resActiveColsDifferent = ArrayUtils.IndexWhere(activeArrayDifferent, (el) => el == 1);

            var resStrDifferent = Helpers.StringifyVector(resActiveColsDifferent);

            int hamDistDifferent = GetHammingDistance(newActiveArray, activeArrayDifferent);

            reportFile.AppendLine();
            reportFile.Append($"Active Columns of Prediction of Different Image than Trained Image({predictionImageNameDifferent}):");
            reportFile.AppendLine();
            reportFile.Append(resStrDifferent);
            reportFile.AppendLine();
            reportFile.Append($"Number of Active Columns of Prediction of Different Image than Trained Image({predictionImageNameDifferent}): {resActiveColsDifferent.Length}");
            reportFile.AppendLine();
            reportFile.Append($"Hamming Distance between Trained Image({trainingImageName}) and Prediction of Different Image than Trained Image({predictionImageNameDifferent}): {hamDistDifferent}");
            reportFile.AppendLine();
            reportFile.Append($"Hamming Distance in % between Trained Image({trainingImageName}) and Prediction of Different Image than Trained Image({predictionImageNameDifferent}): {Math.Round((double)(hamDistDifferent * 100) / (outputWidth * outputHeight), 4)}%");
            reportFile.AppendLine();

            //Prediction with Noise of given percentages
            double[] givenNoisePercentages = { 0, 1, 5, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 };

            for (int i = 0; i < givenNoisePercentages.Length; i++)
            {
                int[] oldInputVector = ReadImageData(trainingImageName, imageHeight, imageWidth);

                int[] inputVectorNoisePercent = MakeSomeNoise(oldInputVector, givenNoisePercentages[i]/100);                

                int[] activeArrayNoisePercent = new int[outputWidth * outputHeight];

                MakeBinaryFile(inputVectorNoisePercent, imageHeight, imageWidth, $"{trainingImageName}-Noise{givenNoisePercentages[i]}p");

                sp.compute(mem, inputVectorNoisePercent, activeArrayNoisePercent, false);

                var resActiveColsPercent = ArrayUtils.IndexWhere(activeArrayNoisePercent, (el) => el == 1);

                var resStrPercent = Helpers.StringifyVector(resActiveColsPercent);

                int hamDistNoisePercent = GetHammingDistance(newActiveArray, activeArrayNoisePercent);

                reportFile.AppendLine();
                reportFile.Append($"Active Columns of Prediction of Trained Image({trainingImageName}) with Noise of {givenNoisePercentages[i]} percent:");
                reportFile.AppendLine();
                reportFile.Append(resStrPercent);
                reportFile.AppendLine();
                reportFile.Append($"Number of Active Columns of Prediction of Trained Image({trainingImageName}) with Noise of {givenNoisePercentages[i]} percent: {resActiveColsPercent.Length}");
                reportFile.AppendLine();
                reportFile.Append($"Hamming Distance between Trained Image({trainingImageName}) and Prediction of same Image with Noise of {givenNoisePercentages[i]} percent: {hamDistNoisePercent}");
                reportFile.AppendLine();
                reportFile.Append($"Hamming Distance in % between Trained Image({trainingImageName}) and Prediction of same Image with Noise of {givenNoisePercentages[i]} percent: {Math.Round((double)(hamDistNoisePercent*100) / (outputWidth * outputHeight), 4)}%");
                reportFile.AppendLine();
                reportFile.Append($"Output Overlap versus Input Overlap for Trained Image({trainingImageName}) and Prediction of same Image with Noise of {givenNoisePercentages[i]} percent: Output Overlap: {1 - Math.Round((double)(hamDistNoisePercent * 100) / (outputWidth * outputHeight), 4) / 100}, Input Overlap: {1 - givenNoisePercentages[i] / 100}");
                reportFile.AppendLine();
            }
            
            using (StreamWriter writer = File.CreateText(Path.Combine(AppContext.BaseDirectory, $"Output/report-{trainingImageName}.txt")))
            {
                string text = reportFile.ToString();
                writer.Write(text);
            }          

        }

        /// <summary>
        /// Test Method for testing of Stabilization of Similar Images with Shift
        /// </summary>
        [TestMethod]
        public void TestMethod2()
        {
            var parameters = Helpers.GetDefaultParams(); ;

            var imageWidth = 60;
            var imageHeight = 60;

            int outputWidth = 120;
            int outputHeight = 120;

            String trainingImageName = "Lamp.png";
            String predictionImageNameShift = "Lamp75Shift.PNG";

            bool withTraining = true;

            StringBuilder reportFile = new StringBuilder();

            reportFile.Append($"Given Image Width: {imageWidth}");
            reportFile.AppendLine();
            reportFile.Append($"Given Image Height: {imageHeight}");
            reportFile.AppendLine();
            reportFile.Append($"Given Output Column Width: {outputWidth}");
            reportFile.AppendLine();
            reportFile.Append($"Given Output Column Height: {outputHeight}");
            reportFile.AppendLine();

            parameters.setInputDimensions(new int[] { imageWidth, imageHeight });
            parameters.setColumnDimensions(new int[] { outputWidth, outputHeight });

            parameters.setNumActiveColumnsPerInhArea(0.02 * outputWidth * outputHeight);

            var sp = new SpatialPooler();

            var mem = new Connections();
            parameters.apply(mem);
            sp.init(mem);

            int[] activeArray = new int[outputWidth * outputHeight];

            int[] inputVector = ReadImageData(trainingImageName, imageHeight, imageWidth);

            int[] newActiveArray = new int[outputWidth * outputHeight];
            double[][] newActiveArrayDouble = new double[1][];
            newActiveArrayDouble[0] = new double[newActiveArray.Length];

            if (withTraining)
            {
                //Training 
                sp.compute(mem, inputVector, activeArray, true);

                var activeCols = ArrayUtils.IndexWhere(activeArray, (el) => el == 1);

                double[][] oldActiveArrayDouble = new double[1][];
                oldActiveArrayDouble[0] = new double[activeArray.Length];
                for (int i = 0; i < activeArray.Length; i++)
                {
                    oldActiveArrayDouble[0][i] = activeArray[i];
                }

                int isTrained = 0;

                while (isTrained == 0)
                {
                    sp.compute(mem, inputVector, newActiveArray, true);

                    activeCols = ArrayUtils.IndexWhere(newActiveArray, (el) => el == 1);

                    for (int i = 0; i < newActiveArray.Length; i++)
                    {
                        newActiveArrayDouble[0][i] = newActiveArray[i];
                    }

                    if (GetHammingDistances(oldActiveArrayDouble, newActiveArrayDouble, true)[0] == 100)
                    {
                        isTrained = 1;
                    }
                    else
                    {
                        isTrained = 0;
                        oldActiveArrayDouble = newActiveArrayDouble;
                    }
                }

                var str = Helpers.StringifyVector(activeCols);

                reportFile.AppendLine();
                reportFile.Append($"Active Columns of Trained Image({trainingImageName}):");
                reportFile.AppendLine();
                reportFile.Append(str);
                reportFile.AppendLine();
                reportFile.Append($"Number of Active Columns of Trained Image({trainingImageName}): {activeCols.Length}");
                reportFile.AppendLine();
            }
            else
            {
                //Without Training 
                sp.compute(mem, inputVector, newActiveArray, false);

                var activeCols = ArrayUtils.IndexWhere(newActiveArray, (el) => el == 1);

                var str = Helpers.StringifyVector(activeCols);

                reportFile.AppendLine();
                reportFile.Append($"Active Columns of Untrained Image({trainingImageName}):");
                reportFile.AppendLine();
                reportFile.Append(str);
                reportFile.AppendLine();
                reportFile.Append($"Number of Active Columns of Untrained Image({trainingImageName}): {activeCols.Length}");
                reportFile.AppendLine();
            }


            //Prediction with Shift
            int[] inputVectorShift = ReadImageData(predictionImageNameShift, imageHeight, imageWidth);

            int[] activeArrayShift = new int[outputWidth * outputHeight];

            sp.compute(mem, inputVectorShift, activeArrayShift, false);

            var resActiveColsShift = ArrayUtils.IndexWhere(activeArrayShift, (el) => el == 1);

            var resStrShift = Helpers.StringifyVector(resActiveColsShift);

            for (int i = 0; i < newActiveArray.Length; i++)
            {
                newActiveArrayDouble[0][i] = newActiveArray[i];
            }

            double[][] activeArrayShiftDouble = new double[1][];
            activeArrayShiftDouble[0] = new double[activeArrayShift.Length];
            for (int i = 0; i < activeArrayShift.Length; i++)
            {
                activeArrayShiftDouble[0][i] = activeArrayShift[i];
            }

            double hammingDistancePercentage = GetHammingDistances(newActiveArrayDouble, activeArrayShiftDouble, true)[0];

            reportFile.AppendLine();
            reportFile.Append($"Active Columns of Prediction of Trained Image with Shift({predictionImageNameShift}):");
            reportFile.AppendLine();
            reportFile.Append(resStrShift);
            reportFile.AppendLine();
            reportFile.Append($"Number of Active Columns of Prediction of Trained Image with Shift({predictionImageNameShift}): {resActiveColsShift.Length}");
            reportFile.AppendLine();
            reportFile.Append($"Hamming Distance in % between Trained Image({trainingImageName}) and Prediction of same Image with Shift({predictionImageNameShift}): {100-hammingDistancePercentage}%");
            reportFile.AppendLine();
            reportFile.Append($"Output Overlap for Trained Image({trainingImageName}) and Prediction of same Image with Shift({predictionImageNameShift}): Output Overlap: {hammingDistancePercentage/100}");
            reportFile.AppendLine();

            using (StreamWriter writer = File.CreateText(Path.Combine(AppContext.BaseDirectory, $"Output/report-{trainingImageName}.txt")))
            {
                string text = reportFile.ToString();
                writer.Write(text);
            }

        }

        /// <summary>
        /// Returns Binarized Image in integer array and creates file of Binarized Image
        /// </summary>
        /// <param name="imageName">Name of Image to be binarized</param>
        /// <param name="height">Height of Binarized Image</param>
        /// <param name="width">Width of Binarized Image</param>
        /// <returns></returns>
        public int[] ReadImageData(String imageName, int height, int width)
        {
            Binarizer bizer = new Binarizer(targetHeight: height, targetWidth: width);

            var imgName = Path.Combine(Path.Combine(AppContext.BaseDirectory, "Images"), imageName);

            bizer.CreateBinary(imgName, Path.Combine(Path.Combine(AppContext.BaseDirectory, "Output"), $"bin-{imageName}.txt"));

            var binaryString = bizer.GetBinary(imgName);

            int[] intArray = new int[height * width];
            int j = 0;
            for (int i = 0; i < binaryString.Length; i++)
            {
                if (binaryString[i].Equals('0'))
                {
                    intArray[j] = 0;
                    j++;
                }
                else if (binaryString[i].Equals('1'))
                {
                    intArray[j] = 1;
                    j++;
                }
            }
            return intArray;
        }

        /// <summary>
        /// Returns Noisy Integer Array
        /// </summary>
        /// <param name="inputArray">Input Array in which noise is added</param>
        /// <param name="percent">Percentage of Noise which is to be added</param>
        /// <returns></returns>
        public int[] MakeSomeNoise(int[] inputArray, double percent)
        {
            int[] noisyArray = inputArray;
            Random rn = new Random();
            List<int> randomList = new List<int>();
            int index;
            while(!(randomList.Count == (int)(percent* inputArray.Length)))
            {
                index = rn.Next(0, inputArray.Length);
                if (!randomList.Contains(index))
                    randomList.Add(index);
            }
            foreach (int item in randomList)
            {
                if(noisyArray[item] == 0)
                {
                    noisyArray[item] = 1;
                }
                else if (noisyArray[item] == 1)
                {
                    noisyArray[item] = 0;
                }
            }            
            return noisyArray;
        }

        /// <summary>
        /// Returns Hamming Distance between two Arrays
        /// </summary>
        /// <param name="referenceArray">Array from which we have to compare</param>
        /// <param name="givenArray">Array which is given</param>
        /// <returns></returns>
        public int GetHammingDistance(int[] referenceArray, int[] givenArray)
        {
            int unmatchedIndex = 0;

            for (int i = 0; i < referenceArray.Length; i++)
            {
                if (referenceArray[i] != givenArray[i])
                {
                    unmatchedIndex++;
                }
            }

            return unmatchedIndex;
        }

        /// <summary>
        /// Returns Hamming Distance between two Arrays
        /// </summary>
        /// <param name="originArray">Array from which we have to compare</param>
        /// <param name="comparingArray">Array which is given</param>
        /// <returns></returns>
        public static double[] GetHammingDistances(double[][] originArray, double[][] comparingArray, bool countNoneZerosOnly = false)
        {
            double[][] hDistance = new double[originArray.Length][];
            double[] h = new double[originArray.Length];
            double[] hammingDistance = new double[originArray.Length];

            for (int i = 0; i < originArray.Length; i++)
            {
                int len = Math.Max(originArray[i].Length, comparingArray[i].Length);
                int numOfDifferentBits = 0;
                for (int j = 0; j < len; j++)
                {
                    if (originArray[i].Length > j && comparingArray[i].Length > j)
                    {
                        if (originArray[i][j] == comparingArray[i][j])
                        {
                            numOfDifferentBits = numOfDifferentBits + 0;
                        }
                        else
                        {
                            if (countNoneZerosOnly == false)
                                numOfDifferentBits++;
                            else
                            {
                                if (originArray[i][j] == 1)
                                    numOfDifferentBits++;
                            }
                        }
                    }
                    else
                        numOfDifferentBits++;
                }

                h[i] = numOfDifferentBits;
                if (originArray[i].Length > 0 && originArray[i].Count(b => b == 1) > 0)
                {
                    //hammingDistance[i] = ((originArray[i].Length - numOfDifferentBits) * 100 / originArray[i].Length);
                    hammingDistance[i] = ((originArray[i].Count(b => b == 1) - numOfDifferentBits) * 100 / originArray[i].Count(b => b == 1));
                }
                else
                    hammingDistance[i] = double.NegativeInfinity;
            }

            return hammingDistance;
        }

        /// <summary>
        /// Creates file of Binarized Image for Noisy Array
        /// </summary>
        /// <param name="noisyArray">Noisy Array for which binarized file is to be created</param>
        /// <param name="imageHeight">Height of Binarized Image</param>
        /// <param name="imageWidth">Width of Binarized Image</param>
        /// <param name="imageName">Name of Noisy Array</param>
        public void MakeBinaryFile(int[] noisyArray, int imageHeight, int imageWidth, String imageName)
        {
            using (StreamWriter writer = File.CreateText(Path.Combine(AppContext.BaseDirectory, $"Output/bin-{imageName}.txt")))
            {
                StringBuilder imgFile = new StringBuilder();
                int hg = imageHeight;
                int wg = imageWidth;
                for (int i = 0; i < hg; i++)
                {
                    for (int j = 0; j < wg; j++)
                    {
                        imgFile.Append(noisyArray[i*wg+j]);
                    }
                    imgFile.AppendLine();
                }
                string text = imgFile.ToString();
                writer.Write(text);
            }
        }

    }
}

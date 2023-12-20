using Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace Problem
{

    public class Problem : ProblemBase, IProblem
    {
        #region ProblemBase Methods
        public override string ProblemName { get { return "MatrixMultiplication"; } }

        public override void TryMyCode()
        {
            int N = 0 ;
            int[,] output;
            
            N = 2;
            int[,] M1 = { { 1, 1 }, { 1, 0 } };
            int[,] M2 = { { 1, 1 }, { 1, 0 } };

            int[,] expected = { { 2, 1 }, { 1, 1 } };
            output = MatrixMultiplication.MatrixMultiply(M1, M2, N);
            PrintCase(N, M1, M2, output, expected);

            N = 4;
            int[,] K1 = { { 1, -1, 1, -1 }, { 1, -1, 1, -1 }, { 1, -1, 1, -1 }, { 1, -1, 1, -1 } };
            int[,] K2 = { { -1, 1, -1, 1 }, { -1, 1, -1, 1 }, { -1, 1, -1, 1 }, { -1, 1, -1, 1 } };

            int[,] expected2 = { { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 }};
            output = MatrixMultiplication.MatrixMultiply(K1, K2, N);
            PrintCase(N, K1, K2, output, expected2);

            N = 4;
            int[,] J1 = { { 1, -1, 1, -1 }, { 1, -1, 1, -1 }, { 1, -1, 1, -1 }, { 1, -1, 1, -1 } };
            int[,] J2 = { { 1, -1, 1, -1 }, { -1, 1, -1, 1 }, { 1, -1, 1, -1 }, { -1, 1, -1, 1 } };

            int[,] expected3 = { { 4, -4, 4, -4 }, { 4, -4, 4, -4 }, { 4, -4, 4, -4 }, { 4, -4, 4, -4 }};
            output = MatrixMultiplication.MatrixMultiply(J1, J2, N);
            PrintCase(N, J1, J2, output, expected3);


            N = 8;
            int[,] L1 = { { 1, -2, 3, -4, 5, -6, 7, -8 }, 
                            { -1, 2, -3, 4, -5, 6, -7, 8 },
                            { 1, -2, 3, -4, 5, -6, 7, -8 }, 
                            { -1, 2, -3, 4, -5, 6, -7, 8 },
                            { 1, -2, 3, -4, 5, -6, 7, -8 }, 
                            { -1, 2, -3, 4, -5, 6, -7, 8 },
                            { 1, -2, 3, -4, 5, -6, 7, -8 }, 
                            { -1, 2, -3, 4, -5, 6, -7, 8 }};
            int[,] L2 = {   { 1, 0, 0, 0, 0, 0, 0, 0 }, 
                              { 0, 1, 0, 0, 0, 0, 0, 0 }, 
                              { 0, 0, 1, 0, 0, 0, 0, 0 }, 
                              { 0, 0, 0, 1, 0, 0, 0, 0 }, 
                              { 0, 0, 0, 0, 1, 0, 0, 0 }, 
                              { 0, 0, 0, 0, 0, 1, 0, 0 }, 
                              { 0, 0, 0, 0, 0, 0, 1, 0 }, 
                              { 0, 0, 0, 0, 0, 0, 0, 1 }};

            int[,] expected4 = {{ 1, -2, 3, -4, 5, -6, 7, -8 }, 
                                { -1, 2, -3, 4, -5, 6, -7, 8 },
                                { 1, -2, 3, -4, 5, -6, 7, -8 }, 
                                { -1, 2, -3, 4, -5, 6, -7, 8 },
                                { 1, -2, 3, -4, 5, -6, 7, -8 }, 
                                { -1, 2, -3, 4, -5, 6, -7, 8 },
                                { 1, -2, 3, -4, 5, -6, 7, -8 }, 
                                { -1, 2, -3, 4, -5, 6, -7, 8 }};
            output = MatrixMultiplication.MatrixMultiply(L1, L2, N);
            PrintCase(N, L1, L2, output, expected4);
        }

 
        Thread tstCaseThr;
        bool caseTimedOut ;
        bool caseException;

        protected override void RunOnSpecificFile(string fileName, HardniessLevel level, int timeOutInMillisec)
        {
            int testCases;
            int N = 0;
            int[,] output = null;
            int[,] actualResult = null;
            int x=0, y=0;

            Stream s = new FileStream(fileName, FileMode.Open);
            BinaryReader br = new BinaryReader(s);

            testCases = br.ReadInt32();

            int totalCases = testCases;
            int correctCases = 0;
            int wrongCases = 0;
            int timeLimitCases = 0;

            int i = 1;
            while (testCases-- > 0)
            {
                N = br.ReadInt32();
                int[,] M1 = new int[N, N];
                int[,] M2 = new int[N, N];
                actualResult = new int[N, N];

                for (y = 0; y < N; y++)
                {
                    for (x = 0; x < N; x++)
                    {
                        M1[y, x] = br.ReadSByte();
                    }
                }
                for (y = 0; y < N; y++)
                {
                    for (x = 0; x < N; x++)
                    {
                        M2[y, x] = br.ReadSByte();
                    }
                }
                for ( y = 0; y < N; y++)
                {
                    for ( x = 0; x < N; x++)
                    {
                        actualResult[y, x] = br.ReadInt32();
                    }
                }
                caseTimedOut = true;
                caseException = false;
                {
                    tstCaseThr = new Thread(() =>
                    {
                        try
                        {
                            Stopwatch sw = Stopwatch.StartNew();
                            output = MatrixMultiplication.MatrixMultiply(M1, M2, N);
                            sw.Stop();
                            Console.WriteLine("N = {0}, time in sec = {1}", N, sw.ElapsedMilliseconds / 1000.0);
                        }
                        catch
                        {
                            caseException = true;
                            output = null;
                        }
                        caseTimedOut = false;
                    });

                    //StartTimer(timeOutInMillisec);
                    tstCaseThr.Start();
                    tstCaseThr.Join(timeOutInMillisec);
                }
                if (caseTimedOut)       //Timedout
                {
                    Console.WriteLine("Time Limit Exceeded in Case {0}.", i);
                    tstCaseThr.Abort();
                    timeLimitCases++;
                }
                else if (caseException) //Exception 
                {
                    Console.WriteLine("Exception in Case {0}.", i);
                    wrongCases++;
                }
                else if (output != null && CheckOutput(output, actualResult, ref y, ref x))    //Passed
                {
                    Console.WriteLine("Test Case {0} Passed!", i);
                    correctCases++;
                }
                else                    //WrongAnswer
                {
                    Console.WriteLine("Wrong Answer in Case {0}.", i);
                    if (level == HardniessLevel.Easy)
                    {
                        PrintCase(N, M1, M2, output, actualResult);
                    }
                    wrongCases++;
                }

                i++;
            }
            s.Close();
            br.Close();
            Console.WriteLine();
            Console.WriteLine("# correct = {0}", correctCases);
            Console.WriteLine("# time limit = {0}", timeLimitCases);
            Console.WriteLine("# wrong = {0}", wrongCases);
            Console.WriteLine("\nFINAL EVALUATION (%) = {0}", Math.Round((float)correctCases / totalCases * 100, 0)); 
        }

       

        protected override void OnTimeOut(DateTime signalTime)
        {
        }

        public override void GenerateTestCases(HardniessLevel level, int numOfCases)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Helper Methods
        private static void PrintMatrix(int[,] matrix)
        {

            int N = matrix.GetLength(0);
            int M = matrix.GetLength(1);

            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < M; j++)
                {
                    Console.Write(matrix[i, j] + "  ");
                }
                Console.WriteLine();
            }
        }

        private void PrintCase(int N, int[,] M1, int[,] M2, int[,] output, int[,] expected)
        {
            int x=0, y=0;
            Console.WriteLine("N = {0}", N);
            Console.WriteLine("Matrix#1 = "); PrintMatrix(M1);
            Console.WriteLine("Matrix#2 = "); PrintMatrix(M2);
            Console.WriteLine("output = "); if (output != null) PrintMatrix(output); else Console.WriteLine("NULL");
            Console.WriteLine("expected = "); PrintMatrix(expected);
            if (output != null)
            {
                if (CheckOutput(output, expected, ref y, ref x))
                {
                    Console.WriteLine("CORRECT");
                }
                else
                {
                    Console.WriteLine("WRONG: output[{0},{1}] = {2} , expected[{0},{1}] = {3}", y, x, output[y, x], expected[y, x]);
                }
            }
            else
            {
                Console.WriteLine("output is NULL!");
            }
            Console.WriteLine();
        }

        private bool CheckOutput(int[,] output, int[,] actualResult, ref int y, ref int x)
        {
            int N = output.GetLength(0);
            for (y = 0; y < N; y++)
            {
                for (x = 0; x < N; x++)
                {
                    if (output[y, x] != actualResult[y, x]) return false;
                }
            }
            return true;
        }
        #endregion
   
    }
}

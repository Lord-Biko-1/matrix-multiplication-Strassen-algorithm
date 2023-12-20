using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Problem
{
    // *****************************************
    // DON'T CHANGE CLASS OR FUNCTION NAME
    // YOU CAN ADD FUNCTIONS IF YOU NEED TO
    // *****************************************
    public static class MatrixMultiplication
    {
        #region YOUR CODE IS HERE
        
        //Your Code is Here:
        //==================
        /// <summary>
        /// Multiply 2 square matrices in an efficient way [Strassen's Method]
        /// </summary>
        /// <param name="M1">First square matrix</param>
        /// <param name="M2">Second square matrix</param>
        /// <param name="N">Dimension (power of 2)</param>
        /// <returns>Resulting square matrix</returns>
        static public int[,] MatrixMultiply(int[,] M1, int[,] M2, int N)
        {
            //REMOVE THIS LINE BEFORE START CODING
            int[,] finalMatrix = new int[N, N];
            int nos = N/2;


            //base case 
            //N<=40 =>10.19
            //N<=32 =>9.752
            //N<=35 => 9.632
            //N<=38 =>9.746
            //N<=64 =>7.65 best 
            if (N<=64)
            {
                finalMatrix = NaiveMultiplecation(M1, M2, N);
                return finalMatrix;
            }
            //matrix one
            int[,] mat1_a_00 = new int[nos, nos];
            int[,] mat1_b_01 = new int[nos, nos];
            int[,] mat1_c_10 = new int[nos, nos];
            int[,] mat1_d_11 = new int[nos, nos];
            //mmatrix2
            int[,] mat2_e_00 = new int[nos, nos];
            int[,] mat2_f_01 = new int[nos, nos];
            int[,] mat2_g_10 = new int[nos, nos];
            int[,] mat2_h_11 = new int[nos, nos];


            //prepare filling matrix intialized with params to use in strassen equation 
            //mat1=>M1
            //mat2=>M2
            //O(N^2/4)=>(N^2)

            //firstmatrix
            Parallel.For(0, nos, new ParallelOptions { MaxDegreeOfParallelism = 4 },rows =>
            {for (int col = 0; col < nos; col++)
                {
                    //first mat
                    mat1_a_00[rows, col] = M1[rows, col];
                    mat1_b_01[rows, col] = M1[rows, col + nos];
                    mat1_c_10[rows, col] = M1[rows + nos, col];
                    mat1_d_11[rows, col] = M1[rows + nos, col + nos];

                }
            });
            //secound matrix
            Parallel.For(0, nos, new ParallelOptions { MaxDegreeOfParallelism= 4 },rows =>{

                for (int col = 0; col < nos; col++)
                {
                    //second mat
                    mat2_e_00[rows, col] = M2[rows, col];
                    mat2_f_01[rows, col] = M2[rows, col + nos];
                    mat2_g_10[rows, col] = M2[rows + nos, col];
                    mat2_h_11[rows, col] = M2[rows + nos, col + nos];
                }
            });


            //getting 7 parts 

            Task<int[,]> part1 = Task.Factory.StartNew(() => MatrixMultiply(mat1_a_00, DiffOrAdd(mat2_f_01, mat2_h_11, nos, false), nos));
            
            Task<int[,]> part2 = Task.Factory.StartNew(() => MatrixMultiply(DiffOrAdd(mat1_a_00, mat1_b_01, nos, true), mat2_h_11, nos));
           
            Task<int[,]> part3 = Task.Factory.StartNew(() => MatrixMultiply(DiffOrAdd(mat1_c_10, mat1_d_11, nos, true), mat2_e_00, nos));
            
            Task<int[,]> part4 = Task.Factory.StartNew(() => MatrixMultiply(mat1_d_11, DiffOrAdd(mat2_g_10, mat2_e_00, nos, false), nos));
           
            Task<int[,]> part5 = Task.Factory.StartNew(() => MatrixMultiply(DiffOrAdd(mat1_a_00, mat1_d_11, N / 2, true), DiffOrAdd(mat2_e_00, mat2_h_11, nos, true), nos));
            
            Task<int[,]> part6 = Task.Factory.StartNew(() => MatrixMultiply(DiffOrAdd(mat1_b_01, mat1_d_11, nos, false), DiffOrAdd(mat2_g_10, mat2_h_11, nos, true), nos));
            
            Task<int[,]> part7 = Task.Factory.StartNew(() => MatrixMultiply(DiffOrAdd(mat1_a_00, mat1_c_10, nos, false), DiffOrAdd(mat2_e_00, mat2_f_01, nos, true), nos));
            Task.WaitAll(part1, part2, part3, part4, part5, part6, part7);
            /*
            int[,] X_00 = DiffOrAdd(DiffOrAdd(part5.Result, part6.Result, nos,true), DiffOrAdd(part4.Result, part2.Result, nos,false), nos,true);
            int[,] X_01 = DiffOrAdd(part1.Result, part2.Result,nos,true);
            int[,] X_10 = DiffOrAdd(part3.Result, part4.Result,nos,true);
            int[,] X_11 = DiffOrAdd(DiffOrAdd(part5.Result, part7.Result, nos,false), DiffOrAdd(part1.Result, part3.Result, nos,false), nos,true);
            

             for (int i=0;i<nos;i++)
             {
                 for(int j=0;j<nos;j++)
                 {

                     finalMatrix[i, j] = X_00[i,j];
                     finalMatrix[i, j + nos] = X_01[i,j];
                     finalMatrix[i + nos, j] = X_10[i, j];
                     finalMatrix[i + nos, j + nos] = X_11[i, j];
                 }
             }*/

            Parallel.For(0, nos,i => {

                for (int j = 0; j < nos; j++)
                {
                    finalMatrix[i, j] = part5.Result[i, j] + part4.Result[i, j] - part2.Result[i, j] + part6.Result[i, j];
                    finalMatrix[i, j + nos] = part1.Result[i, j] + part2.Result[i, j];
                    finalMatrix[i +nos, j] =part3.Result[i, j] + part4.Result[i, j];
                    finalMatrix[i + nos, j + nos] = part5.Result[i, j] + part1.Result[i, j] - part3.Result[i, j] - part7.Result[i, j];
                }
            });

            return finalMatrix;

        }
        static public int[,] DiffOrAdd(int[,] mat1, int[,] mat2,int N,bool add)
        {
            int[,] result = new int[N, N];
            if(N==1)
            {
                if(add)
                {
                    result[0, 0] = mat1[0, 0] + mat2[0, 0];
                    return result;
                }
                else
                {
                    result[0, 0] = mat1[0, 0] - mat2[0, 0];
                    return result;
                }
            }
            //Time=>8.925
             for(int i=0;i<N;i++)
              {
                  for(int j=0;j<N;j++)
                  {
                      if(add)
                      {
                          result[i, j] = mat1[i, j] + mat2[i, j];
                      }
                      else
                      {
                          result[i, j] = mat1[i, j] - mat2[i, j];
                      }
                  }
              }
            //time=>9.893
            /*Parallel.For(0, N / 2, i => {
                for (int j = 0; j < N; j++)
                {
                    if (add)
                    {
                        result[i, j] = mat1[i, j] + mat2[i, j];
                    }
                    else
                    {
                        result[i, j] = mat1[i, j] - mat2[i, j];
                    }
                }
            });*/


            return result;

        }
        static public int[,] NaiveMultiplecation(int[,]mat1,int[,]mat2,int N)   
        {
            int[,] Result = new int[N, N];

            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    for (int k = 0; k < N; k++)
                    {
                        Result[i, j] += mat1[i, k] * mat2[k, j];
                    }
                }
            }
            return Result;

        }

        #endregion
    }
}

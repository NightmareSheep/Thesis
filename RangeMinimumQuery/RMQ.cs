using System;

namespace RangeMinimumQuery
{
    public class RMQ
    {

        // lookup[i][j] is going to store minimum 
        // value in arr[i..j]. Ideally lookup table 
        // size should not be fixed and should be 
        // determined using n Log n. It is kept 
        // constant to keep code simple. 
        int[,] lookup;
        int[,] indices;

        // Fills lookup array lookup[][] in bottom up manner. 

        public RMQ(int[] arr)
        {
            var n = arr.Length;
            var p = (int)Math.Floor(Math.Log(n,2)) + 1;
            lookup = new int[n, p];
            indices = new int[n, p];

            // Initialize M for the intervals with length 1 
            for (int i = 0; i < n; i++)
            {
                lookup[i, 0] = arr[i];
                indices[i, 0] = i;
            }

            // Compute values from smaller to bigger intervals 
            for (int j = 1; (1 << j) <= n; j++)
            {

                // Compute minimum value for all intervals with 
                // size 2^j 
                for (int i = 0; (i + (1 << j) - 1) < n; i++)
                {

                    if (i >= n || j >= p)
                        continue;

                    // For arr[2][10], we compare arr[lookup[0][7]]  
                    // and arr[lookup[3][10]] 
                    if (lookup[i, j - 1] < lookup[i + (1 << (j - 1)), j - 1])
                    {
                        lookup[i, j] = lookup[i, j - 1];
                        indices[i, j] = indices[i, j - 1];
                    }
                    else
                    {
                        lookup[i, j] = lookup[i + (1 << (j - 1)), j - 1];
                        indices[i, j] = indices[i + (1 << (j - 1)), j - 1];
                    }
                }
            }
        }

        // Returns minimum of arr[L..R] 
        public void Query(int L, int R, out int index, out int value)
        {
            // Find highest power of 2 that is smaller 
            // than or equal to count of elements in given 
            // range. For [2, 10], j = 3 
            int j = (int)Math.Log(R - L + 1,2);

            // Compute minimum of last 2^j elements with first 
            // 2^j elements in range. 
            // For [2, 10], we compare arr[lookup[0][3]] and 
            // arr[lookup[3][3]], 
            if (lookup[L, j] <= lookup[R - (1 << j) + 1, j])
            {
                value = lookup[L, j];
                index = indices[L, j];
            }
            else
            {
                value = lookup[R - (1 << j) + 1, j];
                index = indices[R - (1 << j) + 1, j];
            }
        }
    }
}
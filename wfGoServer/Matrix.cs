using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wfGoServer
{
    class Matrix
    {
        public int[,] matrix;

        public int this[int i,int j]
        {
            get { return matrix[i, j]; }
            set { matrix[i, j] = value; }
        }

        public Matrix()
        {
            int n = ConstNumber.linenum + 1;
            matrix = new int[n,n];//用1，1到19，19

            for(int i=0;i<n;i++)
            {
                for(int j=0;j<n;j++)
                {
                    matrix[i, j] = 0;
                }
            }
        }

        public Matrix Copy()
        {
            Matrix m = new Matrix();
            int n = ConstNumber.linenum;
            for(int i=1;i<=n;i++)
            {
                for(int j=1;j<=n;j++)
                {
                    m[i, j] = this[i, j];
                }
            }
            return m;
        }
    }
}

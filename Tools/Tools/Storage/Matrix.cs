using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Math
{
    interface Matrix<T>
    {
        //public Matrix<T> add();
    }
    class SparseMatrix<T> : Matrix<T>
    {

    }
    class DenseMatrix<T> : Matrix<T>
    {
        T[][] data;
    }
}

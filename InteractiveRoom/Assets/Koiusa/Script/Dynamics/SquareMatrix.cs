using UnityEngine;

/// <summary>
/// N�������s��
/// </summary>
[System.Serializable]
public class SquareMatrix
{
    [SerializeField] private double[] m;
    [SerializeField] private int n;
    [SerializeField] private double det;

    public int N => n;
    public double Det => det; // �s��

    public double this[int i]
    {
        get => m[i];
        set => m[i] = value;
    }

    public double this[int i, int j]
    {
        get => m[i * n + j];
        set => m[i * n + j] = value;
    }

    public SquareMatrix(int n)
    {
        this.n = n;
        m = new double[n * n];
    }

    public SquareMatrix(int n, double[,] values)
    {
        this.n = n;
        m = new double[n * n];
        for (int j = 0; j < n; j++)
        {
            for (int i = 0; i < n; i++)
            {
                this[i, j] = values[i, j];
            }
        }
    }

    public SquareMatrix(int n, double[,] values, double det)
    {
        this.n = n;
        this.det = det;
        m = new double[n * n];
        for (int j = 0; j < n; j++)
        {
            for (int i = 0; i < n; i++)
            {
                this[i, j] = values[i, j];
            }
        }
    }

    public SquareMatrix(Vector3 rhs)
    {
        n = 3;
        this.m = new double[n];
        for (int i = 0; i < n; i++)
        {
            this[i] = rhs[i];
        }
    }

    /// <summary>
    /// �]�u�s��
    /// </summary>
    public SquareMatrix Transpose()
    {
        var matrix = new SquareMatrix(this.n);
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                matrix[j, i] = this[i, j];
            }
        }

        return matrix;
    }

    /// <summary>
    /// �t�s��
    /// </summary>
    public SquareMatrix Inverse()
    {
        var m = new double[n, n];
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                m[i, j] = this[i, j];
            }
        }

        GaussianElimination(n, m, out var det, out double[,] m2, out var I);
        return new SquareMatrix(n, I, det);
    }


    /// <summary>
    /// �|���o���@(�K�E�X�̏����@)
    /// </summary>
    /// <param name="n">�s���E��</param>
    /// <param name="m0">���̍s��</param>
    /// <param name="m">�|���o���@�̌���(�K�i�`)</param>
    /// <param name="I">�s��m0�̋t�s��</param>
    public static void GaussianElimination(
        int n,
        double[,] m0,
        out double det,
        out double[,] m,
        out double[,] I)
    {
        m = (double[,])m0.Clone();

        I = SquareMatrix.Identity(n);

        double diag = 1; // �Ίp�����̏�Z

        bool[,] seen = new bool[n, n];
        for (int targetX = 0; targetX < n; targetX++)
        {
            int srcY = -1; // 0�ł͂Ȃ��v�f
            bool findNonZero = false;  // 0�ł͂Ȃ��v�f��������
            for (int y = 0; y < n; y++)
            {
                if (seen[targetX, y]) continue;

                // 0 �ł͂Ȃ��v�f��T��
                if (m[targetX, y] == 0f) continue;
                srcY = y;

                for (int xi = targetX; xi < n; xi++)
                {
                    seen[xi, srcY] = true;
                }

                findNonZero = true;
                diag *= m[targetX, y];
                break;
            }

            if (!findNonZero)
            {
                diag *= 0.0;
            }
            else
            {
                // ���Ă���s�̍��[��1�ɂȂ�悤�ɁA�s������
                DivideRow(n, I, srcY, m[targetX, srcY]);
                DivideRow(n, m, srcY, m[targetX, srcY]);

                for (int yi = 0; yi < n; yi++)
                {
                    if (yi == srcY) continue;

                    // ���Ă���s��k�{���āA���̍s�ɑ���
                    double k = -m[targetX, yi];
                    AddRow(n, I, srcY, yi, k);
                    AddRow(n, m, srcY, yi, k);
                }
            }

        }

        det = diag;
    }

    public static Vector3 operator *(SquareMatrix lhs, Vector3 rhs)
    {
        Vector3 v = new Vector3();
        for (int i = 0; i < lhs.n; i++)
        {
            float sum = 0f;
            for (int j = 0; j < lhs.n; j++)
            {
                sum += (float)lhs[j, i] * rhs[j];
            }
            v[i] = sum;
        }
        return v;
    }

    /// <summary>
    /// �P�ʍs��
    /// </summary>
    /// <param name="n"></param>
    private static double[,] Identity(int n)
    {
        var m = new double[n, n];
        for (int i = 0; i < n; i++)
        {
            m[i, i] = 1;
        }
        return m;
    }


    /// <summary>
    /// src�s�ڂ�k�{����dst�s�ڂɑ���
    /// </summary>
    static void AddRow(int n, double[,] m, int src, int dst, double k)
    {
        for (int x = 0; x < n; x++)
        {
            m[x, dst] += k * m[x, src];
        }
    }

    /// <summary>
    /// src�s�ڂ�k�Ŋ���
    /// </summary>
    static void DivideRow(int n, double[,] m, int y, double k)
    {
        for (int x = 0; x < n; x++)
        {
            m[x, y] /= k;
        }
    }


    public static SquareMatrix operator *(SquareMatrix lhs, float rhs)
    {
        var matrix = new SquareMatrix(lhs.n);
        int n = lhs.n;

        // matrix��(i, j)�v�f�̌v�Z
        for (int j = 0; j < n; j++)
        {
            for (int i = 0; i < n; i++)
            {
                matrix[i, j] = lhs[i, j] * rhs;
            }
        }
        return matrix;
    }

    public static SquareMatrix operator /(SquareMatrix lhs, float rhs)
    {
        var matrix = new SquareMatrix(lhs.n);
        int n = lhs.n;

        // matrix��(i, j)�v�f�̌v�Z
        for (int j = 0; j < n; j++)
        {
            for (int i = 0; i < n; i++)
            {
                matrix[i, j] = lhs[i, j] / rhs;
            }
        }
        return matrix;
    }

    public static SquareMatrix operator *(SquareMatrix lhs, double rhs)
    {
        var matrix = new SquareMatrix(lhs.n);
        int n = lhs.n;

        for (int j = 0; j < n; j++)
        {
            for (int i = 0; i < n; i++)
            {
                matrix[i, j] = lhs[i, j] * rhs;
            }
        }
        return matrix;
    }

    public static SquareMatrix operator +(SquareMatrix lhs, SquareMatrix rhs)
    {
        if (lhs.n != rhs.n)
            throw new System.Exception("operator+ : invalid matrix size");

        var matrix = new SquareMatrix(lhs.n);
        int n = lhs.n;

        for (int j = 0; j < n; j++)
        {
            for (int i = 0; i < n; i++)
            {
                matrix[i, j] = lhs[i, j] + rhs[i, j];
            }
        }
        return matrix;
    }

    public static SquareMatrix operator *(SquareMatrix lhs, SquareMatrix rhs)
    {
        if (lhs.n != rhs.n)
            throw new System.Exception("operator* : invalid matrix size");

        var matrix = new SquareMatrix(lhs.n);
        int n = lhs.n;

        for (int j = 0; j < n; j++)
        {
            for (int i = 0; i < n; i++)
            {
                double sum = 0f;
                for (int k = 0; k < n; k++)
                {
                    sum += lhs[k, j] * rhs[i, k];
                }
                matrix[i, j] = sum;
            }
        }
        return matrix;
    }
}
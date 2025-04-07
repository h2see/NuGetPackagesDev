using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MultiDimArray
{
    public unsafe class MultiDimArray<T> : IDisposable, IEnumerable<T> where T : unmanaged
    {
        #region Fields

        public static readonly int NumBytesElement = sizeof(T);
        private T* _data = null;
        private T? _initValue = null;
        private List<int>? _shape = null;
        private List<int>? _strides = null;
        private bool _isRowMajor = true;

        #endregion

        #region Constructors

        public MultiDimArray(params int[] shape)
        {
            if (shape == null)
                throw new ArgumentNullException(nameof(shape), "Shape cannot be null.");

            if (shape.Length == 0)
            {
                _data = null;
                _shape = null;
                _strides = null;
                Length = 0;
            }
            else
            {
                int prod = 1;
                foreach (var dim in shape)
                {
                    if (dim <= 0)
                        throw new ArgumentOutOfRangeException(nameof(shape), "All dimensions must be greater than 0.");
                    checked { prod *= dim; }
                }

                Length = prod;
                _shape = shape.ToList();
                DoMemoryCleanup = true;
                _data = (T*)Marshal.AllocHGlobal((IntPtr)NumBytes);
                Unsafe.InitBlockUnaligned(_data, 0, (uint)NumBytes);
            }
        }

        public MultiDimArray(bool doMemoryCleanup, T* dataPointer, params int[] shape)
        {
            if (shape == null)
                throw new ArgumentNullException(nameof(shape), "Shape cannot be null.");

            if (shape.Length == 0)
            {
                _data = null;
                _shape = null;
                _strides = null;
                Length = 0;
            }
            else
            {
                int prod = 1;
                foreach (var dim in shape)
                {
                    if (dim <= 0)
                        throw new ArgumentOutOfRangeException(nameof(shape), "All dimensions must be greater than 0.");
                    checked { prod *= dim; }
                }
                Length = prod;
                _shape = shape.ToList();
                DoMemoryCleanup = doMemoryCleanup;
                _data = dataPointer;
            }
        }

        #endregion

        #region Finalizer / Disposal

        ~MultiDimArray()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (_data != null && DoMemoryCleanup)
                {
                    Marshal.FreeHGlobal((IntPtr)_data);
                    _data = null;
                    DoMemoryCleanup = false;
                }

                if (disposing)
                {
                    _shape = null;
                    _strides = null;
                    Length = 0;
                }

                IsDisposed = true;
            }
        }


        #endregion

        #region Properties

        public T* Data
        {
            get
            {
                EnforceSafeDataAccess();
                return _data;
            }
        }

        public bool IsUserInit { get; private set; } = false;
        public bool IsDisposed { get; private set; } = false;
        public bool DoMemoryCleanup { get; private set; } = false;
        public int Length { get; private set; } = 0;
        public bool IsEmpty => _data == null || Length == 0;
        public int NumDims => _shape == null ? 0 : _shape.Count;
        public int NumBytes
        {
            get
            {
                checked { return Length * NumBytesElement; }
            }
        }

        public List<int> Shape
        {
            get => _shape == null ? new List<int>() : _shape.ToList();
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value), "Shape cannot be null.");

                int prod = (value.Count > 0) ? 1 : 0;
                foreach (var dim in value)
                {
                    if (dim <= 0)
                        throw new ArgumentOutOfRangeException(nameof(value), "All dimensions must be greater than 0.");
                    checked { prod *= dim; }
                }

                if (prod != Length)
                    throw new ArgumentException("The product of the shape dimensions must equal the number of elements in the data.");

                _strides = null;
                _shape = value;
            }
        }

        public bool IsRowMajor
        {
            get { return _isRowMajor; }
            set
            {
                _strides = null;
                _isRowMajor = value;
            }
        }

        public List<int> Strides
        {
            get
            {
                if (_strides == null)
                {
                    if (NumDims == 0)
                    {
                        _strides = new List<int>();
                    }
                    else
                    {
                        if (IsRowMajor)
                        {
                            _strides = new List<int>(new int[NumDims]);
                            _strides[NumDims - 1] = 1;
                            for (int i = NumDims - 2; i >= 0; i--)
                            {
                                _strides[i] = _strides[i + 1] * _shape![i + 1];
                            }
                        }
                        else
                        {
                            _strides = new List<int>(new int[NumDims]);
                            _strides[0] = 1;
                            for (int i = 1; i < NumDims; i++)
                            {
                                _strides[i] = _strides[i - 1] * _shape![i - 1];
                            }
                        }
                    }
                }
                return _strides.ToList();
            }
        }

        public T? InitValue
        {
            get => _initValue;
            set
            {
                EnforceSafeDataAccess();
                if (IsUserInit) { throw new InvalidOperationException("The array has already been initialized."); }
                _initValue = value;
                if (_initValue.HasValue)
                {
                    for (int i = 0; i < Length; i++)
                    {
                        _data[i] = _initValue.Value;
                    }
                    IsUserInit = true;
                }
            }
        }

        #endregion

        #region Indexer

        public T this[params int[] indices]
        {
            get
            {
                EnforceSafeDataAccess();
                int offset = FlattenIndices(indices);
                return _data[offset];
            }
            set
            {
                EnforceSafeDataAccess();
                int offset = FlattenIndices(indices);
                _data[offset] = value;
            }
        }

        #endregion

        #region Public Methods

        public int GetLength(int axis)
        {
            if (axis < 0 || axis >= NumDims)
                throw new ArgumentOutOfRangeException(nameof(axis), "Axis is out of range.");
            return _shape![axis];
        }

        #endregion

        #region IEnumerable<T> Implementation

        public IEnumerator<T> GetEnumerator()
        {
            EnforceSafeDataAccess();
            for (int i = 0; i < Length; i++)
            {
                T v;
                unsafe { v = _data[i]; }
                yield return v;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        #region Private Methods

        private int FlattenIndices(int[] indices)
        {
            if (indices.Length != NumDims)
                throw new ArgumentException("Number of indices must match the number of dimensions.");

            int offset = 0;
            var strides = Strides;
            for (int i = 0; i < NumDims; i++)
            {
                int idx = indices[i];
                if (idx < 0 || idx >= _shape![i])
                    throw new ArgumentOutOfRangeException(nameof(indices),
                        $"Index {i} (value = {idx}) is out of bounds for dimension size {_shape![i]}.");

                offset += idx * strides[i];
            }
            return offset;
        }

        private void EnforceSafeDataAccess()
        {
            ObjectDisposedException.ThrowIf(IsDisposed, this);
            if (_data == null) throw new NullReferenceException("The Data pointer is null.");
        }

        #endregion

        #region Operators
        public static MultiDimArray<T> operator +(
        MultiDimArray<T> lhs,
        MultiDimArray<T> rhs)
        {
            if (lhs is null)
                throw new ArgumentNullException(nameof(lhs));
            if (rhs is null)
                throw new ArgumentNullException(nameof(rhs));

            if (lhs.NumDims != rhs.NumDims)
                throw new ArgumentException("Arrays must have the same number of dimensions.");
            for (int i = 0; i < lhs.NumDims; i++)
            {
                if (lhs.GetLength(i) != rhs.GetLength(i))
                    throw new ArgumentException("Arrays must have the same shape in each dimension.");
            }

            if (typeof(T) != typeof(double))
                throw new NotImplementedException("Addition is not implemented for a MultiDimArray with this element type.");

            MultiDimArray<T> result = new MultiDimArray<T>(lhs.Shape.ToArray());

            unsafe
            {
                NativeMethods.Add((double*)lhs.Data, (double*)rhs.Data, (double*)result.Data, (UIntPtr)lhs.Length);
            }

            return result;
        }
        #endregion
    }


    internal static class NativeMethods
    {
        [DllImport("MultiDimArrayNative.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void Add(
            double* a,
            double* b,
            double* result,
            UIntPtr n);
    }


}

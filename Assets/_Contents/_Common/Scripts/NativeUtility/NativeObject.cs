using System;

namespace Unity.Collections.LowLevel.Unsafe
{
    public unsafe struct NativeObject<T> : IDisposable
        where T : unmanaged
    {
        [NativeDisableUnsafePtrRestriction] readonly T* _buffer;
        readonly Allocator _allocatorLabel;
        
#if ENABLE_UNITY_COLLECTIONS_CHECKS
        [NativeSetClassTypeToNullOnSchedule] DisposeSentinel _disposeSentinel;
        AtomicSafetyHandle _safety;
#endif

        public T Value
        {
            get
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckReadAndThrow(_safety);
#endif
                return *_buffer;
            }
            set
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckWriteAndThrow(_safety);
#endif
                *_buffer = value;
            }
        }

        public bool IsCreated => _buffer != null;

        public T* GetUnsafePtr
        {
            get
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckWriteAndThrow(_safety);
#endif
                return _buffer;
            }
        }

        public T* GetUnsafeReadOnlyPtr
        {
            get
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckReadAndThrow(_safety);
#endif
                return _buffer;
            }
        }

        public T* GetUnsafeBufferPointerWithoutChecks => _buffer;
        
        
        
        public NativeObject(Allocator allocator, T value = default)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (allocator <= Allocator.None)
            {
                throw new ArgumentException("Allocator must be Temp, TempJob or Persistent", nameof(allocator));
            }

            if (!UnsafeUtility.IsBlittable<T>())
            {
                throw new ArgumentException(string.Format("{0} used in NativeObject<{0}> must be blittable", typeof(T)));
            }
#endif

            var size = UnsafeUtility.SizeOf<T>();
            this._buffer = (T*) UnsafeUtility.Malloc(size, UnsafeUtility.AlignOf<T>(), allocator);
            this._allocatorLabel = allocator;
            *this._buffer = value;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            DisposeSentinel.Create(out _safety, out _disposeSentinel, 0, allocator);
#endif
        }

        public void Dispose()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (!UnsafeUtility.IsValidAllocator(_allocatorLabel))
            {
                throw new InvalidOperationException("Can not be Disposed because it was not allocated with a valid allocator.");
            }
            DisposeSentinel.Dispose(ref _safety, ref _disposeSentinel);
#endif
            
            UnsafeUtility.Free(_buffer, _allocatorLabel);
        }
    }
}

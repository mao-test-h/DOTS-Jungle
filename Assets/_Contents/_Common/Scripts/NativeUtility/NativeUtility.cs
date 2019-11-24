namespace Unity.Collections.LowLevel.Unsafe
{
    public static unsafe class NativeUtility
    {
        public static NativeArray<T> PtrToNativeArray<T>(T* ptr, int length)
            where T : unmanaged
        {
            var arr = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>(ptr, length, Allocator.Invalid);

            // これをやらないとNativeArrayのインデクサアクセス時に死ぬ
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref arr, AtomicSafetyHandle.Create());
#endif

            return arr;
        }
    }
}


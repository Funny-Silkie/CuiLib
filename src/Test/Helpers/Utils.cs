using System;

namespace Test.Helpers
{
    /// <summary>
    /// 共通処理を記述します。
    /// </summary>
    internal static class Utils
    {
        /// <summary>
        /// オブジェクトを破棄して自身を返します。
        /// </summary>
        /// <typeparam name="T">破棄可能なオブジェクトの型</typeparam>
        /// <param name="obj">破棄するオブジェクト</param>
        /// <returns><paramref name="obj"/></returns>
        public static T DisposeSelf<T>(this T obj) where T : IDisposable
        {
            obj.Dispose();
            return obj;
        }
    }
}

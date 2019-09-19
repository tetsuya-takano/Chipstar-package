using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Chipstar
{
    [Flags]
    public enum FileReadOption : int
    {
        None = 0,
        /// <summary>
        /// 読み込み失敗したら空で作成する
        /// </summary>
        EmptyIfFailure = 1 << 1,
    }
    [Flags]
    public enum FileWriteOption : int
    {
        None = 0,
    }

    public interface IFileBuilder<T>
    {
        T Read( string path );
        void Write(string path, T obj);
    }
    /// <summary>
    /// Parserと Writer 統合
    /// </summary>
    public class FileBuilder<T> : IFileBuilder<T>
        where T : new()
    {
        
        
        //=======================================
        // 変数
        //=======================================
        private FileReadOption ReadOption { get; }
        private FileWriteOption WriteOption { get; }
        private IFileWriter<T> Writer { get; }
        private IFileParser<T> Parser { get; }

        //=======================================
        //関数
        //=======================================
        /// <summary>
        /// 
        /// </summary>
        public FileBuilder(
            IFileWriter<T> writer, IFileParser<T> parser,
            FileReadOption readOption = FileReadOption.None,
            FileWriteOption writeOption = FileWriteOption.None
        )
        {
            Writer = writer;
            Parser = parser;
            ReadOption = readOption;
            WriteOption = writeOption;
        }
        public T Read(string path)
        {
            try
            {
                var bytes = File.ReadAllBytes(path);
                return Parser.Parse(bytes);
            }
            catch (Exception e)
            {
                if (!ReadOption.HasFlag(FileReadOption.EmptyIfFailure))
                {
                    // エラーにするなら素通し
                    throw;
                }
                ChipstarLog.Log_CatchException(e);
                return new T();
            }
        }

        public void Write(string path, T obj)
        {
            Writer.Write(path, obj);
        }
    }
}

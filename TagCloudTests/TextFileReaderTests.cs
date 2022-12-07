﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using TagCloud.IReaders;

namespace TagCloudTests
{
    public class SingleWordInRowTextFileReaderTests
    {
        private readonly Random random = new Random();
        private string[] data;

        [SetUp]
        public void PrepareRandomString()
        {
            data = Enumerable.Range(20,40).Select(RandomString).ToArray();
        }

        [Test]
        public void SingleWordInRowTextFileReader_Read_Data()
        {
            var path = "TextFileData.txt";
            CreateTextFileWithData(path);

            var fileReader = new SingleWordInRowTextFileReader(path);

            CheckThatDataIsReadBy(fileReader);
            DeleteFile(path);
        }

        [TestCase("TextFileData.csv", TestName = "File extension not supported")]
        public void SingleWordInRowTextFileReader_Ctor_ThrowFileLoadExceptionWhen(string path)
        {
            Action act = () => new SingleWordInRowTextFileReader(path);

            act.Should().Throw<FileLoadException>();
        }

        [TestCase("notExistingFile.txt", TestName = "file extension not exist")]
        public void SingleWordInRowTextFileReader_Ctor_ThrowFileNotFoundExceptionWhen(string path)
        {
            Action act = () => new SingleWordInRowTextFileReader(path);

            act.Should().Throw<FileNotFoundException>();
        }

        private string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private void CreateTextFileWithData(string path)
        {
            using (var file = new StreamWriter(path))
            {
                foreach (var line in data)
                    file.WriteLine(line);
            }
            File.Exists(path).Should().BeTrue();
        }

        private void CheckThatDataIsReadBy(IReader reader)
        {
            var words = reader.ReadWords(); 
            words.Should().BeEquivalentTo(data);
        }

        private void DeleteFile(string path)
        {
            File.Delete(path);
            File.Exists(path).Should().BeFalse();
        }
    }
}

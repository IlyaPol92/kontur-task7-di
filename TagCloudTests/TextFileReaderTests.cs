﻿using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TagCloud.Readers;

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

            var fileReader = new SingleWordInRowTextFileReader();
            fileReader.Open(path);

            CheckThatDataIsReadBy(fileReader);
            DeleteFile(path);
        }

        [TestCase("notExistingFile.txt", TestName = "file extension not exist")]
        public void SingleWordInRowTextFileReader_Ctor_ThrowFileNotFoundExceptionWhen(string path)
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action act = () => 
            {
                var reader = new SingleWordInRowTextFileReader();
                reader.Open(path);
            };

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
﻿using System;
using System.Globalization;
using Aliencube.CloudConverter.Wrapper;
using Aliencube.CloudConverter.Wrapper.DataFormats;
using Aliencube.CloudConverter.Wrapper.Exceptions;
using Aliencube.CloudConverter.Wrapper.Extensions;
using Aliencube.CloudConverter.Wrapper.Interfaces;
using Aliencube.CloudConverter.Wrapper.Options;
using Aliencube.CloudConverter.Wrapper.Requests;
using Aliencube.CloudConverter.Wrapper.Responses;
using FluentAssertions;
using NUnit.Framework;

namespace Aliencube.CloudConvert.Tests
{
    [TestFixture]
    public class ConverterWrapperTest
    {
        private IConverterSettings _settings;
        private IConverterWrapper<MarkdownConverterOptions> _wrapper;

        [SetUp]
        public void Init()
        {
            this._settings = ConverterSettings.CreateInstance();
            this._wrapper = new ConverterWrapper<MarkdownConverterOptions>(this._settings);
        }

        [TearDown]
        public void Cleanup()
        {
            if (this._wrapper != null)
            {
                this._wrapper.Dispose();
            }

            if (this._settings != null)
            {
                this._settings.Dispose();
            }
        }

        [Test]
        public async void GetProcessId_GivenApiKey_ReturnResult()
        {
            var formats = new Formats();
            try
            {
                var response = await this._wrapper.GetProcessResponseAsync(formats.Document.Md, formats.Document.Docx);
                response.Id.Should().NotBeNullOrEmpty();
            }
            catch (Exception ex)
            {
                var error = ex as ErrorResponseException;
                error.Should().NotBeNull();

                var response = error.Error;
                response.Should().BeOfType<ErrorResponse>();
                response.Code.Should().NotBe(200);
            }
        }

        [Test]
        public void GetConvertRequest_GivenParameters_ReturnConvertRequest()
        {
            var formats = new Formats();
            var input = new InputParameters()
                        {
                            InputFormat = formats.Document.Md,
                            InputMethod = InputMethod.Download,
                            Filepath = "https://raw.githubusercontent.com/aliencube/CloudConvert.NET/dev/README.md",
                            Filename = "README.md",
                        };
            var output = new OutputParameters()
                         {
                             DownloadMethod = DownloadMethod.False,
                             OutputStorage = OutputStorage.OneDrive,
                         };
            var conversion = new ConversionParameters<MarkdownConverterOptions>()
                             {
                                 OutputFormat = formats.Document.Docx,
                                 ConverterOptions = new MarkdownConverterOptions()
                                                    {
                                                        InputMarkdownSyntax = MarkdownSyntaxType.Auto
                                                    },
                             };
            var request = this._wrapper.GetConvertRequest(input, output, conversion);
            request.InputMethod.Should().Be(InputMethod.Download.ToLower());
            request.OutputStorage.Should().Be(OutputStorage.OneDrive.ToLower());
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using Esynctraining.Lti.Lms.Common.Dto;
using Esynctraining.Lti.Lms.Common.Dto.Canvas;
using HtmlAgilityPack;

namespace Esynctraining.Lti.Lms.Canvas
{
    /// <summary>
    /// The canvas quiz parser.
    /// </summary>
    internal sealed class CanvasQuizParser
    {
        public static void Parse(CanvasQuizDTO quiz)
        {
            if (quiz.questions == null)
            {
                return;
            }

            foreach (var question in quiz.questions)
            {
                ExtractFiles(question);
                if (question.question_type != null
                    && question.question_type.Equals(
                        "multiple_choice_question",
                        StringComparison.InvariantCultureIgnoreCase))
                {
                    question.is_single = true;
                }

                if (question.question_text == null)
                {
                    question.question_text = question.question_name;
                }
            }
        }

        /// <summary>
        /// The get file id.
        /// </summary>
        /// <param name="fileUrl">
        /// The file url.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetFileId(string fileUrl)
        {
            var fileIndex = fileUrl.IndexOf("/files/", StringComparison.InvariantCultureIgnoreCase);
            if (fileIndex > -1)
            {
                fileUrl = fileUrl.Substring(fileIndex + 7);
            }

            fileIndex = fileUrl.IndexOf("/", StringComparison.InvariantCultureIgnoreCase);
            if (fileIndex > -1)
            {
                return fileUrl.Substring(0, fileIndex);
            }

            return null;
        }

        /// <summary>
        /// The replace file place holder.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="fileIndex">
        /// The file index.
        /// </param>
        /// <param name="file">
        /// The file.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ReplaceFilePlaceHolder(string text, int fileIndex, LmsQuestionFileDTO file)
        {
            var placeHolder = string.Format("[[file_id_{0}]]", fileIndex);
            var img = string.Format(
                "<img src=\"{0}\" width=\"{1}\" height=\"{2}\" />",
                file.fileUrl,
                file.width,
                file.height);
            return text.Replace(placeHolder, img);
        }

        /// <summary>
        /// The extract files.
        /// </summary>
        /// <param name="question">
        /// The question.
        /// </param>
        private static void ExtractFiles(CanvasQuestionDTO question)
        {
            var index = 0;
            question.question_text = ExtractFilesFromText(question.question_text, ref index, question.files);

            foreach (var answer in question.answers)
            {
                answer.text = ExtractFilesFromText(
                    string.IsNullOrWhiteSpace(answer.html) ? answer.text : answer.html,
                    ref index,
                    question.files);
            }
        }

        /// <summary>
        /// The extract files from text.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="fileIndexStart">
        /// The file index start.
        /// </param>
        /// <param name="fileIds">
        /// The file ids.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string ExtractFilesFromText(string text, ref int fileIndexStart, Dictionary<int, LmsQuestionFileDTO> fileIds)
        {
            var textToXml = WebUtility.HtmlDecode(text);
            textToXml = "<p>" + textToXml + "</p>";

            var sb = new StringBuilder();
            var stringWriter = new StringWriter(sb);
            var doc = new HtmlDocument
            {
                OptionOutputAsXml = true,
                OptionCheckSyntax = true,
                OptionFixNestedTags = true,
                OptionAutoCloseOnEnd = true,
                OptionDefaultStreamEncoding = Encoding.UTF8
            };
            doc.LoadHtml(textToXml);
            doc.Save(stringWriter);
            var xhtmlText = sb.ToString();
            
            var textDoc = new XmlDocument();
            textDoc.LoadXml(xhtmlText);
            var images = textDoc.GetElementsByTagName("img");
            foreach (XmlNode img in images)
            {
                int width = 0, height = 0;

                if (img.Attributes != null)
                {
                    int.TryParse(img.Attributes["width"]?.Value ?? string.Empty, out width);
                    int.TryParse(img.Attributes["height"]?.Value ?? string.Empty, out height);
                    
                    fileIds.Add(
                        fileIndexStart,
                        new LmsQuestionFileDTO()
                            {
                                fileName = img.Attributes["alt"]?.Value ?? string.Empty,
                                fileUrl = img.Attributes["src"]?.Value ?? string.Empty,
                                width = width,
                                height = height
                            });
                    
                    img.InnerXml = string.Format("[[file_id_{0}]]", fileIndexStart);
                    
                    fileIndexStart++;
                }
            }

            return textDoc.InnerText;
        }
    }
}

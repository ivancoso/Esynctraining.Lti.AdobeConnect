namespace EdugameCloud.Core.Domain.Formats.Edugame
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using EdugameCloud.Core.Domain.DTO;

    using Entities;

    /// <summary>
    /// Represents Edugame format converter.
    /// </summary>
    public static class EdugameConverter
    {
        /// <summary>
        /// Converts DTO to question.
        /// </summary>
        /// <param name="value">The question.</param>
        /// <param name="questionType">The question type.</param>
        /// <param name="distractors">The distractors.</param>
        /// <param name="getImageFunc">Function that retrieves image content as base64 string.</param>
        /// <param name="getImageNameFunc">Function that retrieves image name.</param>
        /// <returns>The question.</returns>
        public static EdugameQuestion Convert(QuestionDTO value, File image, QuestionType questionType, IEnumerable<Distractor> distractors, Func<File, string> getImageFunc, Func<File, string> getImageNameFunc)
        {
            if (value == null || questionType == null)
            {
                return null;
            }

            var result = new EdugameQuestion
            {
                Distractors = distractors.Where(x => x.Question.Id == value.questionId).Select(d => Convert(d, getImageFunc, getImageNameFunc)).ToList(),
                Feedback = new EdugameQuestionFeedback
                {
                    Correct = value.correctMessage,
                    Incorrect = value.incorrectMessage,
                    CorrectReference = value.correctReference ?? string.Empty,
                    Hint = value.hint ?? string.Empty
                },
                Image = getImageFunc(image),
                ImageName = getImageNameFunc(image),
                Instruction = value.instruction,
                Order = value.questionOrder,
                Score = value.scoreValue,
                Title = value.question,
                IsMandatory = value.isMandatory,
                PageNumber = value.pageNumber ?? 0,
                Restrictions = value.restrictions ?? string.Empty,
                AllowOther = value.allowOther,
                WeightBucketType = value.weightBucketType ?? 0,
                TotalWeightBucket = value.totalWeightBucket ?? 0,
                Type = Convert(questionType),
            };

            return result;
        }

        /// <summary>
        /// Converts DTO to question type.
        /// </summary>
        /// <param name="value">The question type.</param>
        /// <returns>The question type.</returns>
        public static EdugameQuestionType Convert(QuestionType value)
        {
            if (value == null)
            {
                return null;
            }

            var result = new EdugameQuestionType
            {
                Id = value.Id,
                Description = value.Type,
            };

            return result;
        }

        /// <summary>
        /// Converts DTO to distractor.
        /// </summary>
        /// <param name="value">The distractor.</param>
        /// <param name="getImageFunc">Function that retrieves image content as base64 string.</param>
        /// <param name="getImageNameFunc">Function that retrieves image name.</param>
        /// <returns>The distractor.</returns>
        public static EdugameDistractor Convert(Distractor value, Func<File, string> getImageFunc, Func<File, string> getImageNameFunc)
        {
            if (value == null)
            {
                return null;
            }

            var result = new EdugameDistractor
            {
                Image = getImageFunc(value.Image),
                ImageName = getImageNameFunc(value.Image),
                IsCorrect = value.IsCorrect ?? false,
                Order = value.DistractorOrder,
                Title = value.DistractorName,
                DistractorType = value.DistractorType ?? 1
            };

            return result;
        }

        /// <summary>
        /// Converts question to DTO.
        /// </summary>
        /// <param name="value">The question.</param>
        /// <returns>Question DTO.</returns>
        public static Question Convert(EdugameQuestion value, IEnumerable<QuestionType> questionTypes)
        {
            if (value == null)
            {
                return null;
            }

            var result = new Question
            {
                CorrectMessage = value.Feedback.Correct,
                IncorrectMessage = value.Feedback.Incorrect,
                CorrectReference = value.Feedback.CorrectReference,
                Hint = value.Feedback.Hint,
                ScoreValue = value.Score,
                QuestionOrder = value.Order,
                Instruction = value.Instruction,
                QuestionName = value.Title,
                DateCreated = DateTime.Now,
                DateModified = DateTime.Now,
                QuestionType = questionTypes.FirstOrDefault(x => x.Id == value.Type.Id),
                IsActive = true
            };

            return result;
        }

        /// <summary>
        /// Converts distractor to DTO.
        /// </summary>
        /// <param name="value">The distractor.</param>
        /// <param name="question">Question DTO.</param>
        /// <returns>Distractor DTO.</returns>
        public static Distractor Convert(EdugameDistractor value, Question question)
        {
            if (value == null || question == null)
            {
                return null;
            }

            var result = new Distractor
            {
                IsCorrect = value.IsCorrect,
                DistractorOrder = value.Order,
                DistractorName = value.Title,
                Question = question,
                DateCreated = DateTime.Now,
                DateModified = DateTime.Now,
                DistractorType = value.DistractorType == 0 ? 1 : value.DistractorType,
                IsActive = true,
                CreatedBy = question.CreatedBy,
                ModifiedBy = question.ModifiedBy
            };

            return result;
        }
    }
}

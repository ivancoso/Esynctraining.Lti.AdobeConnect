﻿namespace EdugameCloud.Core.Domain.Entities
{
    /// <summary>
    ///     The user role.
    /// </summary>
    public enum QuestionTypeEnum
    {
        /// <summary>
        /// The single multiple choice text.
        /// </summary>
        SingleMultipleChoiceText = 1, 

        /// <summary>
        /// The true false.
        /// </summary>
        TrueFalse = 2, 

        /// <summary>
        /// The matching.
        /// </summary>
        Matching = 3, 

        /// <summary>
        /// The fill in the blank.
        /// </summary>
        FillInTheBlank = 4, 

        /// <summary>
        /// The speedometer.
        /// </summary>
        Speedometer = 5, 

        /// <summary>
        /// The hotspot.
        /// </summary>
        Hotspot = 6, 

        /// <summary>
        /// The single multiple choice image.
        /// </summary>
        SingleMultipleChoiceImage = 7, 

        /// <summary>
        /// The sequence.
        /// </summary>
        Sequence = 8, 

        /// <summary>
        /// The open answer single line.
        /// </summary>
        OpenAnswerSingleLine = 10, 

        /// <summary>
        /// The open answer multi line.
        /// </summary>
        OpenAnswerMultiLine = 11, 

        /// <summary>
        /// The rate.
        /// </summary>
        Rate = 12, 

        /// <summary>
        /// The rate scale likert .
        /// </summary>
        RateScaleLikert = 13,

        /// <summary>
        /// The weighted bucket ratio.
        /// </summary>
        WeightedBucketRatio = 14,

        /// <summary>
        /// The short answer.
        /// </summary>
        ShortAnswer = 15,

        /// <summary>
        /// The essay.
        /// </summary>
        Essay = 16,

        /// <summary>
        /// The numerical.
        /// </summary>
        Numerical = 17,

        /// <summary>
        /// The calculated.
        /// </summary>
        Calculated = 18,

        /// <summary>
        /// The calculated multichoice.
        /// </summary>
        CalculatedMultichoice = 19,

        /// <summary>
        /// The multiple dropdowns.
        /// </summary>
        MultipleDropdowns = 20,

        /// <summary>
        /// The text no question.
        /// </summary>
        TextNoQuestion = 21,



        /// <summary>
        /// Multiple choice with checkboxes for multiple answers (Sakai)
        /// </summary>
        MultipleAnswer = 23
    }
}
//------------------------------------------------------------------------------
// <copyright file="Csv.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <createdby>DBuerer</createdby><creationdate>9/23/2010<creationdate>
//------------------------------------------------------------------------------

// NOTE: COPIED FROM VENUE MAPS.  ALL UNIT TESTING IS IN THAT PROJECT.

using System;
using System.Collections.Generic;
using System.Text;

namespace Tools
{
    /// <summary>
    /// Encapsulates methods for helping loading a Comma Separated Value file.
    /// </summary>
    public static class Csv
    {
        private enum SplitState
        {
            LookingForField,
            ReadingField,
            InteriorQuote,
        }

        /// <summary>
        /// Encodes a string to a CSV string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Encode(string value)
        {
            if (value == null)
            {
                return "";
            }
            else if (value.Contains(" "))
            {
                return '"' + value.Replace("\"", "\"\"") + '"';
            }
            else
            {
                return value;
            }
        }

        /// <summary>
        /// Splits a line into separate strings from a CSV file.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static List<String> SplitLine(string line)
        {
            int i = 0;
            var state = SplitState.LookingForField;
            StringBuilder field = new StringBuilder();
            bool fieldStartedWithQuote = false;
            var result = new List<string>();
            bool done = false;
            while (!done)
            {
                char c = i < line.Length ? line[i] : (char)0;
                switch (state)
                {
                    case SplitState.LookingForField:
                        switch (c)
                        {
                            case '\t': // Whitespace, ignore
                                goto case ' ';
                            case ' ': // Whitespace, ignore
                                ++i;
                                break;
                            case '\"': // Start of quoted field
                                ++i;
                                state = SplitState.ReadingField;
                                field.Clear();
                                fieldStartedWithQuote = true;
                                break;
                            case ',': // Field was empty and hit next comma.
                                ++i;
                                result.Add(null);
                                field.Clear();
                                break;
                            case (char)0:
                                goto case '\n';
                            case '\r': // Field is empty and hit end-of-line.
                                goto case '\n';
                            case '\n': // Field is empty and hit end-of-line.
                                result.Add(null);
                                field.Clear();
                                done = true;
                                break;
                            default: // Must be character.  Validate that it is an okay one?
                                state = SplitState.ReadingField;
                                field.Clear();
                                fieldStartedWithQuote = false;
                                break;
                        }
                        break;
                    case SplitState.InteriorQuote:
                        switch (c)
                        {
                            case '\"':
                                ++i;
                                field.Append('\"');
                                state = SplitState.ReadingField;
                                break;
                            default:
                                if (fieldStartedWithQuote)
                                {
                                    // End quote.
                                    fieldStartedWithQuote = false;
                                }
                                else
                                {
                                    field.Append('\"');
                                }
                                state = SplitState.ReadingField;
                                break;
                        }
                        break;
                    case SplitState.ReadingField:
                        switch (c)
                        {
                            case '\"': // End of field indicated with a quote.
                                state = SplitState.InteriorQuote;
                                ++i;
                                break;
                            case ',':
                                if (fieldStartedWithQuote)
                                {
                                    ++i;
                                    field.Append(c);
                                }
                                else
                                {
                                    ++i;
                                    result.Add(field.ToString());
                                    field.Clear();
                                    state = SplitState.LookingForField;
                                }
                                break;
                            case (char)0:
                                goto case '\n';
                            case '\r':
                                goto case '\n';
                            case '\n':
                                int j = field.Length - 1;
                                while (j >= 0 && field[j] == ' ' || field[j] == '\t')
                                {
                                    --j;
                                }
                                if (j < 0)
                                {
                                    result.Add(null);
                                    field.Clear();
                                }
                                else if (j < field.Length - 1)
                                {
                                    result.Add(field.Append(field.ToString(), 0, j + 1).ToString());
                                    field.Clear();
                                }
                                else
                                {
                                    result.Add(field.ToString());
                                    field.Clear();
                                }
                                done = true;
                                break;
                            default:
                                ++i;
                                field.Append(c);
                                break;
                        }
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }
            return result;
        }
    }
}

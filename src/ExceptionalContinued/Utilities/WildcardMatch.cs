// <copyright file="WildcardMatch.cs" company="H.A. Sullivan">
// Copyright (c) 2016 All Rights Reserved
// </copyright>
// <author>H.A. Sullivan</author>
// <date>04/11/2016  </date>
// <summary>Wildcard matching string extension method</summary>
// MIT License
// 
// Copyright(c) [2016]
// [H.A. Sullivan]
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//

using System.Collections.Generic;
using System.Linq;

namespace ReSharper.ExceptionalContinued.Utilities
{
    public static class WildcardMatch
    {
        public static bool EqualsWildcard(this string text, string wildcardString, bool ignoreCase)
        {
            if (ignoreCase)
            {
                return text.ToLower().EqualsWildcard(wildcardString.ToLower());
            }

            return text.EqualsWildcard(wildcardString);
        }

        private static bool EqualsWildcard(this string text, string wildcardString)
        {
            var    isLike    = true;
            byte   matchCase = 0;
            char[] reversedFilter;
            char[] reversedWord;
            var    currentPatternStartIndex = 0;
            var    lastCheckedHeadIndex     = 0;
            var    lastCheckedTailIndex     = 0;
            var    reversedWordIndex        = 0;
            var    reversedPatterns         = new List<char[]>();

            if (text == null || wildcardString == null)
            {
                return false;
            }

            var word   = text.ToCharArray();
            var filter = wildcardString.ToCharArray();

            //Set which case will be used (0 = no wildcards, 1 = only ?, 2 = only *, 3 = both ? and *
            if (filter.Any(c => c == '?'))
            {
                matchCase += 1;
            }

            if (filter.Any(t => t == '*'))
            {
                matchCase += 2;
            }

            if ((matchCase == 0 || matchCase == 1) && word.Length != filter.Length)
            {
                return false;
            }

            switch (matchCase)
            {
                case 0:
                    isLike = text == wildcardString;
                    break;

                case 1:
                    for (var i = 0; i < text.Length; i++)
                    {
                        if (word[i] != filter[i] && filter[i] != '?')
                        {
                            isLike = false;
                        }
                    }

                    break;

                case 2:
                    //Search for matches until first *
                    for (var i = 0; i < filter.Length; i++)
                    {
                        if (filter[i] != '*')
                        {
                            if (filter[i] != word[i])
                            {
                                return false;
                            }
                        }
                        else
                        {
                            lastCheckedHeadIndex = i;
                            break;
                        }
                    }

                    //Search Tail for matches until first *
                    for (var i = 0; i < filter.Length; i++)
                    {
                        if (filter[filter.Length - 1 - i] != '*')
                        {
                            if (filter[filter.Length - 1 - i] != word[word.Length - 1 - i])
                            {
                                return false;
                            }
                        }
                        else
                        {
                            lastCheckedTailIndex = i;
                            break;
                        }
                    }

                    //Create a reverse word and filter for searching in reverse. The reversed word and filter do not include already checked chars
                    reversedWord   = new char[word.Length   - lastCheckedHeadIndex - lastCheckedTailIndex];
                    reversedFilter = new char[filter.Length - lastCheckedHeadIndex - lastCheckedTailIndex];

                    for (var i = 0; i < reversedWord.Length; i++)
                    {
                        reversedWord[i] = word[word.Length - (i + 1) - lastCheckedTailIndex];
                    }

                    for (var i = 0; i < reversedFilter.Length; i++)
                    {
                        reversedFilter[i] = filter[filter.Length - (i + 1) - lastCheckedTailIndex];
                    }

                    //Cut up the filter into seperate patterns, exclude * as they are not longer needed
                    for (var i = 0; i < reversedFilter.Length; i++)
                    {
                        if (reversedFilter[i] == '*')
                        {
                            if (i - currentPatternStartIndex > 0)
                            {
                                var pattern = new char[i - currentPatternStartIndex];
                                for (var j = 0; j < pattern.Length; j++)
                                {
                                    pattern[j] = reversedFilter[currentPatternStartIndex + j];
                                }

                                reversedPatterns.Add(pattern);
                            }

                            currentPatternStartIndex = i + 1;
                        }
                    }

                    //Search for the patterns
                    foreach (var t in reversedPatterns)
                    {
                        for (var j = 0; j < t.Length; j++)
                        {
                            if (t.Length - 1 - j > reversedWord.Length - 1 - reversedWordIndex)
                            {
                                return false;
                            }

                            if (t[j] != reversedWord[reversedWordIndex + j])
                            {
                                reversedWordIndex += 1;
                                j                 =  -1;
                            }
                            else
                            {
                                if (j == t.Length - 1)
                                {
                                    reversedWordIndex = reversedWordIndex + t.Length;
                                }
                            }
                        }
                    }

                    break;

                case 3:
                    //Same as Case 2 except ? is considered a match
                    //Search Head for matches util first *
                    for (var i = 0; i < filter.Length; i++)
                    {
                        if (filter[i] != '*')
                        {
                            if (filter[i] != word[i] && filter[i] != '?')
                            {
                                return false;
                            }
                        }
                        else
                        {
                            lastCheckedHeadIndex = i;
                            break;
                        }
                    }

                    //Search Tail for matches until first *
                    for (var i = 0; i < filter.Length; i++)
                    {
                        if (filter[filter.Length - 1 - i] != '*')
                        {
                            if (filter[filter.Length - 1 - i] != word[word.Length - 1 - i] &&
                                filter[filter.Length - 1 - i] != '?')
                            {
                                return false;
                            }
                        }
                        else
                        {
                            lastCheckedTailIndex = i;
                            break;
                        }
                    }

                    // Reverse and trim word and filter
                    reversedWord   = new char[word.Length   - lastCheckedHeadIndex - lastCheckedTailIndex];
                    reversedFilter = new char[filter.Length - lastCheckedHeadIndex - lastCheckedTailIndex];

                    for (var i = 0; i < reversedWord.Length; i++)
                    {
                        reversedWord[i] = word[word.Length - (i + 1) - lastCheckedTailIndex];
                    }

                    for (var i = 0; i < reversedFilter.Length; i++)
                    {
                        reversedFilter[i] = filter[filter.Length - (i + 1) - lastCheckedTailIndex];
                    }

                    for (var i = 0; i < reversedFilter.Length; i++)
                    {
                        if (reversedFilter[i] == '*')
                        {
                            if (i - currentPatternStartIndex > 0)
                            {
                                var pattern = new char[i - currentPatternStartIndex];
                                for (var j = 0; j < pattern.Length; j++)
                                {
                                    pattern[j] = reversedFilter[currentPatternStartIndex + j];
                                }

                                reversedPatterns.Add(pattern);
                            }

                            currentPatternStartIndex = i + 1;
                        }
                    }

                    //Search for the patterns
                    foreach (var t in reversedPatterns)
                    {
                        for (var j = 0; j < t.Length; j++)
                        {
                            if (t.Length - 1 - j > reversedWord.Length - 1 - reversedWordIndex)
                            {
                                return false;
                            }

                            if (t[j] != '?' && t[j] != reversedWord[reversedWordIndex + j])
                            {
                                reversedWordIndex += 1;
                                j                 =  -1;
                            }
                            else
                            {
                                if (j == t.Length - 1)
                                {
                                    reversedWordIndex = reversedWordIndex + t.Length;
                                }
                            }
                        }
                    }

                    break;
            }

            return isLike;
        }
    }
}
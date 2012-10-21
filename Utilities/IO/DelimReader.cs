

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Utilities
{
    using NewFile = Action<string>;
    using NoFileExists = Action<string>;
    using ProcessString = Func<string,string>;

    public interface IStringProcessor
    {
        void RefreshForNewFile(string path);
        string ProcessString(string str);
    }

    public enum WhitespaceHandling
    {
        DeleteAllWhitespace,
        NoBehaviour,
    }

    public enum CommentHandling
    {
        /// <summary>
        /// Only ignore lines commented at beginning of line
        /// </summary>
        CommentBegin,
        /// <summary>
        /// Ignore lines that have comments anywhere
        /// </summary>
        CommentAnywhere, 
        /// <summary>
        /// Remove portion of line after the first comment
        /// </summary>
        CommentPartial, 
        /// <summary>
        /// Ignore Comments.
        /// </summary>
        NoBehaviour, 
    }

    public enum ExceptionHandling
    {
        ThrowAll,
        CatchAll,
    }

    public static class ExtensionMethods
    {
        /// <summary>
        /// Strips all instances of stripCharacters from string.
        /// </summary>
        public static string Strip(this string me, params char[] stripCharacters)
        {
            StringBuilder sb = new StringBuilder();
            int k = 0;
            bool skip;
            while (k < me.Length)
            {
                skip = false;
                for (int j = 0; j < stripCharacters.Length; j++)
                {
                    if (me[k] == stripCharacters[j])
                        skip = true;
                }

                if (!skip)
                    sb.Append(me[k]);
                k++;
            }
            return sb.ToString();
        }

        /// <summary>
        /// Returns true if the string only contains characters found in onlyCharacters
        /// </summary>
        public static bool ContainsOnly(this string me, params char[] onlyCharacters)
        {
            int k = 0;
            while (k < me.Length)
            {
                foreach (char only in onlyCharacters)
                    if (me[k] != only)
                        return false;
                k++;
            }
            return true;
        }

        /// <summary>
        /// Returns true if the array contains any characters found in onlyCharacters
        /// </summary>
        public static bool ContainsAny(this char[] me, params char[] onlyCharacters)
        {
            return me.Any(c1 => onlyCharacters.Any(c2 => c2 == c1));
        }
    }

    public class DelimReader
    {
        bool m_ignoreBlankLines;
        bool m_ignoreBlankTokens;
        bool m_ignoreLinesStartingWithDelimiter;

        char[] m_delimiter;
        string m_comment;
        ICollection<char> m_stripChars;

        ExceptionHandling m_exceptionHandling;
        WhitespaceHandling m_whitespaceHandling;
        CommentHandling m_commentHandling;
        StringSplitOptions m_stringSplitOptions;

        ProcessString m_preProcessor;
        ProcessString m_postProcessor;
        ProcessString m_finalProcessor;
        NewFile       m_refreshForFile;
        public NoFileExists NoFileExists;

        #region Properties

        /// <summary>
        /// If true, the reader will ignore any lines that contain no data.
        /// Default: true
        /// </summary>
        public bool IgnoreBlankLines
        {
            get { return m_ignoreBlankLines; }
            set { m_ignoreBlankLines = value; }
        }

        /// <summary>
        /// Collection of characters to strip from each line.
        /// Default: none
        /// </summary>
        public ICollection<char> StripChars
        {
            get { return m_stripChars; }
            set { m_stripChars = value; }
        }

        /// <summary>
        /// If true, the reader will ignore any lines that have no content before the first delimiter.
        /// Default: true
        /// </summary>
        public bool IgnoreLinesStartingWithDelimiter
        {
            get { return m_ignoreLinesStartingWithDelimiter; }
            set { m_ignoreLinesStartingWithDelimiter = value; }
        }

        /// <summary>
        /// If true, the reader will ignore any empty entries in the lines.
        /// Default: false
        /// </summary>
        public bool IgnoreBlankTokens
        {
            get { return m_ignoreBlankTokens; }
            set
            {
                m_ignoreBlankTokens = value;
                if (value == true)
                    m_stringSplitOptions = StringSplitOptions.RemoveEmptyEntries;
                else
                    m_stringSplitOptions = StringSplitOptions.None;
            }
        }


        /// <summary>
        /// Gets or sets the delimiter character.  
        /// Default: ','
        /// </summary>
        public char[] Delimiter
        {
            get { return m_delimiter; }
            set { m_delimiter = value; }
        }

        /// <summary>
        /// Gets or sets the string that denotes a comment in the lines.  
        /// Default: "//"
        /// </summary>
        public string Comment
        {
            get { return m_comment; }
            set { m_comment = value; }
        }

        /// <summary>
        /// Determines the exception handling behaviour of the file reading process.  
        /// Default: ExceptionHandling.CatchAll
        /// </summary>
        public ExceptionHandling ExceptionHandling
        {
            get { return m_exceptionHandling; }
            set { m_exceptionHandling = value; }
        }

        /// <summary>
        /// Gets or sets the behaviour for handling comments in the lines.  
        /// Default: CommentHandling.CommentPartial
        /// </summary>
        public CommentHandling CommentHandling
        {
            get { return m_commentHandling; }
            set { m_commentHandling = value; }
        }

        /// <summary>
        /// Gets or sets the behaviour for handling whitespace in the lines.  
        /// Default: WhitespaceHandling.NoBehaviour
        /// </summary>
        public WhitespaceHandling WhitespaceHandling
        {
            get { return m_whitespaceHandling; }
            set { m_whitespaceHandling = value; }
        }


        /// <summary>
        /// Gets or sets the string PreProcessor.
        /// The PreProcessor is called before any other processing is done on a string, and before the string is added to the list.
        /// Default: null
        /// </summary>
        public ProcessString PreProcessor
        {
            get { return m_preProcessor; }
            set { m_preProcessor = value; }
        }

        /// <summary>
        /// Gets or sets the string PostProcessor.
        /// The PostProcessor is called after any other processing is done on a string, and before the string is added to the list.
        /// Default: null
        /// </summary>
        public ProcessString PostProcessor
        {
            get { return m_postProcessor; }
            set { m_postProcessor = value; }
        }


        /// <summary>
        /// Gets or sets the string FinalProcessor.
        /// The FinalProcessor is called after all strings have been Pre & Post-Processed, and added to the list.  It is run on each string in order.
        /// Default: null
        /// </summary>
        public ProcessString FinalProcessor
        {
            get { return m_finalProcessor; }
            set { m_finalProcessor = value; }
        }

        /// <summary>
        /// Gets or sets a function to call when a new file is loaded.
        /// Default: null
        /// </summary>
        public NewFile RefreshForFile
        {
            get { return m_refreshForFile; }
            set { m_refreshForFile = value; }
        }

        #endregion Properties

        /// <summary>
        /// Default constructor for DelimReader.
        /// Defaults: ',', @"//", CommentHandling.CommentPartial, ExceptionHandling.ThrowAll, WhitespaceHandling.NoBehaviour, true, false
        /// </summary>
        public DelimReader()
            : this(new char[] { ',' }, @"//", CommentHandling.CommentPartial, ExceptionHandling.ThrowAll, WhitespaceHandling.NoBehaviour, true, false, true, null)
        {
        }
        public DelimReader(char[] delimiter,
            string comment,
            CommentHandling commentHandling,
            ExceptionHandling exceptionHandling,
            WhitespaceHandling whitespaceHandling,
            bool ignoreBlankLines,
            bool ignoreBlankTokens,
            bool ignoreLinesStartingWithDelimiter,
            ICollection<char> stripChars
            )
        {
            m_delimiter = delimiter;
            m_comment = comment;
            m_commentHandling = commentHandling;
            m_exceptionHandling = exceptionHandling;
            m_whitespaceHandling = whitespaceHandling;
            m_ignoreBlankLines = ignoreBlankLines;
            IgnoreBlankTokens = ignoreBlankTokens;
            m_ignoreLinesStartingWithDelimiter = ignoreLinesStartingWithDelimiter;
            m_stripChars = stripChars;
        }

        private void NewFile(string file)
        {
            if(m_refreshForFile != null) m_refreshForFile(file);
        }

        public List<string> ReadToString(string file)
        {
            if (!File.Exists(file))
            {
                if (NoFileExists != null)
                    NoFileExists(file);
                return new List<string>();
            }

            NewFile(file);

            bool stripChars = false;
            try
            {
                if (m_stripChars != null)
                    stripChars = true;

                var lines = new List<string>();
                StreamReader reader = new StreamReader(file);
                string line;

                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();

                    if(m_preProcessor != null)
                        line = m_preProcessor(line);

                    if (stripChars)
                        foreach (char c in m_stripChars)
                            line = line.Strip(c);

                    if (m_whitespaceHandling == WhitespaceHandling.DeleteAllWhitespace)
                        line = line.Strip(' ');

                    int index = line.IndexOf(m_comment);
                    switch (m_commentHandling)
                    {
                        case CommentHandling.CommentBegin:
                            if (index == 0)
                                continue;
                            break;
                        case CommentHandling.CommentAnywhere:
                            if (index >= 0)
                                continue;
                            break;
                        case CommentHandling.CommentPartial:
                            if (index == 0)
                                continue;
                            else if (index > 0)
                                line = line.Substring(0, index);
                            break;
                        case CommentHandling.NoBehaviour:
                        default:
                            break;
                    }

                    if (m_ignoreLinesStartingWithDelimiter && line.Length > 0 && m_delimiter.Contains(line[0]))
                        continue;

                    if (m_ignoreBlankLines)
                        if (line.Length == 0 || line.ContainsOnly(m_delimiter))
                            continue;

                    if(m_postProcessor != null)
                        line = m_postProcessor(line);

                    lines.Add(line);
                }
                reader.Close();

                if (m_finalProcessor != null)
                    for (int k = 0; k < lines.Count; k++)
                        lines[k] = m_finalProcessor(lines[k]);

                return lines;
            }
            catch (Exception)
            {
                if (m_exceptionHandling == ExceptionHandling.ThrowAll)
                    throw;
            }

            return null;
        }

        /// <summary>
        /// Reads the file and returns the lines split on the delimiter.
        /// </summary>
        public List<string[]> ReadToStringArray(string file)
        {
            return SplitLines(ReadToString(file));
        }

        /// <summary>
        /// Returns a Dictionary of string arrays, keyed on the value in the string array at indexOfKey.
        /// </summary>
        public Dictionary<string, string[]> ReadToStringArrayDictionary(string file, int indexOfKey)
        {
            var Map = new Dictionary<string, string[]>();
            var lines = ReadToStringArray(file);
            foreach (var line in lines)
            {
                if (line.Length >= indexOfKey)
                    if (!Map.ContainsKey(line[indexOfKey]))
                        Map.Add(line[indexOfKey], line);
            }
            return Map;
        }

        /// <summary>
        /// Splits each line in lines on the delimiter and returns the list of string arrays.
        /// </summary>
        public List<string[]> SplitLines(List<string> lines)
        {
            if (lines == null)
                return null;

            var list = new List<string[]>(lines.Count);
            foreach (var line in lines)
            {
                if (line.Length == 0 && m_ignoreBlankLines)
                    continue;
                list.Add(line.Split(m_delimiter, m_stringSplitOptions));
            }
            return list;
        }
    }
}







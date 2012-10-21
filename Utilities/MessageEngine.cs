using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Utilities
{


    [TestFixture]
    class MessageCreatorTester
    {
        [Test]
        public void Test()
        {
            Dictionary<string, string> m = new Dictionary<string, string>()
            {
                {"m1", "asd {abc} asdd"},
                {"m2", "asd {double} {} {abc} asddf"},
            };

            MessageCreator mc = new MessageCreator
            (
                getMessage: mid => m[mid],
                formatParameter: (f, v) => Parameters.FormatParameter(f, v)
            ); 

            Assert.AreEqual(mc.FormatMessage("m2", "b|c|d"), "asd bb c abc asddf");
            Assert.AreNotEqual(mc.FormatMessage("m2", "b|c|d"), "asd bb  abc asddf");
        }

        class Parameters
        {
            public static string FormatParameter(string func, string value)
            {
                switch (func)
                {
                    case "double":
                        return value + value;
                    case "":
                        return value;
                    case "abc":
                        return "abc";
                }

                return value;
            }
        }
    }

    public class MessageCreator
    {
        public MessageCreator(Func<string, string> getMessage, Func<string, string, string> formatParameter)
        {
            GetMessage = getMessage;
            FormatParameter = formatParameter;
        }

        // Returns the text of the message, given a message ID.  This text will have parameters like {0}, {1} in it.
        readonly Func<string, string> GetMessage;
        // Formats the given string using the given format function.
        readonly Func<string, string, string> FormatParameter;

        /*public string FormatMessage(string messageID, string parameters)
        {
            var format = GetParameterFormatters(messageID);
            var p = parameters.Split('|');
            for (int i = 0; i < p.Length; i++)
            {
                p[i] = FormatParameter(format[i], p[i]);
            }
            return string.Format(GetMessage(messageID), p);
        }*/

        public string FormatMessage(string messageID, string parameters)
        {
            string message = GetMessage(messageID);
            var p = parameters.Split('|');
            int pCount = 0;
            StringBuilder paramBuilder = new StringBuilder();
            StringBuilder sb = new StringBuilder();
            bool inParam = false;
            for (int i = 0; i < message.Length; i++)
            {
                if (message[i] == '{')
                {
                    paramBuilder = new StringBuilder();
                    inParam = true;
                }
                else if (message[i] == '}')
                {
                    inParam = false;
                    sb.Append(FormatParameter(paramBuilder.ToString(), p[pCount++]));
                }
                else if (inParam)
                    paramBuilder.Append(message[i]);
                else
                    sb.Append(message[i]);
            }

            return sb.ToString();
        }
    }

}

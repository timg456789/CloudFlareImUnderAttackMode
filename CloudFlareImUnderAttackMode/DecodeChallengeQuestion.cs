using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CloudFlareImUnderAttackMode
{
    public class DecodeChallengeQuestion
    {
        private static Dictionary<int, List<string>> NumberEncodings => new Dictionary<int, List<string>>
        {
            {0, new List<string> {"+[]"}},
            {1, new List<string> {"+!![]"}},
            {2, new List<string> {"!+[]+!![]"}},
            {3, new List<string> {"!+[]+!![]+!![]"}},
            {4, new List<string> {"!+[]+!![]+!![]+!![]"}},
            {5, new List<string> {"!+[]+!![]+!![]+!![]+!![]"}},
            {6, new List<string> {"!+[]+!![]+!![]+!![]+!![]+!![]"}},
            {7, new List<string> {"!+[]+!![]+!![]+!![]+!![]+!![]+!![]"}},
            {8, new List<string> {"!+[]+!![]+!![]+!![]+!![]+!![]+!![]+!![]"}},
            {9, new List<string> {"!+[]+!![]+!![]+!![]+!![]+!![]+!![]+!![]+!![]"}}
        };

        enum MathOperator
        {
            Multiply,
            Subtract,
            Add
        }

        public string GetClearanceUrl(string html)
        {
            var challengeQuestionsAnswer = Decode(html, "images.nga.gov");
            var xhtml = html
                .Replace("&hellip;", string.Empty)
                .Replace("<br>", string.Empty);

            var xmlDoc = XmlDocFactory.Create(xhtml);

            var challengeFormNode = xmlDoc.SelectSingleNode("//form[@id='challenge-form']");
            var jschlVcNode = challengeFormNode.SelectSingleNode("//input[@name='jschl_vc']");
            var passNode = challengeFormNode.SelectSingleNode("//input[@name='pass']");

            var vcVar = jschlVcNode.Attributes["value"].InnerText;
            var passVar = passNode.Attributes["value"].InnerText;

            var clearanceUrl = $"http://images.nga.gov/cdn-cgi/l/chk_jschl?jschl_vc={vcVar}&pass={passVar}&jschl_answer={challengeQuestionsAnswer}";

            return clearanceUrl;
        }

        public int Decode(string obfuscatedAndEncoded, string domain)
        {
            List<string> obfuscatedPieces = GetObfuscatedPieces(obfuscatedAndEncoded);

            string decodedAssignment = DecodeObfuscatedPiece(obfuscatedPieces.First());
            int decodedAssignmentParsed = GetNumericAssignment(decodedAssignment);
            obfuscatedPieces.RemoveAt(0);

            foreach (var obfuscatedPiece in obfuscatedPieces)
            {
                var decodedPiece = DecodeObfuscatedPiece(obfuscatedPiece);
                var nextDecodedAssignment = ConcatenateIntsForAssignment(decodedPiece);
                int nextDecodedAssignmentParsed = GetNumericAssignment(nextDecodedAssignment);
                decodedAssignmentParsed = CalculateNextAssignment(decodedAssignmentParsed, nextDecodedAssignmentParsed,
                    GetMathOperator(nextDecodedAssignment));
            }

            decodedAssignmentParsed = decodedAssignmentParsed + domain.Length;

            return decodedAssignmentParsed;
        }

        public string ConcatenateInts(string decodedPiece)
        {
            return decodedPiece.Replace("(", string.Empty)
                .Replace(")", string.Empty)
                .Replace("+", string.Empty);
        }

        public string DecodeObfuscatedPiece(string obfuscatedPiece)
        {
            foreach (KeyValuePair<int, List<string>> numberEncoding in NumberEncodings.OrderByDescending(x => x.Key))
            {
                foreach (var encoding in numberEncoding.Value)
                {
                    obfuscatedPiece = obfuscatedPiece.Replace(encoding + "+[]", numberEncoding.Key.ToString()); // May be padded with 0 and 0 is a valid value in and of itself.
                    obfuscatedPiece = obfuscatedPiece.Replace(encoding, numberEncoding.Key.ToString());
                }
            }

            return obfuscatedPiece;
        }

        private int CalculateNextAssignment(int current, int numeric, MathOperator mathOperator)
        {
            if (mathOperator == MathOperator.Multiply)
            {
                return current * numeric;
            }
            else if (mathOperator == MathOperator.Add)
            {
                return current + numeric;
            }
            else if (mathOperator == MathOperator.Subtract)
            {
                return current - numeric;
            }
            else
            {
                throw new Exception("Unknown operator: " + mathOperator);
            }
        }

        private MathOperator GetMathOperator(string hasOperator)
        {
            if (hasOperator.Contains("*="))
            {
                return MathOperator.Multiply;
            }
            else if (hasOperator.Contains("-="))
            {
                return MathOperator.Subtract;
            }
            else if (hasOperator.Contains("+"))
            {
                return MathOperator.Add;
            }
            else
            {
                throw new Exception("Unknown operator: " + hasOperator);
            }
        }

        private int GetNumericAssignment(string decodedAssignment)
        {
            return int.Parse(Regex.Replace(decodedAssignment, "[^0-9]", ""));
        }

        private string ConcatenateIntsForAssignment(string obfuscatedAssignment)
        {
            var obfuscatedVarOperatorAndValue = obfuscatedAssignment.Split('=');
            obfuscatedAssignment = obfuscatedVarOperatorAndValue[0] + "=" +
                                   ConcatenateInts(obfuscatedVarOperatorAndValue[1]);
            return obfuscatedAssignment;
        }

        private List<string> GetObfuscatedPieces(string obfuscated)
        {
            int obfuscatedStart = obfuscated.IndexOf("var s,t,o,p,b,r,e,a,k,i,n,g,f, ", StringComparison.OrdinalIgnoreCase);
            var newObfuscated = obfuscated.Substring(obfuscatedStart);
            newObfuscated = newObfuscated.Replace("var s,t,o,p,b,r,e,a,k,i,n,g,f, ", string.Empty);
            var obfuscatedVariableIndex = newObfuscated.IndexOf("=", StringComparison.OrdinalIgnoreCase);
            var obfuscatedVariable = newObfuscated.Substring(0, obfuscatedVariableIndex);
            var pieces = newObfuscated.Split(';').ToList();
            pieces = pieces
                .Where(x => x.StartsWith(obfuscatedVariable, StringComparison.Ordinal))
                .ToList();
            return pieces;
        }
    }
}

using System;

namespace KataRailFenceCiper
{       /*Create two functions to encode and then decode a string using the Rail Fence Cipher. 
         * This cipher is used to encode a string by placing each character successively in a diagonal along a set of "rails". 
         * First start off moving diagonally and down. When you reach the bottom, 
         * reverse direction and move diagonally and up until you reach the top rail. 
         * Continue until you reach the end of the string. Each "rail" is then read left to right to derive the encoded string.

        For example, the string "WEAREDISCOVEREDFLEEATONCE" could be represented in a three rail system as follows:

        W       E       C       R       L       T       E
          E   R   D   S   O   E   E   F   E   A   O   C  
            A       I       V       D       E       N    
        The encoded string would be:

        WECRLTEERDSOEEFEAOCAIVDEN
        Write a function/method that takes 2 arguments, a string and the number of rails, and returns the ENCODED string.

        Write a second function/method that takes 2 arguments, an encoded string and the number of rails, and returns the DECODED string.

        For both encoding and decoding, assume number of rails >= 2 and that passing an empty string will return an empty string.

        Note that the example above excludes the punctuation and spaces just for simplicity. 
        There are, however, tests that include punctuation. Don't filter out punctuation as they are a part of the string.
         * 
         * 
         * Ok so how we do it?
         * Encoding:
         * decide which array to put the char in by:
         * have an int called crement initialised to 1 which when we have put a char in the last array (rail) we set to -1 
         * then when we have put a char in array (rail) 0 we set back to 1
         * as we work thru the list we (in or de) crement the array (rail) the char is being put in
         * 
         * then we combine the arrays ignoring empty values, can be an obscure char if there's any issues.
         * 
         * Decoding:
         * H !e,Wdloollr 4 rails 13 chars long index 0 to 12 
         * 
         * H    ' '    !
         *  e   , W   d
         *   l o   o l
         *    l     r                                                                                                                      
         * 
         * WECRLTEERDSOEEFEAOCAIVDEN 25 letters long , index 0 - 24
         * W       E       C       R       L       T       E
             E   R   D   S   O   E   E   F   E   A   O   C  
               A       I       V       D       E       N       
         * Chars on row 0 is index multiplied by 4 
         * 
         *  This is what we need ! :D
         * Can we work out indexes of chars on first row 
         * I think the multiplier for row ones index is; the no. of rails + no. of rails - 2 (on the way back up we wont be using the top or bottom rail)
         * Then for each subsequent row put chars in first avaliable index i.e just after a taken index or just before a taken index
         * if we're on the last row just put the remaining chars in the first avaliable space
         */
    class Program
    {
        static void Main(string[] args)
        {
            int rails = 9;
            string expected = Encode("WEAREDISCOVEREDFLEEATONCE", rails); //max index 39
            Console.WriteLine(Decode(expected, rails));

            //Console.WriteLine(Decode("veritatis Voluptate distinctio ipsam provident, facere ipsa minima non alias exercitationem Deserunt molestiae. eveniet excepturi autem fugiat maiores!jk officiis quasi iure iure iure sit! Porro quibusdam unde! unde, Amet asperiores", rails));
            //Console.WriteLine(Decode("H !e,Wdloollr", 4));
            //Console.WriteLine(Decode("WECRLTEERDSOEEFEAOCAIVDEN", rails));
        }

        public static string Encode(string s, int n)
        {
            //Console.WriteLine($"Encode: {s} {n}");
            int rails = n > 2 ? n : 2;
            char[,] arrs = new char[rails, s.Length];
            for (int i = 0; i < rails; i++)
            {
                for (int j = 0; j < s.Length; j++)
                {
                    arrs[i, j] = '\0';
                }
            }

            int crement = 1;
            int arr = 0;
            for (int i = 0; i < s.Length; i++)
            {
                //Console.WriteLine($"Array: {arr} position: {i} Char: {s[i]}");
                arrs[arr, i] = s[i];
                if (arr == rails-1)
                {
                    crement = -1;
                }
                if (arr == 0)
                {
                    crement = 1;
                }
                arr += crement;
            }

            string outString = "";

            for (int i = 0; i < rails; i++)
            {
                for (int j = 0; j < s.Length; j++)
                {
                    if (arrs[i, j] != '\0')
                    {
                        outString += arrs[i, j];
                    }
                }
            }
            //Console.WriteLine($"Encoded: {outString}");
            return outString;
        }

        static int currentChar;
        static bool findFirstEmpty;
        static int avaliableSpace;
        static int largestApex;
        public static string Decode(string s, int n)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;

            //Console.WriteLine($"Decode: {s} {n}");
            char[] output = new char[s.Length];
            for (int i = 0; i < output.Length; i++) output[i] = '\0';

            currentChar =  FillFromFirstRow(s, n, output);
            avaliableSpace = 0;
            findFirstEmpty = true;
            largestApex = FindLargestApex(s, n);
            //Console.WriteLine($"Largest apex: {largestApex}");
            //Console.WriteLine($"Current index: {currentChar} ({s[currentChar]})");
            //Console.WriteLine(output);

            for (int i = 0; i < output.Length; i++)
            {
                avaliableSpace = FindNextAvaliableSpace(output);
                if (avaliableSpace == -1) break;

                if (currentChar + i < output.Length)
                {
                    output[avaliableSpace] = s[currentChar + i];
                }
                //Console.WriteLine($"Putting char {s[currentChar + i]} at {avaliableSpace}");

                string debugString = "";
                for (int j = 0; j < output.Length; j++)
                {
                    debugString += " " + output[j];
                }
                //Console.WriteLine(debugString);
            }

           
            return new string(output);
        }

        static int FindLargestApex(string s, int n)
        {
            int result = 0;
            for (int i = 0; i < s.Length; i++)
            {
                result = (n - 1) * i;
                if (result > s.Length -1)
                {
                    return (n - 1) * (i - 1);
                }
            }
            return 0;
        }

        static int FindNextAvaliableSpace(char[] output)
        {
            if (AssignLastChar(output)) return output.Length - 1;

            if (findFirstEmpty)
            {
                for (int i = avaliableSpace; i < output.Length; i++)
                {
                    if (output[i] == '\0')
                    {
                        //Console.WriteLine($"Found first empty at {i}");
                        findFirstEmpty = false;
                        return i;
                    }
                }
                //Console.WriteLine("At end of string looking for first empty, going back to the start");
                for (int i = 0; i < output.Length; i++)
                {
                    if (output[i] == '\0')
                    {
                        //Console.WriteLine($"Found first empty at {i}");
                        findFirstEmpty = false;
                        return i;
                    }
                }
                return -1;
            }
            else
            {
                for (int i = avaliableSpace; i < output.Length; i++)
                {
                    if (i < output.Length - 1)
                    {
                        if (output[i] == '\0' && output[i + 1] != '\0')
                        {
                            //Console.WriteLine($"Found last empty at {i}");
                            findFirstEmpty = true;
                            return i;
                        }
                    }
                }
               // Console.WriteLine("At end of string looking for last empty, going back to start and looking for first empty");
                for (int i = 0; i < output.Length; i++)
                {
                    if (output[i] == '\0')
                    {
                        //Console.WriteLine($"Found first empty at {i}");
                        findFirstEmpty = false;
                        return i;
                    }
                }
                return -1;
            }
        }

        static bool AssignLastChar(char[] output)
        {
            if (largestApex - avaliableSpace == (output.Length - 1) - largestApex)
            {
                //Console.WriteLine("!!!!!Assigning last char");
                return true;
            }
            return false;
        }

        static int FillFromFirstRow(string s, int n, char[] output)
        {
            for (int i = 0; i < s.Length; i++)
            {
                int ind = i * ((2 * n) - 2);
                if (ind >= s.Length)
                {
                    return i;
                }
                output[ind] = s[i];
                //Console.WriteLine($"output: {ind} is: {s[i]}");
            }
            return 0;
        }
    }
}

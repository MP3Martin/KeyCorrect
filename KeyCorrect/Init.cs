using InputInterceptorNS;
using static KeyCorrect.Program;

namespace KeyCorrect {
    internal static class Init {
        internal static void Run() {
            // start of init
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            List<int>? alphabetNums = new List<int> { 28, 57 };
            for (int i = 16; i <= 25; i++) {
                alphabetNums.Add(i);
            }
            for (int i = 30; i <= 38; i++) {
                alphabetNums.Add(i);
            }
            for (int i = 44; i <= 50; i++) {
                alphabetNums.Add(i);
            }

            // create a list of keycodes that represent the simple alphabet
            foreach (int num in alphabetNums) {
                alphabetKeyCodes.Add((KeyCode)num);
            }

            alphabetNums = null;

            foreach (int index in Enumerable.Range(97, 122 - 97 + 1)) {
                alphabetCharactersAndMoreAsString.Add(((char)index).ToString());
            }
            foreach (int index in Enumerable.Range(65, 90 - 65 + 1)) {
                alphabetCharactersAndMoreAsString.Add(((char)index).ToString());
            }
            foreach (string symbol in new List<string> { ".", ",", " " }) {
                alphabetCharactersAndMoreAsString.Add(symbol);
            }

            // end of init
        }
    }
}

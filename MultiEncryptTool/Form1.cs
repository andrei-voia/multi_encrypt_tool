using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace MultiEncryptTool
{
    public partial class Form1 : Form
    {
        //defined
        private const int MODULE_VALUE = 125;
        private const int ITERATIONS = 10;
        private const int MIN_ITERATIONS = 2;
        private const int MAX_ITERATIONS = 1000;
        private const int LABEL_RESET_LOCATION = 232;
        //variables
        private string encrypted = "";
        private string text = "";
        private string key = "";
        private Random random = new Random();

        //constructor
        public Form1()
        {
            InitializeComponent();

            textBox3.Text = "" + ITERATIONS;
            textBox3.Visible = false;
            label8.Visible = false;
            label5.Text = "(Length: " + richTextBox1.Text.Length + ")";
            label5.Left = LABEL_RESET_LOCATION - TextRenderer.MeasureText(label5.Text, label5.Font).Width;
            label11.Text = "(Length: " + richTextBox3.Text.Length + ")";
            label11.Left = LABEL_RESET_LOCATION - TextRenderer.MeasureText(label11.Text, label11.Font).Width;
            label9.Text = "";

            //block the output text to be modified
            richTextBox1.ReadOnly = true;
            richTextBox1.BackColor = SystemColors.Window;
        }

        //programs used inside the program to make sure it won't crash and work properly
        #region auxiliary programs
        //gets the iterations you want to be made and also checks for wrong inputs
        private int getIterationValue()
        {
            int iterations = ITERATIONS;

            if (textBox3.Text == "")
            {
                MessageBox.Show("There is no iteration value.\nValue was autoset to " + ITERATIONS);
                textBox3.Text = "" + ITERATIONS;
            }

            else if (Convert.ToInt32(textBox3.Text) < MIN_ITERATIONS)
            {
                MessageBox.Show("There should be at least " + MIN_ITERATIONS + " iterations to be made.\n\nValue was autoset to " + ITERATIONS);
                textBox3.Text = "" + ITERATIONS;
            }

            else if (Convert.ToInt32(textBox3.Text) > MAX_ITERATIONS)
            {
                MessageBox.Show("There are too many iterations, please choose a number below " + MAX_ITERATIONS + ".\nValue was autoset to " + ITERATIONS);
                textBox3.Text = "" + ITERATIONS;
            }

            else iterations = Convert.ToInt32(textBox3.Text);

            return iterations;
        }

        //delete the last character
        private string deleteLastCharacter(string textBox)
        {
            return textBox.Substring(0, textBox.Length - 1);
        }

        //reset stuff
        private void resetAll()
        {
            richTextBox3.Text = "";
            textBox2.Text = "";
            textBox3.Text = "" + ITERATIONS;
            richTextBox1.Text = "";
            encrypted = "";
            text = "";
            key = "";
            radioButton1.Checked = true;
            radioButton3.Checked = true;
            checkBox1.Checked = false;
            label11.Text = "(Length: " + richTextBox3.Text.Length + ")";
            label11.Left = LABEL_RESET_LOCATION - TextRenderer.MeasureText(label11.Text, label11.Font).Width;
            label5.Text = "(Length: " + richTextBox1.Text.Length + ")";
            label5.Left = LABEL_RESET_LOCATION - TextRenderer.MeasureText(label5.Text, label5.Font).Width;
            label9.Text = "";
        }

        //creates new key for the No-Key option
        private string setNoKeyWord(string text)
        {
            int min = 3;
            int max, size;

            if (text.Length < 7) max = text.Length;
            else max = 7;

            size = setRandom(min, max);

            char[] key = new char[size];

            for (int i = 0; i < size; i++)
            {
                key[i] = (char)setRandom(33, 120);
            }

            return new string(key);
        }

        //find out if the text and key are wrote down
        private bool inputsOk()
        {
            if (richTextBox3.Text == "")
            {
                richTextBox1.Text = "";
                label5.Text = "(Length: " + richTextBox1.Text.Length + ")";
                label5.Left = LABEL_RESET_LOCATION - TextRenderer.MeasureText(label5.Text, label5.Font).Width;
                MessageBox.Show("No text found.");
                return false;
            }

            if (textBox2.Text == "")
            {
                richTextBox1.Text = "";
                label5.Text = "(Length: " + richTextBox1.Text.Length + ")";
                label5.Left = LABEL_RESET_LOCATION - TextRenderer.MeasureText(label5.Text, label5.Font).Width;
                MessageBox.Show("No key found.");
                return false;
            }
            return true;
        }

        //checks if the text is long enough or the key short enough
        private bool checkTextAndKeyLength()
        {
            if (richTextBox3.Text.Length < textBox2.Text.Length)
            {
                MessageBox.Show("Error: You must enter a shorter Key or a longer Text.");
                return false;
            }

            return true;
        }

        //checks if the metohds you choose are existent in the list of available methods
        private string getComboName()
        {
            if (comboBox1.Text == "Basic Encrypt") return comboBox1.Text;
            else if (comboBox1.Text == "Long Encrypt") return comboBox1.Text;
            else if (comboBox1.Text == "Short Encrypt") return comboBox1.Text;

            return "";
        }

        //this checks if everything is good to go to run the encryption
        private bool checkSelections()
        {
            if (checkBox1.Checked == true)
            {
                if (richTextBox3.Text == "")
                {
                    richTextBox1.Text = "";
                    label5.Text = "(Length: " + richTextBox1.Text.Length + ")";
                    MessageBox.Show("No text found.");
                    return false;
                }
            }
            else if (inputsOk() == false) return false;
            if (radioButton1.Checked == false && radioButton2.Checked == false)
            {
                MessageBox.Show("Please choose a type.");
                return false;
            }
            if (getComboName() == "")
            {
                MessageBox.Show("Please choose a valid method.");
                return false;
            }

            return true;
        }

        //check if the encrypted text is safe, meaning that it is decryptable too
        private bool isSafe(string encrypted, int difficulty)
        {
            if (text != decrypt(encrypted, key))
            {
                MessageBox.Show("Error while trying to Safe-Encrypt text.");
                return false;
            }

            return true;
        }

        //get if the difficulty is checked for input
        private int getDifficulty()
        {
            if (radioButton3.Checked == true) return 1;
            if (radioButton4.Checked == true) return 2;
            if (radioButton5.Checked == true) return 3;

            MessageBox.Show("Please choose a difficulty step for encryption.");
            return 0;
        }
        #endregion

        //ENCRYPTION method
        private string encrypt(string text, string key, int difficulty)
        {
            string encrypted = "";
            encrypted = text;
            int randomNumber;
            int[] order = new int[5];
            int choice = 0;

            //set the random number
            randomNumber = setRandomNumber();

            setOrder(ref order, key);

            for (int i = 1; i <= 4; i++)
            {
                //shift the entire word right
                if (order[i] == 1) encrypted = shiftLeft(encrypted, (key.Length % 6));
                //reverse the word HERE
                else if (order[i] == 2) encrypted = reverse(encrypted);
                //increment the ASCII code
                else if (order[i] == 3) encrypted = incrementASCII(encrypted, (1 + key.Length % 5));
                //scatter the characters
                else if (order[i] == 4) if (difficulty > 2)
                    {
                        int distance = 2 + key.Length % 5 - key.Length / 10;
                        int offset = 1 + key.Length % 3;
                        if (distance < 0) distance = -1 * distance;
                        if (offset < 0) offset = -1 * offset;
                        if (distance < offset)
                        {
                            int aux = distance;
                            distance = offset;
                            offset = aux;
                        }

                        encrypted = scatter(encrypted, distance, offset);
                    }
            }
            //change to random
            if (difficulty > 1) encrypted = doXOR(encrypted, key, randomNumber);

            if (difficulty > 2) encrypted = addRandomizedASCII(encrypted);
            //include here buildMatrix!!
            if (difficulty > 2) encrypted = buildMatrix(encrypted, key, randomNumber);

            encrypted = includeRandom(encrypted, key, randomNumber);

            if (difficulty > 1)
            {
                encrypted = converToBin(encrypted);

                for (int i = 0; i < key.Length; i++)
                    if (key[i] % 2 == 1) choice += key[i];

                if (choice % 2 == 0)
                {
                    //scatter the bits
                    if (difficulty > 2) encrypted = scatterBits(encrypted, key);
                    //change the bits between them
                    encrypted = interchangeBits(encrypted, key);
                }
                else
                {
                    //change the bits between them
                    if (difficulty > 2) encrypted = interchangeBits(encrypted, key);
                    //scatter the bits
                    encrypted = scatterBits(encrypted, key);
                }
            }

            //this has the purpose of making the decryption knowing how to decrypt it
            encrypted = setAdititonalKey(encrypted, difficulty, checkBox1.Checked);
            encrypted = addDifficulty(encrypted, difficulty);

            return encrypted;
        }

        //DECRYPTION method
        private string decrypt(string encrypted, string key)
        {
            string decrypted = "";
            decrypted = encrypted;
            int randomNumber = 0;
            int[] order = new int[5];
            int choice = 0;
            int difficulty = 0;

            //extrats the difficulty at which the encryption will be decripted, between 1 and 3
            decrypted = extractDifficulty(decrypted, ref difficulty);
            decrypted = getAdditionalKey(decrypted, difficulty, ref key);

            if (difficulty > 1)
            {
                for (int i = 0; i < key.Length; i++)
                    if (key[i] % 2 == 1) choice += key[i];

                if (choice % 2 == 0)
                {
                    decrypted = interchangeBits(decrypted, key);
                    if (difficulty > 2) decrypted = scatterBits(decrypted, key);
                }
                else
                {
                    decrypted = scatterBits(decrypted, key);
                    if (difficulty > 2) decrypted = interchangeBits(decrypted, key);
                }

                decrypted = converToDecimal(decrypted);
            }

            decrypted = extractRandom(decrypted, key, ref randomNumber);

            if (difficulty > 2) decrypted = decryptMatrix(decrypted, key, randomNumber);

            if (difficulty > 2) decrypted = extractRandomizedASCII(decrypted);
            //change to random
            if (difficulty > 1) decrypted = reverseXOR(decrypted, key, randomNumber);

            setOrder(ref order, key);

            for (int i = 4; i >= 1; i--)
            {
                if (order[i] == 1) decrypted = shiftRight(decrypted, (key.Length % 6));
                else if (order[i] == 2) decrypted = reverse(decrypted);
                else if (order[i] == 3) decrypted = incrementASCII(decrypted, -1 * (1 + key.Length % 5));   //reverse of the incrementation (decrement)
                else if (order[i] == 4) if (difficulty > 2)
                    {
                        int distance = 2 + key.Length % 5 - key.Length / 10;
                        int offset = 1 + key.Length % 3;
                        if (distance < 0) distance = -1 * distance;
                        if (offset < 0) offset = -1 * offset;
                        if (distance < offset)
                        {
                            int aux = distance;
                            distance = offset;
                            offset = aux;
                        }

                        decrypted = scatter(decrypted, distance, offset);
                    }
            }

            return decrypted;
        }

        //ENCRYPTING
        #region encrypting process
        private int setRandom(int lower, int upper)
        {
            //set a random number between 10 and 99
            return random.Next(lower, upper + 1);
        }

        private int setRandomNumber()
        {
            int randomNumber = 0;
            randomNumber = setRandom(setRandom(10, 50), setRandom(60, 99));
            if (randomNumber % 10 == 0)
            {
                randomNumber += setRandom(1, 9);
            }
            return randomNumber;
        }

        private string shiftLeft(string encrypted, int n)
        {
            char[] localCharacter = encrypted.ToCharArray();

            for (int i = 0; i < localCharacter.Length; i++)
            {
                if (i + n < localCharacter.Length)
                    localCharacter[i] = encrypted[i + n];
                else
                    localCharacter[i] = encrypted[i + n - localCharacter.Length];
            }

            return new string(localCharacter);
        }

        private string reverse(string encrypted)
        {
            char[] localCharacter = encrypted.ToCharArray();

            for (int i = 0; i < localCharacter.Length / 2; i++)
            {
                char aux = localCharacter[i];
                localCharacter[i] = localCharacter[localCharacter.Length - 1 - i];
                localCharacter[localCharacter.Length - 1 - i] = aux;
            }

            return new string(localCharacter);
        }

        private string incrementASCII(string encrypted, int n)
        {
            char[] localCharacter = encrypted.ToCharArray();

            for (int i = 0; i < localCharacter.Length; i++)
            {
                localCharacter[i] = (char)((int)localCharacter[i] + n);
            }

            return new string(localCharacter);
        }

        private string scatter(string encrypted, int distance, int offset)
        {
            char[] localCharacter = encrypted.ToCharArray();

            for (int i = distance; i < localCharacter.Length; i = i + distance + offset)
            {
                char aux = localCharacter[i];
                localCharacter[i] = localCharacter[i - distance];
                localCharacter[i - distance] = aux;
            }

            return new string(localCharacter);
        }

        private void setElement(ref int[] order, int rest, int i)
        {
            if (order[rest] == 0) order[rest] = i;

            else if (order[1] == 0) order[1] = i;
            else if (order[2] == 0) order[2] = i;
            else if (order[3] == 0) order[3] = i;
            else if (order[4] == 0) order[4] = i;
        }

        private void setOrder(ref int[] order, string key)   //0, 1, 2 and 3
        {
            order[1] = 0;
            order[2] = 0;
            order[3] = 0;
            order[4] = 0;

            if (key.Length < 3)
            {
                order[1] = 1;   //shiftLeft
                order[2] = 2;   //reverse
                order[3] = 3;   //incrementASCII
                order[4] = 4;   //scatter
                return;
            }

            int space = key.Length / 3;
            int i;
            for (i = 1; i <= 3; i++)
            {
                char a = key[i * space - 1];
                int rest = (int)a % 4 + 1;

                setElement(ref order, rest, i);
            }
            setElement(ref order, 1, 4);
        }

        private void include(ref char[] localCharacter, char character, int position)
        {
            int i;
            int size = localCharacter.Length;
            Array.Resize(ref localCharacter, localCharacter.Length + 1);

            for (i = size; i > position; i--)
            {
                localCharacter[i] = localCharacter[i - 1];
            }

            localCharacter[position] = character;
        }

        private string doXOR(string encrypted, string key, int random)
        {
            char[] localCharacter = encrypted.ToCharArray();

            int value = 0;
            int countWords = random + random % 10;

            for (int i = 0; i < key.Length; i++) value += key[i];

            while (value >= random * 3 + value % 10) value /= 2;

            for (int i = 0; i < localCharacter.Length; i++)
            {
                countWords++;
                int module = MODULE_VALUE;   //125 works
                int count = 0;

                localCharacter[i] = (char)(localCharacter[i] ^ ((countWords + value) % module));
                int localValue = localCharacter[i];

                while (localValue < 34 || localValue > 125)  //exclude 126 because we will have problem with randomizedASCII
                {
                    localCharacter[i] = (char)(localCharacter[i] ^ ((countWords + value) % module));
                    module -= 5;
                    count++;

                    localCharacter[i] = (char)(localCharacter[i] ^ ((countWords + value) % module));
                    localValue = localCharacter[i];
                }


                if (module != MODULE_VALUE)
                {
                    int j;
                    for (j = 0; j < count; j++)
                    {
                        include(ref localCharacter, (char)33, i);
                        i++;
                    }
                }
            }

            return new string(localCharacter);
        }

        private int getLongest(char[] locationCharacter)
        {
            int count = 1;
            int longest = 0;
            int i;
            for (i = 1; i < locationCharacter.Length; i++)
            {
                if (locationCharacter[i] == locationCharacter[i - 1]) count++;
                else count = 1;

                if (count > longest) longest = count;
            }

            return longest;
        }

        private string addRandomizedASCII(string encrypted)
        {
            char[] localCharacter = encrypted.ToCharArray();

            int length = getLongest(localCharacter) + 10;
            include(ref localCharacter, (char)(33 + length), 0);

            int i;
            for (i = 0; i < localCharacter.Length; i++)
            {
                if ((int)localCharacter[i] + 1 + i % length <= 126)
                    localCharacter[i] = (char)((int)localCharacter[i] + 1 + i % length);

                else
                {
                    localCharacter[i] = (char)(((int)localCharacter[i] + 1 + i % length) - 126 + 33);
                }
            }

            return new string(localCharacter);
        }

        private string includeRandom(string encrypted, string key, int random)
        {
            char[] localCharacter = encrypted.ToCharArray();

            int a;
            int mode = 1;
            int value = (int)key[0] / 10;

            int i;
            for (i = 1; i < key.Length; i++)
            {
                if (mode == 1) value *= (int)key[i] / 10;
                else value -= (int)key[i] / 10;

                mode = -1 * mode;
            }

            a = value % 10;    //or just %(random/10)
            char first = (char)(30 + 4 * (random / 10 + a));  //first digit
            char second = (char)(120 - 4 * (random % 10 + a)); //second digit

            include(ref localCharacter, second, 0);
            include(ref localCharacter, first, localCharacter.Length);

            return new string(localCharacter);
        }

        private int getPow(int a, int power)
        {
            int i;
            if (power == 0) return 1;
            int sum = 1;

            for (i = 1; i <= power; i++)
                sum = sum * a;

            return sum;
        }

        private string converToBin(string encrypted)
        {
            char[] localCharacter = encrypted.ToCharArray();

            int count = 0;
            char[] binaryCharacter = new char[localCharacter.Length * 8];

            int i, j;
            for (i = 0; i < localCharacter.Length; i++)
            {
                int[] bin = new int[8];
                int countBin = 8;
                for (j = 0; j < 8; j++) bin[j] = 0;

                char n = localCharacter[i];

                //save in bin a character transformed in binary
                while (n != 0)
                {
                    countBin--;
                    if ((int)(n & 1) != 0)
                        bin[countBin] = 1;
                    else
                        bin[countBin] = 0;
                    n >>= 1;
                }
                //save in bin a character transformed in binary
                while (countBin != 0)
                {
                    countBin--;
                    bin[countBin] = 0;
                }
                //save it in binaryCharacter
                for (j = 0; j < 8; j++)
                {
                    binaryCharacter[count] = (char)bin[j];
                    count++;
                }
                //interchange
                for (j = 0; j <= 3; j++)
                {
                    int aux = bin[j];
                    bin[j] = bin[7 - j];
                    bin[7 - j] = aux;
                }

                int dec = 0;
                for (j = 0; j < 8; j++)
                {
                    if (bin[j] == 1)
                        dec += getPow(2, j);
                }
            }

            char[] save = new char[localCharacter.Length * 8];
            for (i = 0; i < count; i++)
            {
                save[i] = (char)(48 + (int)binaryCharacter[i]);
            }

            return new string(save);
        }

        private string scatterBits(string encrypted, string key)
        {
            char[] localCharacter = encrypted.ToCharArray();

            int i;
            int coordinate;
            int sum = 0;
            for (i = 0; i < key.Length; i++)
                if ((int)key[i] % 2 == 0)
                    sum += key[i];

            coordinate = sum / key.Length;
            if (coordinate < 1) coordinate = 1;

            while (coordinate < localCharacter.Length)
            {
                for (i = 0; i < key.Length; i++)
                {
                    if (coordinate + i >= localCharacter.Length) break;
                    if (localCharacter[coordinate + i] == '0') localCharacter[coordinate + i] = '1';
                    else localCharacter[coordinate + i] = '0';
                }
                coordinate += coordinate;
            }

            return new string(localCharacter);
        }

        private string interchangeBits(string encrypted, string key)
        {
            char[] localCharacter = encrypted.ToCharArray();

            int i;
            int offset = 1 + localCharacter.Length % key.Length;
            int advance = 1 + key.Length;

            for (i = offset; i < localCharacter.Length; i = i + advance)
            {
                if (i + offset >= localCharacter.Length) break;
                char aux = localCharacter[i];
                localCharacter[i] = localCharacter[i + offset];
                localCharacter[i + offset] = aux;
            }

            return new string(localCharacter);
        }

        private string buildMatrix(string encrypted, string key, int random)
        {
            char[] localCharacter = encrypted.ToCharArray();

            int i, j;
            int offset = 1 + (random % 10) / 3;
            int start = 1 + (random / 10) * 2;
            int n = localCharacter.Length;
            int len = 0;

            //setting the width
            for (i = 0; i < key.Length; i++) len += (int)key[i];

            if (random < 40) len = len % random;
            else len = len % random / 3;

            int m = 10 + len;

            if (start >= m) start = (random / 10);
            if (start >= m) start = 1;

            char[,] matrix = new char[m, n];

            for (j = 0; j < n; j++)
                for (i = 0; i < m; i++)
                {
                    matrix[i, j] = '\0';
                }

            for (j = 0; j < n; j++)
            {
                matrix[start, j] = localCharacter[j];
                int next = this.random.Next(0, m);

                int charOffset = next - start;

                if (j != n - 1)
                    matrix[(start + offset) % m, j] = (char)('N' + charOffset);
                start = next;

                for (i = 0; i < m; i++)
                {
                    if (matrix[i, j] == '\0') matrix[i, j] = (char)new Random().Next(33, 127);
                }
            }

            char[] save = new char[n * m];

            int count = 0;
            for (j = 0; j < n; j++)
            {
                for (i = 0; i < m; i++)
                {
                    save[count] = matrix[i, j];
                    count++;
                }
            }

            return new string(save);
        }

        private string addDifficulty(string encrypted, int difficulty)
        {
            char[] localCharacter = encrypted.ToCharArray();


            if ((localCharacter[localCharacter.Length - 1] == (char)(48) ||
                localCharacter[localCharacter.Length - 1] == (char)(49)) &&
                (localCharacter[localCharacter.Length - 2] == (char)(48) ||
                localCharacter[localCharacter.Length - 2] == (char)(49)))
            {
                Array.Resize(ref localCharacter, localCharacter.Length + 2);
                //set difficulty to 1
                if (difficulty == 1)
                {
                    localCharacter[localCharacter.Length - 2] = (char)48;
                    localCharacter[localCharacter.Length - 1] = (char)48;
                }
                else if (difficulty == 2)
                {
                    localCharacter[localCharacter.Length - 2] = (char)48;
                    localCharacter[localCharacter.Length - 1] = (char)49;
                }
                else if (difficulty == 3)
                {
                    localCharacter[localCharacter.Length - 2] = (char)49;
                    localCharacter[localCharacter.Length - 1] = (char)48;
                }
            }

            else
            {
                Array.Resize(ref localCharacter, localCharacter.Length + 1);

                if (difficulty == 1) localCharacter[localCharacter.Length - 1] = (char)setRandom(50, 75);
                else if (difficulty == 2) localCharacter[localCharacter.Length - 1] = (char)setRandom(76, 100);
                else if (difficulty == 3) localCharacter[localCharacter.Length - 1] = (char)setRandom(101, 126);
                else
                {
                    MessageBox.Show("Error in setting the difficulty.");
                    this.Close();
                }
            }

            return new string(localCharacter);
        }

        private string setAdititonalKey(string encrypted, int difficulty, bool shouldAdd)
        {
            char[] localCharacter = encrypted.ToCharArray();

            if (difficulty == 1)
            {
                //set an even (par) random number
                int evenNumber = setRandom(33, 120);
                if (evenNumber % 2 != 0) evenNumber++;

                //if we don't check no-key checkbox
                if (shouldAdd == false)
                {
                    Array.Resize(ref localCharacter, localCharacter.Length + 1);
                    localCharacter[localCharacter.Length - 1] = (char)evenNumber;
                }

                //if we check no-key checkbox
                else
                {
                    Array.Resize(ref localCharacter, localCharacter.Length + key.Length + 2);
                    localCharacter[localCharacter.Length - 1] = (char)(evenNumber + 1);
                    localCharacter[localCharacter.Length - 2] = (char)(33 + key.Length);
                    for (int i = 0; i < key.Length; i++)
                    {
                        localCharacter[localCharacter.Length - i - 3] = (char)((int)key[i] + 5);
                    }
                }
            }

            else
            {
                //if we don't check no-key checkbox
                if (shouldAdd == false)
                {
                    encrypted += "0";
                    return encrypted;
                }

                //if we check no-key checkbox
                else
                {
                    string newkey = key + key.Length;
                    newkey = converToBin(newkey);
                    newkey += "1";
                    encrypted += newkey;
                    return encrypted;
                }
            }

            return new string(localCharacter);
        }
        #endregion

        //DECRYPTING
        #region decrypting process
        private string getAdditionalKey(string encrypted, int difficulty, ref string key)
        {
            char[] localCharacter = encrypted.ToCharArray();

            if (difficulty == 1)
            {
                bool shouldAdd;
                if ((int)localCharacter[localCharacter.Length - 1] % 2 == 0) shouldAdd = false;
                else shouldAdd = true;

                //if we don't check no-key checkbox
                if (shouldAdd == false)
                {
                    Array.Resize(ref localCharacter, localCharacter.Length - 1);
                }

                //if we check no-key checkbox
                else
                {
                    int length = (int)localCharacter[localCharacter.Length - 2] - 33;
                    char[] newKey = new char[length];

                    for (int i = 0; i < length; i++)
                    {
                        newKey[i] = (char)((int)localCharacter[localCharacter.Length - i - 3] - 5);
                    }

                    key = new string(newKey);
                    Array.Resize(ref localCharacter, localCharacter.Length - length - 2);
                }
            }

            else
            {
                if (localCharacter[localCharacter.Length - 1] == '0')
                {
                    Array.Resize(ref localCharacter, localCharacter.Length - 1);
                }

                else
                {
                    //delete the last character which tells you that this is a NO-KEY type
                    Array.Resize(ref localCharacter, localCharacter.Length - 1);

                    //set the length
                    char[] lungime = new char[8];
                    for (int i = 0; i < 8; i++)
                    {
                        lungime[i] = localCharacter[localCharacter.Length - 8 + i];
                    }

                    string aux = converToDecimal(new string(lungime));
                    int length = (int)aux[0] - 48;

                    //set the key
                    char[] lungime2 = new char[8 * length];
                    for (int i = 0; i < 8 * length; i++)
                    {
                        lungime2[i] = localCharacter[localCharacter.Length - 8 - 8 * length + i];
                    }

                    aux = converToDecimal(new string(lungime2));

                    key = aux;
                    Array.Resize(ref localCharacter, localCharacter.Length - 8 - 8 * length);
                }
            }

            return new string(localCharacter);
        }

        private string extractDifficulty(string encrypted, ref int difficulty)
        {
            char[] localCharacter = encrypted.ToCharArray();

            if ((localCharacter[localCharacter.Length - 1] == (char)(48) ||
                localCharacter[localCharacter.Length - 1] == (char)(49)) &&
                (localCharacter[localCharacter.Length - 2] == (char)(48) ||
                localCharacter[localCharacter.Length - 2] == (char)(49)))
            {
                int first = (int)localCharacter[localCharacter.Length - 2] - 48;
                int second = (int)localCharacter[localCharacter.Length - 1] - 48;

                if (first == 0 && second == 0) difficulty = 1;
                else if (first == 0 && second == 1) difficulty = 2;
                else if (first == 1 && second == 0) difficulty = 3;
                else
                {
                    MessageBox.Show("Error while trying to extract difficulty.");
                    this.Close();
                }

                Array.Resize(ref localCharacter, localCharacter.Length - 2);
            }

            else
            {
                int number = (int)localCharacter[localCharacter.Length - 1];

                if (50 <= number && number <= 75) difficulty = 1;
                else if (75M <= number && number <= 100) difficulty = 2;
                else if (101 <= number && number <= 126) difficulty = 3;
                else
                {
                    //for safety purposes
                    MessageBox.Show("Error in setting the difficulty.");
                    this.Close();
                }

                Array.Resize(ref localCharacter, localCharacter.Length - 1);
            }

            return new string(localCharacter);
        }

        private string shiftRight(string decrypted, int n)
        {
            char[] localCharacter = decrypted.ToCharArray();

            int i;
            for (i = 0; i < localCharacter.Length; i++)
            {
                if (i - n >= 0)
                    localCharacter[i] = decrypted[i - n];
                else
                    localCharacter[i] = decrypted[localCharacter.Length + i - n];
            }

            return new string(localCharacter);
        }

        private string reverseXOR(string decrypted, string key, int random)
        {
            char[] localCharacter = decrypted.ToCharArray();

            int value = 0;
            int countWords = random + random % 10;

            //calculate stuff
            for (int i = 0; i < key.Length; i++) value += key[i];
            while (value >= random * 3 + value % 10) value /= 2;

            int count = 0;

            for (int i = 0; i < localCharacter.Length; i++)
            {
                int module = MODULE_VALUE;   //125 works
                if (localCharacter[i] == '!')
                {
                    count++;

                    for (int j = i; j < localCharacter.Length - 1; j++)
                    {
                        localCharacter[j] = localCharacter[j + 1];
                    }
                    Array.Resize(ref localCharacter, localCharacter.Length - 1);
                    i--;
                }

                //this  happens when you have actual encrypted code
                else
                {
                    countWords++;
                    //set the module
                    module = module - (5 * count);

                    localCharacter[i] = (char)(localCharacter[i] ^ ((countWords + value) % module));

                    count = 0;
                }
            }

            return new string(localCharacter);
        }

        private string extractRandomizedASCII(string decrypted)
        {
            char[] localCharacter = decrypted.ToCharArray();

            int length = 0;
            int i;
            for (i = 0; i < localCharacter.Length; i++)
            {
                if (i == 0)
                {
                    length = localCharacter[i] - 34;
                    continue;
                }

                if ((int)localCharacter[i] - 1 - i % length >= 33)
                    localCharacter[i] = (char)((int)localCharacter[i] - 1 - i % length);

                else
                {
                    int x;
                    int y;
                    x = localCharacter[i] - 33;
                    y = 126 + x;
                    localCharacter[i] = (char)(y - 1 - i % length);
                }
            }
            deleteIncluded(ref localCharacter, 0);

            return new string(localCharacter);
        }

        private void deleteIncluded(ref char[] localCharacter, int position)
        {
            for (int i = position; i < localCharacter.Length - 1; i++)
            {
                localCharacter[i] = localCharacter[i + 1];
            }
            Array.Resize(ref localCharacter, localCharacter.Length - 1);
        }

        private string extractRandom(string decrypted, string key, ref int randomNumber)
        {
            char[] localCharacter = decrypted.ToCharArray();

            int random;
            int randomFirst, randomSecond;
            int second = localCharacter[0];
            int first = localCharacter[localCharacter.Length - 1];

            deleteIncluded(ref localCharacter, 0);
            deleteIncluded(ref localCharacter, localCharacter.Length - 1);

            int a;
            int mode = 1;
            int value = (int)key[0] / 10;

            int i;
            for (i = 1; i < key.Length; i++)
            {
                if (mode == 1) value *= (int)key[i] / 10;
                else value -= (int)key[i] / 10;

                mode = -1 * mode;
            }

            a = value % 10;
            randomFirst = ((first - 30) / 4 - a);
            randomSecond = (second - 120) / (-4) - a;

            random = randomFirst * 10 + randomSecond;

            randomNumber = random;
            return new string(localCharacter);
        }

        private string converToDecimal(string decrypted)
        {
            char[] localCharacter = decrypted.ToCharArray();

            int countBinary = 0;
            int i, j;
            char[] binaryCharacter = new char[decrypted.Length / 8];

            for (i = 0; i < localCharacter.Length; i = i + 8)
            {
                int[] bin = new int[8];
                int count = 7;

                for (j = i; j <= i + 7; j++)
                {
                    bin[count] = (int)localCharacter[j] - 48;
                    count--;
                }

                //make it decimal
                int dec = 0;
                for (j = 0; j < 8; j++)
                {
                    if (bin[j] == 1)
                        dec += getPow(2, j);
                }

                //save it in decimal string
                binaryCharacter[countBinary] = (char)dec;
                countBinary++;
            }

            return new string(binaryCharacter);
        }

        private string decryptMatrix(string decrypted, string key, int random)
        {
            char[] localCharacter = decrypted.ToCharArray();

            int i, j;
            int offset = 1 + (random % 10) / 3;
            int start = 1 + (random / 10) * 2;
            int len = 0;

            //setting the width
            for (i = 0; i < key.Length; i++) len += (int)key[i];

            if (random < 40) len = len % random;
            else len = len % random / 3;

            int m = 10 + len;
            int n = localCharacter.Length / m;

            if (start >= m) start = (random / 10);
            if (start >= m) start = 1;

            char[,] matrix = new char[m, n];

            for (j = 0; j < n; j++)
            {
                for (i = 0; i < m; i++)
                {
                    matrix[i, j] = '\n';
                }
            }

            int count = 0;
            for (j = 0; j < n; j++)
            {
                for (i = 0; i < m; i++)
                {
                    matrix[i, j] = localCharacter[count];
                    count++;
                }
            }

            char[] save = new char[n];
            //rebuild pointer
            for (j = 0; j < n; j++)
            {
                save[j] = matrix[start, j];
                start = start + (int)(matrix[(start + offset) % m, j] - 'N');
            }

            return new string(save);
        }       
        #endregion

        //programs the buttons to do specific tasks and specific actions
        #region buttons and actions
        //RESET button
        private void button6_Click(object sender, EventArgs e)
        {
            resetAll();
        }

        //ENCRYPT button, encrypts based on your personal encryption choise
        private void button8_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            richTextBox1.Text = "";
            label5.Text = "(Length: " + richTextBox1.Text.Length + ")";
            label5.Left = LABEL_RESET_LOCATION - TextRenderer.MeasureText(label5.Text, label5.Font).Width;
            label9.Text = "Loading...";
            this.Refresh();

            if (checkTextAndKeyLength() == false)
            {
                this.Cursor = Cursors.Hand;
                label9.Text = "";
                return;
            }

            if (checkSelections() == false)
            {
                this.Cursor = Cursors.Hand;
                label9.Text = "";
                return;
            }

            string method = getComboName();
            string type;
            string localEncrypted = "";
            text = Convert.ToString(richTextBox3.Text);
            key = Convert.ToString(textBox2.Text);
            encrypted = "";

            //set the key
            if (checkBox1.Checked == true) key = setNoKeyWord(text);

            //set the difficulty steps / 1 - lowest / 2 - normal / 3 - highest
            int difficulty = getDifficulty();
            if (difficulty == 0)
            {
                this.Cursor = Cursors.Hand;
                label9.Text = "";
                return;
            };

            //choose what type of encryption do you want
            if (radioButton1.Checked == true) type = "Normal";
            else type = "Safe";

            try
            {
                //apply the chosen methods
                if (method == "Basic Encrypt") localEncrypted = basicEncrypt(difficulty);
                else if (method == "Short Encrypt") localEncrypted = shortEncrypt(difficulty);
                else if (method == "Long Encrypt") localEncrypted = longEncrypt(difficulty);

            }
            catch
            {
                //reset the visuals
                richTextBox1.Text = "";
                label5.Text = "(Length: " + richTextBox1.Text.Length + ")";
                label5.Left = LABEL_RESET_LOCATION - TextRenderer.MeasureText(label5.Text, label5.Font).Width;
                label9.Text = "";
                this.Cursor = Cursors.Hand;

                MessageBox.Show("An error occurred while trying to encrypt text.");
                return;
            }

            //if the safe method is chosen
            if (type == "Safe") if (isSafe(localEncrypted, difficulty) == false)
                {
                    this.Cursor = Cursors.Hand;
                    label9.Text = "";
                    return;
                }

            //show in the textbox the output of the encryption
            richTextBox1.Clear();
            richTextBox1.Text = localEncrypted;
            label5.Text = "(Length " + localEncrypted.Length + ")";
            label5.Left = LABEL_RESET_LOCATION - TextRenderer.MeasureText(label5.Text, label5.Font).Width;

            //reset stuff like no key generated key to be wrote in the textbox
            textBox2.Visible = true;
            label2.Visible = true;
            checkBox1.Checked = false;
            label9.Text = "Done!";
            this.Cursor = Cursors.Hand;
        }

        //SAVE TO FILE option
        private void button9_Click(object sender, EventArgs e)
        {
            label9.Text = "Loading...";

            string path = System.Windows.Forms.Application.StartupPath;
            path += "\\Encrypted.txt";

            if (richTextBox1.Text == "")
            {
                MessageBox.Show("No Encrypted text found.");
                label9.Text = "";
                return;
            }

            try
            {
                System.IO.File.WriteAllText(path, richTextBox1.Text);
                MessageBox.Show("Operation ended succesfully.\nEncrypted text copied to \"\\Encrypted.txt\"");
            }
            catch
            {
                MessageBox.Show("Error while trying to copy Encrypted text in folder.");
                label9.Text = "";
                return;
            }

            label9.Text = "Done!";
        }

        //READ FROM FILE option
        private void button1_Click(object sender, EventArgs e)
        {
            label9.Text = "Loading...";

            string path = System.Windows.Forms.Application.StartupPath;
            path += "\\Text.txt";
            string text = "";

            try
            {
                text = System.IO.File.ReadAllText(path);
                if (text == "")
                {
                    MessageBox.Show("Text input file is empty.");
                    label9.Text = "";
                    return;
                }

                richTextBox3.Text = text;

                MessageBox.Show("Operation ended succesfully.\nText copied from \"\\Text.txt\"");
            }
            catch
            {
                MessageBox.Show("Error while trying to copy Text from folder.\n\"\\Text.txt\" was not found.");
                label9.Text = "";
                return;
            }

            label9.Text = "Done!";
        }

        //easy encryption method use
        private string basicEncrypt(int difficulty)
        {
            encrypted = encrypt(text, key, difficulty);
            return encrypted;
        }

        //normal encryption method use
        private string shortEncrypt(int difficulty)
        {
            int iterations = getIterationValue();
            string save = "";

            encrypted = encrypt(text, key, difficulty);
            save = encrypted;

            for (int i = 2; i <= iterations; i++)
            {
                encrypted = encrypt(text, key, difficulty);
                if (encrypted.Length < save.Length) save = encrypted;
            }

            return save;
        }

        //hard encryption method use
        private string longEncrypt(int difficulty)
        {
            int iterations = getIterationValue();
            string save = "";

            encrypted = encrypt(text, key, difficulty);
            save = encrypted;

            for (int i = 2; i <= iterations; i++)
            {
                encrypted = encrypt(text, key, difficulty);
                if (encrypted.Length > save.Length) save = encrypted;
            }

            return save;
        }

        //check NO-KEY encryption option
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            string method = getComboName();

            if (checkBox1.Checked == true)
            {
                textBox2.Visible = false;
                textBox2.Text = "";
                label2.Visible = false;
            }
            else
            {
                textBox2.Visible = true;
                label2.Visible = true;
            }
        }

        //resets everything it needs to be reset when you change the method you use
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string method = getComboName();

            if (comboBox1.Text == "Basic Encrypt")
            {
                label8.Visible = false;
                textBox3.Visible = false;
                textBox3.Text = "" + ITERATIONS;
            }
            else
            {
                label8.Visible = true;
                textBox3.Visible = true;
                textBox3.Text = "" + ITERATIONS;
            }
        }

        //update every time you write something in the text
        private void richTextBox3_TextChanged(object sender, EventArgs e)
        {
            label11.Text = "(Length: " + richTextBox3.Text.Length + ")";
            label11.Left = LABEL_RESET_LOCATION - TextRenderer.MeasureText(label11.Text, label11.Font).Width;
        }

        //check if interations input are numbers, not letters or something else
        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (textBox3.Text.Length == 0) return;

            bool isOk = false;

            for (int i = 48; i <= 57; i++)
            {
                if ((int)textBox3.Text[textBox3.Text.Length - 1] == i)
                {
                    isOk = true;
                    break;
                }
            }

            if (isOk == false)
            {
                textBox3.Text = deleteLastCharacter(textBox3.Text);
                MessageBox.Show("Invalid iteration character. Only numbers are allowed.");
            }
        }
        #endregion

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged_1(object sender, EventArgs e)
        {

        }
    }
}

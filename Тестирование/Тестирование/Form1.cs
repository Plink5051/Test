using System.IO;
using System;
using System.Windows.Forms;

namespace Тестирование
{
    public partial class Form1 : Form
    {
        
        Int32 questionCount; // Счет вопросов
        Int32 correctAnswers;  // Количество правильных ответов
        Int32 wrongAnswers; // Количество не правильных ответов
        String[] WrongAnswers; // Массив вопросов, на которые даны неправильные ответы:
        Int32 numCorrectAnswer;  // Номер правильного ответа
        Int32 selectAnswer;  // Выбраный номер ответа,  
        StreamReader reader;
        public Form1()
        {
            InitializeComponent();
            CenterToScreen();
            AutoSize = true;
            Text = "Тест по Литературе";
            MaximizeBox = false;//нельзя развернуть окно
            FormBorderStyle = FormBorderStyle.Fixed3D;//нельзя изменить размер окна
            button3.Text = "Перемешать вопросы";
            button3.AutoSize = true;
            button2.AutoSize = true;

        }
        private void Form1_Load(object sender, EventArgs e)
        {
            button1.Text = "Следующий вопрос";
            button2.Text = "Выход";
            // Подписка на событие изменение состояния
            // переключателей RadioButton:
            radioButton1.CheckedChanged += new EventHandler(Changeswitch);
            radioButton2.CheckedChanged += new EventHandler(Changeswitch);
            radioButton3.CheckedChanged += new EventHandler(Changeswitch);
            radioButton4.CheckedChanged += new EventHandler(Changeswitch);
           StartTest();
        }
        //перемешивает  файл
        void MixQuestion()
        {
            reader.Close();
            string dest = Directory.GetCurrentDirectory() + @"\Testrus.txt";
            var encoding = System.Text.Encoding.UTF8;
            var lines = File.ReadAllLines(dest);
            var rand = new Random();
            for (int i = lines.Length - 1; i > 0; i--)
            {
                int j = rand.Next(i);

                var temp = lines[i - 1];
                lines[i - 1] = lines[j];
                lines[j] = temp;
            }
            File.WriteAllLines(dest, lines, encoding);
            radioButton1.Visible = true;
            radioButton2.Visible = true;
            radioButton3.Visible = true;
            radioButton4.Visible = true;
        }
        void StartTest()
        {
            var encoding = System.Text.Encoding.GetEncoding(1251);
            try
            {
                // Создание экземпляра StreamReader для чтения из файла
                reader = new StreamReader(Directory.GetCurrentDirectory() + @"\Testrus.txt", encoding); 
               
                // Обнуление всех счетчиков:
                questionCount = 0; correctAnswers = 0; wrongAnswers = 0;
                // Задаем размер массива для НеПравилОтветы:
                WrongAnswers = new String[100];
            }
            catch (Exception message)
            {   // Отчет о всех ошибках:
                MessageBox.Show(message.Message, "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            ReadNextQuestion();
        }
        void ReadNextQuestion()
        {
            String [] tmp = reader.ReadLine().Split('^');
            Int32 index = 0;
            label1.Text = tmp[index++];  

            // Считывание вариантов ответа:
            radioButton1.Text = tmp[index++];
            radioButton2.Text = tmp[index++];
            radioButton3.Text = tmp[index++];
            radioButton4.Text = tmp[index++];
            // Выясняем, какой ответ - правильный:
            numCorrectAnswer = 1;
            // Переводим все переключатели в состояние "выключено":
            radioButton1.Checked = false;
            radioButton2.Checked = false;
            radioButton3.Checked = false;
            radioButton4.Checked = false;
            // Первую кнопку задаем не активной, пока не выберет
            // вариант ответа
            button1.Enabled = false;

            questionCount++;//счет вопросов
            // Проверка, конец ли файла:
            if (reader.EndOfStream == true) button1.Text = "Завершить";
        }
        void Changeswitch(Object sender, EventArgs e)
        {
            // Кнопка "Следующий вопрос" становится активной, и ей
            // передаем фокус:
            button1.Enabled = true; button1.Focus();
            RadioButton _switch = (RadioButton)sender;
            var tmp = _switch.Name;
            // Выясняем выбранный  номер ответа:
            selectAnswer = Int32.Parse(tmp.Substring(11));
        }
        private void button1_Click(object sender, EventArgs e)
        {
            // Щелчок на кнопке
            // "Следующий вопрос/Завершить/Начать тестирование сначала"
            // Счет правильных ответов:
            if (selectAnswer == numCorrectAnswer) correctAnswers++;
                                                
            if (selectAnswer != numCorrectAnswer)
            {
                // Счет неправильных ответов:
                wrongAnswers++;
                // Запоминаем вопросы с неправильными ответами:
                WrongAnswers[wrongAnswers] = label1.Text;
            }
            if (button1.Text == "Начать тестирование сначала")
            {
                button1.Text = "Следующий вопрос";
                // Переключатели становятся видимыми, доступными для выбора:
                radioButton1.Visible = true;
                radioButton2.Visible = true;
                radioButton3.Visible = true;
                radioButton4.Visible = true;
                // Переход к началу файла:
                StartTest(); return;
            }
            if (button1.Text == "Завершить")
            {
                // Закрываем текстовый файл:
                reader.Close();
                // Переключатели делаем невидимыми:
                radioButton1.Visible = false;
                radioButton2.Visible = false;
                radioButton3.Visible = false;
                radioButton4.Visible = false;
                // Формируем оценку за тест:
                label1.Text = String.Format("Тестирование завершено.\n" +
                    "Правильных ответов: {0} из {1}.\n" +
                    "Оценка в пятибальной системе: {2:F2}.", correctAnswers,
                    questionCount, (correctAnswers * 5.0F) / questionCount);
                // 5F - это максимальная оценка
                button1.Text = "Начать тестирование сначала";
                // Вывод вопросов, на которые "Вы дали неправильный ответ":
                var Str = "СПИСОК ВОПРОСОВ, НА КОТОРЫЕ ВЫ ДАЛИ " +
                          "НЕПРАВИЛЬНЫЙ ОТВЕТ:\n\n";
                for (int i = 1; i <= wrongAnswers; i++)
                    Str = Str + WrongAnswers[i] + "\n";

                // Если есть неправильные ответы, то вывести через
                // MessageBox список соответствующих вопросов:
                if (wrongAnswers != 0) MessageBox.Show(
                                          Str, "Тестирование завершено");
            } // Конец условия if (button1.Text == "Завершить")
            if (button1.Text == "Следующий вопрос") ReadNextQuestion();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Вы точно хотите выйти?",
                "Сообщение",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
           if(result == DialogResult.Yes) this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MixQuestion();
            StartTest();

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}

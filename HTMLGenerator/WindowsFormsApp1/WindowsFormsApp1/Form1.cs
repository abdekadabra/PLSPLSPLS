using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private string currentImagePath;

        private ToolTip toolTip;
        private Timer tooltipTimer;
        public Form1()
        {
            InitializeComponent();

            // Инициализируем HTML-документ при запуске формы
            UpdateHtmlDocument();

            // Подключаем событие KeyDown для bodyTextBox
            bodyTextBox.KeyDown += bodyTextBox_KeyDown;

            // Очистим PictureBox
            pictureBox.Image = null;

            // Инициализация ToolTip
            toolTip = new ToolTip();
            toolTip.AutoPopDelay = 1000; // Время отображения подсказки
            toolTip.InitialDelay = 5000; // Начальная задержка перед отображением подсказки
            toolTip.ReshowDelay = 100; // Задержка перед повторным отображением подсказки
            toolTip.ShowAlways = false; // Устанавливаем false, чтобы ToolTip не отображался всегда

            // Инициализация Timer
            tooltipTimer = new Timer();
            tooltipTimer.Interval = 5000; // Интервал в миллисекундах (в данном случае, 5 секунд)
            tooltipTimer.Tick += TooltipTimer_Tick; // Обработчик события срабатывания таймера

            // Добавляем подсказку к кнопке
            toolTip.SetToolTip(questionButton, "Путь к изображение и само изображение не должны содержать пробелов");

            questionButton.Click += questionButton_Click;

        }
        private void questionButton_Click(object sender, EventArgs e)
        {
            // Показываем подсказку при клике на кнопку
            toolTip.Show("Используйте изображение без пробелов в названии", questionButton, 0, questionButton.Height, 5000);
            tooltipTimer.Start(); // Запускаем таймер
        }
        private void TooltipTimer_Tick(object sender, EventArgs e)
        {
            // Скрываем подсказку и останавливаем таймер
            toolTip.Hide(questionButton);
            tooltipTimer.Stop();
        }
        private void UpdateHtmlDocument()
        {
            // Получаем данные из TextBox'ов и ComboBox
            string title = titleTextBox.Text;
            string headContent = headTextBox.Text;
            string bodyContent = bodyTextBox.Text; // Добавлено получение содержимого bodyTextBox

            // Формируем HTML - код
            string htmlCode = GenerateHtmlDocument(title, headContent, bodyContent);


            // Отображаем HTML-код в WebBrowser
            webBrowser.DocumentText = htmlCode;
        }

        private string GenerateHtmlDocument(string title, string headContent, string bodyContent)
        {
            // Формируем HTML-код
            StringBuilder htmlBuilder = new StringBuilder();

            htmlBuilder.AppendLine("<!DOCTYPE html>");
            htmlBuilder.AppendLine("<html>");

            // Начинаем <head>
            htmlBuilder.AppendLine("<head>");
            htmlBuilder.AppendLine("<meta charset=\"utf-8\">");

            // Добавляем иконку сайта
            htmlBuilder.AppendLine("<link rel=\"icon\" href=\"favicon.ico\" type=\"image/x-icon\">");

            // Здесь добавьте стили, если необходимо

            // Вставляем заголовок из TextBox'а внутри <head>
            htmlBuilder.AppendLine($"<title>{title}</title>");

            // Закрываем <head>
            htmlBuilder.AppendLine("</head>");

            // Начинаем <body>
            htmlBuilder.AppendLine("<body>");

            // Вставляем заголовок внутри <body>
            htmlBuilder.AppendLine($"<h1>{headContent}</h1>");

            // Вставляем содержимое bodyTextBox внутри <body>
            htmlBuilder.AppendLine($"<p>{bodyContent}</p>");

            // Здесь вы можете добавить другие элементы внутрь <body>

            // Закрываем <body>
            htmlBuilder.AppendLine("</body>");

            // Закрываем <html>
            htmlBuilder.AppendLine("</html>");

            return htmlBuilder.ToString();
        }



        private void SaveHtmlToFile(string htmlCode, string fileName)
        {
            try
            {
                // Получаем путь к папке, в которой находится HTML-файл
                string folderPath = Path.GetDirectoryName(fileName);

                // Копируем изображение в ту же папку
                string newImagePath = Path.Combine(folderPath, Path.GetFileName(currentImagePath));
                File.Copy(currentImagePath, newImagePath, true);

                // Заменяем путь к изображению в HTML-коде на абсолютный
                htmlCode = htmlCode.Replace(currentImagePath, newImagePath);

                // Сохраняем HTML-документ в файл
                using (StreamWriter sw = new StreamWriter(fileName))
                {
                    sw.Write(htmlCode);
                }

                MessageBox.Show($"HTML-документ сохранен в файл: {fileName}", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при сохранении файла: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            // Получаем данные из TextBox'ов
            string title = titleTextBox.Text;
            string headContent = headTextBox.Text;
            string bodyContent = bodyTextBox.Text; // Добавлено получение содержимого bodyTextBox


            // Формируем HTML-код
            string htmlCode = GenerateHtmlDocument(title, headContent, bodyContent);

            // Сохраняем HTML-код в файл
            SaveHtmlToFile(htmlCode, "output.html");
        }


        private void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }

        private void bodyTextBox_TextChanged(object sender, EventArgs e)
        {
            // При изменении текста в bodyTextBox обновляем HTML-документ
            UpdateHtmlDocument();
        }

        private void headTextBox_TextChanged(object sender, EventArgs e)
        {
            UpdateHtmlDocument();
        }

        private void bodyTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            // Проверяем, были ли нажаты клавиши Shift и Enter
            if (e.Shift && e.KeyCode == Keys.Enter)
            {
                // Добавляем тег переноса строки в текстовое поле
                bodyTextBox.SelectedText = "<br>";

                // Предотвращаем дальнейшую обработку события KeyDown
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void pictureBox_Click(object sender, EventArgs e)
        {
            // Открывайте изображение во внешнем редакторе, если оно выбрано
            if (!string.IsNullOrEmpty(currentImagePath) && File.Exists(currentImagePath))
            {
                System.Diagnostics.Process.Start("mspaint.exe", currentImagePath);
            }
        }

        private void addImageButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Изображения (*.jpg; *.jpeg; *.png; *.gif)|*.jpg; *.jpeg; *.png; *.gif|Все файлы (*.*)|*.*";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Получаем выбранный путь к файлу изображения
                    currentImagePath = openFileDialog.FileName;

                    // Отображаем изображение в PictureBox
                    pictureBox.Image = Image.FromFile(currentImagePath);

                    // Добавляем тег <img> с выбранным изображением в bodyTextBox
                    bodyTextBox.SelectedText = $"<img src=\"{currentImagePath}\"";
                }
            }
        }

        private void editImageButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(currentImagePath))
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Изображения (*.jpg; *.jpeg; *.png; *.gif)|*.jpg; *.jpeg; *.png; *.gif|Все файлы (*.*)|*.*";
                    openFileDialog.RestoreDirectory = true;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        // Получаем новый путь к файлу изображения
                        string newImagePath = openFileDialog.FileName;

                        // Заменяем тег <img> в bodyTextBox на новый путь к изображению и обновленные размеры
                        bodyTextBox.Text = Regex.Replace(
                            bodyTextBox.Text,
                            $"<img src=\"{Regex.Escape(currentImagePath)}\" alt=\"Описание изображения\"(.*?)>",
                            $"<img src=\"{newImagePath}\" alt=\"Описание изображения\" style=\"width:{pictureBox.Image?.Width}px; height:{pictureBox.Image?.Height}px;\">"
                        );

                        // Обновляем текущий путь к изображению
                        currentImagePath = newImagePath;
                    }
                }
            }

            else
            {
                MessageBox.Show("Выберите изображение для редактирования.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
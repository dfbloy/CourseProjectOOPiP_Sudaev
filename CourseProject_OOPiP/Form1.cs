using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace CourseProject_OOPiP
{
    public partial class Form1 : Form
    {
        private DataRepository<Publication> _repository;
        private readonly string _storagePath = "app_data.xml";
        private bool _isProgrammaticAdd = false;

        private readonly Dictionary<string, string> _columnHeaders = new Dictionary<string, string>
        {
            { "Title", "Название" },
            { "Year", "Год издания" },
            { "Price", "Цена" },
            { "Author", "Автор" },
            { "Isbn", "ISBN код" },
            { "IssueNumber", "Номер выпуска" },
            { "Periodicity", "Периодичность" }
        };

        public Form1()
        {
            InitializeComponent();
            _repository = new DataRepository<Publication>(_storagePath);
            InitializeGridColumns();
            ApplyButtonColors();
            dataGridView1.DataSource = _repository.GetAll();

            dataGridView1.CellFormatting += DataGridView1_CellFormatting;
            dataGridView1.CellValidating += DataGridView1_CellValidating;
        }

        private void InitializeGridColumns()
        {
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.Columns.Clear();

            foreach (var kp in _columnHeaders)
            {
                var col = new DataGridViewTextBoxColumn
                {
                    Name = kp.Key,
                    HeaderText = kp.Value,
                    DataPropertyName = kp.Key
                };
                dataGridView1.Columns.Add(col);
            }
        }

        private void ApplyButtonColors()
        {
            button3.BackColor = Color.FromArgb(200, 240, 200);
            button3.FlatStyle = FlatStyle.Flat;
            button3.FlatAppearance.BorderSize = 1;

            button7.BackColor = Color.FromArgb(200, 225, 255);
            button7.FlatStyle = FlatStyle.Flat;
            button7.FlatAppearance.BorderSize = 1;
        }

        private void DataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (_isProgrammaticAdd) return;
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var row = dataGridView1.Rows[e.RowIndex];
            var item = row.DataBoundItem as Publication;

            if (IsPublicationEmpty(item)) return;

            var propName = dataGridView1.Columns[e.ColumnIndex].DataPropertyName;
            string newValue = e.FormattedValue?.ToString() ?? string.Empty;

            var validatableItem = row.DataBoundItem as IValidatable;
            if (validatableItem == null) return;

            var currentCell = dataGridView1[e.ColumnIndex, e.RowIndex];
            if (currentCell.ReadOnly) return;

            string errorMessage = validatableItem.ValidateField(propName, newValue);

            if (errorMessage == null && propName == "Isbn")
            {
                var currentBook = row.DataBoundItem as Book;
                if (!_repository.IsIsbnUnique(newValue, currentBook))
                {
                    errorMessage = "Книга с таким ISBN уже существует в базе.";
                }
            }

            if (errorMessage != null)
            {
                MessageBox.Show(errorMessage, "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true;
            }
        }

        private void DataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var row = dataGridView1.Rows[e.RowIndex];
            var item = row.DataBoundItem as Publication;
            if (item == null) return;

            var currentCell = dataGridView1[e.ColumnIndex, e.RowIndex];
            var propName = dataGridView1.Columns[e.ColumnIndex].DataPropertyName;

            if (item is Book)
            {
                row.DefaultCellStyle.BackColor = Color.FromArgb(230, 245, 230);
                if (propName == "IssueNumber" || propName == "Periodicity")
                {
                    currentCell.ReadOnly = true;
                    currentCell.Style.BackColor = Color.LightGray;
                    currentCell.Style.SelectionBackColor = Color.DarkGray;
                    e.Value = "";
                    e.FormattingApplied = true;
                }
                else
                {
                    currentCell.ReadOnly = false;
                    currentCell.Style.BackColor = Color.Empty;
                    currentCell.Style.SelectionBackColor = Color.Empty;
                }
            }
            else if (item is Magazine)
            {
                row.DefaultCellStyle.BackColor = Color.FromArgb(230, 240, 255);
                if (propName == "Author" || propName == "Isbn")
                {
                    currentCell.ReadOnly = true;
                    currentCell.Style.BackColor = Color.LightGray;
                    currentCell.Style.SelectionBackColor = Color.DarkGray;
                    e.Value = "";
                    e.FormattingApplied = true;
                }
                else
                {
                    currentCell.ReadOnly = false;
                    currentCell.Style.BackColor = Color.Empty;
                    currentCell.Style.SelectionBackColor = Color.Empty;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string query = textBox1.Text;
            dataGridView1.DataSource = _repository.Search(query);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            dataGridView1.DataSource = _repository.GetAll();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            _isProgrammaticAdd = true;
            var newItem = new Book
            {
                Title = null,
                Year = null,
                Price = null,
                Author = null,
                Isbn = null
            };
            _repository.Add(newItem);
            _isProgrammaticAdd = false;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            _isProgrammaticAdd = true;
            var newItem = new Magazine
            {
                Title = null,
                Year = null,
                Price = null,
                IssueNumber = null,
                Periodicity = null
            };
            _repository.Add(newItem);
            _isProgrammaticAdd = false;
        }

        private static bool IsPublicationEmpty(Publication item)
        {
            if (item == null) return true;

            if (item is Book book)
            {
                return string.IsNullOrWhiteSpace(book.Title)
                    && book.Year == null
                    && book.Price == null
                    && string.IsNullOrWhiteSpace(book.Author)
                    && string.IsNullOrWhiteSpace(book.Isbn);
            }

            if (item is Magazine mag)
            {
                return string.IsNullOrWhiteSpace(mag.Title)
                    && mag.Year == null
                    && mag.Price == null
                    && mag.IssueNumber == null
                    && string.IsNullOrWhiteSpace(mag.Periodicity);
            }

            return false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            dataGridView1.EndEdit();

            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Выберите запись для удаления.", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var selectedItem = dataGridView1.CurrentRow.DataBoundItem as Publication;
            if (selectedItem == null) return;

            if (!IsPublicationEmpty(selectedItem))
            {
                string typeName = selectedItem is Book ? "книгу" : "журнал";
                string title = string.IsNullOrWhiteSpace(selectedItem.Title)
                    ? "(без названия)"
                    : $"«{selectedItem.Title}»";

                var result = MessageBox.Show(
                    $"Вы действительно хотите удалить {typeName} {title}?",
                    "Подтверждение удаления",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2);

                if (result != DialogResult.Yes) return;
            }

            _repository.Remove(selectedItem);

            if (dataGridView1.DataSource != _repository.GetAll())
            {
                dataGridView1.DataSource = _repository.GetAll();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                var item = row.DataBoundItem as IValidatable;
                if (item == null) continue;

                foreach (DataGridViewColumn col in dataGridView1.Columns)
                {
                    int rIdx = row.Index;
                    int cIdx = col.Index;
                    if (dataGridView1[cIdx, rIdx].ReadOnly) continue;

                    var cellVal = dataGridView1[cIdx, rIdx].Value;
                    string strVal = cellVal?.ToString() ?? string.Empty;

                    string error = item.ValidateField(col.DataPropertyName, strVal);
                    if (error != null)
                    {
                        dataGridView1.CurrentCell = dataGridView1[cIdx, rIdx];
                        MessageBox.Show($"Невозможно сохранить файл!\n\nСтрока {rIdx + 1}, колонка '{col.HeaderText}':\n{error}", "Ошибка валидации перед сохранением", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }
                }
            }

            _repository.Save();
            MessageBox.Show("Данные успешно проверены и сохранены в XML.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            _repository.Load();
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = _repository.GetAll();
            MessageBox.Show("Данные успешно загружены из XML.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
